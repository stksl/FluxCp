using System.Collections.Immutable;
namespace Fluxcp.Syntax;
public sealed class LiteralValue : VariableValue
{
    public readonly LiteralType LiteralType;
    public readonly IList<SyntaxToken> Literal;
    
    public LiteralValue(IList<SyntaxToken> literal, LiteralType type)
    {
        Literal = literal;
        LiteralType = type;
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
        if (parser.SaveEquals(0, SyntaxKind.NumberToken))
        {
            literalValue = new LiteralValue(new SyntaxToken[] {parser.syntaxTokens[offset++]}, LiteralType.Number); // for now without floating-point numbers
        }
        else if (parser.SaveEquals(0, node => node.Kind == SyntaxKind.TrueToken || node.Kind == SyntaxKind.FalseToken)) 
        {
            literalValue = new LiteralValue(new SyntaxToken[] {parser.syntaxTokens[offset++]}, LiteralType.Boolean); // for now without floating-point numbers
        }
        // string literal
        else if (parser.SaveEquals(0, SyntaxKind.DoubleQuotesToken))
        {
            int prev = offset;
            List<SyntaxToken> literal = new List<SyntaxToken>();
            literal.Add(parser.syntaxTokens[offset++]); // double quote
            while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.DoubleQuotesToken))
            {
                if (parser.SaveEquals(0, SyntaxKind.BackSlashToken)) 
                    offset++;
                literal.Add(parser.syntaxTokens[offset++]);


            }
            literal.Add(parser.syntaxTokens[offset++]); // double quote
            literalValue = new LiteralValue(literal, LiteralType.String);
        }
        // character literal
        else if (parser.SaveEquals(0, SyntaxKind.SingleQuoteToken) && parser.SaveEquals(2, SyntaxKind.SingleQuoteToken))
        {
            literalValue = new LiteralValue(new SyntaxToken[] {parser.syntaxTokens[offset], 
            parser.syntaxTokens[offset + 1], 
            parser.syntaxTokens[offset + 2]}, LiteralType.Character);
            offset += 3; // skipping character literals and value
        }
        return literalValue;
    }
}
public enum LiteralType 
{
    Unknown = 0,
    Number,
    String,
    Character,
    Boolean
}