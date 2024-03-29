﻿using System.Text;
using Fluxcp.Syntax;

namespace Fluxcp;
// Generates tokens

public sealed class Lexer
{
    private readonly SourceText text;
    private readonly CompilationUnit cUnit;
    
    private int position;
    private object _sync = new object();
    private int currLine = 1;
    private SyntaxToken current;
    public SyntaxToken Current => (SyntaxToken)current.Clone();

    // base language keywords
    private readonly (string, SyntaxKind)[] BaseKeywords = 
    {
        ("use", SyntaxKind.UseStatementToken),
        ("if", SyntaxKind.IfStatementToken),
        ("else", SyntaxKind.ElseStatementToken),
        ("for", SyntaxKind.ForStatementToken),
        ("while", SyntaxKind.WhileStatementToken),
        ("return", SyntaxKind.ReturnStatementToken),
        ("struct", SyntaxKind.StructDefineToken),
        ("importlib", SyntaxKind.ImportLibToken),
        ("skip", SyntaxKind.SkipToken),
        ("true", SyntaxKind.TrueToken),
        ("false", SyntaxKind.FalseToken)
    };
    public unsafe Lexer(SourceText tr, CompilationUnit unit)
    {
        text = tr;
        position = 0;
        current = new SyntaxToken(text, 0) { Kind = SyntaxKind.StartOfFileToken, Length = 0, Offset = 0 };
        cUnit = unit;
    }
    private SyntaxToken ChangeCurr(SyntaxKind kind, int length, bool changeOffset = true) 
    {
        current = new SyntaxToken(text, currLine) {Kind = kind, Offset = position, Length = length};
        if (changeOffset) position += length;
        return Current;
    }
    private bool SaveEquals(int offset, char val) 
    {
        return position + offset < text.Length && text[position + offset] == val;
    }
    private bool SaveEquals(int offset, Func<char, bool> funcPred) 
    {
        return position + offset < text.Length && funcPred(text[position + offset]);
    }
    // analyze next token
    public SyntaxToken Lex()
    {
        lock (_sync)
        {
            return LexNode();
        }
    }
    private SyntaxToken LexNode()
    {
        if (position >= text.Length) 
        {
            if (current.Kind != SyntaxKind.EndOfFileToken) ChangeCurr(SyntaxKind.EndOfFileToken, 0);
            else cUnit.Logger?.ShowDebug("Lexing a finished text");
            return Current;
        }
        switch (text[position])
        {
            case ';':
                return ChangeCurr(SyntaxKind.SemicolonToken, 1);
            case '.':
                return ChangeCurr(SyntaxKind.DotToken, 1);
            case ',':
                return ChangeCurr(SyntaxKind.CommaToken, 1);

            case >= '0' and <= '9':
                return LexNumber();
        }
        // pipeline
        return ParseTrivia();
    }
    private SyntaxToken ParseTrivia() 
    {
        switch(text[position]) 
        {
            case '#' when SaveEquals(1, '#'):
                int prev = position;
                position += 2;
                while (SaveEquals(0, ch => ch != '#') || SaveEquals(1, ch => ch != '#')) position++;
                position += 2;
                return ChangeCurr(SyntaxKind.CommentToken, position - prev, false);
            case ' ':
                return ChangeCurr(SyntaxKind.WhitespaceToken, 1);
            case '\n':
                currLine++;
                return ChangeCurr(SyntaxKind.EndOfLineToken, 1);

        }

        //pipeline
        return LexBaseOperators();
    }
    private SyntaxToken LexBaseOperators() 
    {
        switch (text[position]) 
        {
            case '=':
                // if its logical 'is equal'
                if (SaveEquals(1, '=')) 
                {
                    return ChangeCurr(SyntaxKind.IsEqualToken, 2);
                }
                return ChangeCurr(SyntaxKind.EqualsToken, 1);

            case '(':
                return ChangeCurr(SyntaxKind.OpenParentheseToken, 1);
            case ')':
                return ChangeCurr(SyntaxKind.CloseParentheseToken, 1);
            case '{':
                return ChangeCurr(SyntaxKind.OpenBraceToken, 1);
            case '}':
                return ChangeCurr(SyntaxKind.CloseBraceToken, 1);
            case '[':
                return ChangeCurr(SyntaxKind.OpenBracketToken, 1);
            case ']':
                return ChangeCurr(SyntaxKind.CloseBracketToken, 1);

            case '+':
                return ChangeCurr(SyntaxKind.PlusToken, 1);
            case '-':
                if (SaveEquals(1, char.IsDigit)) 
                {
                    return LexNumber();
                }
                return ChangeCurr(SyntaxKind.MinusToken, 1);
            case '*':
                return ChangeCurr(SyntaxKind.StarToken, 1);
            case '/':
                return ChangeCurr(SyntaxKind.SlashToken, 1);
            case '\\':
                return ChangeCurr(SyntaxKind.BackSlashToken, 1);
            case '%':
                return ChangeCurr(SyntaxKind.PercentToken, 1);

            // actually literals, but i put them in here
            case '\'':
                return ChangeCurr(SyntaxKind.SingleQuoteToken, 1);
            case '"':
                return ChangeCurr(SyntaxKind.DoubleQuotesToken, 1);
            case '_':
                return ChangeCurr(SyntaxKind.UnderscoreToken, 1);
            case ':':
                if (SaveEquals(1, ':'))
                    return ChangeCurr(SyntaxKind.CastToken, 2);
                return ChangeCurr(SyntaxKind.ColonToken, 1);
        }
        // pipeline
        return LexLogicalOperators();
    }
    private SyntaxToken LexLogicalOperators() 
    {
        switch(text[position]) 
        {
            case '!' when SaveEquals(1, '='):
                return ChangeCurr(SyntaxKind.IsNotEqualToken, 2);
            case '&':
                if (SaveEquals(1, '&')) position++;
                return ChangeCurr(SyntaxKind.LogicalAndToken, 1);
            case '|':
                if (SaveEquals(1, '|')) position++;
                return ChangeCurr(SyntaxKind.LogicalOrToken, 1);
            case '<':
                return ChangeCurr(SyntaxKind.IsLessToken, 1);
            case '>':
                return ChangeCurr(SyntaxKind.IsMoreToken, 1);
        }
        //pipeline
        return LexKeywords();
    }
    private SyntaxToken LexKeywords() 
    {
        for(int i = 0; i < BaseKeywords.Length; i++) 
        {
            int keywordLength = BaseKeywords[i].Item1.Length;
            if (position + keywordLength < text.Length && text.ToString(position, keywordLength) == BaseKeywords[i].Item1)
            {
                if (SaveEquals(keywordLength, char.IsLetterOrDigit)) return LexUnkownText();
                return ChangeCurr(BaseKeywords[i].Item2, keywordLength);
            }
        }
        //pipeline
        return LexUnkownText();
    }
    // Lexing other stuff that is primarly a text. End of a pipeline
    private SyntaxToken LexUnkownText() 
    {
        StringBuilder sb = new StringBuilder();
        if (SaveEquals(0, char.IsLetter)) 
        {
            int length = 0, offset = position;
            while (SaveEquals(length++, (ch) => char.IsLetterOrDigit(ch) || ch == '_')) 
            {
                sb.Append(text[offset++]);
            }
            length--; // cuz we're on next position (not char.IsLetter)
            return ChangeCurr(SyntaxKind.TextToken, length);
        }

        return ChangeCurr(SyntaxKind.BadToken, 1);
    }
    private SyntaxToken LexNumber() 
    {
        int length = 0;
        if (SaveEquals(0, '-')) length++; // if negative number
        while (SaveEquals(length++, char.IsDigit)) {}
        length--; // cuz on current length offset is not a number
        return ChangeCurr(SyntaxKind.NumberToken, length);
    }
    public bool EndOfFile()
    {
        return current.Kind == SyntaxKind.EndOfFileToken;
    }
    public sealed class LeadingTrivia 
    {
        private readonly Dictionary<SyntaxToken, int> Trivia;
        public LeadingTrivia(Dictionary<SyntaxToken, int> trivia)
        {
            Trivia = trivia;
        }
    }
}