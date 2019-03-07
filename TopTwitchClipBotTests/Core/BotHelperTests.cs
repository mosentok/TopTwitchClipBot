using Discord;
using Discord.WebSocket;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;

namespace TopTwitchClipBotTests.Core
{
    [TestFixture]
    public class BotHelperTests
    {
        Mock<IConfigurationWrapper> _ConfigWrapper;
        BotHelper _BotHelper;
        [SetUp]
        public void SetUp()
        {
            _ConfigWrapper = new Mock<IConfigurationWrapper>();
            _BotHelper = new BotHelper(_ConfigWrapper.Object);
        }
        [Test]
        public void ShouldProcessMessage_DmChannel()
        {
            var message = new Mock<IMessage>();
            var dmChannel = new Mock<IDMChannel>();
            var currentUser = new Mock<IUser>();
            message.Setup(s => s.Channel).Returns(dmChannel.Object);
            var prefixDictionary = new Dictionary<decimal, string>();
            var result = _BotHelper.ShouldProcessMessage(message.Object, currentUser.Object, prefixDictionary);
            message.VerifyAll();
            Assert.That(result.ShouldProcessMessage, Is.False);
            Assert.That(result.UserMessage, Is.Null);
            Assert.That(result.ArgPos, Is.EqualTo(0));
        }
        [Test]
        public void ShouldProcessMessage_BotAuthor()
        {
            var message = new Mock<IMessage>();
            message.Setup(s => s.Author.IsBot).Returns(true);
            var textChannel = new Mock<ITextChannel>();
            message.Setup(s => s.Channel).Returns(textChannel.Object);
            var currentUser = new Mock<IUser>();
            var prefixDictionary = new Dictionary<decimal, string>();
            var result = _BotHelper.ShouldProcessMessage(message.Object, currentUser.Object, prefixDictionary);
            message.VerifyAll();
            Assert.That(result.ShouldProcessMessage, Is.False);
            Assert.That(result.UserMessage, Is.Null);
            Assert.That(result.ArgPos, Is.EqualTo(0));
        }
        [Test]
        public void ShouldProcessMessage_SystemMessage()
        {
            var message = new Mock<ISystemMessage>();
            message.Setup(s => s.Author.IsBot).Returns(false);
            var textChannel = new Mock<ITextChannel>();
            var currentUser = new Mock<IUser>();
            message.Setup(s => s.Channel).Returns(textChannel.Object);
            var prefixDictionary = new Dictionary<decimal, string>();
            var result = _BotHelper.ShouldProcessMessage(message.Object, currentUser.Object, prefixDictionary);
            message.VerifyAll();
            Assert.That(result.ShouldProcessMessage, Is.False);
            Assert.That(result.UserMessage, Is.Null);
            Assert.That(result.ArgPos, Is.EqualTo(0));
        }
        [Test]
        public void ShouldProcessMessage_DefaultPrefix_NoPrefixes()
        {
            var message = new Mock<IUserMessage>();
            message.Setup(s => s.Author.IsBot).Returns(false);
            var textChannel = new Mock<ITextChannel>();
            message.Setup(s => s.Channel).Returns(textChannel.Object);
            var prefixDictionary = new Dictionary<decimal, string>();
            const string defaultPrefix = "!";
            _ConfigWrapper.Setup(s => s["DefaultPrefix"]).Returns(defaultPrefix);
            message.Setup(s => s.Content).Returns("hey what's up");
            var currentUser = new Mock<IUser>();
            const ulong channelId = 123;
            currentUser.Setup(s => s.Id).Returns(channelId);
            var result = _BotHelper.ShouldProcessMessage(message.Object, currentUser.Object, prefixDictionary);
            message.VerifyAll();
            _ConfigWrapper.VerifyAll();
            Assert.That(result.ShouldProcessMessage, Is.False);
            Assert.That(result.UserMessage, Is.Null);
            Assert.That(result.ArgPos, Is.EqualTo(0));
        }
        [Test]
        public void ShouldProcessMessage_CustomPrefix_NoPrefixes()
        {
            var message = new Mock<IUserMessage>();
            message.Setup(s => s.Author.IsBot).Returns(false);
            var textChannel = new Mock<ITextChannel>();
            const ulong channelId = 123;
            textChannel.Setup(s => s.Id).Returns(channelId);
            message.Setup(s => s.Channel).Returns(textChannel.Object);
            var prefixDictionary = new Dictionary<decimal, string> { { channelId, "!" } };
            message.Setup(s => s.Content).Returns("hey what's up");
            var currentUser = new Mock<IUser>();
            currentUser.Setup(s => s.Id).Returns(channelId);
            var result = _BotHelper.ShouldProcessMessage(message.Object, currentUser.Object, prefixDictionary);
            message.VerifyAll();
            Assert.That(result.ShouldProcessMessage, Is.False);
            Assert.That(result.UserMessage, Is.Null);
            Assert.That(result.ArgPos, Is.EqualTo(0));
        }
        [Test]
        public void ShouldProcessMessage_Yes()
        {
            var message = new Mock<IUserMessage>();
            message.Setup(s => s.Author.IsBot).Returns(false);
            var textChannel = new Mock<ITextChannel>();
            const ulong channelId = 123;
            textChannel.Setup(s => s.Id).Returns(channelId);
            message.Setup(s => s.Channel).Returns(textChannel.Object);
            var prefixDictionary = new Dictionary<decimal, string> { { channelId, "!" } };
            message.Setup(s => s.Content).Returns("!topclips of myfavoritestreamer");
            var currentUser = new Mock<IUser>();
            currentUser.Setup(s => s.Id).Returns(channelId);
            var result = _BotHelper.ShouldProcessMessage(message.Object, currentUser.Object, prefixDictionary);
            message.VerifyAll();
            Assert.That(result.ShouldProcessMessage, Is.True);
            Assert.That(result.UserMessage, Is.EqualTo(message.Object));
            Assert.That(result.ArgPos, Is.EqualTo(1));
        }
    }
}
