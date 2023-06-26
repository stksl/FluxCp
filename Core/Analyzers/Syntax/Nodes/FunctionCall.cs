using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class FunctionCall : VariableValue
{
    public string FunctionName;
    public VariableValue[] PassedVals;
    public FunctionCall(string functionName, VariableValue[] passedVals)
    {
        FunctionName = functionName;
        PassedVals = passedVals;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach (VariableValue passedVal in PassedVals) yield return passedVal;
    }
    public static new FunctionCall Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.TextToken)) 
        {
            Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownReference, parser.syntaxTokens[offset].Line);
        }
        string funcName = parser.syntaxTokens[offset].PlainValue;
        offset += 2; // skipping to passed argument or ')'
        List<VariableValue> passedVals = new List<VariableValue>();
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
        {
            VariableValue passedVal = VariableValue.Parse(parser);
            passedVals.Add(passedVal);

            if (!parser.SaveEquals(0, SyntaxKind.CommaToken) && parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
                Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

            offset += parser.SaveEquals(0, SyntaxKind.CloseParentheseToken) ? 0 : 1;
        }
        offset++; // skipping ')'
        return new FunctionCall(funcName, passedVals.ToArray());
    }
}