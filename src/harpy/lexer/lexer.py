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
    _directives: dict[str, TokenType]
    _keywords: dict[str, TokenType]
    _simple_operators: dict[str, TokenType]
    _compound_operatos: dict[str, TokenType]
    _names: list[str]
    _line: int
    _pos: int
    _reset_index: int

    def __init__(self, text: str):
        """Creates a new Lexer to tokenize the given string.

        Args:
            text (str): String to tokenize.
        """

        self._text = text
        self._directives = {}
        self._keywords = {}
        self._compound_operators = {}
        self._simple_operators = {}
        self._names = []
        self._line = 1
        self._index = 0
        self._pos = 0
        self._reset_index = 0

        for type in TokenType:
            directive = type.preprocessor_directive()
            if directive is not None:
                self._directives[directive] = type
                continue

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
            c = self._advance()

            match c:
                case "#":
                    return self._read_preprocessor_directive()
                case "/":
                    # Potential comments.
                    match self._peek():
                        case "/":
                            return self._read_line_comment()
                        case "*":
                            return self._read_block_comment()
                        case _:
                            return Token(
                                type=self._simple_operators[c],
                                text=c,
                                line=self._line,
                                position=self._pos,
                            )
                case "[":
                    if self._peek().isalpha():
                        return self._read_str_literal_or_bracket(c)
                    else:
                        return Token(
                            type=self._simple_operators[c],
                            text=c,
                            line=self._line,
                            position=self._pos,
                        )
                case '"' | "'":
                    return self._read_str_literal_or_bracket(c)
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
                    elif c + (c1 := self._peek()) in self._compound_operators:
                        self._advance()
                        return Token(
                            type=self._compound_operators[c + c1],
                            text=c + c1,
                            line=self._line,
                            position=self._pos,
                        )
                    elif c in self._simple_operators:
                        return Token(
                            type=self._simple_operators[c],
                            text=c,
                            line=self._line,
                            position=self._pos,
                        )
                    elif c.isalpha():
                        return self._read_name()
                    else:
                        if c == "\n":
                            self._line += 1
                            self._pos = 0
                        # Ignore all other characters (whitespace, etc.)
                        pass

        # Once we've reached the end of the string, just return EOF tokens. We'll
        # just keep returning them as many times as we're asked so that the
        # parser's lookahead doesn't have to worry about running out of tokens.
        return Token(type=TokenType.EOF, text="\0", line=self._line, position=self._pos)

    def _read_preprocessor_directive(self) -> Token:
        start_index = self._index - 1
        self._set_reset_index(index=start_index)
        directive = ""

        while (c := self._advance()).isalpha():
            directive += c

        if directive.lower() in self._directives:
            while True:
                match self._peek():
                    case "\n" | "\r" | "\0":
                        return Token(
                            type=self._directives[directive.lower()],
                            text=self._text[start_index : self._index],
                            line=self._line,
                            position=self._pos,
                        )
                    case _:
                        self._advance()

        self._reset(offset=1)

        return Token(type=TokenType.NE1, text="#", line=self._line, position=self._pos)

    def _read_line_comment(self) -> Token:
        start_index = self._index - 1

        # Consume second '/'.
        self._advance()

        while True:
            match self._peek():
                case "\n" | "\r" | "\0":
                    return Token(
                        type=TokenType.LINE_COMMENT,
                        text=self._text[start_index : self._index],
                        line=self._line,
                        position=self._pos,
                    )
                case _:
                    self._advance()

    def _read_block_comment(self) -> Token:
        start_index = self._index - 1

        while True:
            match c := self._advance():
                case "*":
                    match self._advance():
                        case "/":
                            return Token(
                                type=TokenType.BLOCK_COMMENT,
                                text=self._text[start_index : self._index],
                                line=self._line,
                                position=self._pos,
                            )
                        case "\0":
                            raise SyntaxError("Unterminated block comment.")
                        case _:
                            if c == "\n":
                                self._pos = 0
                            pass  # Do nothing, keep advancing.
                case "\0":
                    raise SyntaxError("Unterminated block comment.")
                case _:
                    if c == "\n":
                        self._pos = 0
                    pass  # Do nothing, keep advancing.

    def _read_bool_literal_or_logical(self) -> Token:
        literal = "."

        while (c := self._advance()) not in (".", "\0"):
            literal += c

        literal += "."

        match literal.lower():
            case ".t." | ".f.":
                return Token(
                    type=TokenType.BOOL_LITERAL,
                    text=literal,
                    line=self._line,
                    position=self._pos,
                )
            case ".or.":
                return Token(
                    type=TokenType.OR,
                    text=literal,
                    line=self._line,
                    position=self._pos,
                )
            case ".and.":
                return Token(
                    type=TokenType.AND,
                    text=literal,
                    line=self._line,
                    position=self._pos,
                )
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
                    self._advance()
                case "x":
                    if literal == "0":
                        literal += c
                        hexnum = True
                        self._advance()
                    else:
                        raise SyntaxError(
                            f"Unterminated hexadecimal literal '{literal + c}'."
                        )
                case "a" | "b" | "c" | "d" | "e" | "f":
                    if hexnum:
                        literal += c
                        self._advance()
                    else:
                        raise SyntaxError(f"Invalid numeric literal '{literal + c}'.")
                case ".":
                    if not dotfound:
                        literal += c
                        dotfound = True
                        self._advance()
                    else:
                        raise SyntaxError(
                            f"Second decimal point found in literal '{literal + c}'."
                        )
                case "\0":
                    return Token(
                        type=TokenType.NUM_LITERAL,
                        text=literal,
                        line=self._line,
                        position=self._pos,
                    )
                case _:
                    return Token(
                        type=TokenType.NUM_LITERAL,
                        text=literal,
                        line=self._line,
                        position=self._pos,
                    )

        return Token(
            type=TokenType.NUM_LITERAL,
            text=literal,
            line=self._line,
            position=self._pos,
        )

    def _read_str_literal_or_bracket(self, c: str) -> Token:
        start_index = self._index
        self._set_reset_index(index=start_index)
        literal = c
        endquote = "]" if c == "[" else c

        while (c := self._peek()) != endquote:
            match c:
                case "\0":
                    if len(literal) > 1 and literal[-1:] == endquote:
                        break
                    raise SyntaxError(f"Unterminated string literal '{literal}'.")
                case _:
                    literal += c

            self._advance()

        if endquote == "]" and literal[1:] in self._names:
            self._reset()
            return Token(
                type=TokenType.LEFT_BRACKET,
                text="[",
                line=self._line,
                position=self._pos,
            )

        return Token(
            type=TokenType.STR_LITERAL,
            text=literal + self._advance(),
            line=self._line,
            position=self._pos,
        )

    def _read_keyword(self, kw: str) -> Token | None:
        self._set_reset_index(index=self._index)

        while (c := self._peek()).isalpha():
            kw += c
            self._advance()

        if kw.lower() in self._keywords:
            return Token(
                type=self._keywords[kw.lower()],
                text=kw,
                line=self._line,
                position=self._pos,
            )

        self._reset()
        return None

    def _read_name(self) -> Token:
        start_index = self._index - 1
        self._set_reset_index(index=start_index)

        while self._peek() != "\0":
            if not self._text[self._index].isalnum() and self._text[self._index] != "_":
                break
            self._advance()

        name = self._text[start_index : self._index]

        self._names.append(name)

        return Token(
            type=TokenType.NAME, text=name, line=self._line, position=self._pos
        )

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
                self._pos += 1

        return c

    def _set_reset_index(self, index: int):
        self._reset_index = index

    def _reset(self, offset: int = 0):
        self._pos -= self._index - self._reset_index + offset
        if (
            self._pos < 0
        ):  # Be careful not to reset further than the same line, since the line number will now be incorrect
            self._pos = 0
        self._index = self._reset_index + offset
        self._reset_index = 0
