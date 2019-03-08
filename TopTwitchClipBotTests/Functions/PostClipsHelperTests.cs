﻿using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Models;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Functions
{
    [TestFixture]
    public class PostClipsHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITopTwitchClipBotContext> _Context;
        Mock<ITwitchWrapper> _TwitchWrapper;
        Mock<IDiscordWrapper> _DiscordWrapper;
        PostClipsHelper _Helper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITopTwitchClipBotContext>();
            _TwitchWrapper = new Mock<ITwitchWrapper>();
            _DiscordWrapper = new Mock<IDiscordWrapper>();
            _Helper = new PostClipsHelper(_Log.Object, _Context.Object, _TwitchWrapper.Object, _DiscordWrapper.Object);
        }
        [TestCase(null, "2019-02-13", "2019-02-12", true)]
        [TestCase(1, "2019-02-13", "2019-02-12", false)]
        [TestCase(1, "2019-02-13", "2019-02-13", false)]
        [TestCase(1, "2019-02-13", "2019-02-11", true)]
        public void IsReadyToPost(int? numberOfClipsPerDay, string nowString, string stampString, bool expectedResult)
        {
            var stamp = DateTime.Parse(stampString);
            var existingHistory = new BroadcasterHistoryContainer { Stamp = stamp };
            var existingHistories = new List<BroadcasterHistoryContainer> { existingHistory };
            var yesterday = DateTime.Parse(nowString).AddDays(-1);
            var pendingBroadcasterConfig = new PendingBroadcasterConfig { NumberOfClipsPerDay = numberOfClipsPerDay, ExistingHistories = existingHistories };
            var broadcasters = new List<PendingBroadcasterConfig> { pendingBroadcasterConfig };
            var pendingChannelConfigContainer = new PendingChannelConfigContainer { Broadcasters = broadcasters };
            var channelContainers = new List<PendingChannelConfigContainer> { pendingChannelConfigContainer };
            var result = _Helper.ReadyToPostContainers(channelContainers, yesterday);
            var anyReadyBroadcasters = result.SelectMany(s => s.Broadcasters).Any();
            Assert.That(anyReadyBroadcasters, Is.EqualTo(expectedResult));
        }
        [Test]
        public async Task BuildClipContainers()
        {
            const string broadcaster = "broadcaster";
            var pendingBroadcasterConfig = new PendingBroadcasterConfig { Broadcaster = broadcaster };
            var broadcasters = new List<PendingBroadcasterConfig> { pendingBroadcasterConfig };
            var container = new PendingChannelConfigContainer { Broadcasters = broadcasters };
            var containers = new List<PendingChannelConfigContainer> { container };
            const string topClipsEndpoint = "https://twitch.tv/topclips";
            const string clientId = "123";
            const string accept = "application/json";
            var channelEndpoint = $"{topClipsEndpoint}&channel={broadcaster}";
            var clip = new Clip();
            var clips = new List<Clip> { clip };
            var getClipsResponse = new GetClipsResponse { Clips = clips };
            _TwitchWrapper.Setup(s => s.GetClips(channelEndpoint, clientId, accept)).ReturnsAsync(getClipsResponse);
            var results = await _Helper.BuildClipContainers(topClipsEndpoint, clientId, accept, containers);
            _TwitchWrapper.VerifyAll();
            Assert.That(results.Count, Is.EqualTo(1));
            var result = results[0];
            Assert.That(result.Clips, Is.EqualTo(clips));
            Assert.That(result.Broadcaster, Is.EqualTo(broadcaster));
        }
    }
}
