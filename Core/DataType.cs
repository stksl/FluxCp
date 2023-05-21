using System.Text;
using System.Security.Cryptography;
namespace Fluxcp;
public sealed class DataType : IDataType
{
    private long typeID;
    public long TypeID => typeID;
    public DataType(long typeId)
    {
        typeID = typeId;
    }
    public static explicit operator DataType(long typeId) 
    {
        return new DataType(typeId);
    }
    // just last 64bits SHA256 hashed bytes
    public static DataType FromName(string name) 
    {
        byte[] hashed = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        return (DataType)BitConverter.ToInt64(hashed, hashed.Length - 8);
    }
}