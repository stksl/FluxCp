namespace Fluxcp.Syntax;
public class CopyValue : VariableValue
{
    public virtual string FromVar {get; private set;}
    public CopyValue(string fromVar)
    {
        FromVar = fromVar;
    }
    protected internal CopyValue() {FromVar = "";}
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
            parser.offset += 2;
            return MemberInvoke.Parse(parser);
        }
        string fromVar = parser.syntaxTokens[parser.offset++].PlainValue;
        return new CopyValue(fromVar);
    }
}