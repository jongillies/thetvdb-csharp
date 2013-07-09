using System;
using System.Text.RegularExpressions;
using System.Web;

namespace TheTVDBTools
{
    /// <summary>
    /// Search TheTVDB.Com for TV Show information
    /// </summary>
    public class TVSearcher 
    {
        // Year (9999) Regex
        private readonly Regex YearRegex = new Regex(@"\([0-9][0-9][0-9][0-9]\)"); 

        // API Key is required
        private readonly string _apiKey;

        // Control Internet access
        private bool _InternetAccess;

        /// <summary>
        /// Turn off and on internet access for testing cache files
        /// </summary>
        public bool InternetAccess
        {
            get { return _InternetAccess; }
            set { _InternetAccess = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="apiKey">Specify and API Key</param>
        public TVSearcher (string apiKey)
        {
            _InternetAccess = true;
            _apiKey = apiKey;
        }

        /// <summary>
        /// Get the list of mirrors for data and choose one.
        /// Currently here is only 1 mirror.
        /// </summary>
        /// <returns>Returns the mirror URL</returns>
        private static string GetMirrorURL()
        {
            return "http://thetvdb.com";
        }

        /// <summary>
        /// Given a show ID, get the show information
        /// </summary>
        /// <param name="id">The series ID</param>
        /// <returns>TVSerise</returns>
        public TVSeries GetSeriesByID (string id)
        {
            string url = GetMirrorURL() + "/api/" + _apiKey +"/series/" + id + "/en.xml";

            TVSeries ts = new TVSeries();
                
            ts.LoadFromURL(url);

            return (ts);
        }

        /// <summary>
        /// Try to match a series by showName
        /// If you get more than one match by show name, try to match by the year in the show name, if present
        /// </summary>
        /// <param name="showName">The show name to search</param>
        /// <returns></returns>
        public TVSeries GetSeries (string showName)
        {
            int year = -1;

            // Remove any thing that looks like a (year) in the name, we will use the year later
            MatchCollection matches = YearRegex.Matches(showName);

            foreach (Match y in matches)
            {
                showName = showName.Replace(y.Value, "");

                string yy = y.Value;
                // Remove beginning ( and ending )
                yy = yy.Replace("(", "");
                yy = yy.Replace(")", "");

                year = Convert.ToInt32(yy);
            }
        
            string show = HttpUtility.UrlEncode(showName);

            string url = GetMirrorURL() + "/api/GetSeries.php?seriesname=" + show;

            TVSeries ts = new TVSeries();
                
            ts.LoadFromURL(url);

            // If we have more that 1 show returned
            if (ts.Shows.Count > 1)
            {
                // Try and match by the year
                foreach (DataSeries s in ts.Shows)
                {
                    // Some shows don't have a FirstAired field!
                    if (!String.IsNullOrEmpty(s.FirstAired))
                    {
                        string x = s.FirstAired;
                        x = x.Substring(0, 4);

                        if (Convert.ToInt32(x) == year)
                        {
                            return (GetSeriesByID(s.id));
                        }
                    }
                }
            }

            return (ts);
        }

        /// <summary>
        /// Get a specific how given a show ID (i.e. Episode)
        /// </summary>
        /// <param name="showID">The Episode Number</param>
        /// <returns>TVSeries</returns>
        public TVSeries GetShow (string showID)
        {
            if (!_InternetAccess) throw new System.Net.WebException("Internet set to down");

            string url = GetMirrorURL() + "/api/" + _apiKey +"/series/" + showID + "/all/en.xml";

            TVSeries ts = new TVSeries();

            ts.LoadFromURL(url);
            
            return (ts);
        }

    }
}
