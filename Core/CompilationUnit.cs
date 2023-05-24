using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Fluxcp;
public sealed class CompilationUnit 
{
    public readonly LocalStorage LocalStorage;
    public CompilationUnit()
    {
        LocalStorage = new LocalStorage();
    }
}