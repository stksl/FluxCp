namespace Fluxcp;
public sealed class VarDeclarationNode : SyntaxNode
{
    // variable access level
    public readonly uint Level;
    public readonly string Name;
    public readonly DataType DataType;
    public object? Value;

    public VarDeclarationNode(string name, uint level, DataType typeId)
    {
        Level = level;
        Name = name;
        DataType = typeId;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return null!;
    }
}