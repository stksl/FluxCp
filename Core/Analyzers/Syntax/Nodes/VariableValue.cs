using Fluxcp;
namespace Fluxcp.Syntax;
public abstract class VariableValue : SyntaxNode
{
    public string? ToVar {get; internal set;}
    public static new VariableValue Parse(Parser parser)
    {
        // '\' is an expression literal
        if (parser.SaveEquals(0, SyntaxKind.BackSlashToken)) 
        {
            return ExpressionNode.Parse(parser);
        }
        else if (parser.SaveEquals(0, SyntaxKind.TextToken) && parser.SaveEquals(1, SyntaxKind.OpenParentheseToken)) 
        {
            return FunctionCall.Parse(parser);
        }
        return (VariableValue)LiteralValue.Parse(parser) ?? 
               (VariableValue)CopyValue.Parse(parser);
    }
}