namespace Supercoder.Tools
{
    public class ProcessInfo
    {
        public static string MyName()
        {
            return(System.IO.Path.GetFileName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName)); 
        }
    }
}
