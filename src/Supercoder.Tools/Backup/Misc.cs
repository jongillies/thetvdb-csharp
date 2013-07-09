using System;
using System.Collections.Generic;
using System.Text;

namespace Supercoder.Tools
{
    public class Misc
    {
        public static void PauseIfInIDE()
        {
            if (System.Diagnostics.Debugger.IsAttached )
            {
                Console.WriteLine("Press <ENTER> to continue.");
                Console.Read();
            }
        }
    }
}
