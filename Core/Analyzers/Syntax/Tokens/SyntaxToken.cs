namespace Fluxcp;
// Base class for all of the syntax tokens
public class SyntaxToken : ICloneable
{
    public SyntaxKind Kind;
    public readonly SourceText? Text;
    public readonly int Line;
    public int Offset;
    public int Length;
    public string PlainValue => Text!.ToString(Offset, Length);
    public SyntaxToken(SourceText? text, int line)
    {
        Text = text;
        Line = line;
    }
    public virtual object Clone() 
    {
        return new SyntaxToken(Text, Line) {Kind = this.Kind, Offset = this.Offset, Length = this.Length };
    }
    public override string ToString()
    {
        return $"\\{Kind}\\ At line: {Line} with length {Length}: \n" + PlainValue;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Kind, Offset, Length, Text, Line);
    }
    public override bool Equals(object? obj)
    {
        if (obj is not SyntaxToken syntaxToken) return false;
        return syntaxToken.Kind == Kind 
               && Offset == syntaxToken.Offset
               && Length == syntaxToken.Length 
               && Line == syntaxToken.Line;
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