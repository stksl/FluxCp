using Fluxcp.Syntax;
namespace Fluxcp;
public static class DataTypeExtension 
{
    public static bool IsTypeDefined(this DataType dataType, CompilationUnit compUnit, out StructDefine? foundType) 
    {
        foundType = compUnit.LocalStorage.GetLocalType(dataType);
        return foundType != null;
    }
}