using System.Reflection.Emit;
namespace Fluxcp.LowLevel;
public interface IGenerator 
{
    void Emit(OpCode opCode);
}