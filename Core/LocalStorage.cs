using Fluxcp.Syntax;
namespace Fluxcp;
// a set of hashmaps for local types, methods and variables
public sealed class LocalStorage
{
    private Dictionary<string, StructDefine> localTypes;
    private Dictionary<string, VarDeclarationNode> localVars;
    private Dictionary<string, FunctionDeclaration> localFuncs;
    private Dictionary<string, LocalStorage> dependencies;
    public LocalStorage()
    {
        localTypes = new Dictionary<string, StructDefine>();
        localVars = new Dictionary<string, VarDeclarationNode>();
        localFuncs = new Dictionary<string, FunctionDeclaration>();

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
            localFuncs = dependency.localFuncs;
            localTypes = dependency.localTypes;
            foreach(var childDependency in dependency.dependencies) 
            {
                if (!AddDependency(childDependency.Key, childDependency.Value, true)) 
                {
                    return false;
                }
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
        localVars[varNode.Name] = varNode;
    }
    public VarDeclarationNode? GetLocalVar(string varName) 
    {
        localVars.TryGetValue(varName, out VarDeclarationNode? varNode);
        return varNode;
    }
    public void AddLocalType(StructDefine definedStruct) 
    {
        localTypes[definedStruct.GetDataType().TypeID.ToString()] = definedStruct;
    }
    public StructDefine? GetLocalType(DataType dataType) 
    {
        localTypes.TryGetValue(dataType.TypeID.ToString(), out StructDefine? typeNode);
        return typeNode;
    }
    public void AddLocalFunc(FunctionDeclaration function) 
    {
        localFuncs[function.Header.Name] = function;
    }
    public FunctionDeclaration? GetLocalFunc(string funcName) 
    {
        localFuncs.TryGetValue(funcName, out FunctionDeclaration? funcNode);
        return funcNode;
    }
    public bool RemoveLocalVar(string varName) 
    {
        return localVars.Remove(varName);
    }
    // just clearing locals up
    public void ClearStackFrame() 
    {
        localVars.Clear();
    }
    public bool RemoveLocalFunc(string funcName) 
    {
        return localFuncs.Remove(funcName);
    }
    public bool RemoveLocalType(DataType dataType) 
    {
        // no need for now
        throw new NotSupportedException();
    }
}