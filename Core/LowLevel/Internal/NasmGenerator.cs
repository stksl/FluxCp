using System.Text;

namespace Fluxcp.LowLevel;
internal sealed class NasmGenerator : IGenerator
{
    private readonly Stream strout;
    private readonly CompilationUnit cUnit;
    private EventWaitHandle waitHandle;
    private StringBuilder currInst;
    private int offset;
    public NasmGenerator(Stream strout_, CompilationUnit cUnit_) 
    {
        strout = strout_;
        cUnit = cUnit_;

        waitHandle = new ManualResetEvent(true);
        currInst = new StringBuilder();
        // for now no .data section 
        currInst.Append("\n"+
        ".text\n" + 
        "global main\n" + 
        "main:");
        ExecuteBase();
    }
    // creates a stack frame with initial offset for return type and arguments
    public NasmStackFrame CreateStackFrame(uint retSize, params uint[] argsSize) 
    {
        currInst.Append("\n" +
        "push rbp\n" + 
        "mov rbp, rsp\n" + 
        $"sub rsp, {retSize + argsSize.Sum(i => i)}");
        ExecuteBase();
        return new NasmStackFrame(retSize, argsSize);
    }
    // allocates space on stack (substracts local var type size)
    public void AllocSizeForLocal(uint lsize, NasmStackFrame sFrame) 
    {
        currInst.Append("\n" +
        $"sub rsp, {lsize}");
        ExecuteBase();
        sFrame.Locals.Add(lsize);
    } 
    // removes a current stack frame
    public void RemoveStackFrame() 
    {
        currInst.Append("\n" +
        "mov rsp, rbp\n" + 
        "pop rbp");
        ExecuteBase();
    }
    // update local variable in current stack frame (offset 0 - return var, 1 - local or argument etc.)
    public void UpdateLocal(uint offset, byte[] data, int insertPos, int insertLen, NasmStackFrame sFrame) 
    {
        // todo
        currInst.Append("\n" +
        "mov ");
        ExecuteBase();
    }
    private void ExecuteBase() 
    {
        waitHandle.WaitOne();
        waitHandle.Reset();

        strout.BeginWrite(Encoding.UTF8.GetBytes(currInst.ToString()), offset, currInst.Length, AsyncCallBack, true);
        offset += currInst.Length;
        currInst.Clear();
    }
    private void AsyncCallBack(IAsyncResult ar) 
    {
        if (ar.AsyncState is bool b && b) 
        {
            waitHandle.Set(); // unblocking handle
        }
    }   
}