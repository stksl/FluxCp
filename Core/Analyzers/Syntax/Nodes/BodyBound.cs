using Fluxcp.Errors;
namespace Fluxcp.Syntax;
public sealed class BodyBound : SyntaxNode 
{
    public readonly int Position;
    public int Length {get; internal set;}
    public readonly SyntaxNode Child;
    public BodyBound(int pos)
    {
        Child = SyntaxNode.Empty;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Child;
    }
    public static new BodyBound Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.OpenBraceToken))
        {
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }
        offset++; // skipping '{'
        BodyBound body = new BodyBound(offset);
        SyntaxNode last = body.Child;
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken))
        {
            last.Next = ParseNext(parser);
            last = last.Next;
        }
        body.Length = offset - body.Position;

        offset++; // skipping '}'
        return body;
    }
    private static SyntaxNode ParseNext(Parser parser)
    {
        ref int offset = ref parser.offset;

        SyntaxNode node = null!;
        if (parser.SaveEquals(0, SyntaxKind.TextToken) && parser.SaveEquals(1, SyntaxKind.TextToken))
        {
            node = VarDeclarationNode.Parse(parser);
        }
        if (parser.SaveEquals(0, SyntaxKind.TextToken) && parser.SaveEquals(1, SyntaxKind.EqualsToken))
        {
            int prev = offset;
            offset += 2;
            VariableValue value = VariableValue.Parse(parser);
            value.ToVar = parser.syntaxTokens[prev].PlainValue;
            if (node is VarDeclarationNode varDec) 
            {
                varDec.Value = value;
            }
            else node = value;

        }
        else if (parser.SaveEquals(0, SyntaxKind.TextToken) && parser.SaveEquals(1, SyntaxKind.OpenParentheseToken))
        {
            node = FunctionCall.Parse(parser);
        }
        else if (parser.SaveEquals(0, node => SyntaxFacts.IsKeyword(node.Kind)))
        {
            node = ParseKeywords(parser);
        }

        // all of the statements before have to skip current token to semicolon (expected)
        if (!parser.SaveEquals(0, SyntaxKind.SemicolonToken))
            Error.Execute(parser.logger, ErrorDefaults.SemicolonExpected, parser.syntaxTokens[offset].Line);

        offset++; // skipping ';'
        return node!;
    }
    private static SyntaxNode ParseKeywords(Parser parser)
    {
        switch(parser.syntaxTokens[parser.offset].Kind) 
        {
            case SyntaxKind.ReturnStatementToken:
                return ReturnStatement.Parse(parser);
            case SyntaxKind.IfStatementToken:
                return IfStatement.Parse(parser);
            case SyntaxKind.ElseStatementToken:
                return ElseStatement.Parse(parser);
            case SyntaxKind.WhileStatementToken:
                return WhileStatement.Parse(parser);
            default:
                Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[parser.offset].Line);
                return null!;
        }
    }
}