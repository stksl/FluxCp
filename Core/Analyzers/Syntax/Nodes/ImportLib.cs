using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class ImportLib : SyntaxNode
{
    public readonly FunctionCall Call;
    public readonly VariableValue PathToLib;
    public ImportLib(FunctionCall call, VariableValue pathToLib)
    {
        Call = call;
        PathToLib = pathToLib;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return PathToLib;
        yield return Call;
    }
    public static new ImportLib Parse(Parser parser) 
    {
        if (!parser.SaveEquals(0, SyntaxKind.ImportLibStatement))
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[parser.offset].Line);
        parser.offset++;
        VariableValue pathVar = VariableValue.Parse(parser);
        // next token expected to be function call directly
        FunctionCall call = FunctionCall.Parse(parser);

        return new ImportLib(call, pathVar);
    }
}