using System.Collections;

namespace Fluxcp.Syntax;
// Base class for all of the syntax nodes. Works like doubly-linked list.
public abstract class SyntaxNode
{
    public SyntaxNode()
    {
    }
    // empty syntax node
    public static SyntaxNode Empty => new EmptySyntaxNode();
    private sealed class EmptySyntaxNode : SyntaxNode 
    {
        public override IEnumerable<SyntaxNode> GetChildren()
        {
            return Array.Empty<SyntaxNode>();
        }
    }
    public abstract IEnumerable<SyntaxNode> GetChildren();
    public static SyntaxNode Parse(Parser parser) 
    {
        return null!;
    }
    // next node
    public SyntaxNode? Next { get; internal set; }
    // previous node
    public SyntaxNode? Prev { get; internal set; }
    
}