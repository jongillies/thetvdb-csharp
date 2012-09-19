using NUnit.Framework;
using TheTVDBTools;

namespace TheTVDBToolsTests
{
    [TestFixture]
    public class TVSearcherCacheTests
    {
        private const string APIKEY = "713541F4CE28A6D8";

        [Test]
        public void SearchShow()
        {

            // Setup new search object
            TVSearcherCache tvSearcher = new TVSearcherCache(APIKEY);

            TVSeries tvSearch = tvSearcher.GetSeries("American Dad",@"\\jbod\video-rw\TV\Current\American Dad");

            if (tvSearch.IsSearchResult())
            {
                Assert.That(tvSearch.Series.id == "73141", "{0} should be 73141");
            }
        }

        [Test]
        public void GetShow()
        {
            // Setup new search object
            TVSearcher tvSearcher = new TVSearcher(APIKEY);

            tvSearcher.InternetAccess = false;

            TVSeries tvShow = tvSearcher.GetShow("73141");

            Assert.That((tvShow.Series.id == "73141"));



        }

    }
}