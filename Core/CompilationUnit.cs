using System.Collections;
using System.Diagnostics.CodeAnalysis;
namespace Fluxcp;
public sealed class CompilationUnit 
{
    public int CurrLvl; // our current level on parsing phase. (variable access range)
    public readonly LocalStorage LocalStorage;
    public CompilationUnit()
    {
        LocalStorage = new LocalStorage();
    }
}