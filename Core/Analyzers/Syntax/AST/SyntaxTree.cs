namespace Fluxcp;
// AST (Abstract Syntax Tree)
public class SyntaxTree 
{
    public readonly SyntaxNode Root;
    public SyntaxTree(SyntaxNode root)
    {
        Root = root;
    }
}