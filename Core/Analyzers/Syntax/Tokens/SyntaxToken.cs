namespace Fluxcp;
// Base class for all of the syntax tokens
public class SyntaxToken : ICloneable
{
    public SyntaxKind Kind;
    public readonly SourceText? Text;
    public int Offset;
    public int Length;
    public SyntaxToken(SourceText? text)
    {
        Text = text;
    }
    public virtual object Clone() 
    {
        return new SyntaxToken(Text) {Kind = this.Kind, Offset = this.Offset, Length = this.Length };
    }
    public override string ToString()
    {
        return $"\\{Kind}\\ At position: {Offset} with length {Length}: \n";
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(Kind, Offset, Length, Text);
    }
    public override bool Equals(object? obj)
    {
        if (obj is not SyntaxToken syntaxToken) return false;
        return syntaxToken.Kind == Kind && Offset == syntaxToken.Offset
               && Length == syntaxToken.Length;
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