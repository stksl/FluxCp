namespace Fluxcp;
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
}