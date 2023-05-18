using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace Fluxcp.Cli
{
    public class Program
    {
        public static unsafe void Main()
        {
            Logger logger = new Logger();
            string text = "use FCP.Main;\n" + 
            "void Main()\n" + 
            "{\n" +
            "   i32 use1 = 1050;\n" +
            "   for(u8 i = 0; i < 10; i++) {}" +
            "}";
            SourceText tr = SourceText.FromString(text);

            List<SyntaxToken> syntaxTokens = new List<SyntaxToken>();
            Lexer lexer = new Lexer(tr, logger);

            while (!lexer.EndOfFile()) 
            {
                var curr = lexer.Lex();
                syntaxTokens.Add(curr);
                for(int i = curr.Offset; i < curr.Offset + curr.Length; i++) 
                {
                    Console.Write(curr.Text?[i]);
                }
                System.Console.WriteLine($" {curr.Kind}");
            }
            syntaxTokens.Add(lexer.Lex()); // EOF token

            Parser parser = new Parser(syntaxTokens.ToImmutableArray(), logger);
            parser.Parse();
        }
    }
    internal class Logger : ILogger
    {
        public void ShowDebug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            System.Console.WriteLine("\\Debug-only:\\ " + msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void ShowMsg(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.WriteLine("\\msg:\\ " + msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void ShowWarning(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            System.Console.WriteLine("\\warning:\\ " + msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        public void ShowError(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine("\\error:\\ " + msg);
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}
