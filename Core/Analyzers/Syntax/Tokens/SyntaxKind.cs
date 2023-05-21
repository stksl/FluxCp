namespace Fluxcp;
public enum SyntaxKind
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
    IsLessToken,
    IsMoreToken,
    LogicalAndToken,
    LogicalOrToken,
    //keywords
    StructDefineToken,
    ReturnStatementToken,
    UseStatementToken,
    IfStatementToken,
    ElseStatementToken,
    ForLoopToken,
    WhileLoopToken,
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
}