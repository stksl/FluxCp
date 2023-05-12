namespace Fluxcp;
public class NumberToken : SyntaxToken 
{
    public readonly int Value;
    public NumberToken(int value) : base(SyntaxKind.NumberToken)
    {
        Value = value;
    }
}