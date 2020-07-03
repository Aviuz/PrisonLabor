using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.CompatibilityPatches
{
    internal static class Initialization
    {
        internal static void Run()
        {
            WorkTab.Init();
        }
    }
}
