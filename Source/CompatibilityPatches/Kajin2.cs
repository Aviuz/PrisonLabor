using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.CompatibilityPatches
{
    static class Kajin2
    {
        private static ModSearcher modSeeker;
        internal static void Init()
        {
            ModSearcher modSeeker = new ModSearcher("Kijin Race 2.0");
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
                    var harmony = new Harmony("Harmony_PrisonLabor_Kajin2");
                    harmony.Patch(methodBase, postfix: new HarmonyMethod(typeof(Kajin2).GetMethod("postfix_Job")));
                }
            }
            catch (Exception e)
            {
                Log.Error($"PrisonLaborException: failed to proceed Kijin Race 2.0 mod patches: {e.ToString()}");
            }

        }

        public static Pawn postfix_Job( Pawn __result, IntVec3 c, Map map)
        {
            if (__result == null)
            {
                foreach(Thing thing in GridsUtility.GetThingList(c, map) )
                {
                    Pawn val = thing as Pawn;
                    if (val != null && !val.Dead && !val.Downed && val.IsPrisonerOfColony)
                    {
                        return val;
                    }
                }
            }
            return __result;
        }
        private static MethodBase getTargetMethod()
        {
            return AccessTools.Method("Kijin2.Kijin2PlantCollectedPatch:GetFirstPawnNotDeadOrDowned");
        }
    }
}
