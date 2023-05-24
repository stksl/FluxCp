namespace Fluxcp;
public sealed class FunctionBodyBound : SyntaxNode 
{
    // start of the body (can be used as syntax tokens offset)
    public readonly int Position;
    // length of the body
    public int Length {get; internal set;}
    public SyntaxNode Child;
    public FunctionBodyBound(int position)
    {
        Position = position;
        Child = SyntaxNode.Empty;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Child!;
    }
}