using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.CompatibilityPatches
{
    internal static class Quarry
    {

        private static ModSearcher modSeeker;
        internal static void Init()
        {
            ModSearcher modSeeker = new ModSearcher("Quarry 1.1");
            if (modSeeker.LookForMod())
            {
                Patch();
            }
        }

        private static void Patch()
        {
            try
            {
                MethodBase methodBase = getTargetMethod();
                if (methodBase != null)
                {
                    var harmony = new Harmony("Harmony_PrisonLabor_Quarry");
                    harmony.Patch(methodBase, postfix: new HarmonyMethod(typeof(Quarry).GetMethod("postfix_Job")));
                }
            }
            catch (Exception e)
            {
                Log.Error($"PrisonLaborException: failed to proceed Query mod patches: {e.ToString()}");
            }
        
        }

        public static Job postfix_Job(Job __result, Pawn pawn, Thing t, bool forced)
        {
            WorkTypeDef workDef = DefDatabase<WorkTypeDef>.GetNamed("QuarryMining");

            if (__result != null && !pawn.IsPrisonerOfColony && pawn.Faction != null && pawn.Faction.IsPlayer && PrisonLaborUtility.IsDisabledByLabor(__result.targetA.Cell, pawn, workDef))
            {
                return null;
            }
            return __result;
        }
        private static MethodBase getTargetMethod()
        {    
            WorkGiverDef workDef = DefDatabase<WorkGiverDef>.GetNamed("QRY_MineQuarry");
            return workDef.giverClass.GetMethod("JobOnThing");            
        }
    }
}
