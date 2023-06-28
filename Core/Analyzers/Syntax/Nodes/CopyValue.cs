namespace Fluxcp.Syntax;
public class CopyValue : VariableValue
{
    public string FromVar {get; protected set;}
    public CopyValue(string fromVar)
    {
        FromVar = fromVar;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new CopyValue Parse(Parser parser) 
    {
        if (!parser.SaveEquals(0, SyntaxKind.TextToken))
            return null!;

        if (parser.SaveEquals(1, SyntaxKind.DotToken)) 
        {
            //invoking members as getter
            return MemberInvoke.Parse(parser);
        }
        string fromVar = parser.syntaxTokens[parser.offset++].PlainValue;
        return new CopyValue(fromVar);
    }
}