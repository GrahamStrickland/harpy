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

            match c:
                case "/":
                    # Potential comments.
                    match self._peek():
                        case "/":
                            return self._read_line_comment()
                        case "*":
                            return self._read_block_comment()
                        case _:
                            return Token(self._punctuators[c], c)
                case ":":
                    # Assignment operator.
                    if self._peek() == "=":
                        self._advance()
                        return Token(TokenType.ASSIGN, ":=")
                    else:
                        return Token(self._punctuators[c], c)
                case "=":
                    # Equality relation.
                    if self._peek() == "=":
                        self._advance()
                        return Token(TokenType.EQ, "==")
                    else:
                        # TODO: Find out if this is valid.
                        raise SyntaxError("'=' is not a valid Harbour token")
                case "<":
                    if self._peek() == "=":
                        self._advance()
                        return Token(TokenType.LE, "<=")
                    else:
                        return Token(TokenType.LT, c)
                case ">":
                    if self._peek() == "=":
                        self._advance()
                        return Token(TokenType.GE, ">=")
                    else:
                        return Token(TokenType.GT, c)
                case _:
                    if c in self._punctuators:
                        # Handle punctuation.
                        return Token(self._punctuators[c], c)
                    elif c.isalpha():
                        # Handle names.
                        return self._read_name()
                    else:
                        # Ignore all other characters (whitespace, etc.)
                        pass

        # Once we've reached the end of the string, just return EOF tokens. We'll
        # just keep returning them as many times as we're asked so that the
        # parser's lookahead doesn't have to worry about running out of tokens.
        return Token(TokenType.EOF, "\0")

    def _read_line_comment(self) -> Token:
        start = self._index - 1

        # Consume second '/'.
        self._advance()

        while True:
            match self._peek():
                case "\n" | "\r" | "\0":
                    return Token(
                        TokenType.LINE_COMMENT, self._text[start : self._index]
                    )
                case _:
                    self._advance()

    def _read_block_comment(self) -> Token:
        start = self._index - 1

        while True:
            match self._advance():
                case "*":
                    match self._advance():
                        case "/":
                            return Token(
                                TokenType.BLOCK_COMMENT, self._text[start : self._index]
                            )
                        case "\0":
                            raise SyntaxError("Unterminated block comment.")
                        case _:
                            pass  # Do nothing, keep advancing.
                case "\0":
                    raise SyntaxError("Unterminated block comment.")
                case _:
                    pass  # Do nothing, keep advancing.

    def _read_name(self) -> Token:
        start = self._index - 1
        while self._index < len(self._text):
            if not self._text[self._index].isalpha():
                break
            self._index += 1

        name = self._text[start : self._index]
        return Token(TokenType.NAME, name)

    def _peek(self) -> str:
        if len(self._text) > self._index:
            return self._text[self._index]
        else:
            return "\0"

    def _advance(self) -> str:
        c = "\0"

        if len(self._text) > self._index:
            c = self._text[self._index]
            if len(self._text) > self._index:
                self._index += 1

        return c
