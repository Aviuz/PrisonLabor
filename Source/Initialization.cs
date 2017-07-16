using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Reflection.Emit;
using Verse.AI;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    class Initialization
    {
        public static int version = 5;

        static Initialization()
        {
            HarmonyPatches.run();
            PrisonLaborPrefs.Init();
            checkVersion();
        }

        private static void checkVersion()
        {
            if (PrisonLaborPrefs.Version < 3)
            {
                // only way to check if mod was installed before
                if (PlayerKnowledgeDatabase.IsComplete(DefDatabase<ConceptDef>.GetNamed("PrisonLabor")))
                {
                    Log.Message("Detected older version of PrisonLabor");
                    Tutorials.msgShowVersion0_3 = true;
                }
            }
            if(PrisonLaborPrefs.Version < 5)
            {
                Log.Message("Detected older version of PrisonLabor");
                Tutorials.msgShowVersion0_5 = true;
            }

            Log.Message("Loaded PrisonLabor v" + PrisonLaborPrefs.Version);
            PrisonLaborPrefs.Version = version;
            PrisonLaborPrefs.Save();
        }
    }
}
