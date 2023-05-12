namespace Fluxcp;
// Base class for all of the syntax tokens
public class SyntaxToken 
{
    public readonly SyntaxKind Kind;
    public SyntaxToken(SyntaxKind kind)
    {
        Kind = kind;
    }
    public override string ToString()
    {
        return Kind.ToString();
    }
    public override int GetHashCode()
    {
        return Kind.GetHashCode();
    }
    public override bool Equals(object? obj)
    {
        if (obj is not SyntaxToken syntaxToken) return false;
        return syntaxToken.Kind == Kind;
    }
    public static bool operator ==(SyntaxToken left, SyntaxToken right) 
    {
        return left.Equals(right);
    } 
    public static bool operator !=(SyntaxToken left, SyntaxToken right) 
    {
        return !left.Equals(right);
    }
}