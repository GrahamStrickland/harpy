from typing import override

from harpy.ast import Comment, PreprocessorDirective, SourceRoot
from harpy.ast.expressions import AssignExpression
from harpy.ast.statements import (AssignmentStatement, CallStatement,
                                  FunctionStatement, IfStatement,
                                  LocalVariableDeclaration, ProcedureStatement,
                                  ReturnStatement, Statement,
                                  StaticVariableDeclaration,
                                  WhileLoopStatement)
from harpy.lexer import Lexer, SourceReader, Token, TokenType

from .expression_parser import ExpressionParser
from .parser import Parser


class HarbourParser(Parser):
    """Top-level Parser class for dealing with the entire Harbour grammar."""

    _expression_parser: ExpressionParser
    _reader: SourceReader
    _statements: list[Statement]
    _in_funcproc_def: bool

    def __init__(self, lexer: Lexer):
        self._reader = SourceReader(tokens=lexer)
        self._expression_parser = ExpressionParser(source_reader=self._reader)
        self._statements = []
        self._in_funcproc_def = False

    @override
    def parse(self) -> SourceRoot:
        source_root = SourceRoot()
        for token in self._reader:
            if token.type == TokenType.EOF:
                return source_root
            elif (directive := self.preprocessor_directive(token=token)) is not None:
                source_root.add(node=directive)
            elif (comment := self.comment(token=token)) is not None:
                source_root.add(node=comment)
            elif (stmt := self.statement(token=token)) is not None:
                source_root.add(node=stmt)
            else:
                raise SyntaxError(
                    f"Unable to parse AST node from token '{token.text}' of type '{token.type}' on line {token.line}, column {token.start}."
                )

    def preprocessor_directive(self, token: Token) -> PreprocessorDirective | None:
        if token.type.preprocessor_directive() is not None:
            return PreprocessorDirective(token=token)

        return None

    def comment(self, token) -> Comment | None:
        if token.type in (TokenType.BLOCK_COMMENT, TokenType.LINE_COMMENT):
            return Comment(token=token)

        return None

    def statement(self, token: Token) -> Statement | None:
        if token.type.keyword() is not None:
            match token.type:
                case TokenType.RETURN:
                    return self._return_stmt(token=token)
                case TokenType.STATIC:
                    match self._reader.look_ahead(0).type:
                        case TokenType.FUNCTION:
                            return self._function_def(static=True)
                        case TokenType.PROCEDURE:
                            return self._procedure_def(static=True)
                        case _:
                            return self._static_decln_stmt()
                case TokenType.FUNCTION:
                    return self._function_def()
                case TokenType.PROCEDURE:
                    return self._procedure_def()
                case TokenType.LOCAL:
                    return self._local_decln_stmt()
                case TokenType.IF:
                    return self._if_stmt()
                case TokenType.WHILE:
                    return self._while_loop()
                case _:
                    raise SyntaxError(
                        f"Expected statement, found '{token.text}' at line {token.line}, column {token.start}"
                    )
        elif token.type == TokenType.EOF:
            return None
        elif (stmt := self._call_stmt(token)) is not None:
            return stmt
        elif (stmt := self._assign_stmt(token)) is not None:
            return stmt
        else:
            return None

    def _return_stmt(self, token: Token) -> ReturnStatement:
        if not self._in_funcproc_def:
            raise SyntaxError(
                f"Encountered `return` statement outside of function/procedure definition at line {token.line}, column {token.start}."
            )

        retval = self._expression_parser.parse(optional=True)

        return ReturnStatement(retval=retval)

    def _function_def(self, static: bool = False) -> FunctionStatement | None:
        name, params, body, return_stmt = self._funcproc_def(
            expected=TokenType.FUNCTION, static=static
        )

        return FunctionStatement(
            name=name,
            params=params,
            body=body,
            retval=return_stmt.return_value(),
            static=static,
        )

    def _procedure_def(self, static: bool = False) -> ProcedureStatement | None:
        name, params, body, _ = self._funcproc_def(
            expected=TokenType.PROCEDURE, static=static
        )

        return ProcedureStatement(name=name, params=params, body=body, static=static)

    def _funcproc_def(
        self, expected: TokenType, static: bool = False
    ) -> tuple[str, list[Token], list[Statement], ReturnStatement]:
        if self._in_funcproc_def:
            if static:
                self._reader.put_back(TokenType.STATIC)
            else:
                self._reader.put_back(expected)

            return None

        self._in_funcproc_def = True

        if static:
            _ = self._reader.consume(expected)

        self._in_funcproc_def = True
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

        while (stmt := self.statement(token=self._reader.consume())) is not None:
            body.append(stmt)

        if not isinstance(body[-1], ReturnStatement):
            raise SyntaxError(
                f"Invalid statement at end of function/procedure of type `{body[-1].__class__.__name__}`: '{body[-1].print()}', expected `return` statement."
            )
        else:
            return_stmt = body.pop()

        self._in_funcproc_def = False

        return name, params, body, return_stmt

    def _var_decln_stmt(self, decln_type: str) -> tuple[str, AssignExpression | None]:
        assign_expr = None

        token = self._reader.look_ahead(0)
        if token.type != TokenType.NAME:
            raise SyntaxError(
                f"Expected name after '{decln_type}' keyword, found '{token.text}' at line {token.line}, column {token.start}."
            )

        name = token
        token = self._reader.look_ahead(1)
        if token.type == TokenType.ASSIGN:
            assign_expr = self._expression_parser.parse(reset=True)
        else:
            name = self._reader.consume(TokenType.NAME)

        return name, assign_expr

    def _local_decln_stmt(self) -> LocalVariableDeclaration:
        return LocalVariableDeclaration(*self._var_decln_stmt("local"))

    def _static_decln_stmt(self) -> StaticVariableDeclaration:
        return StaticVariableDeclaration(*self._var_decln_stmt("static"))

    def _if_stmt(self) -> IfStatement:
        ifcond = self._expression_parser.parse(reset=True)
        elifs = []

        ifbody = []
        elsebody = []

        while not self._reader.match(TokenType.ENDIF):
            if self._reader.match(TokenType.ELSE):
                self._reader.consume(TokenType.ELSE)
                while not self._reader.match(TokenType.ENDIF):
                    elsebody.append(self.statement(token=self._reader.consume()))
                break
            elif self._reader.match(TokenType.ELSEIF):
                self._reader.consume(TokenType.ELSEIF)
                elifcond = self._expression_parser.parse(reset=True)
                elifbody = []
                while (
                    not self._reader.match(TokenType.ENDIF)
                    and not self._reader.match(TokenType.ELSE)
                    and not self._reader.match(TokenType.ELSEIF)
                ):
                    elifbody.append(self.statement(token=self._reader.consume()))
                elifs.append((elifcond, elifbody))
            else:
                ifbody.append(self.statement(token=self._reader.consume()))

        self._reader.consume(TokenType.ENDIF)

        return IfStatement(
            ifcond=ifcond,
            ifbody=ifbody,
            elifs=elifs,
            elsebody=elsebody,
        )

    def _while_loop(self) -> WhileLoopStatement:
        cond = self._expression_parser.parse(reset=True)
        body = []

        while not self._reader.match(TokenType.END) and not self._reader.match(
            TokenType.ENDWHILE
        ):
            body.append(self.statement(token=self._reader.consume()))

        if self._reader.match(TokenType.END):
            self._reader.consume(TokenType.END)
            if self._reader.match(TokenType.WHILE):
                self._reader.consume(TokenType.WHILE)
        else:
            self._reader.consume(TokenType.ENDWHILE)

        return WhileLoopStatement(cond=cond, body=body)

    def _call_stmt(self, name: Token) -> CallStatement | None:
        if name.type != TokenType.NAME:
            return None

        call_expr = None
        token = self._reader.look_ahead(0)
        if token.type == TokenType.LEFT_PAREN:
            self._reader.put_back(name)
            call_expr = self._expression_parser.parse(reset=True)
        else:
            return None

        return CallStatement(call_expr=call_expr)

    def _assign_stmt(self, token: Token) -> AssignmentStatement | None:
        self._reader.put_back(token)
        left = self._expression_parser.parse(reset=True)

        if isinstance(left, AssignExpression):
            return AssignmentStatement(assign_expr=left)
        else:
            self._expression_parser.unparse()
            return None
