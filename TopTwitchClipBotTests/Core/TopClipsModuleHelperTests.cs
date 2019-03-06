using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TopTwitchClipBotCore.Helpers;
using TopTwitchClipBotCore.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Core
{
    [TestFixture]
    public class TopClipsModuleHelperTests
    {
        Mock<IFunctionWrapper> _FunctionWrapper;
        TopClipsModuleHelper _TopClipsModuleHelper;
        [SetUp]
        public void SetUp()
        {
            _FunctionWrapper = new Mock<IFunctionWrapper>();
            _TopClipsModuleHelper = new TopClipsModuleHelper(_FunctionWrapper.Object);
        }
        [Test]
        public async Task GetAsync()
        {
            const int channelId = 123;
            var container = new ChannelConfigContainer();
            _FunctionWrapper.Setup(s => s.GetChannelConfigAsync(channelId)).ReturnsAsync(container);
            var result = await _TopClipsModuleHelper.GetAsync(channelId);
            Assert.That(result, Is.EqualTo(container));
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
    }
}
