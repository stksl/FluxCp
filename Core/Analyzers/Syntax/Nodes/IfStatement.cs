using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class IfStatement : LogicalStatement 
{
    //reference to the first body token
    public readonly BodyBound Body;
    public readonly ElseStatement? ElseStatement;
    public IfStatement(ExpressionNode exp, BodyBound body, ElseStatement? elseStatement)
    :base(exp)
    {
        Body = body;
        ElseStatement = elseStatement;
    }
    public static new IfStatement Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.IfStatementToken)) 
        {
            Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }
        offset++;
        ExpressionNode expression = ExpressionNode.Parse(parser);
        BodyBound body = BodyBound.Parse(parser);

        ElseStatement? elseStatement = null;
        if (parser.SaveEquals(0, SyntaxKind.ElseStatementToken))
            elseStatement = ElseStatement.Parse(parser); 

        return new IfStatement(expression, body, elseStatement);
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Condition;
        yield return Body;
        // only else element is child and not the other way around
        if (ElseStatement != null)
            yield return ElseStatement!;
    }
}