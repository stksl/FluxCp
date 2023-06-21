using Fluxcp.Syntax;
using Fluxcp.Errors;
namespace Fluxcp;
// in our case, Builder does what is known as semantic analysis. 
// It will go through the AST and check for semantic validity (types, variable names and scopes etc.)
// And builder will actually generate LLEM bytecode by fully going trough the AST.
public sealed class Builder
{
    private readonly SyntaxTree tree;
    private readonly CompilationUnit unit;
    private readonly ILogger? logger;

    private object _sync = new object();
    public Builder(SyntaxTree tree_, CompilationUnit unit_, ILogger? logger_)
    {
        tree = tree_;
        unit = unit_;
        logger = logger_;
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
                new CompilingOptions(false, useStatement.AssemblyName.Substring(1, asmlngth - 2)), logger);

            CompilationUnit newUnit = build.FullBuild(SourceText.FromString(build.LoadAsync().Result));
            if (!unit.LocalStorage.AddDependency(useStatement.AssemblyName, newUnit.LocalStorage, true)) 
            {
                Error.Execute(logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line FOR NOW
            }
            // todo
            currNode = currNode.Next;
        }
        while (currNode != null) 
        {
            if (currNode is StructDefine structDefine) 
            {
                if (unit.LocalStorage.GetLocalType(DataType.FromName(structDefine.Name)) != null)
                    Error.Execute(logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line for now
                unit.LocalStorage.AddLocalType(structDefine);
            }
            else if (currNode is FunctionDeclaration function) 
            {
                if (unit.LocalStorage.GetLocalFunc(function.Header.Name) != null)
                    Error.Execute(logger, ErrorDefaults.AlreadyDefined, 0); // 0 as line for now
                unit.LocalStorage.AddLocalFunc(function);
            }
            //here we dont execute an error because had already been checked in parser.
            currNode = currNode.Next;
        }
        if (!unit.Options.IsExecutable)
            return;

        FunctionDeclaration? mainEntry = unit.LocalStorage.GetLocalFunc("Main");
        if (mainEntry == null)
            Error.Execute(logger, ErrorDefaults.NoEntryFound, 0); // 0 as line FOR NOW

        // generates nasm output
        GenerateOutput(mainEntry!);
    }
    private void GenerateOutput(FunctionDeclaration mainEntry) 
    {
        
    }
}
