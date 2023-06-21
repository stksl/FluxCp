public struct NasmOpcode : IEquatable<NasmOpcode>
{

    public static NasmOpcode FromString(string rawInstruction) 
    {
        switch(rawInstruction) 
        {
            case "mov":
                break;
            case "jmp":
                break;
            
        }
        return default;
    }
    public bool Equals(NasmOpcode other) 
    {
        return false;
    }

}