namespace Fluxcp;
public sealed class FunctionDeclaration : SyntaxNode 
{
    public readonly FunctionHeader Header;
    public readonly SyntaxNode Body;

    public FunctionDeclaration(FunctionHeader header, SyntaxNode body)
    {
        Header = header;
        Body = body;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Header;
        yield return Body;
    }
}
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
public sealed class FunctionArgument : SyntaxNode
{
    public readonly string Name;
    public readonly DataType DataType;
    public FunctionArgument(string name, DataType typeId)
    {
        Name = name;
        DataType = typeId;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return null!;
    }
}