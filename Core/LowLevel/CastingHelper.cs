using Fluxcp.Syntax;
using System.Text;
namespace Fluxcp.LowLevel;
public static class CastingHelper 
{
    public static byte[] Cast(PrimitiveType fromType, byte[] value, PrimitiveType toType, CompilationUnit cUnit) 
    {
        switch(fromType.Type) 
        {
            // from number
            case PrimitiveTypes.Number:
                switch(toType.Type) 
                {
                    //to number
                    case PrimitiveTypes.Number:
                        if (toType.Size > fromType.Size) 
                        {
                            // if we're parsing a number to a larger number type (for example i32 to i64)
                            int diff = toType.Size - fromType.Size;
                            return Enumerable.Range(0, toType.Size).Select((i, ind) => 
                                ind < diff ? (byte)0 : value[ind - diff]).ToArray();
                        }
                        // parsing a number to a number with less size (for example u16 to byte)
                        return Enumerable.Range(0, toType.Size).Select((i, ind) => 
                            value[ind + fromType.Size - toType.Size]).ToArray();
                    // to character
                    case PrimitiveTypes.Character:
                        if (fromType.Size <= 2)
                            return value;
                        break;
                }
                break;
            // from character
            case PrimitiveTypes.Character:
                switch(toType.Type) 
                {
                    // to number
                    case PrimitiveTypes.Number:
                        return value;
                }
                break;
        }
        return null!;
    }
}