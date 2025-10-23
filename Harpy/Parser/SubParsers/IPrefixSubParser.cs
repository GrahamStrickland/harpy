using Harpy.AST.Expressions;
using Harpy.Lexer;

namespace Harpy.Parser.SubParsers;

/// One of the two interfaces used by the Pratt parser. A
/// <see cref="PrefixOperatorParser" />
/// is
/// associated with a token that appears at the beginning of an expression. Its
/// <see cref="Parse" />
/// method will be called with the consumed leading token, and the
/// parser is responsible for parsing anything that comes after that token.
/// This interface is also used for single-token expressions like variables, in
/// which case
/// <see cref="Parse" />
/// simply doesn't consume any more tokens.
public interface IPrefixSubParser : ISubParser
{
    Expression Parse(ExpressionParser parser, HarbourSyntaxToken token);
}