namespace Fluxcp;
public static class DataTypeExtension 
{
    public static bool IsTypeDefined(this DataType dataType) 
    {
        return LocalStorage.Items.ContainsKey(dataType.TypeID);
    }
}