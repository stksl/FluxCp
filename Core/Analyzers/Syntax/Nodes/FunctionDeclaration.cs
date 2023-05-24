using System.Collections.Immutable;
namespace Fluxcp;
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
}