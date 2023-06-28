using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class SkipNode : SyntaxNode 
{
    public readonly bool SkipAll; // break in C#
    public SkipNode(bool skipAll)
    {
        SkipAll = skipAll;
    }
    public static new SkipNode Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.SkipToken))
            Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        bool skipAll = parser.SaveEquals(1, SyntaxKind.TextToken) && parser.syntaxTokens[offset + 1].PlainValue == "all";
        offset += skipAll ? 2 : 1;
        return new SkipNode(skipAll);
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}