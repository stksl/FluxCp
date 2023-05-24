namespace Fluxcp;
public sealed class ReturnStatement : SyntaxNode 
{
    public object? Value;
    public ReturnStatement(object? value)
    {
        Value = value;
    }
    public override IEnumerable<SyntaxNode> GetChildren() 
    {
        return Array.Empty<SyntaxNode>();
    }
}