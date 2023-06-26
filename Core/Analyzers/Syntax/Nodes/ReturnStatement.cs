using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class ReturnStatement : SyntaxNode 
{
    public VariableValue Value;
    public ReturnStatement(VariableValue value)
    {
        Value = value;
    }
    public override IEnumerable<SyntaxNode> GetChildren() 
    {
        yield return Value;
    }
    public static new ReturnStatement Parse(Parser parser) 
    {
        if (!parser.SaveEquals(0, SyntaxKind.ReturnStatementToken)) 
        {
            Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[parser.offset].Line);
        }
        parser.offset++;
        return new ReturnStatement(VariableValue.Parse(parser));
    }
}