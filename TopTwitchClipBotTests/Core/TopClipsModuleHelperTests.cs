﻿using Discord.Commands;
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
            var result = _TopClipsModuleHelper.BuildChannelConfigEmbed(context.Object, postWhen, streamersText);
            context.VerifyAll();
            _ConfigWrapper.VerifyAll();
            Assert.That(result.Author.Value.Name, Is.EqualTo($"Setup for Channel # {channelName}"));
            Assert.That(result.Author.Value.IconUrl, Is.EqualTo(iconUrl));
            Assert.That(result.Fields[0].Name, Is.EqualTo("Post When?"));
            Assert.That(result.Fields[0].Value, Is.EqualTo(postWhen));
            Assert.That(result.Fields[1].Name, Is.EqualTo("Streamers"));
            Assert.That(result.Fields[1].Value, Is.EqualTo(streamersText));
            Assert.That(result.Fields[2].Name, Is.EqualTo("Need Help?"));
            Assert.That(result.Fields[2].Value, Is.EqualTo(helpText));
        }
    }
}
