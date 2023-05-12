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
    BitwiseOrToken,
    BitwiseAndToken,
    BitwiseXorToken,
    BitwiseShiftToken,
    PercentToken,

    //expressions
    BinaryExpression,

    // boundaries
    EndOfLineToken,
    StartOfFileToken,
    EndOfFileToken,

    //others
    NumberToken,
    OpenParentheseToken,
    CloseParentheseToken,
    WhitespaceToken,
}