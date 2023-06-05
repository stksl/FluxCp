using System.Collections.Immutable;
namespace Fluxcp.Syntax;
public sealed class LiteralValue : VariableValue
{
    public readonly ImmutableArray<SyntaxToken> Literal;
    public readonly int Position;
    public readonly int Length;
    public LiteralValue(ImmutableArray<SyntaxToken> literal, int pos, int length) : base(false)
    {
        Literal = literal;
        Position = pos;
        Length = length;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new LiteralValue Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        LiteralValue literalValue = null!;
        // number or bool literal (1 token)
        if (parser.SaveEquals(0, SyntaxKind.NumberToken) || 
            parser.SaveEquals(0, node => node.Kind == SyntaxKind.TrueToken || node.Kind == SyntaxKind.FalseToken))
        {
            literalValue = new LiteralValue(parser.syntaxTokens, offset++, 1); // for now without floating-point numbers
        }
        // string literal
        else if (parser.SaveEquals(0, SyntaxKind.DoubleQuotesToken))
        {
            int prev = offset;
            while (parser.SaveEquals(1, node => node.Kind != SyntaxKind.DoubleQuotesToken))
            {
                offset++;
            }
            offset += 2; // skipping double quote token
            literalValue = new LiteralValue(parser.syntaxTokens, prev, (offset - 1) - prev);
        }
        // character literal
        else if (parser.SaveEquals(0, SyntaxKind.SingleQuoteToken) && parser.SaveEquals(2, SyntaxKind.SingleQuoteToken))
        {
            offset += 3; // skipping character literals and value
            literalValue = new LiteralValue(parser.syntaxTokens, offset, 2);
        }
        return literalValue;
    }
}