using System;
using System.Collections.Generic;
using System.Text;

namespace TheTVDBTools
{
    /// <summary>
    /// Given a folder, the objective is for TVTool to return the ID of the show.
    /// This may mean gathering the .thedvd.id file or doing a search on the site.
    /// </summary>
    public class TVTool
    {

        private bool _HasError;
        private string _Path;
        private string _ID;
        private string _APIKey;
        private string _ErrorMessage;


        ///<summary>
        ///</summary>
        public bool HasError
        {
            get { return _HasError; }
        }

        ///<summary>
        ///</summary>
        public bool HasID
        {
            get { return !_HasError; }
        }

        ///<summary>
        ///</summary>
        public string ID
        {
            get { return _ID; }
        }

        ///<summary>
        ///</summary>
        ///<param name="APIKey"></param>
        ///<param name="path"></param>
        public TVTool(string APIKey, string path)
        {
            _HasError = true;
            _Path = path;
            _APIKey = APIKey;
            _ErrorMessage = String.Empty;

            TVShowFolder myShow = new TVShowFolder(_Path);

            if (myShow.HasError())
            {
                _HasError = true;
                _ErrorMessage = myShow.ErrorMessage;
                return;
            }


            if (myShow.HasAssignedID)
            {
                _ID = myShow.AssignedID;

                _HasError = false;

                return;
            }

            // If we don't have an assigned ID, we have to search for the show

            TVSeries tvSearch = new TVSeries();

            // Setup new search object
            TVSearcher tvSearcher = new TVSearcher(_APIKey);

            tvSearch = tvSearcher.GetSeries(myShow.ShowName);

            if (tvSearch.Shows.Count > 1)
            {
                _ErrorMessage = String.Format("Ambigious search for: {0}", myShow.ShowName);
                _HasError = true;

                return;
            }

            if (tvSearch.Shows.Count == 0)
            {
                _ErrorMessage = String.Format("Unable to locate: {0}", myShow.ShowName);
                _HasError = true;

                return;
            }

            // Set the ID and the error condition to false;
            _ID = tvSearch.Series.id;
            _HasError = false;

            return;
        }

        ///<summary>
        ///</summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        ///<summary>
        ///</summary>
        ///<param name="showID"></param>
        ///<returns></returns>
        public TVSeries GetShowData(int showID)
        {

            TVSeries show = new TVSeries();

            return (show);
        }
    }
}
