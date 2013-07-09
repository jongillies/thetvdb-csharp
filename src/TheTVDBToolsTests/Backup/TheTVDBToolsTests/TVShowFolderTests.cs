using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using TheTVDBTools;

namespace TheTVDBToolsTests
{
    [TestFixture]
    public class TVShowFolderTests
    {
        private const string SHOW_FOLDER = "TestDataForShows";
        private string _CurrentFolder;
        private string _ShowParentFolder;

        string[] goodShowNames = { 
                                     @"American Dad ()",
                                     @"Battlestar Galactica ( )",
                                     @"Californication ( 2004)",
                                     @"Curb Your Enthusiasm (0000)",
                                     @"Deadwood (10000)",
                                     @"Family Guy",
                                     @"Fringe",
                                     @"Good Eats",
                                     @"John Adams",
                                     @"Lost",
                                     @"Rescue Me (2004)",
                                     @"Scrubs",
                                     @"Sons of Anarchy",
                                     @"South Park",
                                     @"SpongeBob SquarePants",
                                     @"The Closer",
                                     @"The Sarah Connor Chronicles",
                                 };

        string[] yearNames = { 
                                 @"American Dad (0001)",
                                 @"Battlestar Galactica (2003)",
                             };

        [TestFixtureSetUp]
        public void PerFixtureSetup()
        {
            // Normally this will be the bin\Debug folder
            _CurrentFolder = Environment.CurrentDirectory;

            _ShowParentFolder = Path.Combine(_CurrentFolder, SHOW_FOLDER);

            // Create a folder to hold show name folders for testing
            Directory.CreateDirectory(_ShowParentFolder);
        }

        [TestFixtureTearDown]
        public void PerFixtureTearDown()
        {
            // Remove folder 
            Directory.Delete(Path.Combine(_CurrentFolder, SHOW_FOLDER));

        }

        [Test]
        public void TestGoodNames()
        {
            foreach (string name in goodShowNames)
            {
                string showFolder = Path.Combine(_ShowParentFolder, name);

                Directory.CreateDirectory(showFolder);

                TVShowFolder f = new TVShowFolder(showFolder);

                Assert.That(f.IsValid,String.Format("{0} IsValid should return true", name));

                Assert.That((f.Year >= -1) && (f.Year <= 9999), "{0}Year parsed is out of range", name);
                Directory.Delete(showFolder);
            }
        }

        [Test]
        public void TestYearNames()
        {

            // Create and initialize a new Dictionary.
            IDictionary<int, string> Dictionary = new Dictionary<int, string>();
            Dictionary.Add(-1, "Jesse Liberty");
            Dictionary.Add(2003, "Stacey Liberty (2003)");
            Dictionary.Add(9999, "John Galt (1999)");
            Dictionary.Add(0, "Ayn Rand (0000)");
            Dictionary.Add(5, "Ayn Rand (0005)");
            Dictionary.Add(1993, "Ayn Rand (1993)");

            foreach (KeyValuePair<int, string> kvp in Dictionary)
            {
                TVShowFolder f = new TVShowFolder(kvp.Value);

                if (f.Year != -1)
                {
                    Assert.AreEqual(Convert.ToInt32(kvp.Value), Is.EqualTo(kvp.Key));
                }
            }
        }
    }
}