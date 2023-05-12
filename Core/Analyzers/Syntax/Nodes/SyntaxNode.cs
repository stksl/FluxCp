namespace Fluxcp;
// Base class for all of the syntax nodes
public class SyntaxNode 
{

    public SyntaxNode()
    {

    }
}
public class FunctionNode : SyntaxNode 
{
    public readonly string Name;
    public readonly FunctionArgsNode Args;
    public readonly FunctionBodyNode Body;
    public FunctionNode(string name)
    {
        Name = name;
        Body = new FunctionBodyNode();
        Args = new FunctionArgsNode();
    }
}
public class FunctionArgsNode : SyntaxNode 
{

}
public class FunctionBodyNode : SyntaxNode 
{

}