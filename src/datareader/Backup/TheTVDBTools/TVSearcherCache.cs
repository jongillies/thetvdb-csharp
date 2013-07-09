using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TheTVDBTools
{
    /// <summary>
    /// Search a cache file or goto web
    /// </summary>
    public class TVSearcherCache : TVSearcher
    {
        /// <summary>
        /// This is 
        /// </summary>
        /// <param name="apiKey">xxx</param>
        public TVSearcherCache(string apiKey) : base(apiKey)
        {
            
        }

        ///<summary>
        ///</summary>
        ///<param name="theShowName"></param>
        ///<param name="theFolder"></param>
        ///<returns></returns>
        public TVSeries GetSeries(string theShowName, string theFolder)
        {
            TVSeries showResult = new TVSeries();

            string cacheFile = Path.Combine(theFolder, ".show.xml");

            if (File.Exists(cacheFile))
            {
                showResult.LoadFromFile(theFolder);
            }
            else
            {
                showResult = GetSeries(theShowName);

                showResult.WriteCache(cacheFile);

                return (showResult);
            }

            return (showResult);
        }

    }
}
