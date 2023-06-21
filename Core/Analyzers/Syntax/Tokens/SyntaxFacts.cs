namespace Fluxcp.Syntax;
public static class SyntaxFacts 
{
    public static bool IsKeyword(SyntaxKind kind) 
    {
        return (uint)kind - byte.MaxValue <= 9;
    }
    public static bool IsTrivia(SyntaxKind kind) 
    {
        switch(kind) 
        {
            case SyntaxKind.WhitespaceToken:
            case SyntaxKind.EndOfLineToken:
            case SyntaxKind.CommentToken:
                return true;
            default: return false;
        }
    }
    public static bool IsOperand(SyntaxKind kind) 
    {
        return kind == SyntaxKind.NumberToken || kind == SyntaxKind.TextToken;
    }
    public static bool IsMathOperator(SyntaxKind kind) 
    {
        switch(kind) 
        {
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.StarToken:
            case SyntaxKind.SlashToken:
            case SyntaxKind.OpenParentheseToken:
            case SyntaxKind.CloseParentheseToken:
                return true;
            default: return false;
        }
    }
    public static bool IsLogicalOperator(SyntaxKind kind, bool checkOrAnd = true) 
    {
        switch(kind) 
        {
            case SyntaxKind.IsLessToken:
            case SyntaxKind.IsMoreToken:
            case SyntaxKind.IsEqualToken:
                return true;
            default: return (kind == SyntaxKind.LogicalOrToken || kind == SyntaxKind.LogicalAndToken) && checkOrAnd;
        }
    }
    public static bool IsOperator(SyntaxKind kind) => IsMathOperator(kind) || IsLogicalOperator(kind);
}