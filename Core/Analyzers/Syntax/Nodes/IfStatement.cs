using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class IfStatement : LogicalStatement 
{
    public readonly bool IsParenthesized;

    public readonly ExpressionNode Expression;
    //reference to the first body token
    public readonly BodyBound Body;
    public readonly ElseStatement? ElseStatement;
    public IfStatement(ExpressionNode exp, BodyBound body, ElseStatement? elseStatement, bool isParenthesized)
    {
        Expression = exp;
        Body = body;
        ElseStatement = elseStatement;
        IsParenthesized = isParenthesized;
    }
    public static new IfStatement Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.IfStatementToken)) 
        {
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }
        offset++;
        bool parenthesized = parser.SaveEquals(0, SyntaxKind.OpenParentheseToken);
        if (parenthesized) offset++;
        ExpressionNode expression = ExpressionNode.Parse(parser);
        if (parenthesized) 
        {
            if (!parser.SaveEquals(0, SyntaxKind.CloseParentheseToken))
                Error.Execute(parser.logger, ErrorDefaults.OutOfScope, parser.syntaxTokens[offset].Line);
            offset++;
        }
        BodyBound body = BodyBound.Parse(parser);

        ElseStatement elseStatement = parser.SaveEquals(0, SyntaxKind.ElseStatementToken) 
            ? ElseStatement.Parse(parser) : null!;

        return new IfStatement(expression, body, elseStatement, parenthesized);
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Expression;
        yield return Body;
        // only else element is child and not the other way around
        if (ElseStatement != null)
            yield return ElseStatement!;
    }
}
public abstract class LogicalStatement : SyntaxNode
{

}