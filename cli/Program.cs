using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
namespace Fluxcp.Cli
{
    public class Program
    {
        public static async Task<string> GetStringAsync() 
        {
            using(StreamReader sr = new StreamReader("./testfile.fcp")) 
            {
                return await sr.ReadToEndAsync();
            }
        }
        public static unsafe void Main()
        {
            Logger logger = new Logger();
            CompilationUnit compilationUnit = new CompilationUnit();
            string text = GetStringAsync().Result;
            SourceText tr = SourceText.FromString(text);
            List<SyntaxToken> syntaxTokens = new List<SyntaxToken>();
            Lexer lexer = new Lexer(tr, logger, compilationUnit);

            while (!lexer.EndOfFile()) 
            {
                var curr = lexer.Lex();
                syntaxTokens.Add(curr);
            }
            Parser parser = new Parser(syntaxTokens.ToImmutableArray(), logger, compilationUnit);
            var tree = parser.Parse();
            tree.Root.Print(logger);
        }
    }
    internal class Logger : ILogger
    {
        public void ShowDebug(string msg)
        {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            System.Console.WriteLine(msg);
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
