using Fluxcp;
namespace Fluxcp.Syntax;
public abstract class VariableValue : SyntaxNode
{
    // inlining made for not copying values where it's not needed (for example in expressions).
    // Not to be confused with function inlining
    public readonly bool Inline;
    public VariableValue(bool inline)
    {
        Inline = inline;
    }
    public static VariableValue Parse(Parser parser, bool inline)
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
               (VariableValue)CopyValue.Parse(parser) ?? 
                RefValue.Parse(parser);
    }
}