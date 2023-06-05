namespace Fluxcp.Syntax;
public sealed class BinaryExpression : ExpressionNode
{
    public BinaryExpression(VariableValue left, VariableValue right, SyntaxKind operatorKind, int priority)
    {
        Left = left;
        Right = right;
        OperatorKind = operatorKind;
        Priority = priority;
    }
    public BinaryExpression()
    {
        Left = null!; Right = null!;
    }
    // variable value can be an expression
    public VariableValue Left;
    public VariableValue Right;
    public int Priority;
    public SyntaxKind OperatorKind;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public override string ToString()
    {
        string left = (Left is BinaryExpression ? Left.ToString() : Left.GetType().Name)!;
        string right = (Right is BinaryExpression ? Right.ToString() : Right.GetType().Name)!;
        return $"{left} {OperatorKind} {right}";
    }
}