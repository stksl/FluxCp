using System.Collections;
using System.Diagnostics.CodeAnalysis;
namespace Fluxcp;
public sealed class CompilationUnit 
{
    internal int CurrLvl; // our current level on parsing phase. (variable access range)
    internal readonly LocalStorage LocalStorage;
    public readonly CompilingOptions Options;
    public readonly ILogger? Logger;
    public CompilationUnit(CompilingOptions options, LocalStorage? existingStorage, ILogger? logger)
    {
        LocalStorage = existingStorage ?? new LocalStorage();
        Options = options;
        Logger = logger;
    }
}