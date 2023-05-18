using System.Collections.Generic;
namespace Fluxcp.Tests;

public class ParserTest
{
    [Fact]
    public void EvaluateExpression_TEST()
    {
        /* string[] srcs = {"3 + 2",
        "7 * (4 - 2)",
        "12 / 6 + 8",
        "5 * (7 - 3) + 2",
        "(9 - 3) / 2 + 4 * 5",
        "10 - 4 + 3 * (8 + 2)",
        "16 / (4 - 2) + 5 * (7 - 3)",
        "2 * ((5 + 3) / (4 - 2))",
        "7 * (6 + 3) - 2 / (8 - 6)",
        "(12 - 4) / (3 + 1) * (6 + 2) - 5"};
        int[] results = {5, 14, 10, 22, 23, 36, 28, 8, 62, 11};

        for (int i = 0; i < 10; i++)
        {
            SourceText sourceText = SourceText.FromString(srcs[i]);

            Lexer lexer = new Lexer(sourceText);
            List<SyntaxToken> nodes = new List<SyntaxToken>();
            while (lexer.EndOfFile() == false) 
            {
                nodes.Add(lexer.Lex());
            }
            Parser parser = new Parser(nodes);
            Assert.Equal(parser.EvaluateExpression(), results[i]);
         } */
    }
}