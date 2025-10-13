using Harpy.Lexer;

namespace HarpyTests.Utils;

internal class SyntaxTokenUtils
{
    public static bool AssertTokenListsEqual(List<HarbourSyntaxToken> obs, List<HarbourSyntaxToken> expected,
        bool checkTrivia = false)
    {
        if (obs.Count != expected.Count)
        {
            Assert.Fail($"Token count mismatch. Expected {expected.Count}, but got {obs.Count}.");
            return false;
        }

        for (var i = 0; i < obs.Count; i++)
        {
            if (!SyntaxElementsEqual(obs[i], expected[i]))
            {
                Assert.Fail(
                    $"Token lines mismatch at index {i}. "
                    + $"Expected Token({expected[i].Kind}, '{expected[i].Text}', {expected[i].Line}, {expected[i].Start}, {expected[i].End}), "
                    + $"but got Token({obs[i].Kind}, '{obs[i].Text}', {obs[i].Line}, {obs[i].Start}, {obs[i].End})."
                );
                return false;
            }

            if (checkTrivia)
            {
                if (obs[i].LeadingTrivia.Count != expected[i].LeadingTrivia.Count)
                {
                    Assert.Fail(
                        $"Leading trivia count mismatch. Expected {expected[i].LeadingTrivia.Count}, but got {obs[i].LeadingTrivia.Count}.");
                    return false;
                }

                if (obs[i].TrailingTrivia.Count != expected[i].TrailingTrivia.Count)
                {
                    Assert.Fail(
                        $"Trailing trivia count mismatch. Expected {expected[i].TrailingTrivia.Count}, but got {obs[i].TrailingTrivia.Count}.");
                    return false;
                }

                if (!AssertTriviaListsEqual(obs[i].LeadingTrivia, expected[i].LeadingTrivia)) return false;

                if (!AssertTriviaListsEqual(obs[i].TrailingTrivia, expected[i].TrailingTrivia)) return false;
            }
        }

        return true;
    }

    private static bool SyntaxElementsEqual(HarbourSyntaxElement obs, HarbourSyntaxElement expected)
    {
        return obs.Kind == expected.Kind &&
               obs.Text == expected.Text &&
               obs.Line == expected.Line &&
               obs.Start == expected.Start &&
               obs.End == expected.End;
    }

    private static bool AssertTriviaListsEqual(List<HarbourSyntaxTrivia> obsTriviaList,
        List<HarbourSyntaxTrivia> expectedTriviaList)
    {
        for (var j = 0; j < expectedTriviaList.Count; j++)
        {
            var obsTrivia = obsTriviaList[j];
            var expectedTrivia = expectedTriviaList[j];

            if (!SyntaxElementsEqual(obsTrivia, expectedTrivia))
            {
                Assert.Fail(
                    $"Trivia lines mismatch at index {j}. "
                    + $"Expected Trivia({expectedTrivia.Kind}, '{expectedTrivia.Text}', {expectedTrivia.Line}, {expectedTrivia.Start}, {expectedTrivia.End}), "
                    + $"but got Trivia({obsTrivia.Kind}, '{obsTrivia.Text}', {obsTrivia.Line}, {obsTrivia.Start}, {obsTrivia.End}).");
                return false;
            }
        }

        return true;
    }
}