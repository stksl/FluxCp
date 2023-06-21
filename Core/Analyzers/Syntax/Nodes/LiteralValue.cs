using System.Collections.Immutable;
namespace Fluxcp.Syntax;
public sealed class LiteralValue : VariableValue
{
    public readonly IList<SyntaxToken> Literal;
    
    public LiteralValue(IList<SyntaxToken> literal)
    {
        Literal = literal;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new LiteralValue? Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        LiteralValue? literalValue = null;
        // number or bool literal (1 token)
        if (parser.SaveEquals(0, SyntaxKind.NumberToken) || 
            parser.SaveEquals(0, node => node.Kind == SyntaxKind.TrueToken || node.Kind == SyntaxKind.FalseToken))
        {
            literalValue = new LiteralValue(new SyntaxToken[] {parser.syntaxTokens[offset++]}); // for now without floating-point numbers
        }
        // string literal
        else if (parser.SaveEquals(0, SyntaxKind.DoubleQuotesToken))
        {
            int prev = offset;
            List<SyntaxToken> literal = new List<SyntaxToken>();
            literal.Add(parser.syntaxTokens[offset++]); // double quote
            while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.DoubleQuotesToken))
            {
                literal.Add(parser.syntaxTokens[offset++]);
            }
            literal.Add(parser.syntaxTokens[offset++]); // double quote
            literalValue = new LiteralValue(literal);
        }
        // character literal
        else if (parser.SaveEquals(0, SyntaxKind.SingleQuoteToken) && parser.SaveEquals(2, SyntaxKind.SingleQuoteToken))
        {
            literalValue = new LiteralValue(new SyntaxToken[] {parser.syntaxTokens[offset], 
            parser.syntaxTokens[offset + 1], 
            parser.syntaxTokens[offset + 2]});
            offset += 3; // skipping character literals and value
        }
        return literalValue;
    }
}