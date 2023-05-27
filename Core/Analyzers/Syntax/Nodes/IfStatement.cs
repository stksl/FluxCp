namespace Fluxcp.Syntax;
public sealed class IfStatement : LogicalStatement 
{
    public IfStatement(ExpressionNode exp, SyntaxNode body, ElseStatement? elseStatement)
    {
        Expression = exp;
        Body = body;
        ElseStatement = elseStatement;
    }
    public ExpressionNode Expression;
    //pointer to the first body token
    public SyntaxNode Body;
    public ElseStatement? ElseStatement;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        throw new NotImplementedException();
    }
}
public abstract class LogicalStatement : SyntaxNode
{
    public LogicalStatement()
    {
        
    }
}