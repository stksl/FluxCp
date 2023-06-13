namespace Fluxcp.Syntax;
public abstract class LogicalStatement : SyntaxNode
{
    public readonly ExpressionNode Condition;
    public LogicalStatement(ExpressionNode condition)
    {
        Condition = condition;
    }
}