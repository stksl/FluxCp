namespace Fluxcp.Syntax;
public sealed class ElseStatement : LogicalStatement 
{
    public bool IsElseIf => NextIfStatement! != null!;
    public ElseStatement(IfStatement ifStatement, SyntaxNode body)
    {
        IfStatement = ifStatement;
        Body = body;
    }
    public IfStatement IfStatement;
    //pointer to the first body token
    public SyntaxNode Body;
    public ExpressionNode? Expression;
    // will be built a doubly-linked list of else if statements
    public IfStatement? NextIfStatement;
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Body;
    }

}