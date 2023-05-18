namespace Fluxcp;
public sealed class ReturnStatement : SyntaxNode 
{
    public object? Value;
    public ReturnStatement(object? value)
    {
        Value = value;
    }
}