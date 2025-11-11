using Harpy.AST;
using Harpy.AST.Expressions;
using Harpy.AST.Statements;
using Harpy.Lexer;

namespace Harpy.Parser;

/// <summary>
///     Top-level parser class for dealing with the entire Harbour grammar.
/// </summary>
public class Parser
{
    private readonly ExpressionParser _expressionParser;
    private readonly SourceReader _reader;
    private bool _inFunctionOrProcedureDefinition;

    public Parser(Lexer.Lexer lexer)
    {
        _reader = new SourceReader(lexer);
        _expressionParser = new ExpressionParser(_reader);
    }

    public SourceRoot Parse()
    {
        var sourceRoot = new SourceRoot([]);

        foreach (var token in _reader)
        {
            if (token.Kind == HarbourSyntaxKind.EOF) return sourceRoot;

            var statement = ParseStatement(token);
            if (statement == null)
                throw new InvalidSyntaxException(
                    $"Expected statement with first token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");

            sourceRoot.Children.Add(statement);
        }

        throw new InvalidSyntaxException(
            "Expected EOF token at end of source root.");
    }

    private Statement? ParseStatement(HarbourSyntaxToken token)
    {
        Statement? statement;

        if (token.Keyword() != null)
        {
            statement = token.Kind switch
            {
                HarbourSyntaxKind.RETURN => ReturnStatement(token),
                HarbourSyntaxKind.STATIC => _reader.LookAhead().Kind switch
                {
                    HarbourSyntaxKind.FUNCTION => FunctionDefinition(_reader.LookAhead(), true),
                    HarbourSyntaxKind.PROCEDURE => ProcedureDefinition(_reader.LookAhead(), true),
                    _ => StaticDeclarationStatement(token)
                },
                HarbourSyntaxKind.FUNCTION => FunctionDefinition(token),
                HarbourSyntaxKind.PROCEDURE => ProcedureDefinition(token),
                HarbourSyntaxKind.LOCAL => LocalDeclarationStatement(token),
                HarbourSyntaxKind.IF => IfStatement(token),
                HarbourSyntaxKind.WHILE => WhileLoop(token),
                _ => throw new InvalidSyntaxException(
                    $"Expected statement, found '{token.Text}' on line {token.Line}, column {token.Start}, found null.")
            };
        }
        else
        {
            statement = CallStatement(token);
            statement ??= AssignmentStatement(token);
        }

        return statement;
    }

    private ReturnStatement ReturnStatement(HarbourSyntaxToken token)
    {
        if (!_inFunctionOrProcedureDefinition)
            throw new InvalidSyntaxException(
                $"Encountered `return` statement outside of function/procedure definition after token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");

        var returnValue = _expressionParser.Parse(Precedence.NONE, true);

        return new ReturnStatement(returnValue);
    }

    private FunctionStatement FunctionDefinition(HarbourSyntaxToken token, bool isStatic = false)
    {
        FunctionOrProcedureDefinition(token, isStatic, out var name,
            out var parameters,
            out var body, out var returnValue);

        if (returnValue == null)
            throw new InvalidSyntaxException(
                $"Expected return statement after function definition beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
        if (name == null)
            throw new InvalidSyntaxException(
                $"Expected name in function definition beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
        return new FunctionStatement(name, parameters, body, returnValue, isStatic);
    }

    private ProcedureStatement ProcedureDefinition(HarbourSyntaxToken token, bool isStatic = false)
    {
        FunctionOrProcedureDefinition(token, isStatic, out var name,
            out var parameters,
            out var body, out _);

        if (name == null)
            throw new InvalidSyntaxException(
                $"Expected name in procedure definition beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
        return new ProcedureStatement(name, parameters, body, isStatic);
    }

    private void FunctionOrProcedureDefinition(HarbourSyntaxToken token, bool isStatic,
        out HarbourSyntaxToken? name,
        out List<HarbourSyntaxToken> parameters,
        out List<Statement> body, out Expression? returnValue)
    {
        if (_inFunctionOrProcedureDefinition)
        {
            _reader.PutBack(token);
            name = null;
            parameters = [];
            body = [];
            returnValue = null;
            return;
        }

        _inFunctionOrProcedureDefinition = true;

        if (isStatic)
            _reader.Consume(token.Kind);

        name = _reader.Consume(HarbourSyntaxKind.NAME);

        _reader.Consume(HarbourSyntaxKind.LEFT_PAREN);
        parameters = [];

        while (!_reader.Match(HarbourSyntaxKind.RIGHT_PAREN))
        {
            parameters.Add(_reader.Consume(HarbourSyntaxKind.NAME));
            if (_reader.Match(HarbourSyntaxKind.COMMA)) _reader.Consume(HarbourSyntaxKind.COMMA);
            else
                break;
        }

        _reader.Consume(HarbourSyntaxKind.RIGHT_PAREN);

        body = [];

        var next = _reader.LookAhead();
        if (next.Kind is not HarbourSyntaxKind.EOF)
        {
            next = _reader.Consume();
            var statement = ParseStatement(next);

            while (statement != null)
            {
                body.Add(statement);

                next = _reader.LookAhead();
                if (next.Kind is HarbourSyntaxKind.EOF) break;
                next = _reader.Consume();
                statement = ParseStatement(next);
            }
        }

        if (body[^1] is not AST.Statements.ReturnStatement)
            throw new InvalidSyntaxException(
                $"Expected return statement at end of function definition, found '{body[^1].PrettyPrint()}'.");

        returnValue = ((ReturnStatement)body[^1]).ReturnValue();
        body.RemoveAt(body.Count - 1);
    }

    private StaticVariableDeclaration StaticDeclarationStatement(HarbourSyntaxToken token)
    {
        VariableDeclarationStatement(token, out var name, out var assignment);
        return new StaticVariableDeclaration(name, assignment);
    }

    private LocalVariableDeclaration LocalDeclarationStatement(HarbourSyntaxToken token)
    {
        VariableDeclarationStatement(token, out var name, out var assignment);
        return new LocalVariableDeclaration(name, assignment);
    }

    private void VariableDeclarationStatement(HarbourSyntaxToken token, out HarbourSyntaxToken name,
        out Expression? assignment)
    {
        assignment = null;

        var next = _reader.LookAhead();
        if (next.Kind is not HarbourSyntaxKind.NAME)
            throw new InvalidSyntaxException(
                $"Expected name in variable declaration beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");

        name = next;
        next = _reader.LookAhead(1);
        if (next.Kind is HarbourSyntaxKind.ASSIGN)
            assignment = _expressionParser.Parse(Precedence.NONE, false, true);
        else
            name = _reader.Consume(HarbourSyntaxKind.NAME);
    }

    private IfStatement IfStatement(HarbourSyntaxToken token)
    {
        var ifCondition = _expressionParser.Parse(Precedence.NONE, false, true);
        List<Tuple<Expression, List<Statement>>> elseIfConditions = [];
        List<Statement> ifBody = [];
        List<Statement> elseBody = [];

        while (!_reader.Match(HarbourSyntaxKind.ENDIF))
        {
            if (_reader.Match(HarbourSyntaxKind.ELSE))
            {
                var elseToken = _reader.Consume(HarbourSyntaxKind.ELSE);
                while (!_reader.Match(HarbourSyntaxKind.ENDIF))
                    elseBody.Add(ParseStatement(_reader.Consume()) ??
                                 throw new InvalidSyntaxException(
                                     $"Expected statement after else condition beginning with token '{elseToken.Text}' on line {elseToken.Line}, column {elseToken.Start}, found null."));

                break;
            }

            if (_reader.Match(HarbourSyntaxKind.ELSEIF))
            {
                var elseIfToken = _reader.Consume(HarbourSyntaxKind.ELSEIF);

                var elseIfCondition = _expressionParser.Parse(Precedence.NONE, false, true);
                if (elseIfCondition == null)
                    throw new InvalidSyntaxException(
                        $"Expected conditional expression after else if beginning with token '{elseIfToken.Text}' on line {elseIfToken.Line}, column {elseIfToken.Start}, found null.");
                List<Statement> elseIfBody = [];

                while (!_reader.Match(HarbourSyntaxKind.ENDIF) && !_reader.Match(HarbourSyntaxKind.ELSE) &&
                       !_reader.Match(HarbourSyntaxKind.ELSEIF))
                    elseIfBody.Add(ParseStatement(_reader.Consume()) ??
                                   throw new InvalidSyntaxException(
                                       $"Expected statement after else if condition beginning with token '{elseIfToken.Text}' on line {elseIfToken.Line}, column {elseIfToken.Start}, found null."));

                elseIfConditions.Add(new Tuple<Expression, List<Statement>>(elseIfCondition, elseIfBody));
            }
            else
            {
                ifBody.Add(ParseStatement(_reader.Consume()) ??
                           throw new InvalidSyntaxException(
                               $"Expected statement after if condition beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
            }
        }

        _reader.Consume(HarbourSyntaxKind.ENDIF);

        return new IfStatement(ifCondition ??
                               throw new InvalidSyntaxException(
                                   $"Expected conditional expression after if statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null."),
            elseIfConditions, ifBody, elseBody);
    }

    private WhileLoopStatement WhileLoop(HarbourSyntaxToken token)
    {
        var condition = _expressionParser.Parse(Precedence.NONE, false, true);
        List<Statement> body = [];

        while (!_reader.Match(HarbourSyntaxKind.END) && !_reader.Match(HarbourSyntaxKind.ENDWHILE))
            body.Add(ParseStatement(_reader.Consume()) ??
                     throw new InvalidSyntaxException(
                         $"Expected statement after while loop beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));

        if (_reader.Match(HarbourSyntaxKind.END))
        {
            _reader.Consume(HarbourSyntaxKind.END);
            if (_reader.Match(HarbourSyntaxKind.WHILE)) _reader.Consume(HarbourSyntaxKind.WHILE);
        }
        else
        {
            _reader.Consume(HarbourSyntaxKind.ENDWHILE);
        }

        return new WhileLoopStatement(
            condition ?? throw new InvalidSyntaxException(
                $"Expected conditional expression in while loop beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null."),
            body);
    }

    private CallStatement? CallStatement(HarbourSyntaxToken name)
    {
        if (name.Kind is not HarbourSyntaxKind.NAME)
            return null;

        CallExpression? callExpression;
        var token = _reader.LookAhead();
        if (token.Kind is HarbourSyntaxKind.LEFT_PAREN)
        {
            _reader.PutBack(name);
            callExpression = (CallExpression?)_expressionParser.Parse(Precedence.NONE, false, true);
        }
        else
        {
            return null;
        }

        return callExpression != null ? new CallStatement(callExpression) : null;
    }

    private AssignmentStatement? AssignmentStatement(HarbourSyntaxToken token)
    {
        _reader.PutBack(token);
        var leftExpression = _expressionParser.Parse(Precedence.NONE, false, true);

        if (leftExpression is AssignmentExpression expression) return new AssignmentStatement(expression);

        _expressionParser.Unparse();
        return null;
    }
}