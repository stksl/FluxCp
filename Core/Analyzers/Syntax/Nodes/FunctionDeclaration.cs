namespace Fluxcp;
public sealed class FunctionDeclaration : SyntaxNode 
{
    public readonly DataType ReturnType;
    public string Name;
    public FunctionArgument[]? Args;
    public SyntaxNode Body;

    public FunctionDeclaration(ulong returnTypeId, string name, SyntaxNode body)
    {
        ReturnType = (DataType)returnTypeId;
        Name = name;
        Body = body;
    }
}
public class FunctionArgument : SyntaxNode
{
    public readonly string Name;
    public readonly DataType DataType;
    public FunctionArgument(string name, ulong typeId)
    {
        Name = name;
        DataType = (DataType)typeId;
    }
}