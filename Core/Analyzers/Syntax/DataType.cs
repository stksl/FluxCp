namespace Fluxcp;
public sealed class DataType : IDataType
{
    private ulong typeID;
    public ulong TypeID => typeID;
    public DataType(ulong typeId)
    {
        typeID = typeId;
    }
    public static explicit operator DataType(ulong typeId) 
    {
        return new DataType(typeId);
    }
}