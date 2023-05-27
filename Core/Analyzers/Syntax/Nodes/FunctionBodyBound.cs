using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class FunctionBodyBound : SyntaxNode
{
    // start of the body (can be used as syntax tokens offset)
    public readonly int Position;
    // length of the body
    public int Length { get; internal set; }
    public SyntaxNode Child;
    public FunctionBodyBound(int position)
    {
        Position = position;
        Child = SyntaxNode.Empty;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Child!;
    }
    public static FunctionBodyBound Parse(Parser parser, FunctionHeader header)
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.OpenBraceToken))
        {
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }
        offset++; // skipping '{'
        FunctionBodyBound body = new FunctionBodyBound(offset);
        SyntaxNode last = body.Child;
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken))
        {
            last.Next = ParseBody(parser, header);
            last = last.Next;
        }
        body.Length = offset - body.Position;

        offset++; // skipping '}'
        return body;
    }
    private static SyntaxNode ParseBody(Parser parser, FunctionHeader header)
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
            if (node is VarDeclarationNode varNode)
            {
                // setting init value for simplicity (the only time that parser sets variable value directly)
                varNode.Value = value;
            }
            else node = value;
        }
        else if (parser.SaveEquals(0, SyntaxKind.TextToken) && parser.SaveEquals(1, SyntaxKind.OpenParentheseToken))
        {
            // probably function call
            FunctionDeclaration? localFunc = parser.compilationUnit.LocalStorage.GetLocalFunc(parser.syntaxTokens[offset].PlainValue);
            if (localFunc == null)
                Error.Execute(parser.logger, ErrorDefaults.UnknownReference, parser.syntaxTokens[offset].Line);

            offset += 2; // skipping to passed argument or ')'
            List<VariableValue> passedVals = new List<VariableValue>();
            while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
            {
                VariableValue passedVal = VariableValue.Parse(parser);
                passedVals.Add(passedVal);

                if (!parser.SaveEquals(0, SyntaxKind.CommaToken) && parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
                    Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

                offset += parser.SaveEquals(0, SyntaxKind.CloseParentheseToken) ? 0 : 1;
            }
            offset++; // skipping ')'
            node = new FunctionCall(localFunc!, passedVals.ToArray());
        }
        else if (parser.SaveEquals(0, node => SyntaxFacts.IsKeyword(node.Kind)))
        {
            // keywords
            node = ParseKeywords();
        }

        // all of the statements before have to skip current token to semicolon (expected)
        if (!parser.SaveEquals(0, SyntaxKind.SemicolonToken))
            Error.Execute(parser.logger, ErrorDefaults.SemicolonExpected, parser.syntaxTokens[offset].Line);

        offset++; // skipping ';'
        return node!;
    }
    private static SyntaxNode ParseKeywords()
    {

        return null!;
    }
}