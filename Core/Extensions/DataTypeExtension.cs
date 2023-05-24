namespace Fluxcp;
public static class DataTypeExtension 
{
    public static bool IsTypeDefined(this DataType dataType, CompilationUnit compUnit) 
    {
        return compUnit.LocalStorage.GetLocalType(dataType) != null;
    }
}