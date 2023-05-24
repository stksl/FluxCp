namespace Fluxcp;
public sealed class StructDefine : SyntaxNode
{
    public readonly string Name;
    public Dictionary<string, StructField> Fields {get; internal set;}
    public Dictionary<string, FunctionDeclaration> Functions {get; internal set;}
    public StructDefine(string name)
    {
        Name = name;
        Fields = new Dictionary<string, StructField>();
        Functions = new Dictionary<string, FunctionDeclaration>();
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(var field in Fields) yield return field.Value;
        foreach(var func in Functions) yield return func.Value;
    }
    public override int GetHashCode()
    {
        return (int)GetDataType().TypeID;
    }
    public DataType GetDataType() 
    {
        return DataType.FromName(Name);
    }
}