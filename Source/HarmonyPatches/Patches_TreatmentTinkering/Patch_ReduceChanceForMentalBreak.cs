using HarmonyLib;
using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_TreatmentTinkering
{
    /// <summary>
    /// This patch is reducing chance of mental break for prisoners with low treatment
    /// </summary>
    [HarmonyPatch(typeof(MentalStateHandler), "TryStartMentalState")]
    static class Patch_ReduceChanceForMentalBreak
    {
        static bool Prefix(MentalStateHandler __instance, bool __result, MentalStateDef stateDef, string reason, bool forceWake, bool causedByMood, Pawn otherPawn)
        {
            if (!causedByMood)
                return true;

            Pawn pawn = (Pawn)(typeof(MentalStateHandler).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance));

            if (pawn.IsPrisonerOfColony)
            {
                var need = pawn.needs.TryGetNeed<Need_Treatment>();
                TreatmentCategory treatmentCat = need.CurCategory;
                bool suspended = false;
                float chance = 0f;

                switch (treatmentCat)
                {
                    case TreatmentCategory.Normal:
                        chance = 0.1f;
                        break;
                    case TreatmentCategory.Bad:
                        chance = 0.5f;
                        break;
                    case TreatmentCategory.VeryBad:
                        chance = 1f;
                        break;
                }

                suspended = UnityEngine.Random.value < chance;

                if (suspended)
                {
                    __result = false;
                    return false;
                }
            }
            return true;
        }
    }
}
