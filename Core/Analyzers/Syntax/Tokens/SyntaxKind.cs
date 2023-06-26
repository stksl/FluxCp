namespace Fluxcp.Syntax;
public enum SyntaxKind : uint
{
    //defaults
    BadToken = 0,

    //operators
    PlusToken,
    MinusToken,
    SlashToken,
    BackSlashToken,
    StarToken,
    PercentToken,
    EqualsToken,
    OpenParentheseToken,
    CloseParentheseToken,
    OpenBraceToken,
    CloseBraceToken,
    OpenBracketToken,
    CloseBracketToken,
    ColonToken,
    SemicolonToken,
    CommaToken,
    DotToken,
    CastToken,
    //logical operators
    IsEqualToken,
    IsNotEqualToken,
    IsLessToken,
    IsMoreToken,
    LogicalAndToken,
    LogicalOrToken,
    // boundaries
    EndOfLineToken,
    StartOfFileToken,
    EndOfFileToken,

    //others
    NumberToken,
    WhitespaceToken,
    CommentToken,
    TextToken,
    SingleQuoteToken,
    DoubleQuotesToken,
    UnderscoreToken,
    //keywords
    StructDefineToken = 0xff,
    ReturnStatementToken,
    UseStatementToken,
    IfStatementToken,
    ElseStatementToken,
    ForStatementToken,
    WhileStatementToken,
    ImportLibStatement,
    TrueToken,
    FalseToken,
}