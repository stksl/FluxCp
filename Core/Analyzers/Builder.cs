using Fluxcp.Syntax;
using Fluxcp.Errors;
using Fluxcp.LowLevel;
using System.Reflection.Emit;
namespace Fluxcp;
// in our case, Builder does what is known as semantic analysis. 
// It will go through the AST and check for semantic validity (types, variable names and scopes etc.)
// And builder will actually generate Netwide Assembler (NASM) code by fully going trough the AST.
public sealed class Builder
{
    private readonly SyntaxTree tree;
    private readonly CompilationUnit unit;
    private readonly Dictionary<FunctionDeclaration, bool> localLabels;
    private object _sync = new object();
    private NasmGenerator generator;
    public Builder(SyntaxTree tree_, CompilationUnit unit_)
    {
        tree = tree_;
        unit = unit_;
        generator = null!;
        localLabels = new Dictionary<FunctionDeclaration, bool>();
    }
    public void Build() 
    {
        lock(_sync) 
        {
            if (tree.Root is not SyntaxTree.ProgramBound)
                throw new InvalidDataException("Tree root is not program bound!");

            FullBuild();
        }
    }
    // makes semantic analysis and generates an output file that is NASM for now.
    private void FullBuild() 
    {
        SyntaxNode? currNode = tree.Root.Next;
        while (currNode is UseStatement useStatement) 
        {
            int asmlngth = useStatement.AssemblyName.Length;

            var build = new FullBuildInternal(
                new CompilingOptions(false, useStatement.AssemblyName.Substring(1, asmlngth - 2)), unit.Logger);

            CompilationUnit newUnit = build.FullBuild(SourceText.FromString(build.LoadAsync().Result));
            if (!unit.LocalStorage.AddDependency(useStatement.AssemblyName, newUnit.LocalStorage, true)) 
            {
                Error.Execute(unit.Logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line FOR NOW
            }
            // todo
            currNode = currNode.Next;
        }
        while (currNode != null) 
        {
            if (currNode is StructDefine structDefine) 
            {
                if (unit.LocalStorage.GetLocalType(DataType.FromName(structDefine.Name)) != null)
                    Error.Execute(unit.Logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line for now
                //
                unit.LocalStorage.AddLocalType(structDefine);
            }
            else if (currNode is FunctionDeclaration function) 
            {
                if (unit.LocalStorage.GetLocalFunc(function.Header.Name) != null)
                    Error.Execute(unit.Logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line for now
                unit.LocalStorage.AddLocalFunc(function);
            }
            //here we dont execute an error because had already been checked in parser.
            currNode = currNode.Next;
        }

        FunctionDeclaration? mainEntry = unit.LocalStorage.GetLocalFunc("Main");

        if (mainEntry != null && !unit.Options.IsExecutable)
            Error.Execute(unit.Logger, ErrorDefaults.UnknownDeclaration, 0); // 0 as line for now
        else if (!unit.Options.IsExecutable)
            return;
        else if (mainEntry == null || mainEntry.Header.Args.Length != 0)
            Error.Execute(unit.Logger, ErrorDefaults.NoEntryFound, 0); // 0 as line FOR NOW

        // generates nasm output
        using StreamWriter sw = new StreamWriter("/home/ubuntupc/Desktop/test.asm");
        generator = new NasmGenerator(sw.BaseStream);

        BuildFunction(mainEntry!);
    }
    private void BuildFunction(FunctionDeclaration function) 
    {
        if (!function.Header.ReturnType.IsTypeDefined(unit, out StructDefine? retType))
            Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line FOR NOW

        List<uint> argsSize = new List<uint>();
        // counting arguments size to create a stack frame
        foreach(FunctionArgument arg in function.Header.Args)
        {
            if (!arg.DataType.IsTypeDefined(unit, out StructDefine? foundType))
                Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line for now

            argsSize.Add(foundType!.GetSize(unit));
        }
        // creating stack frame
        NasmStackFrame frame = generator.CreateStackFrame(retType!.GetSize(unit), function.Header.Name, argsSize.ToArray());

        // building body
        SyntaxNode bodyNode = function.Body.Child;
        while (bodyNode != null!) 
        {
            void parseVarVal(VariableValue varVal) 
            {
                VarDeclarationNode? variable = unit.LocalStorage.GetLocalVar(varVal.ToVar!);
                if (variable == null) 
                    Error.Execute(unit.Logger, ErrorDefaults.UnknownReference, 0); // 0 as line FOR NOW
                else if (variable == null) 
                {
                    // allocating on stack anyways TODO
                    // if / copyValue :: u64  123 :: u64/
                    generator.AllocSizeForLocal(0, frame);
                }
                StructDefine? varType = unit.LocalStorage.GetLocalType(variable!.DataType);

                if (varType == null)
                    Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line for now
                if (varVal is LiteralValue litVal) 
                {
                    litVal.CastTo = variable?.DataType;

                    byte[] literalData = litVal.GetBytes(unit);
                    if (literalData == null)
                        Error.Execute(unit.Logger, ErrorDefaults.UnknownDeclaration, 0); // 0 as line FOR NOW
                    
                    generator.UpdateLocal((uint)frame.Locals.Count, literalData!, 0, literalData!.Length, frame);
                }
                else if (varVal is CopyValue copVal) 
                {
                    VarDeclarationNode? fromVar = unit.LocalStorage.GetLocalVar(copVal.FromVar);
                    if (fromVar == null)
                        Error.Execute(unit.Logger, ErrorDefaults.UnknownReference, 0); // 0 as line for NOW
                    else if (fromVar.Value == null)
                        Error.Execute(unit.Logger, ErrorDefaults.UnknownReference, 0); // 0 as line for now

                    byte[] copyData = fromVar!.Value!.GetBytes(unit);
                    if (copyData == null)
                        Error.Execute(unit.Logger, ErrorDefaults.UnknownDeclaration, 0); // 0 as line for now

                    generator.UpdateLocal((uint)frame.Locals.Count, copyData!, 0, copyData!.Length, frame);
                }
                else if (varVal is FunctionCall funcCall) 
                {
                    FunctionDeclaration? func = unit.LocalStorage.GetLocalFunc(funcCall.FunctionName);
                    if (func == null)
                        Error.Execute(unit.Logger, ErrorDefaults.UnknownReference, 0); // 0 as line for now

                    foreach(VariableValue passedVal in funcCall.PassedVals) 
                    {
                        parseVarVal(passedVal);
                    }
                    generator.CallLabel(func!.Header.Name);
                    localLabels[func] = true; // no need in value
                }

            }
            if (bodyNode is VarDeclarationNode varDecl) 
            {
                if (unit.LocalStorage.GetLocalVar(varDecl.Name) != null)
                    Error.Execute(unit.Logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line FOR NOW
                unit.LocalStorage.AddLocalVar(varDecl);

                if (!varDecl.DataType.IsTypeDefined(unit, out StructDefine? locVarType))
                    Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line FOR NOW
                
                generator.AllocSizeForLocal(locVarType!.GetSize(unit), frame);
                if (varDecl.Value != null) 
                {
                    parseVarVal(varDecl.Value);
                }

            }
            else if (bodyNode is VariableValue varVal) 
            {
                parseVarVal(varVal);
            }
            bodyNode = bodyNode.Next!;
        }

        // the end of the function. Resetting stack pointer
        unit.LocalStorage.ClearStackFrame();
        generator.RemoveStackFrame();

        // recursively generating new labels
        foreach(KeyValuePair<FunctionDeclaration, bool> label in localLabels) 
        {
            if (!localLabels.Remove(label.Key))
                Error.Execute(unit.Logger, ErrorDefaults.BaseError, 0); // 0 as line for now

            BuildFunction(label.Key);
        }
    }
}