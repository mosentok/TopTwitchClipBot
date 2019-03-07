﻿using NUnit.Framework;
using TopTwitchClipBotCore.Helpers;

namespace TopTwitchClipBotTests.Core
{
    [TestFixture]
    public class TopClipsModuleHelperTests
    {
        TopClipsModuleHelper _TopClipsModuleHelper;
        [SetUp]
        public void SetUp()
        {
            _TopClipsModuleHelper = new TopClipsModuleHelper();
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
    }
}
