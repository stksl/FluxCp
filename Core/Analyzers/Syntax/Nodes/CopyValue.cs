
namespace Fluxcp.Syntax;
public sealed class CopyValue : VariableValue
{
    public readonly VarDeclarationNode From;
    // inlining is supported. If we are inlining we do not want to access 'To' but just grab the value
    public readonly VarDeclarationNode? To;
    public CopyValue(VarDeclarationNode from, VarDeclarationNode? to, bool inline) : base(inline)
    {
        From = from;
        To = to;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new CopyValue Parse(Parser parser) => Parse(parser, false);
    public static new CopyValue Parse(Parser parser, bool inline) 
    {
        ref int offset = ref parser.offset;

        // 0 is copyFrom and -2 is copyTo
        // (-2)copyTo = (0)copyFrom;
        if (!parser.SaveEquals(0, SyntaxKind.TextToken) && (!parser.SaveEquals(-2, SyntaxKind.TextToken) && !inline))
            return null!;

        VarDeclarationNode? copyFrom = parser.compilationUnit.LocalStorage.GetLocalVar(parser.syntaxTokens[offset].PlainValue);
        if (copyFrom == null)
            return null!;
        VarDeclarationNode? copyTo = !inline ? parser.compilationUnit.LocalStorage.GetLocalVar(parser.syntaxTokens[offset - 2].PlainValue) : null;

        offset++; // skipping name
        return new CopyValue(copyFrom!, copyTo, inline);
    }
}