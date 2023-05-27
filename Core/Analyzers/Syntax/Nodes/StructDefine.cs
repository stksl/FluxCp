using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class StructDefine : SyntaxNode
{
    public readonly string Name;
    public Dictionary<string, StructField> Fields {get; internal set;}
    public Dictionary<string, FunctionDeclaration> Functions {get; internal set;}
    public StructDefine(string name)
    {
        Name = name;
        Fields = new Dictionary<string, StructField>();
        Functions = new Dictionary<string, FunctionDeclaration>();
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
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }
        offset++; // skiping to structure's name

        string name = parser.syntaxTokens[offset].PlainValue;
        DataType structType = DataType.FromName(name);

        if (structType.IsTypeDefined(parser.compilationUnit))
            Error.Execute(parser.logger, ErrorDefaults.AlreadyDefined, parser.syntaxTokens[offset].Line);

        // for the recursive references
        StructDefine currStruct = new StructDefine(name);
        parser.compilationUnit.LocalStorage.AddLocalType(currStruct);

        offset += 2; // skiping after '{'
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken))
        {
            // if that's not correct field/function declaration
            if (parser.SaveEquals(0, node => node.Kind != SyntaxKind.TextToken) ||
                parser.SaveEquals(1, node => node.Kind != SyntaxKind.TextToken))
            {
                Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
            }

            // only function/field declaration
            string typeName = parser.syntaxTokens[offset].PlainValue;
            if (!DataType.FromName(typeName).IsTypeDefined(parser.compilationUnit))
            {
                Error.Execute(parser.logger, ErrorDefaults.UnknownType, parser.syntaxTokens[offset].Line);
            }

            if (parser.SaveEquals(2, SyntaxKind.SemicolonToken))
            {
                // that's a field
                StructField field = StructField.Parse(parser);
                currStruct.Fields[field.Name] = field;
            }
            else if (parser.SaveEquals(2, SyntaxKind.OpenParentheseToken))
            {
                // that's a function
                FunctionDeclaration function = FunctionDeclaration.Parse(parser);
                currStruct.Functions[function.Header.Name] = function;
            }
            else
            {
                Error.Execute(parser.logger, ErrorDefaults.SemicolonExpected, parser.syntaxTokens[offset].Line);
            }
        }
        offset++; // skipping '}'

        return currStruct;
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