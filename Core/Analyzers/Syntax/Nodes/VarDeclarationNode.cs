using System.Collections.Immutable;
using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class VarDeclarationNode : SyntaxNode
{
    // variable access level
    public readonly int Level;
    public readonly string Name;
    public readonly DataType DataType;
    public VariableValue? Value;
    public VarDeclarationNode(string name, int level, DataType typeId, VariableValue? value)
    {
        Level = level;
        Name = name;
        DataType = typeId;
        Value = value;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Value!;
    }
    public static new VarDeclarationNode Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        DataType varType = DataType.FromName(parser.syntaxTokens[offset].PlainValue);
        string varName = parser.syntaxTokens[offset + 1].PlainValue;

        // variable declaration
        if (parser.compilationUnit.LocalStorage.GetLocalType(varType) == null)
        {
            Error.Execute(parser.logger, ErrorDefaults.UnknownType, parser.syntaxTokens[offset].Line);
        }
        else if (parser.compilationUnit.LocalStorage.GetLocalVar(varName) != null)
        {
            Error.Execute(parser.logger, ErrorDefaults.AlreadyDefined, parser.syntaxTokens[offset + 1].Line);
        }

        // saving local variable name and level
        VarDeclarationNode node = new VarDeclarationNode(varName, 0, varType, null); // 0 as level FOR NOW!
        parser.compilationUnit.LocalStorage.AddLocalVar((VarDeclarationNode)node);

        offset += parser.SaveEquals(2, SyntaxKind.EqualsToken) ? 1 : 2; // skipping to var declaration/semicolon
        return node;
    }
}
public abstract class VariableValue : SyntaxNode
{
    public static new VariableValue Parse(Parser parser)
    {
        LiteralValue literal = LiteralValue.Parse(parser);
        return literal == null ? CopyValue.Parse(parser) : literal;
    }
}
public sealed class LiteralValue : VariableValue
{
    public readonly ImmutableArray<SyntaxToken> Literal;
    public readonly int Position;
    public readonly int Length;
    public LiteralValue(ImmutableArray<SyntaxToken> literal, int pos, int length)
    {
        Literal = literal;
        Position = pos;
        Length = length;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new LiteralValue Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        LiteralValue literalValue = null!;
        // number literal
        if (parser.SaveEquals(0, SyntaxKind.NumberToken))
        {
            literalValue = new LiteralValue(parser.syntaxTokens, offset++, 1); // for now without floating-point numbers
        }
        // string literal
        else if (parser.SaveEquals(0, SyntaxKind.DoubleQuotesToken))
        {
            int prev = offset;
            while (parser.SaveEquals(1, node => node.Kind != SyntaxKind.DoubleQuotesToken))
            {
                offset++;
            }
            offset += 2; // skipping double quote token
            literalValue = new LiteralValue(parser.syntaxTokens, prev, (offset - 1) - prev);
        }
        // character literal
        else if (parser.SaveEquals(0, SyntaxKind.SingleQuoteToken) && parser.SaveEquals(2, SyntaxKind.SingleQuoteToken))
        {
            offset += 3; // skipping character literals and value
            literalValue = new LiteralValue(parser.syntaxTokens, offset, 2);
        }
        return literalValue;
    }
}
public sealed class CopyValue : VariableValue
{
    public VarDeclarationNode From;
    public VarDeclarationNode To;
    public CopyValue(VarDeclarationNode from, VarDeclarationNode to)
    {
        From = from;
        To = to;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
    public static new CopyValue Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        // 0 is copyFrom and -2 is copyTo
        // (-2)copyTo = (0)copyFrom;
        if (!parser.SaveEquals(0, SyntaxKind.TextToken) && !parser.SaveEquals(-2, SyntaxKind.TextToken))
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

        VarDeclarationNode? copyFrom = parser.compilationUnit.LocalStorage.GetLocalVar(parser.syntaxTokens[offset].PlainValue);
        VarDeclarationNode? copyTo = parser.compilationUnit.LocalStorage.GetLocalVar(parser.syntaxTokens[offset - 2].PlainValue);
        if (copyFrom == null || copyTo == null)
            Error.Execute(parser.logger, ErrorDefaults.UnknownReference, parser.syntaxTokens[offset].Line);
        offset++; // skipping name
        return new CopyValue(copyFrom!, copyTo!);
    }
}
public sealed class RefValue : VariableValue
{
    public VariableValue ReferenceTo;
    public RefValue(VariableValue refTo)
    {
        ReferenceTo = refTo;

        // not used for now
        throw new NotSupportedException("Ref values are not supported for now");
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        return Array.Empty<SyntaxNode>();
    }
}