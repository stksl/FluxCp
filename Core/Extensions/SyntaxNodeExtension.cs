namespace Fluxcp;
public static class SyntaxNodeExtension 
{
    public static SyntaxNode? Find(this SyntaxNode node, Func<SyntaxNode, bool> filter) 
    {
        while (node != null) 
        {
            if (filter(node)) break;
            node = node.Next!;
        }
        return node;
    }
}