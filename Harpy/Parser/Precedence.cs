namespace Harpy.Parser;

/// <summary>
///     Defines the different precedence levels used by the infix parsers. These
///     determine how a series of infix expressions will be grouped. For example,
///     <c>a + b * c - d</c> will be parsed as <c>(a + (b * c)) - d</c> because <c>*</c> has higher
///     precedence than <c>+</c> and <c>-</c>. Here, bigger numbers mean higher precedence.
/// </summary>
public enum Precedence
{
    NONE = 0,
    ASSIGNMENT,
    SUMEQ,
    MULTEQ,
    EXPEQ,
    OR,
    AND,
    EQRELATION,
    ORDRELATION,
    SUM,
    PRODUCT,
    EXPONENT,
    PREFIX,
    POSTFIX,
    INDEX,
    CALL,
    ACCESS
}