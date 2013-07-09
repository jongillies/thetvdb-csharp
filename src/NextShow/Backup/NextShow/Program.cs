using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Supercoder.Tools;
using TheTVDBTools;

// "\\jbod\video\TV\Classic\Gilligans Island"   this show has a ".thetvdb.id" file
// "\\jbod\video\TV\Classic\The Tick (2001)"
// "\\jbod\video\TV\Classic\X-Files (1993)"
// "\\jbod\video\TV\Current\Rescue Me (2004)"
// "\\jbod\video\TV\Current\Family Guy"
// "\\jbod\video\TV\Current\Fringe"
// "\\jbod\video-rw\TV\Current\Sons of Anarchy"
// "\\jbod\video-rw\TV\Current\Curb Your Enthusiasm"
// "\\jbod\video\TV\Current\Battlestar Galactica"

namespace NextShow
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            ProgramOptions options = new ProgramOptions();

            // Parse command line options any errors and you get Usage
            if (options.ParseArgs(args) == false) ProgramOptions.Usage();

            // We must have at least 1 path to process files, else Usage and exit
            if (options.PathList.Count < 1) ProgramOptions.Usage("You must specify at least one TVShowFolder.");

            List<String> validPaths = new List<string>();

            foreach (string p in options.PathList)
            {
                Console.WriteLine("Processing: {0}", p);

                TVShowFolder f = new TVShowFolder(p);

                if (f.IsValid)
                {
                    validPaths.Add(p);
                }
                else
                {
                    Console.WriteLine("INGNORED! NOT A VALID PATH: {0}", p);
                }
            }

            // Read program options from the App.Config
            AppConfigOptions AppConfig = new AppConfigOptions();


            // Setup new search object
            TVSearcher tvSearcher = new TVSearcher(AppConfig.ApiKey);

            // Search each path
            foreach (string p in validPaths)
            {

                TVShowFolder myShow = new TVShowFolder(p);

                Console.WriteLine("Looking for show: {0}", myShow.ShowName);

                string showID;

                if (myShow.HasAssignedID)
                {
                    showID = myShow.AssignedID;
                    Console.WriteLine("Has Assigned ID: {0}", showID);
                }
                else
                {
                    TVSeries tvSearch = tvSearcher.GetSeries(myShow.ShowName);

                    if (tvSearch.IsSearchResult())
                    {
                        foreach (DataSeries s in tvSearch.Shows)
                        {
                            Console.WriteLine("Located: {0} {1} {2}", s.id, s.FirstAired, s.SeriesName);
                        }
                    }

                    if (tvSearch.Shows.Count > 1)
                    {
                        Console.WriteLine("Ambigious search for: {0}", myShow.ShowName);
                        Console.WriteLine("Create a .thetvdb.id file with the show ID as the 1st line.");
                        continue;
                    }

                    if (tvSearch.Shows.Count == 0)
                    {
                        Console.WriteLine("Unable to locate: {0}", myShow.ShowName);
                        continue;
                    }

                    showID = tvSearch.Series.id;
                }

                TVSeries tvShow = tvSearcher.GetShow(showID);

                if (!tvShow.HasEpisodes())
                {
                    Console.WriteLine("Unable to locate any episode data!");
                    continue;
                }


                if (tvShow.Series.Status == "Ended")
                {
                    Console.WriteLine("No more episodes :-( The show has ended!");
                    continue;
                }

                // Load up a list of the existing files in this folder
                TVFiles myTVFiles = new TVFiles(p);

                bool foundNewEpisode = false;

                foreach (DataEpisode de in tvShow.Episodes)
                {
                    DateTime date;

                    try
                    {
                        date = DateTime.ParseExact(de.FirstAired, "yyyy-MM-dd",
                                                   System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        // Probably a blank date so it is in the future.
                        continue;
                    }

                    if (date >= DateTime.Now)
                    {
                        foundNewEpisode = true;
                        Console.WriteLine("NEXT AIR: {0}\t{1}\t{2}\t{3}x{4}\t{5}", de.FirstAired, tvShow.Series.id, tvShow.Series.SeriesName, de.SeasonNumber, de.EpisodeNumber, de.EpisodeName);
                        break;
                    }

                    if (!myTVFiles.Exist(Convert.ToInt32(de.SeasonNumber), Convert.ToInt32(de.EpisodeNumber)))
                    {
                        Console.WriteLine("MISSING!  {0}\t{1}\t{2}\t{3}x{4}\t{5}", de.FirstAired, tvShow.Series.id, tvShow.Series.SeriesName, de.SeasonNumber, de.EpisodeNumber, de.EpisodeName);
                    }

                }

                if (!foundNewEpisode)
                {
                    Console.WriteLine("Unable to locate a new episode.");
                }

            }

            Misc.PauseIfInIDE();
        }

        #region Nested type: AppConfigOptions

        public class AppConfigOptions
        {
            private string _apikey;
            private string _namemask99x99;
            private string _namemasks99e99;

            public string ApiKey
            {
                get
                {
                    _apikey = ConfigurationManager.AppSettings["apikey"];

                    if (String.IsNullOrEmpty(_apikey))
                    {
                        Console.WriteLine("ERROR! <ApiKey> is not defined in App.config!");
                        Environment.Exit(1);
                    }

                    return _apikey;
                }
            }

            public string namemasks99e99
            {
                get
                {
                    _namemasks99e99 = ConfigurationManager.AppSettings["namemasks99e99"];

                    if (String.IsNullOrEmpty(_apikey))
                    {
                        Console.WriteLine("ERROR! <namemasks99e00> is not defined in App.config!");
                        Environment.Exit(1);
                    }

                    return _namemasks99e99;
                }
            }

            public string namemask99x99
            {
                get
                {
                    _namemask99x99 = ConfigurationManager.AppSettings["namemask99x99"];

                    if (String.IsNullOrEmpty(_apikey))
                    {
                        Console.WriteLine("ERROR! <namemask99x99> is not defined in App.config!");
                        Environment.Exit(1);
                    }

                    return _namemask99x99;
                }
            }
        }

        #endregion

        #region Nested type: ProgramOptions

        public class ProgramOptions
        {
            private readonly List<string> _PathList = new List<string>();
            private bool _Debug;

            public List<string> PathList
            {
                get { return _PathList; }
            }

            public bool Debug
            {
                get { return _Debug; }
            }

            public void PrintOptions()
            {
                Console.WriteLine("DEBUG            : " + Debug);
            }

            public bool ParseArgs(string[] args)
            {
                GetOpt programArgs = new GetOpt(args);

                try
                {
                    programArgs.SetOpts(new string[] { "d" });
                    programArgs.Parse();
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }

                _Debug = programArgs.IsDefined("d");

                _PathList.Clear();

                foreach (string s in programArgs.Args)
                {
                    _PathList.Add(s);
                }

                return true;
            }

            public static void UsageMessages(string message)
            {
                Console.WriteLine(
                    "Usage: {0} [-d] TVShowFolder [TVShowfolder...]",
                    Path.GetFileName(ProcessInfo.MyName()));
                Console.WriteLine("");
                Console.WriteLine("    TVShowFolder - Folder containing TV Show Folders");
                Console.WriteLine("");
                Console.WriteLine("  The TVShowFolder is used to find the show on TheTVDB.com.");
                Console.WriteLine("  If the search is ambigious, you can put the year of the show in the folder name:");
                Console.WriteLine("  Example:  X-Files (1993)");
                Console.WriteLine("  If it still can't match, create a text file called .thedbdb.id in the TVShowfolder");
                Console.WriteLine("  and add one line that is the shoes ID from TheTVDB.com.");
                Console.WriteLine("  In the case of The X-Files, the show ID is 77398.");
                Console.WriteLine("");

                if (!String.IsNullOrEmpty(message)) Console.WriteLine(message);

                Console.WriteLine("");
                Misc.PauseIfInIDE();
                Environment.Exit(1);
            }

            public static void Usage(string message)
            {
                UsageMessages(message);
            }

            public static void Usage()
            {
                UsageMessages(String.Empty);
            }
        }

        #endregion
    }
}