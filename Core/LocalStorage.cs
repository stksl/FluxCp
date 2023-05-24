namespace Fluxcp;
// a set of hashmaps for local types, methods and variables
public sealed class LocalStorage
{
    private readonly IDictionary<string, object> Locals;
    public LocalStorage()
    {
        Locals = new Dictionary<string, object>();
    }
    public void AddLocalVar(VarDeclarationNode varNode) 
    {
        Locals[LocalStorageFacts.LocalVar + varNode.Name] = varNode;
    }
    public VarDeclarationNode? GetLocalVar(string varName) 
    {
        Locals.TryGetValue(LocalStorageFacts.LocalVar + varName, out object? varNode);
        return varNode as VarDeclarationNode;
    }
    public void AddLocalType(StructDefine definedStruct) 
    {
        Locals[LocalStorageFacts.LocalType + DataType.FromName(definedStruct.Name).TypeID] = definedStruct;
    }
    public StructDefine? GetLocalType(DataType dataType) 
    {
        Locals.TryGetValue(LocalStorageFacts.LocalType + dataType.TypeID, out object? typeNode);
        return typeNode as StructDefine;
    }
    public void AddLocalFunc(FunctionDeclaration function) 
    {
        Locals[LocalStorageFacts.LocalFunc + function.Header.Name] = function;
    }
    public FunctionDeclaration? GetLocalFunc(string funcName) 
    {
        Locals.TryGetValue(LocalStorageFacts.LocalFunc + funcName, out object? funcNode);
        return funcNode as FunctionDeclaration;
    }
}