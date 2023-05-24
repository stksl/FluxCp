namespace Fluxcp;
public sealed class FunctionHeader : SyntaxNode 
{
    public readonly DataType ReturnType;
    public readonly string Name;
    public readonly FunctionArgument[] Args;
    public FunctionHeader(DataType returnType, string name, FunctionArgument[] args)
    {
        ReturnType = returnType;
        Name = name;
        Args = args;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(FunctionArgument arg in Args) yield return arg;
    }
}