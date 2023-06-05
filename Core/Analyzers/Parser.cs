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
    /* public int EvaluateExpression() 
    {
        Stack<SyntaxKind> ops = new Stack<SyntaxKind>();
        Stack<int> vals = new Stack<int>();

        for(int i = 0; i < syntaxTokens.Length; i++) 
        {
            switch(syntaxTokens[i].Kind) 
            {
                case SyntaxKind.OpenParentheseToken:
                    ops.Push(syntaxTokens[i].Kind);
                    break;
                case SyntaxKind.CloseParentheseToken:
                    while (ops.Peek() != SyntaxKind.OpenParentheseToken) 
                    {
                        vals.Push(DoOperation(vals.Pop(), vals.Pop(), ops.Pop()));
                    }
                    ops.Pop();
                    break;
                case SyntaxKind.NumberToken:
                    vals.Push(0); // for now
                    break;
                case SyntaxKind.PlusToken:
                case SyntaxKind.MinusToken:
                case SyntaxKind.SlashToken:
                case SyntaxKind.StarToken:
                    while (ops.Count > 0 && CheckPriority(syntaxTokens[i].Kind, ops.Peek())) 
                    {
                        vals.Push(DoOperation(vals.Pop(), vals.Pop(), ops.Pop()));
                    }
                    ops.Push(syntaxTokens[i].Kind);
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