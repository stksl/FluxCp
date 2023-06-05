namespace Fluxcp.Syntax;
public sealed class RefValue : VariableValue 
{
    public readonly VarDeclarationNode From;
    public readonly VarDeclarationNode To;
    public RefValue(VarDeclarationNode from, VarDeclarationNode to) : base(false)
    {
        From = from;
        To = to;
    }

    // refToVal - reference to a value and 
    public static new RefValue Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;

        // 0 is refFrom and -2 is refTo
        // (-2)refTo = (0)refFrom;
        if (!parser.SaveEquals(0, SyntaxKind.TextToken) && !parser.SaveEquals(-2, SyntaxKind.TextToken))
            return null!;

        VarDeclarationNode? refFrom = parser.compilationUnit.LocalStorage.GetLocalVar(parser.syntaxTokens[offset].PlainValue);
        VarDeclarationNode? refTo = parser.compilationUnit.LocalStorage.GetLocalVar(parser.syntaxTokens[offset - 2].PlainValue);
        if (refFrom == null || refTo == null)
            return null!;

        offset++; // skipping name
        return new RefValue(refFrom!, refTo!);
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}