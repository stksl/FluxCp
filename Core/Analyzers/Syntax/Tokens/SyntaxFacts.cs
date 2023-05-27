namespace Fluxcp.Syntax;
public static class SyntaxFacts 
{
    public static bool IsKeyword(SyntaxKind kind) 
    {
        return (uint)kind - byte.MaxValue <= 6;
    }
    public static bool IsTrivia(SyntaxKind kind) 
    {
        switch(kind) 
        {
            case SyntaxKind.WhitespaceToken:
            case SyntaxKind.EndOfLineToken:
            case SyntaxKind.CommentToken:
                return true;
            default:
                return false;
        }
    }
}