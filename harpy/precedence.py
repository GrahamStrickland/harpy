from enum import Enum


class Precedence(Enum):
    """Defines the different precedence levels used by the infix parsers. These
    determine how a series of infix expressions will be grouped. For example,
    `a + b * c - d` will be parsed as `(a + (b * c)) - d` because `*` has higher
    precedence than `+` and `-`. Here, bigger numbers mean higher precedence.
    Ordered in increasing precedence.
    """

    ASSIGNMENT = 1
    SUMEQ = 2
    MULTEQ = 3
    EXPEQ = 4
    EQRELATION = 5
    ORDRELATION = 6
    SUM = 7
    PRODUCT = 8
    EXPONENT = 9
    PREFIX = 10
    POSTFIX = 11
    CALL = 12
