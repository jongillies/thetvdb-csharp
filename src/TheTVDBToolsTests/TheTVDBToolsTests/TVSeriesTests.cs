using System;
using NUnit.Framework;
using TheTVDBTools;

namespace TheTVDBToolsTests
{
    [TestFixture]
    public class TVSeriesTests
    {
        [Test]
        public void TestFileNameCleaner()
        {
            string[] goodNames = { 
                                     @"abcdefghijklmnopqrstuvwxyz",
                                 };

            string[] badNames = { 
                                    @"!", 
                                    @"xx\\xx\\xx",
                                    @"xx/xx////xx",
                                    @"foo: bar",
                                    @"xx/xx////xx",
                                };

            foreach (string name in goodNames)
            {
                string cleanName = TVSeries.CleanName(name);

                Assert.That(name == cleanName, String.Format("{0} should pass", name));
            }

            foreach (string name in badNames)
            {
                string cleanName = TVSeries.CleanName(name);

                Assert.That(name != cleanName, String.Format("{0} should NOT pass", name));

            }


        }
    }
}