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
    public static void Print(this SyntaxNode node, ILogger logger) 
    {
        void print(SyntaxNode syntaxNode, string nested) 
        {
            logger.ShowDebug(nested + syntaxNode.GetType().Name);
            foreach(SyntaxNode child in syntaxNode.GetChildren()) print(child, nested + "\t");

            if (syntaxNode.Next != null && syntaxNode.Next != node.Next)
                print(syntaxNode.Next, nested);
        }

        print(node, "");
    }
}