using Fluxcp;
namespace Fluxcp.Syntax;
public abstract class VariableValue : SyntaxNode
{
    public string? ToVar {get; internal set;}
    public DataType? CastTo {get; internal set;}
    public bool IsCasted => CastTo != null;
    public static new VariableValue Parse(Parser parser)
    {
        VariableValue resultValue = null!;
        // '\' is an expression literal
        if (parser.SaveEquals(0, SyntaxKind.BackSlashToken)) 
        {
            return ExpressionNode.Parse(parser);
        }
        else if (parser.SaveEquals(0, SyntaxKind.TextToken) && parser.SaveEquals(1, SyntaxKind.OpenParentheseToken)) 
        {
            resultValue = FunctionCall.Parse(parser);
        }
        else  
        {
            resultValue = LiteralValue.Parse(parser)! ?? 
                (VariableValue)CopyValue.Parse(parser);
        }
        
        if (parser.SaveEquals(0, SyntaxKind.CastToken)) 
        {
            resultValue.CastTo = DataType.FromName(parser.syntaxTokens[parser.offset + 1].PlainValue);
            parser.offset += 2;
        }
        return resultValue;
    }
}