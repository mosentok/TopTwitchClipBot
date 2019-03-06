using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Functions
{
    [TestFixture]
    public class PostBroadcasterConfigHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITopTwitchClipBotContext> _Context;
        PostBroadcasterConfigHelper _Helper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITopTwitchClipBotContext>();
            _Helper = new PostBroadcasterConfigHelper(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task PostBroadcasterConfigAsync()
        {
            const int channelId = 123;
            const string broadcaster = "broadcaster";
            _Log.Setup(s => s.LogInformation($"Posting broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'."));
            var inputContainer = new BroadcasterConfigContainer();
            var outputContainer = new BroadcasterConfigContainer();
            _Context.Setup(s => s.SetBroadcasterConfigAsync(channelId, broadcaster, inputContainer)).ReturnsAsync(outputContainer);
            _Log.Setup(s => s.LogInformation($"Posted broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'."));
            var result = await _Helper.PostBroadcasterConfigAsync(channelId, broadcaster, inputContainer);
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result, Is.EqualTo(result));
        }
    }
}
