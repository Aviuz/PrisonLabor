﻿using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{

    [HarmonyPatch(typeof(RestUtility), "FindBedFor", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(GuestStatus) } )]
    class Patch_RestUtility
    {
        //Don't try to take wounded to unreachable bed
        static Building_Bed Postfix(Building_Bed __result, Pawn sleeper, Pawn traveler, bool checkSocialProperness, bool ignoreOtherReservations, GuestStatus? guestStatus)
        {
            if(__result != null && traveler.IsPrisonerOfColony && !traveler.CanReach(__result, PathEndMode.ClosestTouch, traveler.NormalMaxDanger()))
            {
                return null;
            }
            return __result;
        }
    }


    [HarmonyPatch(typeof(WorkGiver_RescueDowned), "HasJobOnThing")]
    class Patch_WorkGiver_RescueDowned
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> inst)
        {
            var codes = new List<CodeInstruction>(inst);
            for (int i = 0; i < codes.Count(); i++)
            {
                if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
                {
                    DebugLogger.debug($"WorkGiver_RescueDowned patch: {mBase.ReflectedType.Name}.{mBase.Name}");
                    yield return new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod(nameof(PrisonLaborUtility.GetPawnFaction)));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        private static bool ShouldPatch(CodeInstruction actual, CodeInstruction prev)
        {
            return prev.opcode == OpCodes.Ldarg_1 && actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("RimWorld.Faction get_Faction()");
        }
    }
}
