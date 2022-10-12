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
using Verse.Noise;

namespace KijinCompatibility.HarmonyPatches
{
    [HarmonyPatch]
    class PrisonerHarvestResourcesPatch
    {
        static MethodBase TargetMethod()
        {
            return AccessTools.Method("Kijin2.Kijin2PlantCollectedPatch:GetFirstPawnNotDeadOrDowned");
        }
        static Pawn Postfix(Pawn __result, IntVec3 c, Map map)
        {
            if (__result == null)
            {
                foreach (Thing thing in GridsUtility.GetThingList(c, map))
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
    }
}
