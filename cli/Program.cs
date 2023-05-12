using Fluxcp;
using System.Text;
namespace Fluxcp.Cli 
{
    public class Program 
    {
        public static unsafe void Main() 
        {
            string text = "10 - 4 + 3 * (8 + 2)";
            fixed (char* ch = text) 
            {
                Fluxcp.SourceText tr = new Fluxcp.SourceText(ch, text.Length);

                Lexer lexer = new Lexer(tr);
                List<SyntaxToken> SyntaxTokens = new List<SyntaxToken>();
                while (!lexer.EndOfFile()) 
                {
                    SyntaxTokens.Add((SyntaxToken)lexer.Lex());
                }

                Parser parser = new Parser(SyntaxTokens);

                System.Console.WriteLine(parser.EvaluateExpression());
            }
        }
    }
}
