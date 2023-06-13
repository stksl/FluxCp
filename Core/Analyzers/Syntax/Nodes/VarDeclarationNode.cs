using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class VarDeclarationNode : SyntaxNode
{
    public readonly string Name;
    public readonly DataType DataType;
    public VariableValue? Value;
    public VarDeclarationNode(string name, DataType typeId, VariableValue? value)
    {
        Name = name;
        DataType = typeId;
        Value = value;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        if (Value != null)
            yield return Value!;
    }
    public static new VarDeclarationNode Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        DataType varType = DataType.FromName(parser.syntaxTokens[offset].PlainValue);
        string varName = parser.syntaxTokens[offset + 1].PlainValue;

        // saving local variable name and level
        VarDeclarationNode node = new VarDeclarationNode(varName, varType, null);

        offset += parser.SaveEquals(2, SyntaxKind.EqualsToken) ? 1 : 2; // skipping to var value/semicolon
        return node;
    }
}