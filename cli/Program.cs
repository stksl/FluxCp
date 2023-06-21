using System.Text;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using Fluxcp.Syntax;
using Fluxcp.Errors;
using System.Reflection.Emit;
namespace Fluxcp.Cli
{
    public class Program
    {
        public static async Task<string> GetStringAsync()
        {
            using (StreamReader sr = new StreamReader("./testfile.fcp"))
            {
                return await sr.ReadToEndAsync();
            }
        }
        public static unsafe void Main()
        {
            ILogger logger = new Logger();
            CompilationUnit unit = new CompilationUnit(new CompilingOptions(true, "./testfile.fcp"), null);

            string text = GetStringAsync().Result;

            SourceText tr = SourceText.FromString(text);

            List<SyntaxToken> syntaxTokens = new List<SyntaxToken>();
            Dictionary<int, SyntaxToken> leadingTrivia = new Dictionary<int, SyntaxToken>();

            Lexer lexer = new Lexer(tr, logger);
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (!lexer.EndOfFile())
            {
                SyntaxToken lexem = lexer.Lex();
                if (SyntaxFacts.IsTrivia(lexem.Kind)) 
                {
                    leadingTrivia[lexem.Offset] = lexem;
                }
                else syntaxTokens.Add(lexem);
            }
            Parser parser = new Parser(syntaxTokens, leadingTrivia, logger);
            SyntaxTree tree = parser.Parse();  
            Builder builder = new Builder(tree, unit, logger); 
            builder.Build();
            sw.Stop();
            System.Console.WriteLine(tree.Root.Print());
            logger.ShowDebug(sw.ElapsedMilliseconds + "ms");
        }
        private static SyntaxNode? GetNode(Type type, SyntaxNode node_, ref int offset)
        {
            if (node_.GetType() == type) 
            {
                if (offset == 0)
                    return node_;
                else offset--;
            }
            SyntaxNode? node = null;
            foreach (var child in node_.GetChildren())
            {
                node = GetNode(type, child, ref offset);
                if (node?.GetType() == type) 
                {
                    if (offset == 0)
                        return node;
                    else offset--;
                }
            }
            
            if (node_.Next != null)
            {
                node = GetNode(type, node_.Next, ref offset);
                if (node?.GetType() == type) 
                {
                    if (offset == 0)
                        return node;
                    else offset--;
                }
            }
            return null;
        }
    }
    public sealed class Logger : ILogger
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
