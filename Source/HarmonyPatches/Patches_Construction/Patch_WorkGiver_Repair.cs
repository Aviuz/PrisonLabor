using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.WorkUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch(typeof(WorkGiver_Repair))]
    class Patch_WorkGiver_Repair
    {

        [HarmonyPrefix]
        [HarmonyPatch("HasJobOnThing")]
        [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
        static bool HasJobOnThingPrefix(ref bool __result, Pawn pawn,Thing t, bool forced)
        {
            if (pawn.IsPrisonerOfColony)
            {
                __result = ConstructionUtils.HasJobOnThingFixed(pawn, t, forced);
                return false;
            }
            return true;

        }
    }
}
