from typing import override

from .expression_parser import ExpressionParser
from .expressions import AssignExpression
from .lexer import Lexer
from .parser import Parser
from .source_reader import SourceReader
from .statements import (CallStatement, FunctionStatement, IfStatement,
                         LocalVariableDeclaration, ProcedureStatement,
                         Statement, StaticVariableDeclaration)
from .token import Token
from .token_type import TokenType


class HarbourParser(Parser):
    """Top-level Parser class for dealing with the entire Harbour grammar."""

    _expression_parser: ExpressionParser
    _reader: SourceReader
    _statements: list[Statement]

    def __init__(self, lexer: Lexer):
        self._reader = SourceReader(tokens=lexer)
        self._expression_parser = ExpressionParser(source_reader=self._reader)
        self._statements = []

    @override
    def parse(self) -> list[Statement]:
        while (stmt := self.statement()) is not None:
            self._statements.append(stmt)

        return self._statements

    def statement(self) -> Statement | None:
        for token in self._reader:
            if (token_type := token.get_type()) == TokenType.EOF:
                return None
            elif token_type.keyword() is not None:
                match token_type:
                    case TokenType.STATIC:
                        match self._reader.look_ahead(0).get_type():
                            case TokenType.FUNCTION:
                                return self._function_defn(static=True)
                            case TokenType.PROCEDURE:
                                return self._procedure_defn(static=True)
                            case _:
                                return self._static_decln_stmt()
                    case TokenType.FUNCTION:
                        return self._function_defn()
                    case TokenType.PROCEDURE:
                        return self._procedure_defn()
                    case TokenType.LOCAL:
                        return self._local_decln_stmt()
                    case TokenType.IF:
                        return self._if_stmt()
                    case _:
                        raise SyntaxError(
                            f"Expected statement, found '{token.get_text()}'"
                        )
            else:
                return self._call_stmt(token)

    def _function_defn(self, static: bool = False) -> FunctionStatement:
        if static:
            _ = self._reader.consume(TokenType.FUNCTION)

        name = self._reader.consume(TokenType.NAME)

        _ = self._reader.consume(TokenType.LEFT_PAREN)
        params = []

        while not self._reader.match(TokenType.RIGHT_PAREN):
            params.append(self._reader.consume(TokenType.NAME))
            if self._reader.match(TokenType.COMMA):
                self._reader.consume(TokenType.COMMA)
            else:
                break

        _ = self._reader.consume(TokenType.RIGHT_PAREN)

        body = []

        while not self._reader.match(TokenType.RETURN):
            body.append(self.statement())

        self._reader.consume(TokenType.RETURN)
        retval = self._expression_parser.parse()

        return FunctionStatement(
            name=name, params=params, body=body, retval=retval, static=static
        )

    def _procedure_defn(self, static: bool = False) -> ProcedureStatement:
        if static:
            _ = self._reader.consume(TokenType.PROCEDURE)

        name = self._reader.consume(TokenType.NAME)

        _ = self._reader.consume(TokenType.LEFT_PAREN)
        params = []

        while not self._reader.match(TokenType.RIGHT_PAREN):
            params.append(self._reader.consume(TokenType.NAME))
            if self._reader.match(TokenType.COMMA):
                self._reader.consume(TokenType.COMMA)
            else:
                break

        _ = self._reader.consume(TokenType.RIGHT_PAREN)

        body = []

        while not self._reader.match(TokenType.RETURN):
            body.append(self.statement())

        self._reader.consume(TokenType.RETURN)

        return ProcedureStatement(name=name, params=params, body=body, static=static)

    def _var_decln_stmt(self, decln_type: str) -> tuple[str, AssignExpression | None]:
        assign_expr = None

        token = self._reader.look_ahead(0)
        if token.get_type() != TokenType.NAME:
            raise SyntaxError(
                f"Expected name after {decln_type} keyword, found '{token.get_text()}'"
            )

        name = token
        token = self._reader.look_ahead(1)
        if token.get_type() == TokenType.ASSIGN:
            assign_expr = self._expression_parser.parse()
        else:
            name = self._reader.consume(TokenType.NAME)

        return name, assign_expr

    def _local_decln_stmt(self) -> LocalVariableDeclaration:
        return LocalVariableDeclaration(*self._var_decln_stmt("local"))

    def _static_decln_stmt(self) -> StaticVariableDeclaration:
        return StaticVariableDeclaration(*self._var_decln_stmt("static"))

    def _if_stmt(self) -> IfStatement:
        ifcond = self._expression_parser.parse()
        elifs = []

        ifbody = []
        elsebody = []

        while not self._reader.match(TokenType.ENDIF):
            if self._reader.match(TokenType.ELSE):
                self._reader.consume(TokenType.ELSE)
                while not self._reader.match(TokenType.ENDIF):
                    elsebody.append(self.statement())
                break
            elif self._reader.match(TokenType.ELSEIF):
                self._reader.consume(TokenType.ELSEIF)
                elifcond = self._expression_parser.parse()
                elifbody = []
                while not self._reader.match(TokenType.ELSE) and not self._reader.match(
                    TokenType.ELSEIF
                ):
                    elifbody.append(self.statement())
                elifs.append((elifcond, elifbody))
            else:
                ifbody.append(self.statement())

        self._reader.consume(TokenType.ENDIF)

        return IfStatement(
            ifcond=ifcond,
            ifbody=ifbody,
            elifs=elifs,
            elsebody=elsebody,
        )

    def _call_stmt(self, name: Token):
        if name.get_type() != TokenType.NAME:
            raise SyntaxError(f"Expected call expression, found '{name.get_text()}'")
        self._reader.put_back(name)

        call_expr = None
        token = self._reader.look_ahead(1)
        if token.get_type() == TokenType.LEFT_PAREN:
            call_expr = self._expression_parser.parse()

        return CallStatement(call_expr=call_expr)
