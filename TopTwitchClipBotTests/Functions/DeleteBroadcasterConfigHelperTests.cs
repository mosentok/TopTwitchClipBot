using Moq;
using NUnit.Framework;
using System.Threading.Tasks;
using TopTwitchClipBotFunctions.Helpers;
using TopTwitchClipBotFunctions.Wrappers;
using TopTwitchClipBotModel;

namespace TopTwitchClipBotTests.Functions
{
    public class DeleteBroadcasterConfigHelperTests
    {
        Mock<ILoggerWrapper> _Log;
        Mock<ITopTwitchClipBotContext> _Context;
        DeleteBroadcasterConfigHelper _Helper;
        [SetUp]
        public void SetUp()
        {
            _Log = new Mock<ILoggerWrapper>();
            _Context = new Mock<ITopTwitchClipBotContext>();
            _Helper = new DeleteBroadcasterConfigHelper(_Log.Object, _Context.Object);
        }
        [TestCase(null)]
        [TestCase("")]
        public async Task DeleteBroadcasterConfigAsync_NullOrEmpty(string broadcaster)
        {
            const int channelId = 123;
            _Log.Setup(s => s.LogInformation($"Deleting broadcaster config for channel '{channelId}'."));
            _Context.Setup(s => s.DeleteBroadcasterConfigAsync(channelId)).Returns(Task.CompletedTask);
            _Log.Setup(s => s.LogInformation($"Deleted broadcaster config for channel '{channelId}'."));
            var task = _Helper.DeleteBroadcasterConfigAsync(channelId, broadcaster);
            await task;
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
        [Test]
        public async Task DeleteBroadcasterConfigAsync()
        {
            const int channelId = 123;
            const string broadcaster = "broadcaster";
            _Log.Setup(s => s.LogInformation($"Deleting broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'."));
            _Context.Setup(s => s.DeleteBroadcasterConfigAsync(channelId, broadcaster)).Returns(Task.CompletedTask);
            _Log.Setup(s => s.LogInformation($"Deleted broadcaster config for channel '{channelId}' broadcaster '{broadcaster}'."));
            var task = _Helper.DeleteBroadcasterConfigAsync(channelId, broadcaster);
            await task;
            _Log.VerifyAll();
            _Context.VerifyAll();
            Assert.That(task.IsCompletedSuccessfully, Is.True);
        }
    }
}
