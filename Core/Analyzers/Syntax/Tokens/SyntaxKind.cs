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
    SemicolonToken,
    CommaToken,
    DotToken,
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
    //keywords
    StructDefineToken = 0xff,
    ReturnStatementToken,
    UseStatementToken,
    IfStatementToken,
    ElseStatementToken,
    ForStatementToken,
    WhileStatementToken,
    TrueToken,
    FalseToken,
}