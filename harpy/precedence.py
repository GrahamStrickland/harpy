from enum import Enum


class Precedence(Enum):
    """Defines the different precedence levels used by the infix parsers. These
    determine how a series of infix expressions will be grouped. For example,
    `a + b * c - d` will be parsed as `(a + (b * c)) - d` because `*` has higher
    precedence than `+` and `-`. Here, bigger numbers mean higher precedence.
    Ordered in increasing precedence.
    """

    ASSIGNMENT = 1
    SUM = 2
    PRODUCT = 3
    EXPONENT = 4
    PREFIX = 5
    POSTFIX = 6
    CALL = 7
