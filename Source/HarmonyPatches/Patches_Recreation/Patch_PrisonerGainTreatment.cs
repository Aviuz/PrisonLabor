using HarmonyLib;
using PrisonLabor.Core.Recreation;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Recreation
{
    [HarmonyPatch(typeof(JoyUtility), "JoyTickCheckEnd")]
    class Patch_PrisonerGainTreatment
    {
        static bool Prefix(ref bool __result, Pawn pawn, JoyTickFullJoyAction fullJoyAction, float extraJoyGainFactor, Building joySource)
        {
            if (pawn.IsPrisonerOfColony)
            {
                __result = RecreationUtils.PrisonerJoyTickCheckEnd(pawn, fullJoyAction, extraJoyGainFactor, joySource);
                return false;
            }
            return true;
        }
    }
}
