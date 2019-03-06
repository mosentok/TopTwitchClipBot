using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Functions
{
    [TestFixture]
    public class GetChannelConfigHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITopTwitchClipBotContext> _Context;
        GetChannelConfigHelper _Helper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITopTwitchClipBotContext>();
            _Helper = new GetChannelConfigHelper(_Log.Object, _Context.Object);
        }
        [Test]
        public async Task GetChannelConfigAsync()
        {
            const int channelId = 123;
            _Log.Setup(s => s.LogInformation($"Getting channel config for channel '{channelId}'."));
            var container = new ChannelConfigContainer();
            _Context.Setup(s => s.GetChannelConfigAsync(channelId)).ReturnsAsync(container);
            _Log.Setup(s => s.LogInformation($"Got channel config for channel '{channelId}'."));
            var result = await _Helper.GetChannelConfigAsync(channelId);
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(result, Is.EqualTo(container));
        }
    }
}
