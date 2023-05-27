namespace Fluxcp.Syntax;
public sealed class FunctionArgument : SyntaxNode
{
    public readonly string Name;
    public readonly DataType DataType;
    public FunctionArgument(string name, DataType typeId)
    {
        Name = name;
        DataType = typeId;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new FunctionArgument Parse(Parser parser) 
    {
        parser.offset += 2;
        return new FunctionArgument(parser.syntaxTokens[parser.offset - 1].PlainValue, DataType.FromName(parser.syntaxTokens[parser.offset - 2].PlainValue));
    }
}