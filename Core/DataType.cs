using System.Text;
using System.Security.Cryptography;
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
    // just last 64bits SHA256 hashed bytes
    public static ulong FromName(string name) 
    {
        byte[] hashed = SHA256.HashData(Encoding.UTF8.GetBytes(name));
        return BitConverter.ToUInt64(hashed, hashed.Length - 8);
    }
}