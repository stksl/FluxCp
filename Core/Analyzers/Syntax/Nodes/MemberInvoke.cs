using Fluxcp.Errors;
namespace Fluxcp.Syntax;
// member invoking is a variable value. 
// Actually both. We can get a value and store to some var to set some value
public abstract class MemberInvoke : CopyValue 
{
    public MemberInvoke? NextInvoke {get; internal set;}
    public VariableValue? Value {get; internal set;}
    public bool SetterCalled => Value != null;
    public override string FromVar => GetFullName();
    public string GetName() 
    {
        string currName = "";
        if (this is FieldInvoke fieldinv) currName = fieldinv.FieldName;
        else if (this is FunctionInvoke funcinv) currName = funcinv.Call.FunctionName;
        return currName;
    }
    public string GetFullName() 
    {
        string name = GetName();
        return NextInvoke == null ? name : $"{name}.{NextInvoke.GetFullName()}";
    }
    public static new MemberInvoke Parse(Parser parser) 
    {
        ref int offset = ref parser.offset;
        // either a field or function invoke, but can be recursive
        // someVar.someField.someFieldField.someFunction().otherField;

        if (!parser.SaveEquals(0, SyntaxKind.TextToken))
            Error.Execute(parser.logger, ErrorDefaults.UnknownDeclaration, parser.syntaxTokens[offset].Line);
        MemberInvoke curr = null!;
        if (parser.SaveEquals(1, SyntaxKind.OpenParentheseToken)) 
        {
            curr = new FunctionInvoke(FunctionCall.Parse(parser));
        }
        else 
        {
            curr = new FieldInvoke(parser.syntaxTokens[offset].PlainValue);
            offset++;
        }

        if (parser.SaveEquals(0, SyntaxKind.DotToken)) 
        {
            offset++;
            curr.NextInvoke = Parse(parser);
        }
        //only as a setter
        else if (parser.SaveEquals(0, SyntaxKind.EqualsToken)) 
        {
            offset++;
            VariableValue value = VariableValue.Parse(parser);
            curr.Value = value;
        }
        return curr;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        if (NextInvoke != null)
            yield return NextInvoke;
    }
}
public sealed class FieldInvoke : MemberInvoke 
{
    public readonly string FieldName;
    public FieldInvoke(string fieldName)
    {
        FieldName = fieldName;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(SyntaxNode baseNode in base.GetChildren()) yield return baseNode;
    }
}
public sealed class FunctionInvoke : MemberInvoke 
{
    public readonly FunctionCall Call;
    public FunctionInvoke(FunctionCall call)
    {
        Call = call;
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(SyntaxNode baseNode in base.GetChildren()) yield return baseNode;

        yield return Call;
    }
}