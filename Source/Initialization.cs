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
        public static int version = 4;
        public static bool oldPlayerNotification = false;

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
                    oldPlayerNotification = true;
                }
            }
            else
            {
                Log.Message("Detected PrisonLabor v" + PrisonLaborPrefs.Version);
            }
            //PrisonLaborPrefs.Version = version;
            PrisonLaborPrefs.Save();
        }
    }
}
