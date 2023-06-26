using Fluxcp.Syntax;
namespace Fluxcp;
internal sealed class FullBuildInternal 
{
    private readonly CompilingOptions options;
    private readonly ILogger? logger;
    public FullBuildInternal(CompilingOptions options_, ILogger? logger_)
    {
        options = options_;
        logger = logger_;
    }
    public async Task<string> LoadAsync() 
    {
        using StreamReader sr = new StreamReader(options.EntryPoint);
        // very unefficient
        return await sr.ReadToEndAsync();
    }
    public CompilationUnit FullBuild(SourceText sourceText) 
    {
        CompilationUnit unit = new CompilationUnit(options, null, logger);

        Lexer lexer = new Lexer(sourceText, unit);
        List<SyntaxToken> tokens = new List<SyntaxToken>();
        Dictionary<int, SyntaxToken> trivia = new Dictionary<int, SyntaxToken>();
        while (!lexer.EndOfFile()) 
        {
            SyntaxToken token = lexer.Lex();
            if (SyntaxFacts.IsTrivia(token.Kind)) 
            {
                trivia[token.Offset] = token;
            }
            else tokens.Add(token);
        }
        Parser parser = new Parser(tokens, trivia, unit);
        SyntaxTree tree = parser.Parse();
        Builder builder = new Builder(tree, unit);
        builder.Build();
        return unit;
    }
}