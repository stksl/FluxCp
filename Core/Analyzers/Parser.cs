using System.Collections.Immutable;
using Fluxcp.Errors;
using Fluxcp.Syntax;
namespace Fluxcp;
public sealed class Parser
{
    #region DI
    internal readonly ImmutableArray<SyntaxToken> syntaxTokens;
    internal readonly ILogger? logger;
    internal readonly CompilationUnit compilationUnit;
    #endregion
    internal int offset;
    private object _sync = new object();
    public Parser(ImmutableArray<SyntaxToken> syntaxTokens_, ILogger? logger_, CompilationUnit compilationUnit_)
    {
        // see todo.txt for more info on that. will be fixed
        syntaxTokens = syntaxTokens_.Where(token => token.Kind != SyntaxKind.WhitespaceToken &&
            token.Kind != SyntaxKind.CommentToken && token.Kind != SyntaxKind.EndOfLineToken).ToImmutableArray();

        logger = logger_;
        compilationUnit = compilationUnit_;
    }
    internal bool SaveEquals(int offset, Func<SyntaxToken, bool> pred)
    {
        return this.offset + offset < syntaxTokens.Length && pred(syntaxTokens[this.offset + offset]);
    }
    // simplifier
    internal bool SaveEquals(int offset, SyntaxKind kind)
    {
        return SaveEquals(offset, i => i.Kind == kind);
    }
    public SyntaxTree Parse()
    {
        lock (_sync)
        {
            return BuildAST();
        }
    }
    private SyntaxTree BuildAST()
    {
        SyntaxTree tree = new SyntaxTree();
        SyntaxNode last = tree.Root;
        // adding dependencies
        while (SaveEquals(0, SyntaxKind.UseStatementToken))
        {
            // finding the last element of out tree, for now its only UseStatements.
            last.Next = UseStatement.Parse(this);
            last = last.Next;
        }

        while (offset < syntaxTokens.Length - 1)
        {
            // parsing only functons or structs
            if (SaveEquals(0, SyntaxKind.StructDefineToken))
            {
                last.Next = StructDefine.Parse(this);
                last = last.Next;
            }
            else if (SaveEquals(0, SyntaxKind.TextToken) &&
                SaveEquals(1, SyntaxKind.TextToken) &&
                SaveEquals(2, SyntaxKind.OpenParentheseToken))
            {
                if (compilationUnit.LocalStorage.GetLocalFunc(syntaxTokens[offset + 1].PlainValue) != null)
                    Error.Execute(logger, ErrorDefaults.AlreadyDefined, syntaxTokens[offset + 1].Line);
                
                compilationUnit.LocalStorage.AddLocalFunc(new FunctionDeclaration(
                    new FunctionHeader(
                        null!, syntaxTokens[offset + 1].PlainValue, null!
                    ),
                    null!
                )); // for now, the thing is we have to allow recursion

                FunctionDeclaration function = FunctionDeclaration.Parse(this);
                compilationUnit.LocalStorage.AddLocalFunc(function);
                last.Next = function;
                last = last.Next;
            }
            else
            {
                Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);
            }
        }
        return tree;
    }
    private StructDefine ParseStructDefine()
    {
        if (SaveEquals(1, node => node.Kind != SyntaxKind.TextToken) ||
            SaveEquals(2, node => node.Kind != SyntaxKind.OpenBraceToken))
        {
            Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);
        }
        offset++; // skiping to structure's name

        string name = syntaxTokens[offset].PlainValue;
        DataType structType = DataType.FromName(name);

        if (structType.IsTypeDefined(compilationUnit))
            Error.Execute(logger, ErrorDefaults.AlreadyDefined, syntaxTokens[offset].Line);

        // for the recursive references
        StructDefine currStruct = new StructDefine(name);
        compilationUnit.LocalStorage.AddLocalType(currStruct);

        offset += 2; // skiping after '{'
        while (SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken))
        {
            // if that's not correct field/function declaration
            if (SaveEquals(0, node => node.Kind != SyntaxKind.TextToken) ||
                SaveEquals(1, node => node.Kind != SyntaxKind.TextToken))
            {
                Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);
            }

            // only function/field declaration
            string typeName = syntaxTokens[offset].PlainValue;
            if (!DataType.FromName(typeName).IsTypeDefined(compilationUnit))
            {
                Error.Execute(logger, ErrorDefaults.UnknownType, syntaxTokens[offset].Line);
            }

            if (SaveEquals(2, SyntaxKind.SemicolonToken))
            {
                // that's a field

                string fieldName = syntaxTokens[++offset].PlainValue;
                currStruct.Fields[fieldName] = new StructField(fieldName, DataType.FromName(typeName));
                //skiping next semicolon and going after semicolon
                offset += 2;
            }
            else if (SaveEquals(2, SyntaxKind.OpenParentheseToken))
            {
                // that's a function
                FunctionDeclaration function = ParseFunctionDeclaration();
                currStruct.Functions[function.Header.Name] = function;
            }
            else
            {
                Error.Execute(logger, ErrorDefaults.SemicolonExpected, syntaxTokens[offset].Line);
            }
        }
        offset++; // skipping '}'

        return currStruct;
    }
    private FunctionDeclaration ParseFunctionDeclaration()
    {
        FunctionHeader header = ParseFunctionHeader();
        FunctionBodyBound bodyBound = ParseFunctionBody(header);

        return new FunctionDeclaration(header, bodyBound);
    }
    private FunctionHeader ParseFunctionHeader()
    {
        if (!SaveEquals(0, SyntaxKind.TextToken) ||
            !SaveEquals(1, SyntaxKind.TextToken) ||
            !SaveEquals(2, SyntaxKind.OpenParentheseToken))
        {
            Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);
        }
        else if (!DataType.FromName(syntaxTokens[offset].PlainValue).IsTypeDefined(compilationUnit))
        {
            Error.Execute(logger, ErrorDefaults.UnknownType, syntaxTokens[offset].Line);
        }

        DataType returnType = DataType.FromName(syntaxTokens[offset].PlainValue);
        string functionName = syntaxTokens[++offset].PlainValue;

        offset += 2; // going after '(' 

        Dictionary<string, FunctionArgument> args = new Dictionary<string, FunctionArgument>();
        while (SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken))
        {
            if (!SaveEquals(0, SyntaxKind.TextToken) || !SaveEquals(1, SyntaxKind.TextToken))
                Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);

            else if (!DataType.FromName(syntaxTokens[offset].PlainValue).IsTypeDefined(compilationUnit))
                Error.Execute(logger, ErrorDefaults.UnknownType, syntaxTokens[offset].Line);
            else if (args.ContainsKey(syntaxTokens[offset + 1].PlainValue))
                Error.Execute(logger, ErrorDefaults.AlreadyDefined, syntaxTokens[offset + 1].Line);

            DataType argType = DataType.FromName(syntaxTokens[offset].PlainValue);
            string argName = syntaxTokens[++offset].PlainValue;

            args[argName] = new FunctionArgument(argName, argType);

            if (SaveEquals(1, node => node.Kind != SyntaxKind.CommaToken) &&
                !SaveEquals(1, SyntaxKind.CloseParentheseToken))
                Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);

            offset += SaveEquals(1, SyntaxKind.CloseParentheseToken) ? 1 : 2; // going to the next argument, skipping ','
        }
        offset++; // skiping ')'
        FunctionArgument[] argsArr = args.Select(i => i.Value).ToArray();
        return new FunctionHeader(returnType, functionName, argsArr);
    }
    private FunctionBodyBound ParseFunctionBody(FunctionHeader header)
    {
        if (!SaveEquals(0, SyntaxKind.OpenBraceToken))
        {
            Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);
        }
        offset++; // skipping '{'
        FunctionBodyBound body = new FunctionBodyBound(offset);
        SyntaxNode last = body.Child;
        while (SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken))
        {
            last.Next = ParseSyntaxNode(header);
            last = last.Next;
        }
        body.Length = offset - body.Position;

        offset++; // skipping '}'
        return body;
    }
    // main method for parsing syntax nodes in functions
    private SyntaxNode ParseSyntaxNode(FunctionHeader header)
    {
        SyntaxNode node = null!;
        if (SaveEquals(0, SyntaxKind.TextToken) && SaveEquals(1, SyntaxKind.TextToken))
        {
            DataType varType = DataType.FromName(syntaxTokens[offset].PlainValue);
            string varName = syntaxTokens[offset + 1].PlainValue;

            // variable declaration
            if (compilationUnit.LocalStorage.GetLocalType(varType) == null)
            {
                Error.Execute(logger, ErrorDefaults.UnknownType, syntaxTokens[offset].Line);
            }
            else if (compilationUnit.LocalStorage.GetLocalVar(varName) != null || header.Args.FirstOrDefault(arg => arg.Name == varName) != null)
            {
                Error.Execute(logger, ErrorDefaults.AlreadyDefined, syntaxTokens[offset + 1].Line);
            }

            // saving local variable name and level
            node = new VarDeclarationNode(varName, 0, varType, null); // 0 as level FOR NOW!
            compilationUnit.LocalStorage.AddLocalVar((VarDeclarationNode)node);

            offset += SaveEquals(2, SyntaxKind.EqualsToken) ? 1 : 2; // skipping to var declaration/semicolon
        }
        if (SaveEquals(0, SyntaxKind.TextToken) && SaveEquals(1, SyntaxKind.EqualsToken))
        {
            int prev = offset;
            offset += 2;
            VariableValue value = ParseVariableValue();
            if (node is VarDeclarationNode varNode)
            {
                // setting init value for simplicity (the only time that parser sets variable value directly)
                varNode.Value = value;
            }
            else node = value;
        }
        else if (SaveEquals(0, SyntaxKind.TextToken) && SaveEquals(1, SyntaxKind.OpenParentheseToken))
        {
            // probably function call
            FunctionDeclaration? localFunc = compilationUnit.LocalStorage.GetLocalFunc(syntaxTokens[offset].PlainValue);
            if (localFunc == null)
                Error.Execute(logger, ErrorDefaults.UnknownReference, syntaxTokens[offset].Line);
            
            offset += 2; // skipping to passed argument or ')'
            List<VariableValue> passedVals = new List<VariableValue>();
            while (SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken)) 
            {
                VariableValue passedVal = ParseVariableValue();
                passedVals.Add(passedVal);

                if (!SaveEquals(0, SyntaxKind.CommaToken) && SaveEquals(0, node => node.Kind != SyntaxKind.CloseParentheseToken)) 
                    Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);

                offset += SaveEquals(0, SyntaxKind.CloseParentheseToken) ? 0 : 1;
            }
            offset++; // skipping ')'
            node = new FunctionCall(localFunc!, passedVals.ToArray());
        }
        else if (SaveEquals(0, node => SyntaxFacts.IsKeyword(node.Kind)))
        {
            // keywords
            node = ParseKeywords();
        }

        // all of the statements before have to skip current token to semicolon (expected)
        if (!SaveEquals(0, SyntaxKind.SemicolonToken))
            Error.Execute(logger, ErrorDefaults.SemicolonExpected, syntaxTokens[offset].Line);

        offset++; // skipping ';'
        return node!;
    }
    private SyntaxNode ParseKeywords()
    {

        return null!;
    }
    private VariableValue ParseVariableValue()
    {
        return ParseLiteralVarValue();
    }
    private VariableValue ParseLiteralVarValue() 
    {
        // number literal
        if (SaveEquals(0, SyntaxKind.NumberToken))
        {
            return new LiteralValue(syntaxTokens, offset++, 1); // for now without floating-point numbers
        }
        // string literal
        else if (SaveEquals(0, SyntaxKind.DoubleQuotesToken))
        {
            int prev = offset;
            while (SaveEquals(1, node => node.Kind != SyntaxKind.DoubleQuotesToken))
            {
                offset++;
            }
            offset += 2; // skipping double quote token
            return new LiteralValue(syntaxTokens, prev, (offset - 1) - prev);
        }
        // character literal
        else if (SaveEquals(0, SyntaxKind.SingleQuoteToken) && SaveEquals(2, SyntaxKind.SingleQuoteToken))
        {
            offset += 3; // skipping character literals and value
            return new LiteralValue(syntaxTokens, offset, 2);
        }
        // pipeline
        return ParseCopyVarValue();
    }
    private VariableValue ParseCopyVarValue() 
    {
        // 0 is copyFrom and -2 is copyTo
        // (-2)copyTo = (0)copyFrom;
        if (!SaveEquals(0, SyntaxKind.TextToken) && !SaveEquals(-2, SyntaxKind.TextToken)) 
            Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);
        
        VarDeclarationNode? copyFrom = compilationUnit.LocalStorage.GetLocalVar(syntaxTokens[offset].PlainValue);
        VarDeclarationNode? copyTo = compilationUnit.LocalStorage.GetLocalVar(syntaxTokens[offset - 2].PlainValue);
        if (copyFrom == null || copyTo == null)
            Error.Execute(logger, ErrorDefaults.UnknownReference, syntaxTokens[offset].Line);
        offset++; // skipping name
        return new CopyValue(copyFrom!, copyTo!);
    }
    private UseStatement ParseUseStatement()
    {
        if (!SaveEquals(1, SyntaxKind.TextToken) && !SaveEquals(1, SyntaxKind.DoubleQuotesToken))
        {
            Error.Execute(logger, ErrorDefaults.UnknownDeclaration, syntaxTokens[offset].Line);

        }
        string reference;
        offset++; // skipping to text/doubleQuote
        if (syntaxTokens[offset].Kind == SyntaxKind.DoubleQuotesToken)
        {
            reference = "\"";
            while (syntaxTokens[offset + 1].Kind != SyntaxKind.DoubleQuotesToken)
            {
                reference += syntaxTokens[++offset].PlainValue;
            }
            reference += "\"";
            offset += 3; // skipping quote and semicolon (expected)
        }
        else
        {
            reference = syntaxTokens[offset].PlainValue;
            offset += 2; // skipping semicolon (expected)
        }

        if (!SaveEquals(-1, SyntaxKind.SemicolonToken))
            Error.Execute(logger, ErrorDefaults.SemicolonExpected, syntaxTokens[offset].Line);

        return new UseStatement(reference);
    }
    /* public int EvaluateExpression() 
    {
        Stack<SyntaxKind> ops = new Stack<SyntaxKind>();
        Stack<int> vals = new Stack<int>();

        for(int i = 0; i < SyntaxTokens.Count; i++) 
        {
            switch(SyntaxTokens[i].Kind) 
            {
                case SyntaxKind.OpenParentheseToken:
                    ops.Push(SyntaxTokens[i].Kind);
                    break;
                case SyntaxKind.CloseParentheseToken:
                    while (ops.Peek() != SyntaxKind.OpenParentheseToken) 
                    {
                        vals.Push(DoOperation(vals.Pop(), vals.Pop(), ops.Pop()));
                    }
                    ops.Pop();
                    break;
                case SyntaxKind.NumberToken:
                    vals.Push(((NumberToken)SyntaxTokens[i]).Value);
                    break;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.StarToken:
                    while (ops.Count > 0 && CheckPriority(SyntaxTokens[i].Kind, ops.Peek())) 
                    {
                        vals.Push(DoOperation(vals.Pop(), vals.Pop(), ops.Pop()));
                    }
                    ops.Push(SyntaxTokens[i].Kind);
                    break;
                default:
                    break;
            }
        }
        while (ops.Count > 0) 
        {
            vals.Push(DoOperation(vals.Pop(), vals.Pop(), ops.Pop()));
        }
        return vals.Pop();
    }
    // returns whether oper1 operator priority is less or same as oper2
    private bool CheckPriority(SyntaxKind oper1, SyntaxKind oper2) 
    {
        if ((oper1 == SyntaxKind.StarToken || oper1 == SyntaxKind.SlashToken) &&
            (oper2 == SyntaxKind.PlusToken || oper2 == SyntaxKind.MinusToken)) 
            return false;
        else if (oper2 == SyntaxKind.OpenParentheseToken || oper2 == SyntaxKind.CloseParentheseToken) 
            return false;
        
        return true;
    }
    private int DoOperation(int op2, int op1, SyntaxKind operator_) 
    {
        switch(operator_) 
        {
            case SyntaxKind.PlusToken:
                return op1 + op2;
            case SyntaxKind.MinusToken:
                return op1 - op2;
            case SyntaxKind.StarToken:
                return op1 * op2;
            case SyntaxKind.SlashToken:
                return op1 / op2;

        }
        return 0;
    } */
}