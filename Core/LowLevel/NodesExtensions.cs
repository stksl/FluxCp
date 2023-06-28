using Fluxcp.Syntax;
using Fluxcp.Errors;
using System.Text;
namespace Fluxcp.LowLevel;
// low level nodes extensions
public static class NodesExtensions 
{
    public static uint GetSize(this StructDefine structDefine, CompilationUnit cUnit) 
    {
        if (structDefine.Size != default || structDefine.BaseType.HasValue) 
            return structDefine.Size == default ? structDefine.BaseType!.Value.Size : structDefine.Size;

        foreach(StructField field in structDefine.Fields.Values) 
        {
            StructDefine? fieldStruct = null;
            if (!field.DataType.IsTypeDefined(cUnit, out fieldStruct)) 
            {
                Error.Execute(cUnit.Logger, ErrorDefaults.UnknownType, 0); // 0 as line for now
            }
            structDefine.Size += GetSize(fieldStruct!, cUnit);
        }
        return structDefine.Size;
    }
    public static byte[] GetBytes(this VariableValue value, CompilationUnit cUnit) 
    {
        // low level structure:
        //0x00: 0x11 0x22 0x33 0x44 0x55 0x66 0x77 0x88 - type definition
        //0x08: fields values
        byte[] outputBytes = null!;
        if (value is LiteralValue literalValue) 
        {
            PrimitiveType literalType = default;
            outputBytes = GetLiteralBytes(literalValue, cUnit, ref literalType);
            if (value.IsCasted) 
            {
                StructDefine? castType = null; 
                if (!value.CastTo!.IsTypeDefined(cUnit, out castType) || !castType!.BaseType.HasValue)
                    Error.Execute(cUnit.Logger, ErrorDefaults.UnknownType, 0); // 0 as LINE FOR NOW
                byte[] castedBytes = 
                    CastingHelper.Cast(literalType, outputBytes, castType!.BaseType!.Value, cUnit);

                
                outputBytes = castedBytes!;
            }

        }
        else if (value is CopyValue copyValue) 
        {
            VarDeclarationNode? fromVar = cUnit.LocalStorage.GetLocalVar(copyValue.FromVar);
            return GetBytes(fromVar!.Value!, cUnit);
        }
        return outputBytes;
    }
    private static byte[] GetLiteralBytes(LiteralValue literal, CompilationUnit cUnit, ref PrimitiveType type) 
    {
        if (literal.Literal.Count < 1) return null!;

        switch(literal.LiteralType) 
        {
            case LiteralType.Number:
                byte[] rawData = null!;
                if (long.TryParse(literal.Literal[0].PlainValue, out long i64)) 
                {
                    type = PrimitiveType.GetNumber(i64);
                    rawData = BitConverter.GetBytes(i64);
                }
                else if (ulong.TryParse(literal.Literal[0].PlainValue, out ulong u64)) 
                {
                    type = PrimitiveType.GetUNumber(u64);
                    rawData = BitConverter.GetBytes(u64);
                }
                Array.Resize(ref rawData, type.Size);
                return rawData;
            case LiteralType.Boolean:
                type = PrimitiveType.Boolean;
                return new byte[1] {literal.Literal[0].Kind == SyntaxKind.TrueToken ? (byte)1 : (byte)0};
            case LiteralType.String or LiteralType.Character:
                List<byte> charsBytes = new List<byte>();
                for(int i = 1; i < literal.Literal.Count - 1; i++) 
                {
                    charsBytes.AddRange(Encoding.Unicode.GetBytes(literal.Literal[i].PlainValue));
                }

                type = literal.Literal[0].Kind == SyntaxKind.DoubleQuotesToken ? PrimitiveType.String : PrimitiveType.Character;
                return charsBytes.ToArray();
            default:
                return null!;
        }
    }
}