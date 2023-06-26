namespace Fluxcp.Errors;
public sealed class Error 
{
    #region DI
    private readonly ILogger? logger;
    private readonly ErrorDefaults Id;
    private readonly int line;
    #endregion
    private Error(ErrorDefaults id, ILogger? logger_, int line_)
    {
        Id = id;
        logger = logger_;
        line = line_;
    }
    public void Throw() 
    {
        logger?.ShowError($"line {line}: " + Id.ToString());
        Environment.Exit((int)Id);
    }
    public static void Execute(ILogger? logger, ErrorDefaults errId, int line) 
    {
        new Error(errId, logger, line).Throw();
    }
}
public enum ErrorDefaults : int
{
    BaseError = -0x01,
    SemicolonExpected = -0xa,
    UnknownType = -0xff,
    UnknownDeclaration = -0x1c,
    UnknownReference = -0x4a,
    AlreadyDefined = -0xad,
    OutOfScope = -0xbb,
    NoEntryFound = -0x0ef,
    UnableToCast = -0xce

}