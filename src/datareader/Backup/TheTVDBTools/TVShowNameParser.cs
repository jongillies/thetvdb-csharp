using System;
using System.Text.RegularExpressions;

namespace TheTVDBTools
{
    /// <summary>
    /// Given a file name that contains some epsiode and season numbering scheme, this class will parse thoes numbers
    /// and return values for thoes numbers.
    /// 
    /// </summary>
    public class TVShowNameParser
    {
        private readonly Regex SEregex = new Regex("[sS][0-9]+[eE][0-9]+"); // Parse S99E99 naming (Season, Episode)
        private readonly Regex SMregex = new Regex("[sS][0-9]+[mM][0-9]+"); // Parse S99E99 naming (Season, Minisode)
        private readonly Regex Xregex = new Regex("[0-9]+[xX][0-9]+");      // Parse 99x99 naming (Season, Episode)

        // This format is not "favored" and will be treated as a S99E99
        private readonly Regex SeasonEpisoderegex = new Regex("Season.[0-9]+ Episode.[0-9]+");      // Parse Season 99 Episode 99 naming

        private int  _Season;            // Season Number (0-99)
        private int  _Episode;           // Episode Number (0-99)
        private bool _AmbigiousNaming;   // True if naming was ambiguous (Might contain "show 01x01 - 01x02.avi")
        private bool _wasSENaming;       // Was original name S99E99 naming?
        private bool _wasSMNaming;       // Was original name S99M99 naming?
        private bool _wasXNaming;        // Was original name 99X99 naming?

        /// <summary>
        /// Set Initialization options.  This should be called by all constructors.
        /// </summary>
        private void SetInitOptions ()
        {
            _Season = -1;
            _Episode = -1;
            _AmbigiousNaming = false;
            _wasSENaming = false;
            _wasSMNaming = false;
            _wasXNaming = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">File name of the file.  Should NOT include a path.</param>
        public TVShowNameParser(string fileName)
        {
            SetInitOptions();

            Parse(fileName);
        }

        /// <summary>
        /// Was the show S99E99 named?
        /// </summary>
        public bool wasSENaming
        {
            get { return _wasSENaming; }
        }

        /// <summary>
        /// Was the show S99M99 named?
        /// </summary>
        public bool wasSMNaming
        {
            get { return _wasSMNaming; }
        }

        /// <summary>
        /// Was the show 99X99 named?
        /// </summary>
        public bool wasXNaming
        {
            get { return _wasXNaming; }
        }

        /// <summary>
        /// Return the season number
        /// </summary>
        public int Season
        {
            get { return _Season; }
        }

        /// <summary>
        /// Return the episode number
        /// </summary>
        public int Episode
        {
            get { return _Episode; }
        }

        /// <summary>
        /// Was the named ambiguously? (Contaied x01 and 1x02 in name?)
        /// </summary>
        public bool AmbigiousNaming
        {
            get { return _AmbigiousNaming; }
        }

        /// <summary>
        /// Did we get a valid season and episode?
        /// </summary>
        /// <returns>Returns TRUE if we have a valid season and episode</returns>
        public bool Matched()
        {
            return (_Season != -1 && _Episode != -1);
        }


        /// <summary>
        /// Parse the fileName into it's components.
        /// </summary>
        /// <param name="fileName">File name of the file.  Should NOT include a path.</param>
        private void Parse(string fileName)
        {
            // Try to match "Season 1 Episode 1" type naming
            if (SeasonEpisoderegex.IsMatch(fileName))
            {
                _wasSENaming = true;

                MatchCollection matches = SeasonEpisoderegex.Matches(fileName);

                if (matches.Count == 1)
                {
                    string info = matches[0].Value.ToUpper();

                    info = info.Replace("SEASON ", "");
                    info = info.Replace("EPISODE ", "");

                    string[] blah = info.Split(' ');
                    string season = blah[0];
                    string episode = blah[1];

                    _Season = Convert.ToInt32(season);
                    _Episode = Convert.ToInt32(episode);
                }
                else
                {
                    _AmbigiousNaming = true;
                }

            }

            // Try to match "S01E01" type naming
            if (SEregex.IsMatch(fileName))
            {
                _wasSENaming = true;

                MatchCollection matches = SEregex.Matches(fileName);

                if (matches.Count == 1)
                {
                    string info = matches[0].Value.ToUpper();

                    string[] blah = info.Split('E');
                    string season = blah[0].TrimStart('S');
                    string episode = blah[1];

                    _Season = Convert.ToInt32(season);
                    _Episode = Convert.ToInt32(episode);
                }
                else
                {
                    _AmbigiousNaming = true;
                }

            }

            // Try to match "S99M99" type naming
            if (SMregex.IsMatch(fileName))
            {
                _wasSMNaming = true;

                MatchCollection matches = SMregex.Matches(fileName);

                if (matches.Count == 1)
                {
                    string info = matches[0].Value.ToUpper();

                    string[] blah = info.Split('M');
                    string season = blah[0].TrimStart('S');
                    string episode = blah[1];

                    _Season = Convert.ToInt32(season);
                    _Episode = Convert.ToInt32(episode);
                }
                else
                {
                    _AmbigiousNaming = true;
                }

            }

            // Try to match "99x99" type naming
            if (Xregex.IsMatch(fileName))
            {
                _wasXNaming = true;

                MatchCollection matches = Xregex.Matches(fileName);

                if (matches.Count == 1)
                {
                    string info = matches[0].Value.ToUpper();

                    string[] blah = info.Split('X');
                    string season = blah[0];
                    string episode = blah[1];

                    _Season = Convert.ToInt32(season);
                    _Episode = Convert.ToInt32(episode);
                }
                else
                {
                    _AmbigiousNaming = true;
                }

            }

        }

    }

}