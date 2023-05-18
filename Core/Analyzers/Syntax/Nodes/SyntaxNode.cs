namespace Fluxcp;
// Base class for all of the syntax nodes
public abstract class SyntaxNode
{
    public SyntaxNode()
    {
    }
    public virtual object? Emit() 
    {
        return null;
    }
}