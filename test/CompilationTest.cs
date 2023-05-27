using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading;
using Fluxcp.Syntax;
namespace Fluxcp.Tests;
public class CompilationTest
{
    [Fact]
    public async Task Main_TEST()
    {
        for (int i = 0; i < 1; i++)
        {
            CompilationUnit compilationUnit = new CompilationUnit();
            string text = await ReadAllFrom($"/home/ubuntupc/Desktop/FluxCp/test/test_files/test{i}.fcp");
            SourceText tr = SourceText.FromString(text);
            List<SyntaxToken> syntaxTokens = new List<SyntaxToken>();
            Lexer lexer = new Lexer(tr, null, compilationUnit);
            while (!lexer.EndOfFile()) 
            {
                var curr = lexer.Lex();
                syntaxTokens.Add(curr);
            }
            Parser parser = new Parser(syntaxTokens.ToImmutableArray(), null, compilationUnit);
            var tree = parser.Parse();
            string expected = await ReadAllFrom($"/home/ubuntupc/Desktop/FluxCp/test/test_files/expected/test{i}.txt");
            Assert.True(tree.Root.Print() == expected);
        }
    }
    private async Task<string> ReadAllFrom(string path)
    {
        using StreamReader sr = new StreamReader(path);
        return await sr.ReadToEndAsync();
    }
}