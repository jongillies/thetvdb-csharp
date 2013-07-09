using System;
using System.Collections.Generic;
using System.IO;

namespace TheTVDBTools
{
    /// <summary>
    /// Given a folder, gather a list of files in that folder.  These are TV Show files.
    /// Store this in a dictionary.  This allows you to check if a show is in the folder via the dictionary.
    /// </summary>
    public class TVFiles
    {
        readonly IDictionary<string, string> _ShowList = new Dictionary<string, string>();

        /// <summary>
        /// Return a string based on the input parameters, this is used as the key to the dictionary
        /// </summary>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <returns></returns>
        private static string Name(int theSeason, int theEpisode)
        {
            string season = String.Format("{0:00}", theSeason);
            string episode = String.Format("{0:00}", theEpisode);

            return(season + episode);
        }

        /// <summary>
        /// Does this show exist in the list?
        /// </summary>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <returns></returns>
        public bool Exist(int theSeason, int theEpisode)
        {
            string name = Name(theSeason, theEpisode);

            try
            {
                return _ShowList[name] == name;
                    
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// TVFiles constructor.  Given the show folder, load the shows into the dictionary
        /// </summary>
        /// <param name="path">The TV Show folder</param>
        public TVFiles(string path)
        {

            TVShowFolder f = new TVShowFolder(path);

            if (!f.IsValid)
            {
                return;
            }

            DirectoryInfo dir = new DirectoryInfo(f.Location);
            foreach (FileInfo fi in dir.GetFiles("*.*"))
            {
                // Ignore any . files
                if (fi.Name.StartsWith(".")) continue;

                TVShowNameParser myNameParser = new TVShowNameParser(fi.Name);

                if (myNameParser.Matched())
                {
                    try
                    {
                        _ShowList.Add(Name(myNameParser.Season, myNameParser.Episode),
                                      Name(myNameParser.Season, myNameParser.Episode));
                    }
                    catch (ArgumentException)
                    {
                        // TODO: This should perhaps write to a delgate and not to the console
                        Console.WriteLine("ERROR! Already exists!  You have a duplicate! {0}", Name(myNameParser.Season, myNameParser.Episode));
                    }

                }
            }
        }
    }
}
