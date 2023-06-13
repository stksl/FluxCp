
namespace Fluxcp.Syntax;
public sealed class CopyValue : VariableValue
{
    public readonly string FromVar;
    public CopyValue(string fromVar)
    {
        FromVar = fromVar;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new CopyValue Parse(Parser parser) 
    {
        if (!parser.SaveEquals(0, SyntaxKind.TextToken))
            return null!;
        
        string fromVar = parser.syntaxTokens[parser.offset++].PlainValue;
        return new CopyValue(fromVar);
    }
}