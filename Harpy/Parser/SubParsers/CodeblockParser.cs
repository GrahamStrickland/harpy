using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     Parser for a codeblock like <c>{ |a| b(), c() }</c>.
/// </summary>
public class CodeblockParser : IPrefixSubParser
{
    public Expression Parse(ExpressionParser parser, HarbourSyntaxToken token)
    {
        List<NameExpression> parameters = [];

        parser.Consume(HarbourSyntaxKind.PIPE);

        // There may be no arguments at all.
        if (!parser.Match(HarbourSyntaxKind.PIPE))
        {
            parameters.Add(ParseParams(parser, token));

            while (parser.Match(HarbourSyntaxKind.COMMA))
            {
                parser.Consume(HarbourSyntaxKind.COMMA);
                parameters.Add(ParseParams(parser, token));
            }
        }

        parser.Consume(HarbourSyntaxKind.PIPE);

        List<Expression> expressions = [];

        if (!parser.Match(HarbourSyntaxKind.RIGHT_BRACE))
        {
            expressions.Add(parser.Parse() ?? throw new InvalidSyntaxException(
                $"Expected parameter in codeblock after token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));

            while (parser.Match(HarbourSyntaxKind.COMMA))
            {
                parser.Consume(HarbourSyntaxKind.COMMA);
                expressions.Add(parser.Parse() ?? throw new InvalidSyntaxException(
                    $"Expected parameter in codeblock after token '{token.Text}' on line {token.Line}, column {token.Start}, found null."));
            }
        }

        parser.Consume(HarbourSyntaxKind.RIGHT_BRACE);

        return new CodeblockExpression(parameters, expressions);
    }

    public Precedence GetPrecedence()
    {
        return Precedence.NONE;
    }

    private static NameExpression ParseParams(ExpressionParser expressionParser, HarbourSyntaxToken token)
    {
        var name = expressionParser.Parse();
        return name as NameExpression ??
               throw new InvalidSyntaxException(
                   $"Expected parameter in codeblock after token '{token.Text}' on line {token.Line}, column {token.Start}, found null.");
    }
}