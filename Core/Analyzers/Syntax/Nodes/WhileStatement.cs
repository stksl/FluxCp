using Fluxcp.Syntax;
using Fluxcp.Errors;
namespace Fluxcp;
public sealed class WhileStatement : LogicalStatement 
{
    public readonly BodyBound Body;
    public WhileStatement(ExpressionNode exp, BodyBound body)
    :base(exp)
    {
        Body = body;
    }
    public static new WhileStatement Parse(Parser parser) 
    {
        if (!parser.SaveEquals(0, SyntaxKind.WhileStatementToken)) 
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[parser.offset].Line);
        parser.offset++;
        ExpressionNode exp = ExpressionNode.Parse(parser);
        BodyBound body = BodyBound.Parse(parser);

        return new WhileStatement(exp, body);
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Condition;
        yield return Body;
    }
}