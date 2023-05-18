namespace Fluxcp;
public sealed class UseStatement : SyntaxNode 
{
    public bool IsPath => AssemblyName[0] == '\'' && AssemblyName[^1] == '\'';
    public readonly string AssemblyName;
    public UseStatement(string assemblyName)
    {
        AssemblyName = assemblyName;
    }
}
