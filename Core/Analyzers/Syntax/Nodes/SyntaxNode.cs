using System.Collections;

namespace Fluxcp;
// Base class for all of the syntax nodes. Works like doubly-linked list.
public abstract class SyntaxNode
{
    public SyntaxNode()
    {
    }
    public abstract IEnumerable<SyntaxNode> GetChildren();
    // next node
    public SyntaxNode? Next { get; internal set; }
    // previous node
    public SyntaxNode? Prev { get; internal set; }
    
}