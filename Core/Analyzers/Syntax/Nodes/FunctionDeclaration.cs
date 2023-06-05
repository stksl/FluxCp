using System.Collections.Immutable;
namespace Fluxcp.Syntax;
public sealed class FunctionDeclaration : SyntaxNode 
{
    public readonly FunctionHeader Header;
    public readonly BodyBound Body;

    public FunctionDeclaration(FunctionHeader header, BodyBound body)
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
        BodyBound bodyBound = BodyBound.Parse(parser);

        return new FunctionDeclaration(header, bodyBound);
    }
}