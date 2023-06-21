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
    // variable value can be an expression
    public readonly VariableValue Left;
    public readonly VariableValue Right;
    public readonly int Priority;
    public readonly  SyntaxKind OperatorKind;
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