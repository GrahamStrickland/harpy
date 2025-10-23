using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// <summary>
///     One of the two parser interfaces used by the Pratt parser. An
///     <see cref="IInfixSubParser" /> is associated with a token that appears in the middle of the
///     expression it parses. Its <see cref="Parse" /> method will be called after the left-hand
///     side has been parsed, and it in turn is responsible for parsing everything
///     that comes after the token. This is also used for postfix expressions, in
///     which case it simply doesn't consume any more tokens in its <see cref="Parse" /> call.
/// </summary>
public interface IInfixSubParser : ISubParser
{
    Expression Parse(ExpressionParser expressionParser, Expression left, HarbourSyntaxToken token);
}