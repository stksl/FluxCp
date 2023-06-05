using Fluxcp.Errors;

namespace Fluxcp.Syntax;
public sealed class FunctionCall : VariableValue
{
    public FunctionDeclaration Function;
    public VariableValue[] PassedVals;
    public FunctionCall(FunctionDeclaration function, VariableValue[] passedVals) : base(false)
    {
        Function = function;
        PassedVals = passedVals;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach (VariableValue passedVal in PassedVals) yield return passedVal;
    }
    public static new FunctionCall Parse(Parser parser)
    {
        ref int offset = ref parser.offset;
        
        FunctionDeclaration? localFunc = parser.compilationUnit.LocalStorage.GetLocalFunc(parser.syntaxTokens[offset].PlainValue);
        if (localFunc == null)
            Error.Execute(parser.logger, ErrorDefaults.UnknownReference, parser.syntaxTokens[offset].Line);

        offset += 2; // skipping to passed argument or ')'
        List<VariableValue> passedVals = new List<VariableValue>();
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
        {
            VariableValue passedVal = VariableValue.Parse(parser, false);
            passedVals.Add(passedVal);

            if (!parser.SaveEquals(0, SyntaxKind.CommaToken) && parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
                Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

            offset += parser.SaveEquals(0, SyntaxKind.CloseParentheseToken) ? 0 : 1;
        }
        offset++; // skipping ')'
        return new FunctionCall(localFunc!, passedVals.ToArray());
    }
}