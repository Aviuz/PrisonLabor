using PrisonLabor.Core.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Other
{
    public static class DebugLogger
    {
        public static void debug(string msg)
        {
            if (PrisonLaborPrefs.DebugLogs)
            {
                Log.Message("[PL] " + msg);
            }
        }

        public static void info(string msg)
        {
            Log.Message("[PL] " + msg);
        }

        internal static void warn(string msg)
        {
            Log.Warning("[PL] " + msg);
        }
    }
}
