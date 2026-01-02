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
    private bool _inLoop;

    public Parser(Lexer.Lexer lexer)
    {
        _reader = new SourceReader(lexer);
        _expressionParser = new ExpressionParser(_reader);
    }

    public SourceRoot Parse(string name)
    {
        var sourceRoot = new SourceRoot(name, []);

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
                HarbourSyntaxKind.FOR => ForLoop(token),
                HarbourSyntaxKind.LOOP => Loop(token),
                HarbourSyntaxKind.EXIT => Exit(token),
                HarbourSyntaxKind.BEGIN => BeginSequence(token),
                _ => throw new InvalidSyntaxException(
                    $"Expected statement, found '{token.Text}' on line {token.Line}, column {token.Start}.")
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
                $"Encountered `return` statement outside of function/procedure definition after token '{token.Text}' on line {token.Line}, column {token.Start}.");

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

        _inFunctionOrProcedureDefinition = false;
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

        _inFunctionOrProcedureDefinition = false;
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
                if (statement is ReturnStatement)
                    break;

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
        return new StaticVariableDeclaration(token, name, assignment);
    }

    private LocalVariableDeclaration LocalDeclarationStatement(HarbourSyntaxToken token)
    {
        VariableDeclarationStatement(token, out var name, out var assignment);
        return new LocalVariableDeclaration(token, name, assignment);
    }

    private void VariableDeclarationStatement(HarbourSyntaxToken token, out HarbourSyntaxToken name,
        out Expression? assignment)
    {
        assignment = null;

        var next = _reader.LookAhead();
        if (next.Kind is not HarbourSyntaxKind.NAME)
            throw new InvalidSyntaxException(
                $"Expected name in variable declaration beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");

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
                                     $"Expected statement after else condition beginning with token '{elseToken.Text}' on line {elseToken.Line}, column {elseToken.Start}."));

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
                                       $"Expected statement after else if condition beginning with token '{elseIfToken.Text}' on line {elseIfToken.Line}, column {elseIfToken.Start}."));

                elseIfConditions.Add(new Tuple<Expression, List<Statement>>(elseIfCondition, elseIfBody));
            }
            else
            {
                ifBody.Add(ParseStatement(_reader.Consume()) ??
                           throw new InvalidSyntaxException(
                               $"Expected statement after if condition beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));
            }
        }

        _reader.Consume(HarbourSyntaxKind.ENDIF);

        return new IfStatement(ifCondition ??
                               throw new InvalidSyntaxException(
                                   $"Expected conditional expression after if statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."),
            elseIfConditions, ifBody, elseBody);
    }

    private WhileLoopStatement WhileLoop(HarbourSyntaxToken token)
    {
        _inLoop = true;
        var condition = _expressionParser.Parse(Precedence.NONE, false, true);
        List<Statement> body = [];

        while (!_reader.Match(HarbourSyntaxKind.END) && !_reader.Match(HarbourSyntaxKind.ENDWHILE))
            body.Add(ParseStatement(_reader.Consume()) ??
                     throw new InvalidSyntaxException(
                         $"Expected statement after while loop beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));

        if (_reader.Match(HarbourSyntaxKind.END))
        {
            _reader.Consume(HarbourSyntaxKind.END);
            if (_reader.Match(HarbourSyntaxKind.WHILE)) _reader.Consume(HarbourSyntaxKind.WHILE);
        }
        else
        {
            _reader.Consume(HarbourSyntaxKind.ENDWHILE);
        }

        _inLoop = false;

        return new WhileLoopStatement(
            condition ?? throw new InvalidSyntaxException(
                $"Expected conditional expression in while loop beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."),
            body);
    }

    private Statement ForLoop(HarbourSyntaxToken token)
    {
        _inLoop = true;

        if (_reader.Match(HarbourSyntaxKind.EACH))
            return ForEachLoop(token);

        var initializer = _expressionParser.Parse(Precedence.NONE, false, true);
        if (initializer is not AssignmentExpression)
            throw new InvalidSyntaxException(
                $"Expected initializer expression in for loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");

        if (!_reader.Match(HarbourSyntaxKind.TO))
            throw new InvalidSyntaxException(
                $"Expected `step` after initialization in for loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");
        _reader.Consume(HarbourSyntaxKind.TO);

        var bound = _expressionParser.Parse(Precedence.NONE, false, true);
        if (bound == null)
            throw new InvalidSyntaxException(
                $"Expected bound expression in for loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");

        Expression? step = null;

        if (_reader.Match(HarbourSyntaxKind.STEP))
        {
            _reader.Consume(HarbourSyntaxKind.STEP);
            step = _expressionParser.Parse(Precedence.NONE, false, true);
            if (step == null)
                throw new InvalidSyntaxException(
                    $"Expected step expression in for loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
        }

        List<Statement> body = [];
        while (!_reader.Match(HarbourSyntaxKind.NEXT))
            body.Add(ParseStatement(_reader.Consume()) ??
                     throw new InvalidSyntaxException(
                         $"Expected statement after for loop beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));

        _reader.Consume(HarbourSyntaxKind.NEXT);

        _inLoop = false;

        return new ForLoopStatement(initializer, bound, step, body);
    }

    private ForEachLoopStatement ForEachLoop(HarbourSyntaxToken token)
    {
        _reader.Consume(HarbourSyntaxKind.EACH);

        if (!_reader.Match(HarbourSyntaxKind.NAME))
            throw new InvalidSyntaxException(
                $"Expected variable name in initialization of for each loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");
        var variable = _reader.Consume(HarbourSyntaxKind.NAME);

        if (!_reader.Match(HarbourSyntaxKind.IN))
            throw new InvalidSyntaxException(
                $"Expected `step` keyword after initialization in for each loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");
        _reader.Consume(HarbourSyntaxKind.IN);

        var collection = _expressionParser.Parse(Precedence.NONE, false, true);
        if (collection == null)
            throw new InvalidSyntaxException(
                $"Expected collection expression in for each loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");

        List<Statement> body = [];
        while (!_reader.Match(HarbourSyntaxKind.NEXT))
            body.Add(ParseStatement(_reader.Consume()) ??
                     throw new InvalidSyntaxException(
                         $"Expected statement after for loop beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));

        _reader.Consume(HarbourSyntaxKind.NEXT);

        _inLoop = false;

        return new ForEachLoopStatement(variable, collection, body);
    }

    private LoopStatement Loop(HarbourSyntaxToken token)
    {
        if (!_inLoop)
            throw new InvalidSyntaxException(
                $"Encountered loop statement outside of a loop; token '{token.Text}' on line {token.Line}, column {token.Start}.");

        return token.Kind != HarbourSyntaxKind.LOOP
            ? throw new InvalidSyntaxException(
                $"Expected loop statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.")
            : new LoopStatement();
    }

    private ExitStatement Exit(HarbourSyntaxToken token)
    {
        if (!_inLoop)
            throw new InvalidSyntaxException(
                $"Encountered exit statement outside of a loop; token '{token.Text}' on line {token.Line}, column {token.Start}.");

        return token.Kind != HarbourSyntaxKind.EXIT
            ? throw new InvalidSyntaxException(
                $"Expected exit statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.")
            : new ExitStatement();
    }

    private BeginSequenceStatement BeginSequence(HarbourSyntaxToken token)
    {
        if (!_reader.Match(HarbourSyntaxKind.SEQUENCE))
            throw new InvalidSyntaxException(
                $"Expected `sequence` keyword after `begin` in begin sequence statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");
        _reader.Consume(HarbourSyntaxKind.SEQUENCE);

        CodeblockExpression? errorHandler = null;
        if (_reader.Match(HarbourSyntaxKind.WITH))
        {
            _reader.Consume(HarbourSyntaxKind.WITH);

            errorHandler = (CodeblockExpression?)_expressionParser.Parse();
            if (errorHandler is null)
                throw new InvalidSyntaxException(
                    $"Expected codeblock after begin sequence statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");
        }

        List<Statement> beginSequenceBody = [];
        while (!_reader.Match(HarbourSyntaxKind.RECOVER) && !_reader.Match(HarbourSyntaxKind.ALWAYS)
                                                         && !_reader.Match(HarbourSyntaxKind.END) &&
                                                         !_reader.Match(HarbourSyntaxKind.ENDSEQUENCE))
            beginSequenceBody.Add(ParseStatement(_reader.Consume()) ??
                                  throw new InvalidSyntaxException(
                                      $"Expected statement after begin sequence statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));

        HarbourSyntaxToken? exception = null;
        List<Statement> recoverBody = [];
        if (_reader.Match(HarbourSyntaxKind.RECOVER))
        {
            _reader.Consume(HarbourSyntaxKind.RECOVER);

            if (_reader.Match(HarbourSyntaxKind.USING))
            {
                _reader.Consume(HarbourSyntaxKind.USING);
                exception = _reader.Consume(HarbourSyntaxKind.NAME);
            }

            while (!_reader.Match(HarbourSyntaxKind.ALWAYS)
                   && !_reader.Match(HarbourSyntaxKind.END) &&
                   !_reader.Match(HarbourSyntaxKind.ENDSEQUENCE))
                recoverBody.Add(ParseStatement(_reader.Consume()) ??
                                throw new InvalidSyntaxException(
                                    $"Expected statement after `recover` keyword in begin sequence statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));
        }

        List<Statement> alwaysBody = [];
        if (_reader.Match(HarbourSyntaxKind.ALWAYS))
        {
            _reader.Consume(HarbourSyntaxKind.ALWAYS);

            while (!_reader.Match(HarbourSyntaxKind.END) &&
                   !_reader.Match(HarbourSyntaxKind.ENDSEQUENCE))
                alwaysBody.Add(ParseStatement(_reader.Consume()) ??
                               throw new InvalidSyntaxException(
                                   $"Expected statement after begin sequence statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}."));
        }

        if (_reader.Match(HarbourSyntaxKind.END))
        {
            _reader.Consume(HarbourSyntaxKind.END);
            if (_reader.Match(HarbourSyntaxKind.SEQUENCE)) _reader.Consume(HarbourSyntaxKind.SEQUENCE);
        }
        else if (_reader.Match(HarbourSyntaxKind.ENDSEQUENCE))
        {
            _reader.Consume(HarbourSyntaxKind.ENDSEQUENCE);
        }
        else
        {
            throw new InvalidSyntaxException(
                $"Expected end of begin sequence statement beginning with token '{token.Text}' on line {token.Line}, column {token.Start}.");
        }

        return new BeginSequenceStatement(errorHandler, exception, beginSequenceBody, recoverBody, alwaysBody);
    }

    private CallStatement? CallStatement(HarbourSyntaxToken name)
    {
        if (name.Kind is not HarbourSyntaxKind.NAME)
            return null;

        CallExpression? callExpression;
        var token = _reader.LookAhead();
        if (token.Kind is HarbourSyntaxKind.LEFT_PAREN or HarbourSyntaxKind.COLON)
        {
            _reader.PutBack(name);
            var expression = _expressionParser.Parse(Precedence.NONE, false, true);
            if (expression is not CallExpression possibleCallExpression)
            {
                _expressionParser.Unparse();
                _reader.Consume(HarbourSyntaxKind.NAME);
                return null;
            }

            callExpression = possibleCallExpression;
        }
        else
        {
            return null;
        }

        return new CallStatement(callExpression);
    }

    private AssignmentStatement? AssignmentStatement(HarbourSyntaxToken token)
    {
        _reader.PutBack(token);
        var leftExpression = _expressionParser.Parse(Precedence.NONE, false, true);

        if (leftExpression is AssignmentExpression expression) return new AssignmentStatement(expression);

        _expressionParser.Unparse();
        _reader.Consume(token.Kind);
        return null;
    }
}
