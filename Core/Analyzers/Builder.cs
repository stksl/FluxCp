using Fluxcp.Syntax;
using Fluxcp.Errors;
using Fluxcp.LowLevel;
using System.Reflection.Emit;
namespace Fluxcp;
// in our case, Builder does what is known as semantic analysis. 
// It will go through the AST and check for semantic validity (types, variable names and scopes etc.)
// And builder will actually generate LLEM bytecode by fully going trough the AST.
public sealed class Builder
{
    private readonly SyntaxTree tree;
    private readonly CompilationUnit unit;

    private object _sync = new object();
    public Builder(SyntaxTree tree_, CompilationUnit unit_)
    {
        tree = tree_;
        unit = unit_;
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
        if (!unit.Options.IsExecutable)
            return;

        FunctionDeclaration? mainEntry = unit.LocalStorage.GetLocalFunc("Main");
        if (mainEntry == null)
            Error.Execute(unit.Logger, ErrorDefaults.NoEntryFound, 0); // 0 as line FOR NOW

        // generates nasm output
        GenerateOutput(mainEntry!);
    }
    private void GenerateOutput(FunctionDeclaration mainEntry) 
    {
        NasmGenerator generator = new NasmGenerator(null!, unit);
        if (!mainEntry.Header.ReturnType.IsTypeDefined(unit, out StructDefine? retType))
            Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line FOR NOW
        else if (mainEntry.Header.Args != null)
            Error.Execute(unit.Logger, ErrorDefaults.NoEntryFound, 0); // 0 as line FOR NOW
        
        NasmStackFrame mainFrame = generator.CreateStackFrame(retType!.GetSize(unit));

        SyntaxNode bodyNode = mainEntry.Body.Child;
        while (bodyNode != null!) 
        {
            if (bodyNode is VarDeclarationNode varDecl) 
            {
                if (unit.LocalStorage.GetLocalVar(varDecl.Name) != null)
                    Error.Execute(unit.Logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line FOR NOW
                unit.LocalStorage.AddLocalVar(varDecl);

                if (!varDecl.DataType.IsTypeDefined(unit, out StructDefine? locVarType))
                    Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line FOR NOW
                
                generator.AllocSizeForLocal(locVarType!.GetSize(unit), mainFrame);
                if (varDecl.Value != null)
                    bodyNode = varDecl.Value;
            }
            if (bodyNode is VariableValue varVal) 
            {
                VarDeclarationNode? variable = unit.LocalStorage.GetLocalVar(varVal.ToVar!);
                if (variable == null) 
                    Error.Execute(unit.Logger, ErrorDefaults.UnknownReference, 0); // 0 as line FOR NOW
                else if (variable == null) 
                {
                    // allocating on stack anyways
                    // if / copyValue :: u64  123 :: u64/
                    generator.AllocSizeForLocal(0, mainFrame);
                }
                StructDefine? varType = unit.LocalStorage.GetLocalType(variable!.DataType);

                if (varType == null)
                    Error.Execute(unit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line for now
                if (varVal is LiteralValue litVal) 
                {
                    byte[] literalData = litVal.GetBytes(unit);
                    if (literalData == null)
                        Error.Execute(unit.Logger, ErrorDefaults.UnknownDeclaration, 0); // 0 as line FOR NOW
                    generator.UpdateLocal((uint)mainFrame.Locals.Count + 1, literalData!, 0, literalData!.Length, mainFrame);
                }
                else if (varVal is CopyValue copVal) 
                {

                }
            }
            bodyNode = bodyNode.Next!;
        }
    }
}