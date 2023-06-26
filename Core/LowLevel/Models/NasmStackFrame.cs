using Fluxcp.Syntax;
namespace Fluxcp.LowLevel;
public struct NasmStackFrame 
{
    public readonly uint[] ArgsSize;
    public readonly uint RetSize;
    public List<uint> Locals;
    public NasmStackFrame(uint retSize, params uint[] argsSize)
    {
        RetSize = retSize;
        ArgsSize = argsSize;
        Locals = new List<uint>();
    }
}