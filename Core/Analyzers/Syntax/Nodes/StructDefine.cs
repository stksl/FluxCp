namespace Fluxcp;
public sealed class StructDefine : SyntaxNode
{
    public readonly string Name;
    public readonly Dictionary<string, StructField> Fields;
    public readonly Dictionary<string, FunctionDeclaration> Functions;
    public StructDefine(string name, StructField[] fields, FunctionDeclaration[] functions)
    {
        Name = name;
        Fields = new Dictionary<string, StructField>(fields.Length);
        // referencing to created hashmaps for easier interaction
        for(int i = 0; i < fields.Length; i++) 
        {
            Fields[fields[i].Name] = fields[i];
        }
        Functions = new Dictionary<string, FunctionDeclaration>(functions.Length);
        for(int i = 0; i < functions.Length; i++) 
        {
            Functions[functions[i].Name] = functions[i];
        }
    }
    public override IEnumerable<SyntaxNode> GetChildren()
    {
        foreach(var field in Fields) yield return field.Value;
        foreach(var func in Functions) yield return func.Value;
    }
}