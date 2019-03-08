using Discord.Commands;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Core
{
    [TestFixture]
    public class TopClipsModuleHelperTests
    {
        Mock<IConfigurationWrapper> _ConfigWrapper;
        TopClipsModuleHelper _TopClipsModuleHelper;
        [SetUp]
        public void SetUp()
        {
            _ConfigWrapper = new Mock<IConfigurationWrapper>();
            _TopClipsModuleHelper = new TopClipsModuleHelper(_ConfigWrapper.Object);
        }
        [TestCase("Off", true)]
        [TestCase("off", true)]
        [TestCase("All The Time", true)]
        [TestCase("all the time", true)]
        [TestCase("8 and 22", false)]
        public void ShouldTurnCommandOff(string input, bool expectedResult)
        {
            var result = _TopClipsModuleHelper.ShouldTurnCommandOff(input);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase("8 and 22", true, 3)]
        [TestCase("off", false, 1)]
        public void IsCorrectBetweenLength(string input, bool expectedResult, int expectedLength)
        {
            var result = _TopClipsModuleHelper.IsCorrectBetweenLength(input, out var split);
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(split.Length, Is.EqualTo(expectedLength));
        }
        [TestCase("0", 0, true)]
        [TestCase("1", 1, true)]
        [TestCase("23", 23, true)]
        [TestCase("24", 24, false)]
        [TestCase("asdf", 0, false)]
        public void IsInRange(string postingHourString, int expectedPostingHour, bool expectedResult)
        {
            var result = _TopClipsModuleHelper.IsInRange(postingHourString, out var postingHour);
            Assert.That(postingHour, Is.EqualTo(expectedPostingHour));
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase("all", true)]
        [TestCase("all broadcasters", true)]
        [TestCase("asdf", false)]
        public void ShouldDeleteAll(string broadcaster, bool expectedResult)
        {
            var result = _TopClipsModuleHelper.ShouldDeleteAll(broadcaster);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase(8, 22, "```between 8 and 22```")]
        [TestCase(8, null, "```all the time```")]
        [TestCase(null, 22, "```all the time```")]
        [TestCase(null, null, "```all the time```")]
        public void DeterminePostWhen(int? minPostingHour, int? maxPostingHour, string expectedResult)
        {
            var container = new ChannelConfigContainer { MinPostingHour = minPostingHour, MaxPostingHour = maxPostingHour };
            var result = _TopClipsModuleHelper.DeterminePostWhen(container);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void BuildStreamersText_NoStreamers()
        {
            var container = new ChannelConfigContainer { Broadcasters = new List<BroadcasterConfigContainer>() };
            const string expectedResult = "no streamers have been set up";
            _ConfigWrapper.Setup(s => s["NoStreamersText"]).Returns(expectedResult);
            var result = _TopClipsModuleHelper.BuildStreamersText(container);
            _ConfigWrapper.VerifyAll();
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void BuildStreamersText_SomeStreamers()
        {
            var broadcaster1 = new BroadcasterConfigContainer { Broadcaster = "broadcaster123" };
            var broadcaster2 = new BroadcasterConfigContainer { Broadcaster = "anotherstreamer", NumberOfClipsPerDay = 4 };
            var broadcaster3 = new BroadcasterConfigContainer { Broadcaster = "omegalulmydude" };
            var broadcaster4 = new BroadcasterConfigContainer { Broadcaster = "zzzzzzzzzzz", NumberOfClipsPerDay = 1 };
            var broadcasters = new List<BroadcasterConfigContainer> { broadcaster1, broadcaster2, broadcaster3, broadcaster4 };
            var container = new ChannelConfigContainer { Broadcasters = broadcasters };
            const string streamersBegin = "here's your list of streamers";
            _ConfigWrapper.Setup(s => s["StreamersFieldBeginText"]).Returns(streamersBegin);
            const string newLineDelimiter = "NEWLINE";
            _ConfigWrapper.Setup(s => s["NewLineDelimiter"]).Returns(newLineDelimiter);
            var streamerFormats = new List<string> { "```lessNEWLINE{0}```", "```diffNEWLINE{0}```", "```fixNEWLINE{0}```" };
            _ConfigWrapper.Setup(s => s.Get<List<string>>("StreamersFormats")).Returns(streamerFormats);
            var result = _TopClipsModuleHelper.BuildStreamersText(container);
            _ConfigWrapper.VerifyAll();
            const string expectedResult =
                "here's your list of streamers" +
                "```less\nanotherstreamer, 4 clips per day```" +
                "```diff\nbroadcaster123, no limit```" +
                "```fix\nomegalulmydude, no limit```" +
                "```less\nzzzzzzzzzzz, 1 clip per day```";
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [TestCase(null, "```no limit```")]
        [TestCase(1, "```1 clip at a time```")]
        [TestCase(2, "```2 clips at a time```")]
        public void DetermineClipsAtATime(int? numberOfClipsAtATime, string expectedResult)
        {
            var container = new ChannelConfigContainer { NumberOfClipsAtATime = numberOfClipsAtATime };
            var result = _TopClipsModuleHelper.DetermineClipsAtATime(container);
            Assert.That(result, Is.EqualTo(expectedResult));
        }
        [Test]
        public void BuildChannelConfigEmbed()
        {
            const string channelName = "our channel name";
            const string iconUrl = "https://twitch.tv/icon.png";
            const string helpText = "this is the help text";
            var context = new Mock<ICommandContext>();
            context.Setup(s => s.Channel.Name).Returns(channelName);
            context.Setup(s => s.Guild.IconUrl).Returns(iconUrl);
            _ConfigWrapper.Setup(s => s["HelpQuestionFieldText"]).Returns(helpText);
            const string postWhen = "all the time";
            const string streamersText = "a list of streamers";
            const string clipsAtATime = "some clips at a time";
            var result = _TopClipsModuleHelper.BuildChannelConfigEmbed(context.Object, postWhen, streamersText, clipsAtATime);
            context.VerifyAll();
            _ConfigWrapper.VerifyAll();
            Assert.That(result.Author.Value.Name, Is.EqualTo($"Setup for Channel # {channelName}"));
            Assert.That(result.Author.Value.IconUrl, Is.EqualTo(iconUrl));
            Assert.That(result.Fields[0].Name, Is.EqualTo("Post When?"));
            Assert.That(result.Fields[0].Value, Is.EqualTo(postWhen));
            Assert.That(result.Fields[0].Inline, Is.EqualTo(true));
            Assert.That(result.Fields[1].Name, Is.EqualTo("Clips at a Time"));
            Assert.That(result.Fields[1].Value, Is.EqualTo(clipsAtATime));
            Assert.That(result.Fields[1].Inline, Is.EqualTo(true));
            Assert.That(result.Fields[2].Name, Is.EqualTo("Streamers"));
            Assert.That(result.Fields[2].Value, Is.EqualTo(streamersText));
            Assert.That(result.Fields[2].Inline, Is.EqualTo(false));
            Assert.That(result.Fields[3].Name, Is.EqualTo("Need Help?"));
            Assert.That(result.Fields[3].Value, Is.EqualTo(helpText));
            Assert.That(result.Fields[3].Inline, Is.EqualTo(false));
        }
    }
}
