namespace Fluxcp;
public interface ILogger 
{
    // debug info only
    void ShowDebug(string msg);
    // display message (maybe to the stdout)
    void ShowMsg(string msg);
    // display warning (maybe to the stdout)
    void ShowWarning(string msg);
    // display error that stopped FluxCp compiler. (maybe to the stdout)
    void ShowError(string msg);   
}