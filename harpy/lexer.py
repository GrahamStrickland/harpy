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
    _keywords: dict[str, TokenType]
    _simple_operators: dict[str, TokenType]
    _compound_operatos: dict[str, TokenType]

    def __init__(self, text: str):
        """Creates a new Lexer to tokenize the given string.

        Args:
            text (str): String to tokenize.
        """

        self._index = 0
        self._text = text
        self._keywords = {}
        self._compound_operators = {}
        self._simple_operators = {}

        for type in TokenType:
            keyword = type.keyword()
            if keyword is not None:
                self._keywords[keyword] = type
                continue

            compound_operator = type.compound_operator()
            if compound_operator is not None:
                self._compound_operators[compound_operator] = type
                continue

            simple_operator = type.simple_operator()
            if simple_operator is not None:
                self._simple_operators[simple_operator] = type
                continue

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
                            return Token(self._simple_operators[c], c)
                case '"' | "'":
                    return self._read_str_literal(c)
                case ".":
                    if self._peek().isalpha():
                        return self._read_bool_literal_or_logical()
                    else:
                        return self._read_num_literal(c)
                case "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9":
                    return self._read_num_literal(c)
                case _:
                    if (kw := self._read_keyword(c)) is not None:
                        return kw
                    elif (op := self._read_logical(c)) is not None:
                        return op
                    elif c + (c1 := self._peek()) in self._compound_operators:
                        self._advance()
                        return Token(self._compound_operators[c + c1], c + c1)
                    elif c in self._simple_operators:
                        return Token(self._simple_operators[c], c)
                    elif c.isalpha():
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

    def _read_bool_literal_or_logical(self) -> Token:
        literal = "."

        while (c := self._advance()) not in (".", "\0"):
            literal += c

        literal += "."

        match literal.lower():
            case ".t." | ".f.":
                return Token(TokenType.BOOL_LITERAL, literal)
            case ".or.":
                return Token(TokenType.OR, literal)
            case ".and.":
                return Token(TokenType.AND, literal)
            case _:
                raise SyntaxError(f"Unable to read token '{literal}'.")

    def _read_num_literal(self, c: str) -> Token:
        literal = c
        dotfound = False
        hexnum = False

        while not (c := self._peek()).isspace():
            match c.lower():
                case "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9":
                    literal += c
                case "x":
                    if literal == "0":
                        literal += c
                        hexnum = True
                    else:
                        raise SyntaxError(f"Unterminated hexadecimal literal '{literal + c}'.")
                case "a" | "b" | "c" | "d" | "e" | "f":
                    if hexnum:
                        literal += c
                    else:
                        raise SyntaxError(f"Invalid numeric literal '{literal + c}'.")
                case ".":
                    if not dotfound:
                        literal += c
                        dotfound = True
                    else:
                        raise SyntaxError(f"Second decimal point found in literal '{literal + c}'.")
                case "\0":
                    return Token(TokenType.NUM_LITERAL, literal)

            self._advance()

        return Token(TokenType.NUM_LITERAL, literal)

    def _read_str_literal(self, c: str) -> Token:
        literal = c
        endquote = c

        while (c := self._peek()) != endquote:
            match c:
                case "\0":
                    if len(literal) > 1 and literal[-1:] == endquote:
                        return Token(TokenType.STR_LITERAL, literal + self._advance())

                    raise SyntaxError(f"Unterminated string literal '{literal}'.")
                case _:
                    literal += c

            self._advance()

        return Token(TokenType.STR_LITERAL, literal + self._advance())

    def _read_keyword(self, kw: str) -> Token | None:
        start_index = self._index

        while (c := self._advance()).isalpha():
            kw += c

        if kw.lower() in self._keywords:
            return Token(self._keywords[kw.lower()], kw)

        self._index = start_index
        return None

    def _read_logical(self, op: str) -> Token | None:
        if op != ".":
            return None

        while (c := self._advance()) != ".":
            op += c
        op += c  # Final '.'.

        if op.lower() not in self._compound_operators:
            raise SyntaxError(f"Unterminated logical operator '{op}'.")

        return Token(self._compound_operators[op.lower()], op)

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
