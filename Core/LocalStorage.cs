using Fluxcp.Syntax;
namespace Fluxcp;
// a set of hashmaps for local types, methods and variables
public sealed class LocalStorage
{
    private readonly IDictionary<string, object> Locals;
    private IDictionary<string, LocalStorage> dependencies;
    public LocalStorage()
    {
        Locals = new Dictionary<string, object>();
        dependencies = new Dictionary<string, LocalStorage>();
    }
    // adding a dependency to our local storage, include - to include in the main storage
    public bool AddDependency(string path, LocalStorage dependency, bool include) 
    {
        if (dependencies.ContainsKey(path))
            return false;
        dependencies[path] = dependency;
        if (include) 
        {
            foreach(KeyValuePair<string, object> item in dependency.Locals) 
            {
                Locals[item.Key] = item.Value;
            }
        }
        return true;
    }
    public LocalStorage? GetDependency(string path) 
    {
        return dependencies.ContainsKey(path) ? dependencies[path] : null;
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
    public bool RemoveLocalVar(string varName) 
    {
        return Locals.Remove(LocalStorageFacts.LocalVar + varName);
    }
    public bool RemoveLocalFunc(string funcName) 
    {
        return Locals.Remove(LocalStorageFacts.LocalFunc + funcName);
    }
    public bool RemoveLocalType(DataType dataType) 
    {
        // no need for now
        throw new NotSupportedException();
    }
}