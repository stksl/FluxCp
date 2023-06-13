using Fluxcp.Errors;
namespace Fluxcp.Syntax;
/* public sealed class RefValue : VariableValue 
{
    public readonly string FromVar;
    public RefValue(string fromVar) : base(false)
    {
        FromVar = fromVar;
    }

    // refToVal - reference to a value and 
    public static new RefValue Parse(Parser parser) 
    {
        if (!parser.SaveEquals(0, SyntaxKind.TextToken)) 
        {
            Error.Execute(parser.logger, ErrorDefaults.UnknownReference, parser.syntaxTokens[parser.offset].Line);
        }
        return new RefValue(refFrom!, refTo!);
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
} */