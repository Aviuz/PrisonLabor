using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrisonLabor.CompatibilityPatches
{
    internal static class Initialization
    {
        internal static void Run()
        {            
            //NoWaterNoLife.Init();
            Quarry.Init();
            Kajin2.Init();
        }
    }
}
