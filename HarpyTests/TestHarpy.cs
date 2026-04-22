using Harpy.Lexer;
using Harpy.Parser;

namespace HarpyTests;

[TestClass]
public class TestHarpy
{
    [TestMethod]
    public void TestMultipleFunctions()
    {
        AssertParsedEqualsExpected(
            "/*******\n* Function header\n*******/\n\n#include \"header.ch\"\n\nstatic s_a := .f.\n\nfunction a(b)\n\n    local c := b\n\n    if b > 0\n        return 0\n    endif\n\nreturn c\n\nfunction d(e)\n\n    local c := e\n\n    if e > 0\n        return 0\n    endif\n\nreturn 1",
            """
            SourceRoot(name='test')
            └───VariableDeclaration(
                    scope
                    │
                    └───Token('static',7,[1:7))
                    name
                    │
                    └───Token('s_a',7,[7:11))
                    assignment
                    │
                    └───AssignmentExpression(
                            left
                            │
                            └───NameExpression(
                                    name
                                    │
                                    └───Token('s_a',7,[7:11))
                                )
                            right
                            │
                            └───LiteralExpression(
                                    literal(type=boolean)
                                    │
                                    └───Token('.f.',7,[14:18))
                                )
                        )
                )
            └───FunctionStatement(
                    name
                    │
                    └───Token('a',9,[9:11))
                    parameters
                    │
                    └───Token('b',9,[11:13))
                    body
                    │
                    └───VariableDeclaration(
                            scope
                            │
                            └───Token('local',11,[4:10))
                            name
                            │
                            └───Token('c',11,[10:12))
                            assignment
                            │
                            └───AssignmentExpression(
                                    left
                                    │
                                    └───NameExpression(
                                            name
                                            │
                                            └───Token('c',11,[10:12))
                                        )
                                    right
                                    │
                                    └───NameExpression(
                                            name
                                            │
                                            └───Token('b',11,[15:17))
                                        )
                                )
                        )
                    │
                    └───IfStatement(
                            ifCondition
                            │
                            └───OperatorExpression(
                                    left
                                    │
                                    └───NameExpression(
                                            name
                                            │
                                            └───Token('b',13,[7:9))
                                        )
                                    operator
                                    │
                                    └───Token('>',13,[9:11))
                                    right
                                    │
                                    └───LiteralExpression(
                                            literal(type=number)
                                            │
                                            └───Token('0',13,[11:13))
                                        )
                                )
                            ifBody
                            │
                            └───ReturnStatement(
                                    returnValue
                                    │
                                    └───LiteralExpression(
                                            literal(type=number)
                                            │
                                            └───Token('0',14,[15:17))
                                        )
                                )
                        )
                    returnValue
                    │
                    └───NameExpression(
                            name
                            │
                            └───Token('c',17,[7:9))
                        )
                )
            └───FunctionStatement(
                    name
                    │
                    └───Token('d',19,[9:11))
                    parameters
                    │
                    └───Token('e',19,[11:13))
                    body
                    │
                    └───VariableDeclaration(
                            scope
                            │
                            └───Token('local',21,[4:10))
                            name
                            │
                            └───Token('c',21,[10:12))
                            assignment
                            │
                            └───AssignmentExpression(
                                    left
                                    │
                                    └───NameExpression(
                                            name
                                            │
                                            └───Token('c',21,[10:12))
                                        )
                                    right
                                    │
                                    └───NameExpression(
                                            name
                                            │
                                            └───Token('e',21,[15:17))
                                        )
                                )
                        )
                    │
                    └───IfStatement(
                            ifCondition
                            │
                            └───OperatorExpression(
                                    left
                                    │
                                    └───NameExpression(
                                            name
                                            │
                                            └───Token('e',23,[7:9))
                                        )
                                    operator
                                    │
                                    └───Token('>',23,[9:11))
                                    right
                                    │
                                    └───LiteralExpression(
                                            literal(type=number)
                                            │
                                            └───Token('0',23,[11:13))
                                        )
                                )
                            ifBody
                            │
                            └───ReturnStatement(
                                    returnValue
                                    │
                                    └───LiteralExpression(
                                            literal(type=number)
                                            │
                                            └───Token('0',24,[15:17))
                                        )
                                )
                        )
                    returnValue
                    │
                    └───LiteralExpression(
                            literal(type=number)
                            │
                            └───Token('1',27,[7:9))
                        )
                )

            """
        );
    }

    // TODO: Move this to testing code generation when that is implemented.
    /// <summary>
    ///     Parses the given chunk of code and verifies that it matches the expected
    ///     pretty-printed result.
    /// </summary>
    private static void AssertParsedEqualsExpected(string source, string expected)
    {
        var lexer = new Lexer(source);
        var parser = new Parser(lexer);

        var root = parser.Parse("test");
        var actual = root.PrettyPrint();

        Assert.AreEqual(expected.Replace("\r\n", "\n"), actual);
    }
}