using System;
using NUnit.Framework;
using TheTVDBTools;

namespace TheTVDBToolsTests
{
    [TestFixture]
    public class TVShowNameParserTests
    {
        [Test]
        public void ParseS99E99Namgs()
        {
            string[] goodNames = { 
                                     @"s001e001.avi",
                                     @"My Show Season 1 Episode 4.avi",
                                     @"s001e001.avi",
                                     @"My Show - S01E02 - Show Name.avi", 
                                     @"My Show - s01e02 - Show Name.avi", 
                                    // @"My Show - s01_e02 - Show Name.avi", 
                                     @"s001m001.avi",
                                 };

            string[] badNames = { 
                                    @"my show se01.avi", 
                                    @"myinese02.avi",
                                    @"My Show Season1 Episode4.avi",
                                };

            foreach (string name in goodNames)
            {
                TVShowNameParser show = new TVShowNameParser(name);   

                Assert.That((show.Episode > -1) && (show.Episode < 100), String.Format("{0} should pass (Episode not in range)", name));
                Assert.That((show.Season > -1) && (show.Season < 100), String.Format("{0} should pass (Season not in range)", name));
            }

            foreach (string name in badNames)
            {
                TVShowNameParser show = new TVShowNameParser(name);

                Assert.That((show.Episode == -1) || (show.Episode == -1), String.Format("{0} should NOT pass", name));
                
            }

        }
    }
}