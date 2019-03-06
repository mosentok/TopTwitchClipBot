using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Functions
{
    [TestFixture]
    public class PostChannelConfigHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITopTwitchClipBotContext> _Context;
        PostChannelConfigHelper _Helper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITopTwitchClipBotContext>();
            _Helper = new PostChannelConfigHelper(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task RunAsync()
        {
            const int channelId = 123;
            _Log.Setup(s => s.LogInformation($"Posting channel config for channel '{channelId}'."));
            var inputContainer = new ChannelConfigContainer();
            var outputContainer = new ChannelConfigContainer();
            _Context.Setup(s => s.SetChannelConfigAsync(channelId, inputContainer)).ReturnsAsync(outputContainer);
            _Log.Setup(s => s.LogInformation($"Posted channel config for channel '{channelId}'."));
            var result = await _Helper.PostChannelConfigAsync(channelId, inputContainer);
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result, Is.EqualTo(outputContainer));
        }
    }
}
