namespace Fluxcp.Errors;
public sealed class Error 
{
    private readonly string message;
    public Error(string message_)
    {
        message = message_;
    }
    public static void Execute(ILogger logger, ErrorDefaults errId) 
    {
        logger.ShowError(errId.ToString());
        Environment.Exit((int)errId);
    }
}
public enum ErrorDefaults : int
{
    BaseError = -0x01,
    SemicolonExpected = -0xa,
    UnknownType = -0xff,
    UnknownDeclaration = -0x1c,
    UnknownReference = -0x4a,
    AlreadyDefined = -0xad
}