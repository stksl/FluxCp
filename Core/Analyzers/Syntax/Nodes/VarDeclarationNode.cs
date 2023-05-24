using System.Collections.Immutable;
namespace Fluxcp;
public sealed class VarDeclarationNode : SyntaxNode
{
    // variable access level
    public readonly int Level;
    public readonly string Name;
    public readonly DataType DataType;
    public VariableValue? Value;
    public VarDeclarationNode(string name, int level, DataType typeId)
    {
        Level = level;
        Name = name;
        DataType = typeId;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Value!;
    }
}
public abstract class VariableValue : SyntaxNode
{
}
public sealed class LiteralValue : VariableValue 
{
    public readonly ImmutableArray<SyntaxToken> Literal;
    public readonly int Position;
    public readonly int Length;
    public LiteralValue(ImmutableArray<SyntaxToken> literal, int pos, int length)
    {
        Literal = literal;
        Position = pos;
        Length = length;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}
public sealed class CopyValue : VariableValue 
{
    public VariableValue CopyFrom;
    public CopyValue(VariableValue from)
    {
        CopyFrom = from;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}
public sealed class RefValue : VariableValue 
{
    public VariableValue ReferenceTo;
    public RefValue(VariableValue refTo)
    {
        ReferenceTo = refTo;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}