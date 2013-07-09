using System;
using System.IO;
using System.Text.RegularExpressions;

namespace TheTVDBTools
{
    /// <summary>
    /// Given a folder path that contains TV show videos, this class will parse that name and return usefull information.
    /// 
    /// If the folder contains "c:\myshows\My Show (1993)", then object.Year will contain the year and
    /// object.Name will contain "My Show" and object.Location will contain "c:\myshows\My Show (1993)".
    /// 
    /// The folder input to the constructor is always dereferenced to the absolute path.
    /// 
    /// The code will also search for a ".thetvdb.xml" file which would contain the show id from TheTVDB.com.
    /// This is used in case the folder name is not unique along with the year to force a match.
    /// </summary>
    public class TVShowFolder
    {
        private const string ID_FILE = ".thetvdb.id";

        private readonly Regex YearRegex = new Regex(@"\([0-9][0-9][0-9][0-9]\)"); // Parse Year in Folder name (9999)

        private string _ShowName;       // Show name as parsed from the input
        private string _ErrorMessage;   // Eror message return value
        private bool   _IsValid;        // Is the folder input valid?
        private string _Location;       // The parent folder of the show
        private int    _Year;           // Show year if (yyyy) is presnet in the input
        private string _AssignedID;     // Does this show have an assigned ID (i.e. folder contains a .thetvdb.xml

        /// <summary>
        /// Return the show year if specified in the folder name
        /// </summary>
        public int Year
        {
            get { return _Year; }
        }

        /// <summary>
        /// Return the assigned id located inside the .thetvdb.xml file
        /// </summary>
        public string AssignedID
        {
            get { return _AssignedID; }
        }

        /// <summary>
        /// Does the show have and assinged id?  (e.g. does a .thetvdb.xml exist and is it valid)
        /// </summary>
        public bool HasAssignedID
        {
            get { return !String.IsNullOrEmpty(_AssignedID); }
        }

        /// <summary>
        /// Return the parent folder location of the show
        /// </summary>
        public string Location
        {
            get { return _Location; }
        }

        /// <summary>
        /// Is this a valid show folder?
        /// </summary>
        public bool IsValid
        {
            get { return _IsValid; }
        }

        /// <summary>
        /// Return true if a processing error has occured.
        /// </summary>
        /// <returns>True if an error occurred</returns>
        public bool HasError ()
        {
            return !String.IsNullOrEmpty(_ErrorMessage);
        }

        /// <summary>
        /// Return an error message if one occurs.
        /// It might not be a valid path or a file or exist.
        /// </summary>
        public string ErrorMessage
        {
            get { return _ErrorMessage; }
        }

        /// <summary>
        /// Return the show name
        /// </summary>
        public string ShowName
        {
            get { return _ShowName; }
        }

        /// <summary>
        /// Constructor: Initalize values and parse the path
        /// </summary>
        /// <param name="thePath">Folder path</param>
        public TVShowFolder(string thePath)
        {
            _IsValid = false;
            _ShowName = string.Empty;
            _ErrorMessage = string.Empty;
            _AssignedID = string.Empty;
            _Year = -1;

            Parse(thePath);
        }

        /// <summary>
        /// Parse the folder path.
        /// The path is alwasy de-referenced to an absolute path. 
        /// </summary>
        /// <param name="thePath">TV Show Folder Path</param>
        public void Parse(string thePath)
        {
            // Calculate the Absolute Path to the folder
            string absPath = Path.GetFullPath(thePath);

            // Remove any trailing directory separator
            if (absPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                absPath = absPath.TrimEnd(Path.DirectorySeparatorChar);
            }

            // Verify the folder is a directory and not a file
            if (!Directory.Exists(absPath))
            {
                if (File.Exists(absPath))
                {
                    _ErrorMessage = String.Format("You specified a file, not a folder: {0}", absPath);
                    _IsValid = false;
                    return;
                }

                _ErrorMessage = String.Format("Folder {0} does not exist!", absPath);
                _IsValid = false;
                return;
            }

            _ShowName = Path.GetFileName(absPath);
            _IsValid = true;
            _Location = absPath;

            // Try to locate a .thetvdb.id file
            string assignedIDPath = Path.Combine(absPath, ID_FILE);

            if (File.Exists(assignedIDPath))
            {
                TextReader tr = new StreamReader(assignedIDPath);
                _AssignedID = tr.ReadLine();
                tr.Close();
            }

            // Try to match a year in the show name
            MatchCollection matches = YearRegex.Matches(_ShowName);

            if (matches.Count == 1)
            {
                string info = matches[0].Value;

                // Remove beginning ( and ending )
                info = info.Replace("(", "");
                info = info.Replace(")", "");

                _Year = Convert.ToInt32(info);
            }

        }

    }

}