using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;

namespace Fluxcp.Tests;
public class CompilationTest
{

    [Fact]
    public void Main_TEST()
    {
        for (int i = 0; i < 1; i++)
        {
            string task = LoadTest(i).Result;
            Logger logger = new Logger();
            CompilationUnit compilationUnit = new CompilationUnit();

            Lexer lexer = new Lexer(SourceText.FromString(task), logger, compilationUnit);
            List<SyntaxToken> tokens = new List<SyntaxToken>();
            while (!lexer.EndOfFile())
            {
                tokens.Add(lexer.Lex());
            }

            Parser parser = new Parser(tokens.ToImmutableArray(), logger, compilationUnit);
            SyntaxTree tree = parser.Parse();
        }
    }
    private async Task<string> LoadTest(int i)
    {
        using StreamReader sr = new StreamReader($"/home/ubuntupc/Desktop/FluxCp/test/test_files/test{i}.fcp");
        return await sr.ReadToEndAsync();
    }
    private class Logger : ILogger
    {
        public void ShowDebug(string msg)
        {
            throw new System.NotImplementedException();
        }

        public void ShowError(string msg)
        {
            throw new System.NotImplementedException();
        }

        public void ShowMsg(string msg)
        {
            throw new System.NotImplementedException();
        }

        public void ShowWarning(string msg)
        {
            throw new System.NotImplementedException();
        }
    }
}