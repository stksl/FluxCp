using System.Reflection.Emit;
namespace Fluxcp.LowLevel;
// Netwide assembler generator. .fcp files are compiled into .nasm and then linked and executed.
internal sealed class NasmGenerator : IGenerator 
{
    private readonly TextWriter textWriter;
    public NasmGenerator(TextWriter textWriter_)
    {
        textWriter = textWriter_;
    }
    public void Emit(OpCode opCode) 
    {
    }
}