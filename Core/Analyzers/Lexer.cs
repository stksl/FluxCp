namespace Fluxcp;
// Generates tokens
public sealed class Lexer
{
    private readonly SourceText text;
    private int position;
    private SyntaxToken current;
    public SyntaxToken Current => current;
    private object _sync = new object();
    public unsafe Lexer(SourceText tr)
    {
        text = tr;
        position = 0;
        current = new SyntaxToken(SyntaxKind.StartOfFileToken);
    }
    // analyze next token
    public SyntaxToken Lex()
    {
        lock (_sync)
        {
            LexNode();
        }
        return current;
    }
    private SyntaxToken LexNode()
    {
        if (EndOfFile()) {
            if (current.Kind != SyntaxKind.EndOfFileToken) current = new SyntaxToken(SyntaxKind.EndOfFileToken);
            return current;
        } 
        switch (text[position])
        {
            case ' ':
                current = new SyntaxToken(SyntaxKind.WhitespaceToken);
                position++;
                break;
            case >= '0' and <= '9':
                LexNumber();
                break;
            case '\n':
                current = new SyntaxToken(SyntaxKind.EndOfLineToken);
                position++;
                break;
            default:
                LexOperators();
                break;
        }
        return current;
    }
    private SyntaxToken LexNumber()
    {
        current = new NumberToken(GetNumber());

        return current;
    }
    private SyntaxToken LexOperators() 
    {
        switch (text[position++]) 
        {
            case '(':
                current = new SyntaxToken(SyntaxKind.OpenParentheseToken);
                break;
            case ')':
                current = new SyntaxToken(SyntaxKind.CloseParentheseToken);
                break;

            case '+':
                current = new SyntaxToken(SyntaxKind.PlusToken);
                break;
            case '-':
                if (!EndOfFile() && char.IsDigit(text[position])) 
                {
                    LexNumber();
                }
                else current = new SyntaxToken(SyntaxKind.MinusToken);
                break;
            case '*':
                current = new SyntaxToken(SyntaxKind.StarToken);
                break;
            case '/':
                current = new SyntaxToken(SyntaxKind.SlashToken);
                break;
            case '%':
                current = new SyntaxToken(SyntaxKind.PercentToken);
                break;
            default:

                break;
        }
        return current;
    }
    private int GetNumber()
    {
        string value = "";
        if (text[position] == '-') value = "-";
        while (!EndOfFile() && char.IsDigit(text[position]))
        {
            value += text[position++];
        }
        return int.Parse(value);

    }
    public bool EndOfFile()
    {
        return position >= text.Length;
    }
}