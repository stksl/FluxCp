namespace Fluxcp;
public sealed class BinaryExpression : ExpressionNode
{
    public BinaryExpression(BinaryOperand left, BinaryOperand right, SyntaxKind operatorKind)
    {
        Left = left;
        Right = right;
        OperatorKind = operatorKind;
    }
    public BinaryOperand Left;
    public BinaryOperand Right;
    public SyntaxKind OperatorKind;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        throw new NotImplementedException();
    }
}
public class BinaryOperand 
{
    // whether its a literal operand ("", 123, 1.5, ' ').
    public bool IsLiteral;
    public object? Value;
    public DataType? DataType;
}