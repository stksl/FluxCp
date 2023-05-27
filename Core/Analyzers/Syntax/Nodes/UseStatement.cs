using System.Collections.Immutable;
using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class UseStatement : SyntaxNode 
{
    public bool IsPath => AssemblyName[0] == '"' && AssemblyName[^1] == '"';
    public readonly string AssemblyName;
    public UseStatement(string assemblyName)
    {
        AssemblyName = assemblyName;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new UseStatement Parse(Parser parser)
    {
        ref int offset = ref parser.offset;

        if (!parser.SaveEquals(1, SyntaxKind.TextToken) && !parser.SaveEquals(1, SyntaxKind.DoubleQuotesToken))
        {
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

        }
        string reference;
        parser.offset++; // skipping to text/doubleQuote
        if (parser.syntaxTokens[offset].Kind == SyntaxKind.DoubleQuotesToken)
        {
            reference = "\"";
            while (parser.syntaxTokens[offset + 1].Kind != SyntaxKind.DoubleQuotesToken)
            {
                reference += parser.syntaxTokens[++offset].PlainValue;
            }
            reference += "\"";
            offset += 3; // skipping quote and semicolon (expected)
        }
        else
        {
            reference = parser.syntaxTokens[offset].PlainValue;
            offset += 2; // skipping semicolon (expected)
        }

        if (!parser.SaveEquals(-1, SyntaxKind.SemicolonToken))
            Error.Execute(parser.logger, ErrorDefaults.SemicolonExpected, parser.syntaxTokens[offset].Line);

        return new UseStatement(reference);
    }
}
