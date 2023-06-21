using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Immutable;
using Fluxcp.Syntax;
namespace Fluxcp.Tests;

public class ParserTest
{
    private int testCount = 2;
    private async Task<string> LoadTextAsync(string path) 
    {
        using StreamReader sr = new StreamReader(path);
        return await sr.ReadToEndAsync();
    }

    [Fact]
    public async Task Build_Tree_TEST()
    {
        for(int i = 0; i < testCount; i++) 
        {
            SourceText text = SourceText.FromString(await LoadTextAsync($"/home/ubuntupc/Desktop/FluxCp/test/test_files/test{i}.fcp"));

            List<SyntaxToken> tokens = new List<SyntaxToken>();
            Dictionary<int, SyntaxToken> leadingTrivia = new Dictionary<int, SyntaxToken>();

            Lexer lexer = new Lexer(text, null);
            while (!lexer.EndOfFile())
            {
                SyntaxToken lexem = lexer.Lex();
                if (SyntaxFacts.IsTrivia(lexem.Kind)) 
                {
                    leadingTrivia[lexem.Offset] = lexem;
                }
                else tokens.Add(lexem);
            }
            Parser parser = new Parser(tokens, leadingTrivia, null);
            SyntaxTree tree = parser.Parse();
            //getting full AST as string
            string treeStr = tree.Root.Print();

            Assert.Equal(treeStr, await LoadTextAsync($"/home/ubuntupc/Desktop/FluxCp/test/test_files/parse_tree/test{i}.fcp"));
        }
    }
}