using Harmony;
using PrisonLabor.Tweaks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches
{
    static class Triggers_UpgradeSave
    {

        [HarmonyPatch(typeof(Pawn_JobTracker))]
        [HarmonyPatch(nameof(Pawn_JobTracker.ExposeData))]
        static class Trigger_Pawn_JobTracker
        {
            static bool Prefix()
            {
                SaveUpgrader.Pawn_JobTracker();
                return true;
            }
        }


        [HarmonyPatch(typeof(Job))]
        [HarmonyPatch(nameof(Job.ExposeData))]
        static class Trigger_Job
        {
            static bool Prefix()
            {
                SaveUpgrader.Job();
                return true;
            }
        }
    }
}
