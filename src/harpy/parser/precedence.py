from enum import Enum


class Precedence(Enum):
    """Defines the different precedence levels used by the infix parsers. These
    determine how a series of infix expressions will be grouped. For example,
    `a + b * c - d` will be parsed as `(a + (b * c)) - d` because `*` has higher
    precedence than `+` and `-`. Here, bigger numbers mean higher precedence.
    Ordered in increasing precedence.
    """

    NONE = 0
    ASSIGNMENT = 1
    SUMEQ = 2
    MULTEQ = 3
    EXPEQ = 4
    OR = 5
    AND = 6
    EQRELATION = 7
    ORDRELATION = 8
    SUM = 9
    PRODUCT = 10
    EXPONENT = 11
    PREFIX = 12
    POSTFIX = 13
    INDEX = 14
    CALL = 15
    ACCESS = 16
