namespace Fluxcp.LowLevel;
public struct PrimitiveType 
{
    public static readonly PrimitiveType Byte;
    public static readonly PrimitiveType Integer16;
    public static readonly PrimitiveType Integer32;
    public static readonly PrimitiveType Integer64;
    public static readonly PrimitiveType UInteger16;
    public static readonly PrimitiveType UInteger32;
    public static readonly PrimitiveType UInteger64;
    public static readonly PrimitiveType Boolean;
    public static readonly PrimitiveType Void;
    public static readonly PrimitiveType Float32;
    public static readonly PrimitiveType Float64;
    public static readonly PrimitiveType String;
    public static readonly PrimitiveType Character;

    public readonly ushort Size;
    public readonly PrimitiveTypes Type;
    private readonly string asStr;
    static PrimitiveType() 
    {
        Byte = new PrimitiveType(1, "byte", PrimitiveTypes.Number);
        Integer16 = new PrimitiveType(2, "i16", PrimitiveTypes.Number);
        Integer32 = new PrimitiveType(4, "i32", PrimitiveTypes.Number);
        Integer64 = new PrimitiveType(8, "i64", PrimitiveTypes.Number);
        UInteger16 = new PrimitiveType(2, "u16", PrimitiveTypes.Number);
        UInteger32 = new PrimitiveType(4, "u32", PrimitiveTypes.Number);
        UInteger64 = new PrimitiveType(8, "u64", PrimitiveTypes.Number);
        Boolean = new PrimitiveType(1, "bool", PrimitiveTypes.Boolean);
        Void = new PrimitiveType(1, "void", PrimitiveTypes.Void);
        Float32 = new PrimitiveType(4, "f32", PrimitiveTypes.Number);
        Float64 = new PrimitiveType(8, "f64", PrimitiveTypes.Number);
        String = new PrimitiveType(2, "string", PrimitiveTypes.String); // string does not have constant length.
        Character = new PrimitiveType(2, "char", PrimitiveTypes.Character);
    }
    internal PrimitiveType(ushort size, string asStr_, PrimitiveTypes type)
    {
        Size = size;
        asStr = asStr_;
        Type = type;
    }
    public static PrimitiveType GetUNumber(ulong val) 
    {
        switch(val) 
        {
            case <= byte.MaxValue:
                return Byte;
            case <= ushort.MaxValue:
                return UInteger16;
            case <= uint.MaxValue:
                return UInteger32;
            case <= ulong.MaxValue:
                return UInteger64;
        }
    }
    public static PrimitiveType GetNumber(long val)
    {
        switch(val) 
        {
            case > int.MaxValue or < int.MinValue:
                return Integer64;
            case > short.MaxValue or < short.MinValue:
                return Integer32;
            case > byte.MaxValue or < 0:
                return Integer16;
            default:
                return Byte;
        }
    }
    public override string ToString()
    {
        return asStr;
    }
}
public enum PrimitiveTypes 
{
    Void,
    Character,
    String,
    Boolean,
    Number,
}