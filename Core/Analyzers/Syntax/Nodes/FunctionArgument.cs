namespace Fluxcp;
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
}