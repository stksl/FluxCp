using Fluxcp.Syntax;
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
    public static string Print(this SyntaxNode node, string nesting = "") 
    {
        if (node == null) return string.Empty;

        System.Text.StringBuilder sb = new System.Text.StringBuilder("\n" + nesting + node.GetType().Name);
        foreach(SyntaxNode child in node.GetChildren()) sb.Append(Print(child, nesting + "\t"));
        if (node is not SyntaxTree.ProgramBound)
            sb.Append(Print(node.Next!, nesting));
        return sb.ToString();
    }
}