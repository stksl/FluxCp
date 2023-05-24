using System.Collections.Generic;
namespace Fluxcp.Tests;

public class LexerTest 
{
    [Fact]
    public unsafe void Lexer_TEST_MAIN() 
    {
        string[] tests = 
        {
            "i32 myInt = 10902;",

            "use FCP.Main;",

            "byte GetByte(bool someArg)\n" +
            "{return SomeFunc(someArg);}",


        };
        SyntaxToken[][] testsTokens = new SyntaxToken[tests.Length][];
        testsTokens[0] = new SyntaxToken[] 
        {
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.TextToken, Offset = 0, Length = 3},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 3, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.TextToken, Offset = 4, Length = 5},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 9, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.EqualsToken, Offset = 10, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 11, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.NumberToken, Offset = 12, Length = 5},
            new SyntaxToken(SourceText.FromString(tests[0]), 0) {Kind = SyntaxKind.SemicolonToken, Offset = 17, Length = 1},
        };
        testsTokens[1] = new SyntaxToken[] 
        {
            new SyntaxToken(SourceText.FromString(tests[1]), 0) {Kind = SyntaxKind.UseStatementToken, Offset = 0, Length = 3},
            new SyntaxToken(SourceText.FromString(tests[1]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 3, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[1]), 0) {Kind = SyntaxKind.TextToken, Offset = 4, Length = 3},
            new SyntaxToken(SourceText.FromString(tests[1]), 0) {Kind = SyntaxKind.DotToken, Offset = 7, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[1]), 0) {Kind = SyntaxKind.TextToken, Offset = 8, Length = 4},
            new SyntaxToken(SourceText.FromString(tests[1]), 0) {Kind = SyntaxKind.SemicolonToken, Offset = 12, Length = 1},
        };
        testsTokens[2] = new SyntaxToken[] 
        {
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.TextToken, Offset = 0, Length = 4},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 4, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.TextToken, Offset = 5, Length = 7},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.OpenParentheseToken, Offset = 12, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.TextToken, Offset = 13, Length = 4},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 17, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.TextToken, Offset = 18, Length = 7},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.CloseParentheseToken, Offset = 25, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.EndOfLineToken, Offset = 26, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.OpenBraceToken, Offset = 27, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.ReturnStatementToken, Offset = 28, Length = 6},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.WhitespaceToken, Offset = 34, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.TextToken, Offset = 35, Length = 8},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.OpenParentheseToken, Offset = 43, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.TextToken, Offset = 44, Length = 7},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.CloseParentheseToken, Offset = 51, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.SemicolonToken, Offset = 52, Length = 1},
            new SyntaxToken(SourceText.FromString(tests[2]), 0) {Kind = SyntaxKind.CloseBraceToken, Offset = 53, Length = 1},


        };
        for(int i = 0; i < tests.Length; i++) 
        {
            SourceText sourceText = SourceText.FromString(tests[i]);
            CompilationUnit compilationUnit = new CompilationUnit();
            List<SyntaxToken> syntaxTokens = new List<SyntaxToken>();
            Lexer lexer = new Lexer(sourceText, null, compilationUnit);
            while (!lexer.EndOfFile()) 
            {
                syntaxTokens.Add(lexer.Lex());
            }
            for(int j = 0; j < testsTokens[i].Length; j++) 
            {
                if (syntaxTokens[j] != testsTokens[i][j]) 
                {
                    System.Console.WriteLine(syntaxTokens[j]);
                    System.Console.WriteLine(testsTokens[i][j]);
                }
                Assert.True(syntaxTokens[j] == testsTokens[i][j]);
            }
        }
    }
}