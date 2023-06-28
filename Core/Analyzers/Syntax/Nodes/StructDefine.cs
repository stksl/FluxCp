using Fluxcp.Errors;
using Fluxcp.LowLevel;
namespace Fluxcp.Syntax;
public sealed class StructDefine : SyntaxNode
{
    public readonly string Name;
    public readonly PrimitiveType? BaseType; // null is for custom type
    public readonly Dictionary<string, StructField> Fields;
    public readonly Dictionary<string, FunctionDeclaration> Functions;
    public uint Size {get; internal set;} // this size is for custom types only
    public StructDefine(string name, Dictionary<string, StructField> fields, 
        Dictionary<string, FunctionDeclaration> functions, PrimitiveType? primitiveType)
    {
        Name = name;
        Fields = fields;
        Functions = functions;
        BaseType = primitiveType;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(var field in Fields) yield return field.Value;
        foreach(var func in Functions) yield return func.Value;
    }
    public static new StructDefine Parse(Parser parser)
    {
        ref int offset = ref parser.offset;

        if (parser.SaveEquals(1, node => node.Kind != SyntaxKind.TextToken) ||
            parser.SaveEquals(2, node => node.Kind != SyntaxKind.OpenBraceToken))
        {
            Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }
        offset++; // skiping to structure's name

        string name = parser.syntaxTokens[offset].PlainValue;

        // checking for primitive type
        PrimitiveType? primitiveType = PrimitiveType.FromName(name);

        DataType structType = DataType.FromName(name);

        Dictionary<string, StructField> fields = new Dictionary<string, StructField>();
        Dictionary<string, FunctionDeclaration> functions = new Dictionary<string, FunctionDeclaration>();

        offset += 2; // skiping after '{'
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken))
        {
            // if that's not correct field/function declaration
            if (parser.SaveEquals(0, node => node.Kind != SyntaxKind.TextToken) ||
                parser.SaveEquals(1, node => node.Kind != SyntaxKind.TextToken))
            {
                Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
            }

            // only function/field declaration
            string typeName = parser.syntaxTokens[offset].PlainValue;

            if (parser.SaveEquals(2, SyntaxKind.SemicolonToken))
            {
                // that's a field
                StructField field = StructField.Parse(parser);
                fields[field.Name] = field;
            }
            else if (parser.SaveEquals(2, SyntaxKind.OpenParentheseToken))
            {
                // that's a function
                FunctionDeclaration function = FunctionDeclaration.Parse(parser);
                functions[function.Header.Name] = function;
            }
            else
            {
                Error.Execute(parser.cUnit.Logger, ErrorDefaults.SemicolonExpected, parser.syntaxTokens[offset].Line);
            }
        }
        offset++; // skipping '}'

        return new StructDefine(name, fields, functions, primitiveType);
    }
    public override int GetHashCode()
    {
        return (int)GetDataType().TypeID;
    }
    public DataType GetDataType() 
    {
        return DataType.FromName(Name);
    }
}