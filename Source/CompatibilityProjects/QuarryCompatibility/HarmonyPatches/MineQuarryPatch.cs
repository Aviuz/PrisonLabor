using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;

namespace QuarryCompatibility.HarmonyPatches
{
    [HarmonyPatch]
    class MineQuarryPatch
    {
        static MethodBase TargetMethod()
        {
            WorkGiverDef workDef = DefDatabase<WorkGiverDef>.GetNamed("QRY_MineQuarry");
            return workDef.giverClass.GetMethod("JobOnThing");
        }
        static Job Postfix(Job __result, Pawn pawn, Thing t, bool forced)
        {
            WorkTypeDef workDef = DefDatabase<WorkTypeDef>.GetNamed("QuarryMining");

            if (__result != null && !pawn.IsPrisonerOfColony && pawn.Faction != null && pawn.Faction.IsPlayer && PrisonLaborUtility.IsDisabledByLabor(__result.targetA.Cell, pawn, workDef))
            {
                return null;
            }
            return __result;
        }
    }
}
