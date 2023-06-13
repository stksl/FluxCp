using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class ElseStatement : SyntaxNode 
{
    public bool IsElseIf => NextIfStatement != null;
    //Nullable body bound. Is not null if 'NextIfStatement' is.
    public BodyBound? Body {get; init;}
    // will be built a singly-linked list of else if statements
    public IfStatement? NextIfStatement {get; init;}
    public static new ElseStatement Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.ElseStatementToken)) 
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

        offset++;
        return parser.SaveEquals(0, SyntaxKind.IfStatementToken) ? 
            new ElseStatement() {NextIfStatement = IfStatement.Parse(parser)} : 
            new ElseStatement() {Body = BodyBound.Parse(parser)};
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        if (Body != null)
            yield return Body!;
        else 
            yield return NextIfStatement!;
    }

}