using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public abstract class ExpressionNode : VariableValue
{
    public ExpressionNode() : base(false)
    {

    }
    /* 
    for now only binary expressions are supported.
    valid binary expression is an expression which contains 2 operands and 1 operator.
    Operands can be binary expressions as well 
    */
    // isSingle - does current exp contains only 1 operand (meaning that lvalue is defined)
    public static new ExpressionNode Parse(Parser parser) => Parse(parser, SyntaxKind.BackSlashToken);
    public static ExpressionNode Parse(Parser parser, SyntaxKind bound)
    {
        ref int offset = ref parser.offset;
        offset++;

        VariableValue left = ParseTerm(parser);

        while (parser.SaveEquals(0, node => node.Kind != bound))
        {
            if (SyntaxFacts.IsOperator(parser.syntaxTokens[offset].Kind))
            {
                SyntaxKind op = parser.syntaxTokens[offset].Kind;
                offset++;
                VariableValue right = ParseTerm(parser);
                left = new BinaryExpression(left, right, op, 0);
            }
            else
            {
                Error.Execute(parser.logger, ErrorDefaults.UnknownReference, parser.syntaxTokens[offset].Line);
            }
        }
        offset++;
        return (BinaryExpression)left;
    }

    private static VariableValue ParseTerm(Parser parser)
    {
        ref int offset = ref parser.offset;
        VariableValue left = ParseFactor(parser);

        while (parser.SaveEquals(0, node => SyntaxFacts.IsOperator(node.Kind)))
        {
            SyntaxKind op = parser.syntaxTokens[offset].Kind;
            switch (op)
            {
                case SyntaxKind.CloseParentheseToken:
                case SyntaxKind.LogicalOrToken:
                case SyntaxKind.LogicalAndToken:
                    goto _return;
            }
            if (SyntaxFacts.IsOperator(op))
            {
                offset++;
                VariableValue right = ParseFactor(parser);
                left = new BinaryExpression(left, right, op, 0);
            }
            else
            {
                Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
            }
        }
    _return:
        return left;
    }

    private static VariableValue ParseFactor(Parser parser)
    {
        ref int offset = ref parser.offset;

        if (parser.SaveEquals(0, SyntaxKind.OpenParentheseToken))
        {
            BinaryExpression expression = (BinaryExpression)Parse(parser, SyntaxKind.CloseParentheseToken);
            return expression;
        }
        return VariableValue.Parse(parser, true);
    }
}