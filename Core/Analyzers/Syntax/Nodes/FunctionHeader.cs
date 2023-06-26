using Fluxcp.Errors;

namespace Fluxcp.Syntax;
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
    public static new FunctionHeader Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        if (!parser.SaveEquals(0, SyntaxKind.TextToken) ||
            !parser.SaveEquals(1, SyntaxKind.TextToken) ||
            !parser.SaveEquals(2, SyntaxKind.OpenParentheseToken))
        {
            Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        }

        DataType returnType = DataType.FromName(parser.syntaxTokens[offset].PlainValue);
        string functionName = parser.syntaxTokens[++offset].PlainValue;

        offset += 2; // going after '(' 

        Dictionary<string, FunctionArgument> args = new Dictionary<string, FunctionArgument>();
        while (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
        {
            if (!parser.SaveEquals(0, SyntaxKind.TextToken) || !parser.SaveEquals(1, SyntaxKind.TextToken))
                Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

            else if (args.ContainsKey(parser.syntaxTokens[offset + 1].PlainValue))
                Error.Execute(parser.cUnit.Logger, ErrorDefaults.AlreadyDefined, parser.syntaxTokens[offset + 1].Line);

            FunctionArgument arg = FunctionArgument.Parse(parser);
            args[arg.Name] = arg;

            if (parser.SaveEquals(0, node => node.Kind != SyntaxKind.CommaToken) &&
                !parser.SaveEquals(0, SyntaxKind.CloseParentheseToken))
                Error.Execute(parser.cUnit.Logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);

            offset += parser.SaveEquals(0, SyntaxKind.CloseParentheseToken) ? 0 : 1; // going to the next argument, skipping ','
        }
        offset++; // skiping ')'
        FunctionArgument[] argsArr = args.Select(i => i.Value).ToArray();
        return new FunctionHeader(returnType, functionName, argsArr);
    }
}