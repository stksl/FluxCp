namespace Fluxcp.Syntax;
public sealed class StructField : SyntaxNode 
{
    public readonly string Name;
    public readonly DataType DataType;
    public StructField(string name, DataType dataType)
    {
        Name = name;
        DataType = dataType;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new StructField Parse(Parser parser) 
    {
        parser.offset += 3; // += 3 because this should be called only after you make sure that structure is:
        // TextToken, TextToken, SemicolonToken
        return new StructField(parser.syntaxTokens[parser.offset - 2].PlainValue, DataType.FromName(parser.syntaxTokens[parser.offset - 3].PlainValue));
    }
}