namespace Fluxcp;
public enum SyntaxKind
{
    //defaults
    BadToken = 0,

    //operators
    PlusToken,
    MinusToken,
    SlashToken,
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

    //logical operators
    IsEqualToken,
    IsLessToken,
    IsMoreToken,
    LogicalAndToken,
    LogicalOrToken,
    //keywords
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