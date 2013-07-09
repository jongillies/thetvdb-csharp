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

namespace RenameShows
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

            foreach (string p in validPaths)
            {
                int TotalEpisodes = 0;
                int RenamedEpisodes = 0;
                int ErrorsEpisodes = 0;


                TVShowFolder myShow = new TVShowFolder(p);

                if (myShow.HasError())
                {
                    Console.WriteLine("Error parsing show name: {0}", myShow.ErrorMessage);
                    continue;
                }

                Console.WriteLine("Looking for show: {0}", myShow.ShowName);

                string outputFile = String.Empty;


                if (String.IsNullOrEmpty(options.OutputPath))
                {
                    outputFile = Platform.IsWindows()
                                     ? Path.Combine(myShow.Location, ".rename.bat")
                                     : Path.Combine(myShow.Location, ".rename.sh");
                }
                else
                {
                    outputFile = Path.Combine(options.OutputPath,
                                              Path.ChangeExtension(myShow.ShowName, Platform.IsWindows() ? "bat" : "sh"));
                }

                TextWriter tw = new StreamWriter(outputFile);

                string showID;

                if (myShow.HasAssignedID)
                {
                    showID = myShow.AssignedID;
                    Console.WriteLine("Has Assigned ID: {0}", showID);
                }
                else
                {
                    TVSeries tvSearch = new TVSeries();

                    tvSearch = tvSearcher.GetSeries(myShow.ShowName);

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
                        Console.WriteLine("Create a .thetvdb.id file wiht the show ID as the 1st line.");
                        continue;
                    }

                    if (tvSearch.Shows.Count == 0)
                    {
                        Console.WriteLine("Unable to locate: {0}", myShow.ShowName);
                        continue;
                    }

                    showID = tvSearch.Series.id;
                }

                Console.WriteLine("Located show Number: {0}", showID);


                TVSeries tvShow = new TVSeries();

                tvShow = tvSearcher.GetShow(showID);

                if (!tvShow.HasEpisodes())
                {
                    Console.WriteLine("Unable to locate any episode data!");
                    continue;
                }

                DirectoryInfo dir = new DirectoryInfo(myShow.Location);
                foreach (FileInfo f in dir.GetFiles("*.*"))
                {
                    // Ignore any . files
                    if (f.Name.StartsWith(".")) continue;

                    TotalEpisodes++;

                    TVShowNameParser myNameParser = new TVShowNameParser(f.Name);

                    if (myNameParser.Matched())
                    {
                        DataEpisode thisShow = tvShow.GetEpisode(myNameParser.Season, myNameParser.Episode);

                        tvShow.nameMaskS99E99 = AppConfig.namemasks99e99;
                        tvShow.nameMask99x99 = AppConfig.namemask99x99;

                        if (thisShow != null)
                        {
                            string newName = String.Empty;

                            if (myNameParser.wasSENaming)
                            {
                                newName = tvShow.SEFileName(myNameParser.Season, myNameParser.Episode,
                                                            Path.GetExtension(f.Name));
                            }

                            if (myNameParser.wasXNaming)
                            {
                                newName = tvShow.XFileName(myNameParser.Season, myNameParser.Episode,
                                                           Path.GetExtension(f.Name));
                            }

                            if (myNameParser.wasSMNaming)
                            {
                                newName = tvShow.SMFileName(myNameParser.Season, myNameParser.Episode,
                                                            Path.GetExtension(f.Name));
                            }

                            if (options.ForceXNaming)
                            {
                                newName = tvShow.XFileName(myNameParser.Season, myNameParser.Episode,
                                                           Path.GetExtension(f.Name));
                            }

                            if (options.ForceENaming)
                            {
                                newName = tvShow.SEFileName(myNameParser.Season, myNameParser.Episode,
                                                            Path.GetExtension(f.Name));
                            }


                            if (newName != f.Name)
                            {
                                RenamedEpisodes++;

                                string sourcePath;
                                string destpath;

                                if (options.UseRelativeNaming)
                                {
                                    sourcePath = f.Name;
                                    destpath = newName;
                                }
                                else
                                {
                                    sourcePath = Path.Combine(myShow.Location, f.Name);
                                    destpath = Path.Combine(myShow.Location, newName);
                                }

                                if (File.Exists(destpath))
                                {
                                    Console.WriteLine("WARNING! {0} already exists!");
                                }

                                if (Platform.IsWindows())
                                {
                                    tw.WriteLine(@"ren ""{0}"" ""{1}"" ", sourcePath, destpath);
                                }
                                else
                                {
                                    tw.WriteLine(@"mv ""{0}"" ""{1}"" ", sourcePath, destpath);
                                }

                                Console.WriteLine("RENAME: {0}", newName);
                            }
                            else
                            {
                                Console.WriteLine("GOOD: {0}", f.Name);
                            }
                        }
                        else
                        {
                            ErrorsEpisodes++;
                            Console.WriteLine("ERROR: {0} (Can't Locate)", f.Name);
                        }
                    }
                    else
                    {
                        if (myNameParser.AmbigiousNaming)
                        {
                            ErrorsEpisodes++;
                            Console.WriteLine("ERROR: {0} (AMBIGIOUS NAMING)", f.Name);
                        }
                        else
                        {
                            ErrorsEpisodes++;
                            Console.WriteLine("ERROR: {0} (CAN'T MATCH)", f.Name);
                        }
                    }
                }

                Console.WriteLine("");
                Console.WriteLine("Total Episodes  : {0}", TotalEpisodes);
                Console.WriteLine("Renamed Episodes: {0}", RenamedEpisodes);
                Console.WriteLine("Episode Errors  : {0}", ErrorsEpisodes);

                if (RenamedEpisodes > 0)
                {
                    Console.WriteLine("");
                    Console.WriteLine("Created {0} for renaming.", outputFile);
                    Console.WriteLine("Please inspect and execute CAREFULLY!");
                    Console.WriteLine("");
                }

                tw.Close();

                // If we didn't rename anything, remove the empty outputFile
                if (RenamedEpisodes == 0)
                {
                    File.Delete(outputFile);
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
            private bool _ForceENaming;
            private bool _ForceXNaming;
            private string _OutputPath;
            private bool _UseRelativeNaming;

            public List<string> PathList
            {
                get { return _PathList; }
            }

            public bool UseRelativeNaming
            {
                get { return _UseRelativeNaming; }
            }

            public bool Debug
            {
                get { return _Debug; }
            }

            public string OutputPath
            {
                get { return _OutputPath; }
            }

            public bool ForceXNaming
            {
                get { return _ForceXNaming; }
            }

            public bool ForceENaming
            {
                get { return _ForceENaming; }
            }

            public void PrintOptions()
            {
                Console.WriteLine("DEBUG            : " + Debug);
                Console.WriteLine("FORCEXNAMING     : " + ForceXNaming);
                Console.WriteLine("FORCEENAMING     : " + ForceENaming);
                Console.WriteLine("OUTPUTPATH       : " + OutputPath);
                Console.WriteLine("USERELATIVENAMING: " + UseRelativeNaming);
            }

            public bool ParseArgs(string[] args)
            {
                GetOpt programArgs = new GetOpt(args);

                try
                {
                    programArgs.SetOpts(new string[] {"d", "x", "e", "r", "o="});
                    programArgs.Parse();
                }
                catch (ArgumentException e)
                {
                    Console.WriteLine(e.Message);
                    return false;
                }

                _Debug = programArgs.IsDefined("d");
                _ForceENaming = programArgs.IsDefined("e");
                _ForceXNaming = programArgs.IsDefined("x");
                _UseRelativeNaming = programArgs.IsDefined("r");

                if (programArgs.IsDefined("o"))
                {
                    _OutputPath = programArgs.GetOptionArg("o");
                }

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
                    "Usage: {0} [-d] [-e] [-x] [-o RenameScriptOutputPath] TVShowFolder [TVShowfolder...]",
                    Path.GetFileName(ProcessInfo.MyName()));
                Console.WriteLine("");
                Console.WriteLine("    -e Force SxxExx Naming for shows (S10E01)");
                Console.WriteLine("    -x Force 99x99 Naming for shows (10x01");
                Console.WriteLine("    -d (print debug messages)");
                Console.WriteLine("    -o OutputPath");
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