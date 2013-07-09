using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace TheTVDBTools
{
    /// <summary>
    /// This class will load a "TheTVDB" based XLM information.
    ///
    /// Normally this information will be located in TheTVDB here (given the series ID):
    ///
    ///     http://www.thetvdb.com/api/apikey/series/73141/all/en.xml
    ///
    /// or for a search:
    ///
    ///    "http://thetvdb.com/api/GetSeries.php?seriesname=american"
    ///
    /// It can also parse results from an XML file.
    ///
    /// </summary>
    public class TVSeries
    {
        // These are the default naming masks if not are provided to the class.
        private const string DEFAULT_NAMEMASKS99E99 = "%SHOW_NAME% S%SEASON%E%EPISODE% - %NAME%";
        private const string DEFAULT_NAMEMASK99X99 = "%SHOW_NAME% %SEASON%x%EPISODE% - %NAME%";

        // Contains the series information
        private DataSeries _Series = new DataSeries();

        // Contains the series information
        private readonly List<DataSeries> _Shows = new List<DataSeries>();

        // Contains the list of episodes
        private readonly List<DataEpisode> _Episodes = new List<DataEpisode>();

        // Calculated number of Seasons
        private int _Seasons;

        // XML Parsing Variables
        Data _xmlData;

        /// <summary>
        /// Return the raw XML Data
        /// </summary>
        public Data XMLData
        {
            get { return _xmlData; }
        }

        readonly XmlSerializer s = new XmlSerializer(typeof(Data));

        // Naming Masks for renaming shows
        private string _nameMaskS99E99;
        private string _nameMask99x99;

        /// <summary>
        /// get/set the S99E99 naming mask
        /// </summary>
        public string nameMaskS99E99
        {
            get { return _nameMaskS99E99; }
            set { _nameMaskS99E99 = value; }
        }

        /// <summary>
        /// get/set the 99x99 naming mask
        /// </summary>
        public string nameMask99x99
        {
            get { return _nameMask99x99; }
            set { _nameMask99x99 = value; }
        }


        /// <summary>
        /// Return TRUE if this is a search result list
        /// </summary>
        /// <returns>True if the Show Count in the XML is > 1 or the Show count == 1 and the Episode count = 0</returns>
        public bool IsSearchResult()
        {
            if (_Shows.Count > 1)
            {
                return true;
            }

            if (_Shows.Count == 1 && _Episodes.Count == 0)
            {
                return true;
            }

            return (false);
        }

        /// <summary>
        /// Return TRUE if this is a Show result (contains 1 show and 0 or more episodes)
        /// </summary>
        /// <returns>True if there is more than 0 epsiodes</returns>
        public bool HasEpisodes()
        {
            return (_Episodes.Count >= 1);
        }

        /// <summary>
        /// Return the Series Information for this show
        /// </summary>
        public DataSeries Series
        {
            get { return _Series; }
        }

        /// <summary>
        /// Return the list of Episodes
        /// </summary>
        public List<DataEpisode> Episodes
        {
            get { return _Episodes; }
        }

        /// <summary>
        /// Return the number of seasons
        /// </summary>
        public int Seasons
        {
            get { return _Seasons; }
        }

        /// <summary>
        /// Return the list of Shows (Must be a search result)
        /// </summary>
        public List<DataSeries> Shows
        {
            get { return _Shows; }
        }

        /// <summary>
        /// Load from an XML file
        /// </summary>
        /// <param name="fileName">path to the file</param>
        public void LoadFromFile(string fileName)
        {
            TextReader r = new StreamReader(fileName);
            _xmlData = (Data)s.Deserialize(r);

            Parse();

            r.Close();
        }

        /// <summary>
        /// Load from a URL string
        /// </summary>
        /// <param name="URLString">URL Path</param>
        public void LoadFromURL(string URLString)
        {
            XmlTextReader r = new XmlTextReader(URLString);

            _xmlData = (Data)s.Deserialize(r);

            Parse();

            r.Close();
        }

        /// <summary>
        /// Count the number of seaons in the episode list
        /// </summary>
        private void CountSeasons()
        {
            _Seasons = 0;

            foreach (DataEpisode de in _Episodes)
            {
                int seasonNumber = Convert.ToInt32(de.SeasonNumber);

                if (seasonNumber > _Seasons)
                {
                    _Seasons = seasonNumber;
                }
            }
        }

        /// <summary>
        /// Naming Macro Replacment
        /// </summary>
        /// <param name="theNameMask">Pass in a naming file mask</param>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <returns></returns>
        private string ReplaceMacros(string theNameMask, int theSeason, int theEpisode)
        {
            // Format theSeason and theEpisode into 0 padded left strings
            string season = String.Format("{0:00}", theSeason);
            string episode = String.Format("{0:00}", theEpisode);

            // Return the eipsode information from the list
            DataEpisode de = GetEpisode(theSeason, theEpisode);

            if (de == null)
            {
                return String.Empty;
            }

            // Standard naming replacments for macros
            theNameMask = theNameMask.Replace("%SHOW_NAME%", Series.SeriesName);
            theNameMask = theNameMask.Replace("%SEASON%", season);
            theNameMask = theNameMask.Replace("%EPISODE%", episode);
            theNameMask = theNameMask.Replace("%NAME%", de.EpisodeName);

            // Extended naming replacments
            theNameMask = theNameMask.Replace("%FIRSTAIRED%", de.FirstAired);
            theNameMask = theNameMask.Replace("%RATING%", de.Rating);

            // NOTE: Would be nice to reflect the assemblie and provide thoes macro replacmens from the XML

            return (theNameMask);
        }

        /// <summary>
        /// Return a SxxExx filename
        /// </summary>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <param name="extension">The file extension</param>
        /// <returns></returns>
        public string SEFileName(int theSeason, int theEpisode, string extension)
        {
            string newName = String.IsNullOrEmpty(_nameMaskS99E99) ? DEFAULT_NAMEMASKS99E99 : _nameMaskS99E99;

            newName = ReplaceMacros(newName, theSeason, theEpisode);

            newName += extension;

            return (CleanName(newName));
        }

        /// <summary>
        /// Return a SxxMxx filename
        /// </summary>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <param name="extension">The file extension</param>
        /// <returns></returns>
        public string SMFileName(int theSeason, int theEpisode, string extension)
        {
            string newName = String.IsNullOrEmpty(_nameMask99x99) ? DEFAULT_NAMEMASK99X99 : _nameMask99x99;

            newName = ReplaceMacros(newName, theSeason, theEpisode);

            newName += extension;

            return (CleanName(newName));
        }

        /// <summary>
        /// Return a 99x99 filename
        /// </summary>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <param name="extension">The file extension</param>
        /// <returns></returns>
        public string XFileName(int theSeason, int theEpisode, string extension)
        {
            string newName = String.IsNullOrEmpty(_nameMask99x99) ? DEFAULT_NAMEMASK99X99 : _nameMask99x99;

            newName = ReplaceMacros(newName, theSeason, theEpisode);

            newName += extension;

            return (CleanName(newName));
        }


        /// <summary>
        /// Return the DataEpisode for the given season nad episode number
        /// </summary>
        /// <param name="theSeason">The Season Number</param>
        /// <param name="theEpisode">The Episode Number</param>
        /// <returns></returns>
        public DataEpisode GetEpisode(int theSeason, int theEpisode)
        {
            DataEpisode x = null;

            foreach (DataEpisode de in _Episodes)
            {
                int seasonNumber = Convert.ToInt32(de.SeasonNumber);
                int episodeNumber = Convert.ToInt32(de.EpisodeNumber);

                if (theSeason == seasonNumber && theEpisode == episodeNumber)
                {
                    x = de;
                }
            }

            return x;
        }

        /// <summary>
        /// Parse the XML and load into the class variables
        /// </summary>
        private void Parse()
        {
            // If you are reading a search result, there will be muliple _Shows objects and no _Expisodes.
            // If you are reading an "all" document for a series, there will be 1 _Serices object and 0 or more _Episodes.
            try
            {
                foreach (Object o in _xmlData.Items)
                {
                    if (o.GetType() == typeof(DataSeries))
                    {
                        _Shows.Add((DataSeries)o);
                    }
                    else if (o.GetType() == typeof(DataEpisode))
                    {
                        _Episodes.Add((DataEpisode)o);
                    }
                }

                if (_Shows.Count == 1)
                {
                    _Series = _Shows[0];

                    CountSeasons();
                }


            }
            catch (Exception)
            {
                // Handle the error?
                return;
            }
        }

        /// <summary>
        /// Clean a name string for use as a file name
        /// </summary>
        /// <param name="theName">The Input Name</param>
        /// <returns>Returns a clean name for the file system</returns>
        public static string CleanName(string theName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            char[] invalidPathChars = Path.GetInvalidPathChars();

            // "!" just messes with my head in a file name, so nuke it!
            // ":" is valid on UNIX, but screws up a Samba share file name, so nuke it!
            // "\" is valid on UNIX, but screws up a Samba share file name, so nuke it!
            char[] customInvalidFileChars = { '!', ':', '\\', '/', '?' };

            char[] myInvalidFileChars = new char[invalidFileChars.Length + invalidPathChars.Length + customInvalidFileChars.Length];
            invalidFileChars.CopyTo(myInvalidFileChars, 0);
            invalidPathChars.CopyTo(myInvalidFileChars, invalidFileChars.Length);
            customInvalidFileChars.CopyTo(myInvalidFileChars, invalidFileChars.Length + invalidPathChars.Length);

            foreach (char invalidFChar in myInvalidFileChars)
            {
                theName = theName.Replace(invalidFChar.ToString(), "_");
            }

            return (theName);
        }

        /// <summary>
        ///  Write the raw _xmlData to the specified file
        /// </summary>
        /// <param name="theFile">File path to write data</param>
        public void WriteCache(string theFile)
        {
            try
            {
                // create a writer and open the file
                TextWriter tw = new StreamWriter(theFile);

                s.Serialize(tw,_xmlData);

                // close the stream
                tw.Close();
            }
            catch (Exception)
            {
              
            }

        }
    }
}