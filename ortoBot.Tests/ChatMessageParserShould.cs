using System;
using Xunit;
using ortoBot.Helpers;

namespace ortoBot.Tests
{
    public class ChatMessageParserShould
    {
        private readonly ChatMessageParser _chatMessageParser;

        public ChatMessageParserShould()
        {
            _chatMessageParser = new ChatMessageParser();
        }

        [Fact]
        public void ReturnRainbowGivenCorrectKeyword()
        {
            var result = ChatMessageParser.GetKeywordValue("hello everybody, i would like the effect tree:rainbow", "tree:");
            Assert.Equal("rainbow", result);
        }

        [Fact]
        public void ReturnNullIfNoKeyword()
        {
            var result = ChatMessageParser.GetKeywordValue("hello everybody, i would like to change the tree", "tree:");
            Assert.Null(result);
        }
    }
}
