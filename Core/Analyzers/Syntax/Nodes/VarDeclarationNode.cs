using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class VarDeclarationNode : SyntaxNode
{
    // variable access level, 0 - the top level (ascending order)
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
        VarDeclarationNode node = new VarDeclarationNode(varName, parser.compilationUnit.CurrLvl, varType, null);
        parser.compilationUnit.LocalStorage.AddLocalVar((VarDeclarationNode)node);

        offset += parser.SaveEquals(2, SyntaxKind.EqualsToken) ? 1 : 2; // skipping to var value/semicolon
        return node;
    }
}