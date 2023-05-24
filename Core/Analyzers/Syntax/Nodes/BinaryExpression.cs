namespace Fluxcp;
public sealed class BinaryExpression : ExpressionNode
{
    public BinaryExpression(VariableValue left, VariableValue right, SyntaxKind operatorKind)
    {
        Left = left;
        Right = right;
        OperatorKind = operatorKind;
    }
    public VariableValue Left;
    public VariableValue Right;
    public SyntaxKind OperatorKind;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}