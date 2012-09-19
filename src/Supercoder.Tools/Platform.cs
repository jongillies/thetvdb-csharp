using System;

namespace Supercoder.Tools
{
    public class Platform
    {
        public static string PlatformString()
        {
            const string msg1 = "This is a Windows operating system.";
            const string msg2 = "This is a Unix operating system.";
            const string msg3 = "ERROR: This platform identifier is invalid.";

            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return(msg1);
                case PlatformID.Unix:
                    return(msg2);
                default:
                    return(msg3);
            }
        }

        public static bool IsWindows()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    return (true);
                default:
                    return (false);
            }
        }

        public static bool IsUnix()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            switch (pid)
            {
                case PlatformID.Unix:
                    return (true);
                default:
                    return (false);
            }
        }

    }
}
