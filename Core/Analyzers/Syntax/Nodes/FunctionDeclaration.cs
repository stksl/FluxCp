using System.Collections.Immutable;
namespace Fluxcp.Syntax;
public sealed class FunctionDeclaration : SyntaxNode 
{
    public readonly FunctionHeader Header;
    public readonly FunctionBodyBound Body;

    public FunctionDeclaration(FunctionHeader header, FunctionBodyBound body)
    {
        Header = header;
        Body = body;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Header;
        yield return Body;
    }
    public static new FunctionDeclaration Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;

        FunctionHeader header = FunctionHeader.Parse(parser);
        FunctionBodyBound bodyBound = FunctionBodyBound.Parse(parser, header);

        return new FunctionDeclaration(header, bodyBound);
    }
}