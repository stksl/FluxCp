using System.Collections.Immutable;
namespace Fluxcp;
public sealed class Parser 
{
    #region DI
    private readonly ImmutableArray<SyntaxToken> syntaxTokens;
    private readonly ILogger logger;
    #endregion
    private int offset;
    private object _sync = new Object();
    public Parser(ImmutableArray<SyntaxToken> syntaxTokens_, ILogger logger_) 
    {
        // ignoring whitespaces, comments and line's endings after lexing
        syntaxTokens = syntaxTokens_.Where(token => token.Kind != SyntaxKind.WhitespaceToken &&
            token.Kind != SyntaxKind.CommentToken && token.Kind != SyntaxKind.EndOfLineToken).ToImmutableArray();
        
        logger = logger_;
    }
    private bool SaveEquals(int offset, Func<SyntaxToken, bool> pred) 
    {
        return this.offset + offset < syntaxTokens.Length && pred(syntaxTokens[this.offset + offset]);
    }
    // simplifier
    private bool SaveEquals(int offset, SyntaxKind kind) 
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
        // adding dependencies
        while (SaveEquals(0, SyntaxKind.UseStatementToken))
        {
            // finding the last element of out tree, for now its only UseStatements.
            SyntaxNode node = tree.Root.Find(node => node.Next == null)!;

            if (SaveEquals(1, SyntaxKind.DoubleQuotesToken) 
                && SaveEquals(2, SyntaxKind.TextToken)
                && SaveEquals(3, SyntaxKind.DoubleQuotesToken)) 
            {
                node.Next = ParseUseStatement(syntaxTokens[offset + 2], true);
            }
            else if (SaveEquals(1, SyntaxKind.TextToken)) 
            {
                node.Next = ParseUseStatement(syntaxTokens[offset + 1], false);
            }
            else 
            {
                logger.ShowError("Unable to add a reference");
                Environment.Exit(-1);
            }
            offset++;
        }
        LoadTypes(tree);

        return tree;
    }
    // loading ALL the types in the project (including dependencies)
    private void LoadTypes(SyntaxTree tree) 
    {
        int prev = offset;
        List<StructDefine> definedStructs = new List<StructDefine>();
_checkNext:
        while (SaveEquals(0, node => node.Kind != SyntaxKind.StructDefineToken)) offset++;

        if (offset >= syntaxTokens.Length) 
        {
            offset = prev;

            // adding all found structs to the AST
            SyntaxNode last = tree.Root.Find(i => i.Next == null)!;
            for(int i = 0; i < definedStructs.Count; i++) 
            {
                last.Next = definedStructs[i];
                last = last.Next;
            }
            return;
        }

        definedStructs.Add(ParseStructDefine());

        goto _checkNext;
    }
    private StructDefine ParseStructDefine() 
    {
        if (SaveEquals(1, node => node.Kind != SyntaxKind.TextToken) || 
            SaveEquals(2, node => node.Kind != SyntaxKind.OpenBraceToken)) 
        {
            logger.ShowError("Uncorrect structure defining!");
            Environment.Exit(-1);
        }
        offset++; // skiping to structure's name

        string name = syntaxTokens[offset].PlainValue;

        List<StructField> fields = new List<StructField>();
        List<FunctionDeclaration> functions = new List<FunctionDeclaration>();

        offset += 2; // skiping after '{'
        while (SaveEquals(0, node => node.Kind != SyntaxKind.CloseBraceToken)) 
        {
            // if that's not correct field/function declaration
            if (SaveEquals(0, node => node.Kind != SyntaxKind.TextToken) || 
                SaveEquals(1, node => node.Kind != SyntaxKind.TextToken)) 
            {
                logger.ShowError("Uncorrect structure body!");
                Environment.Exit(-1);
            }

            // only function/field declaration
            string typeName = syntaxTokens[offset].PlainValue;
            ulong typeId = DataType.FromName(typeName);
            if (!LocalStorage.Items.ContainsKey(typeId)) 
            {
                logger.ShowError("Unknown type!");
                Environment.Exit(-1);
            }

            if (SaveEquals(2, SyntaxKind.SemicolonToken)) 
            {
                // that's field

                string fieldName = syntaxTokens[++offset].PlainValue;

                fields.Add(new StructField(fieldName, new DataType(typeId)));
                //skiping next semicolon and going after semicolon
                offset += 2;
            }
            else if (SaveEquals(1, SyntaxKind.OpenParentheseToken)) 
            {
                // prolly that's function
                offset--; // decrementing our offset to hit function name start token
                functions.Add(ParseFunctionDeclaration());
            }
            else 
            {
                logger.ShowError("Expected ';'");
                Environment.Exit(-1);
            }
        }
        // adding new type to RAM (9 bytes in total for every type)
        LocalStorage.Items[DataType.FromName(name)] = true;
        return new StructDefine(name, fields.ToArray(), functions.ToArray());
    }
    private FunctionDeclaration ParseFunctionDeclaration() 
    {
        string functionName = syntaxTokens[offset].PlainValue;


        return null!;
    }
    private UseStatement ParseUseStatement(SyntaxToken referenceToken, bool isPath) 
    {
        string reference = referenceToken.Text!.ToString(referenceToken.Offset, referenceToken.Length);
        if (isPath) reference = reference.Insert(0, "\"").Insert(reference.Length + 1, "\"");
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