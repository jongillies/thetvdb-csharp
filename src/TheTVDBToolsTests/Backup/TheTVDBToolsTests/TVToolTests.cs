using System;
using NUnit.Framework;
using TheTVDBTools;
using System.Collections.Generic;

namespace TheTVDBToolsTests
{
    [TestFixture]
    public class TVToolTests
    {
        private const string APIKEY = "713541F4CE28A6D8";

        IDictionary<int, string> Dictionary = new Dictionary<int, string>();

        [TestFixtureSetUp]
        public void Setup()
        {
            // Create and initialize a new Dictionary.
            //Dictionary.Add(77345, @"\\jbod\video\TV\Classic\Gilligans Island");
            //Dictionary.Add(76269, @"\\jbod\video\TV\Classic\The Tick (2001)");
            //Dictionary.Add(77398, @"\\jbod\video\TV\Classic\X-Files (1993)");
            //Dictionary.Add(73741, @"\\jbod\video\TV\Current\Rescue Me (2004)");
            //Dictionary.Add(75978, @"\\jbod\video\TV\Current\Family Guy");
            //Dictionary.Add(82066, @"\\jbod\video\TV\Current\Fringe");
            //Dictionary.Add(82696, @"\\jbod\video-rw\TV\Current\Sons of Anarchy");
            Dictionary.Add(76203, @"\\jbod\video-rw\TV\Current\Curb Your Enthusiasm");
            Dictionary.Add(73545, @"\\jbod\video\TV\Current\Battlestar Galactica (2003)");

        }

        [Test]
        public void TestFileNameCleaner()
        {

            foreach (KeyValuePair<int, string> kvp in Dictionary)
            {
                TVTool myShow = new TVTool(APIKEY, kvp.Value);

                if (myShow.HasID)
                {
                    Assert.That(Convert.ToUInt32(myShow.ID) == kvp.Key);

                    Console.WriteLine("{0} is the ID", myShow.ID);
                }
                else
                {
                    Console.WriteLine("Can't locate id:{0}",myShow.ErrorMessage);
                }
            }

        }
    }
}