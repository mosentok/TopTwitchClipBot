using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Models;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Functions
{
    [TestFixture]
    public class PostBroadcasterConfigHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITopTwitchClipBotContext> _Context;
        Mock<ITwitchWrapper> _TwitchWrapper;
        PostBroadcasterConfigHelper _Helper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITopTwitchClipBotContext>();
            _TwitchWrapper = new Mock<ITwitchWrapper>();
            _Helper = new PostBroadcasterConfigHelper(_Log.Object, _Context.Object, _TwitchWrapper.Object);
        }
        [Test]
        public async Task PostBroadcasterConfigAsync()
        {
            const int channelId = 123;
            const string broadcaster = "broadcaster";
            _Log.Setup(s => s.LogInformation($"Checking that broadcaster '{broadcaster}' exists."));
            const string endpoint = "https://twitch.tv/users";
            const string clientId = "kdrgvhidhfskljecsemphgdhdfgfls";
            const string accept = "application/json";
            const string displayName = "BrOaDcAsTeR";
            var getUserResponse = new GetUsersResponse { Users = new List<User> { new User { DisplayName = displayName } } };
            _TwitchWrapper.Setup(s => s.GetUsers(endpoint, clientId, accept, broadcaster)).ReturnsAsync(getUserResponse);
            _Log.Setup(s => s.LogInformation($"Found broadcaster '{broadcaster}' with display name '{displayName}'."));
            _Log.Setup(s => s.LogInformation($"Posting broadcaster config for channel '{channelId}' broadcaster '{displayName}'."));
            var inputContainer = new BroadcasterConfigContainer();
            var outputContainer = new ChannelConfigContainer();
            _Context.Setup(s => s.SetBroadcasterConfigAsync(channelId, displayName, inputContainer)).ReturnsAsync(outputContainer);
            _Log.Setup(s => s.LogInformation($"Posted broadcaster config for channel '{channelId}' broadcaster '{displayName}'."));
            var result = await _Helper.PostBroadcasterConfigAsync(endpoint, clientId, accept, channelId, broadcaster, inputContainer);
            _Log.VerifyAll();
            _TwitchWrapper.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result.ErrorMessage, Is.Null.Or.Empty);
            Assert.That(result.ChannelConfigContainer, Is.EqualTo(outputContainer));
        }
        [Test]
        public async Task PostBroadcasterConfigAsync_NotFound()
        {
            const int channelId = 123;
            const string broadcaster = "broadcaster";
            _Log.Setup(s => s.LogInformation($"Checking that broadcaster '{broadcaster}' exists."));
            const string endpoint = "https://twitch.tv/users";
            const string clientId = "kdrgvhidhfskljecsemphgdhdfgfls";
            const string accept = "application/json";
            var getUserResponse = new GetUsersResponse { Users = new List<User>() };
            _TwitchWrapper.Setup(s => s.GetUsers(endpoint, clientId, accept, broadcaster)).ReturnsAsync(getUserResponse);
            var errorMessage = $"Could not find streamer '{broadcaster}'.";
            _Log.Setup(s => s.LogInformation(errorMessage));
            var inputContainer = new BroadcasterConfigContainer();
            var result = await _Helper.PostBroadcasterConfigAsync(endpoint, clientId, accept, channelId, broadcaster, inputContainer);
            _Log.VerifyAll();
            _TwitchWrapper.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result.ErrorMessage, Is.EqualTo(errorMessage));
            Assert.That(result.ChannelConfigContainer, Is.Null);
        }
    }
}
