using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace HumanUI.Tests
{
    public class HUI_UtilTests
    {
        [Fact]
        public void BoolsFromString_RoundTripsThroughStringFromBools()
        {
            // stringFromBools emits a trailing comma; after a split the final empty token
            // gets parsed back as a (default) False. The historical format always carries
            // that extra False, so test that the original list is a prefix of the round trip.
            var input = new List<bool> { true, false, true, true, false };
            string serialized = HUI_Util.stringFromBools(input);
            List<bool> roundTripped = HUI_Util.boolsFromString(serialized);
            Assert.Equal(input, roundTripped.Take(input.Count));
            Assert.Equal(input.Count + 1, roundTripped.Count);
            Assert.False(roundTripped[^1]);
        }

        [Fact]
        public void StringFromBools_EmitsCommaSeparatedTrailingComma()
        {
            var input = new List<bool> { true, false, true };
            Assert.Equal("True,False,True,", HUI_Util.stringFromBools(input));
        }

        [Fact]
        public void StringFromBools_EmptyListReturnsEmptyString()
        {
            Assert.Equal(string.Empty, HUI_Util.stringFromBools(new List<bool>()));
        }

        [Fact]
        public void BoolsFromString_UnparseableTokensBecomeFalse()
        {
            // Boolean.TryParse on "garbage" leaves bl=false, which is the historical behavior we want to preserve.
            List<bool> result = HUI_Util.boolsFromString("True,garbage,False");
            Assert.Equal(new List<bool> { true, false, false }, result);
        }

        [Fact]
        public void StringFromStrings_JoinsWithPipe()
        {
            var input = new List<string> { "alpha", "beta", "gamma" };
            Assert.Equal("alpha|beta|gamma", HUI_Util.stringFromStrings(input));
        }

        [Fact]
        public void StringsFromString_SplitsOnPipe()
        {
            List<string> result = HUI_Util.stringsFromString("alpha|beta|gamma");
            Assert.Equal(new List<string> { "alpha", "beta", "gamma" }, result);
        }

        [Fact]
        public void StringFromStrings_RoundTripsThroughStringsFromString()
        {
            var input = new List<string> { "one", "two with spaces", "three" };
            string serialized = HUI_Util.stringFromStrings(input);
            List<string> roundTripped = HUI_Util.stringsFromString(serialized);
            Assert.Equal(input, roundTripped);
        }

        [Fact]
        public void StringFromStrings_EmptyListReturnsEmptyString()
        {
            Assert.Equal(string.Empty, HUI_Util.stringFromStrings(new List<string>()));
        }
    }
}
