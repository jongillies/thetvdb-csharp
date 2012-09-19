using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;
using TheTVDBTools;

namespace datareader
{
    
    class Program
    {
        static void Main(string[] args)
        {

            //if (false)
            //{
            //    const string theXML = "C:\\repos\\svn\\src\\TheTVDB\\xml-samples\\77345.xml";

            //    TVSeries tvFile = new TVSeries();

            //    tvFile.LoadFromFile(theXML);

            //    if (tvFile.HasEpisodes())
            //    {
            //        foreach (DataEpisode Episode in tvFile.Episodes)
            //        {
            //            Console.WriteLine("{0} {1}x{2} {3}", tvFile.Series.SeriesName, Episode.SeasonNumber,
            //                              Episode.EpisodeNumber, Episode.EpisodeName);

            //        }
            //    }

            //    TVSeries tvSearch = new TVSeries();

            //    //            tvSearch.LoadFromURL("http://thetvdb.com/api/GetSeries.php?seriesname=american"); // Many results
            //    tvSearch.LoadFromURL("http://thetvdb.com/api/GetSeries.php?seriesname=american%20dad"); // One result

            //    if (tvSearch.IsSearchResult())
            //    {
            //        foreach (DataSeries s in tvSearch.Shows)
            //        {
            //            Console.WriteLine("{0} First Aired:{1}", s.SeriesName, s.FirstAired);

            //        }
            //    }
            //}

//            string showFolder = "\\\\JBOD\\video\\TV\\..\\TV\\Classic\\Gilligans Island\\Gilligans_Island_-_s01e00_Original_Pilot.avi";
//            string showFolder = @"\\JBOD\video\TV\..\TV\Classic\Gilligans Island\";

            const string showFolder = @"\\JBOD\video\TV\Classic\Tru Calling";


            TVShowFolder myShow = new TVShowFolder(showFolder);

            if (myShow.IsValid)
            {
                Console.WriteLine("Show name is {0}",myShow.ShowName);
            }
            else
            {
                Console.WriteLine("Error! {0}",myShow.ErrorMessage);

                Console.WriteLine("Press <ENTER> to continue...");
                Console.ReadLine();
                Environment.Exit(1);
            }


            TVSeries tvSearch = new TVSeries();

            string show = HttpUtility.UrlEncode(myShow.ShowName);

            string url = "http://thetvdb.com/api/GetSeries.php?seriesname=" + show;

            tvSearch.LoadFromURL(url);

            if (tvSearch.IsSearchResult())
            {
                foreach (DataSeries s in tvSearch.Shows)
                {
                    Console.WriteLine("{0} First Aired:{1}", s.SeriesName, s.FirstAired);

                }
            }

            if (tvSearch.Shows.Count != 1)
            {
                Console.WriteLine("Ambigious search for {0}", myShow.ShowName);
                Console.WriteLine("Press <ENTER> to continue...");
                Console.ReadLine();
                Environment.Exit(1);
            }

            string showID = tvSearch.Series.id;

            TVSeries tvShow = new TVSeries();

            //showID = "77345";

            string foo = "http://www.thetvdb.com/api/713541F4CE28A6D8/series/" + showID + "/all/en.xml";

            tvShow.LoadFromURL(foo);

            if (tvShow.HasEpisodes())
            {
                Console.WriteLine("We have episodes!");
            }

            Console.WriteLine("This show has {0} seasons.", tvShow.Seasons);

            
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(myShow.Location);
            foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
            {
                TVShowNameParser myNameParser = new TVShowNameParser(f.Name);

                string fileExtension = Path.GetExtension(f.Name);

                if (myNameParser.Matched())
                {
                    Console.WriteLine("{0} Season {1}, Episode{2}", f.Name, myNameParser.Season, myNameParser.Episode);

                    DataEpisode mine = tvShow.GetEpisode(myNameParser.Season, myNameParser.Episode);

                    if (mine != null)
                    {
                        string season = String.Format("{0:00}", myNameParser.Season);
                        string episode = String.Format("{0:00}", myNameParser.Episode);

                        Console.WriteLine("Located Show: {0}", mine.EpisodeName);

                        string newName = String.Format("{0} - {1}x{2} - {3}{4}",tvShow.Series.SeriesName, season, episode, mine.EpisodeName, Path.GetExtension(f.Name));

                        if (newName != f.Name)
                        {
                            Console.WriteLine("Rename me!");
                        }
                    }

                }
                else
                {
                    if (myNameParser.AmbigiousNaming)
                    {
                        Console.WriteLine("AMBIGIOUS NAMING: {0}", f.Name);
                    }
                    else
                    {
                        Console.WriteLine("CAN'T MATCH: {0}", f.Name);
                    }
                }


            }


            Console.WriteLine("Press <ENTER> to continue...");
            Console.ReadLine();
        }

    }
}
