from .token import Token
from .token_type import TokenType


class Lexer:
    """A very primitive lexer. Takes a string and splits it into a series of
    Tokens. Operators and punctuation are mapped to unique keywords. Names,
    which can be any series of letters, are turned into NAME tokens. All other
    characters are ignored (except to separate names). Numbers and strings are
    not supported. This is really just the bare minimum to give the parser
    something to work with.
    """

    _index: int
    _text: str
    _punctuators: dict[str, TokenType]

    def __init__(self, text: str):
        """Creates a new Lexer to tokenize the given string.

        Args:
            text (str): String to tokenize.
        """

        self._index = 0
        self._text = text
        self._punctuators = {}

        # Register all the TokenTypes that are explicit punctuators.
        for type in TokenType:
            punctuator = type.punctuator()
            if punctuator is not None:
                self._punctuators[punctuator] = type

    def __iter__(self):
        return self

    def __next__(self):
        while self._index < len(self._text):
            c = self._text[self._index]
            self._index += 1

            if c in self._punctuators:
                if (
                    c == ":"
                    and len(self._text) > self._index
                    and self._text[self._index] == "="
                ):
                    self._index += 1
                    return Token(TokenType.ASSIGN, ":=")

                # Handle punctuation.
                return Token(self._punctuators[c], c)
            elif c.isalpha():
                # Handle names.
                start = self._index - 1
                while self._index < len(self._text):
                    if not self._text[self._index].isalpha():
                        break
                    self._index += 1

                name = self._text[start : self._index]
                return Token(TokenType.NAME, name)
            else:
                # Ignore all other characters (whitespace, etc.)
                pass

        # Once we've reached the end of the string, just return EOF tokens. We'll
        # just keep returning them as many times as we're asked so that the
        # parser's lookahead doesn't have to worry about running out of tokens.
        return Token(TokenType.EOF, "")
