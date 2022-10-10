using HarmonyLib;
using PrisonLabor.Core.Other;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Hospitality
{
    [HarmonyPatch]
    public class PrisonersCanBeEntertainers_Patch
    {
        static MethodBase TargetMethod()
        {
            Type classType = AccessTools.TypeByName("Hospitality.Utilities.GuestUtility");
            return AccessTools.Method(classType, "ShouldMakeFriends");
        }

        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            for (int i = 0; i < codes.Count(); i++)
            {
                if (i > 0 && ShouldPatch(codes[i], codes[i - 1]))
                { 
                    yield return new CodeInstruction(OpCodes.Call, typeof(PrisonersCanBeEntertainers_Patch).GetMethod(nameof(PrisonersCanBeEntertainers_Patch.PawnCheck)));
                }
                else
                {
                    yield return codes[i];
                }
            }
        }

        private static bool ShouldPatch(CodeInstruction actual, CodeInstruction prev)
        {
            return prev.opcode == OpCodes.Ldarg_0 && actual.opcode == OpCodes.Callvirt && actual.operand != null && actual.operand.ToString().Contains("Verse.Pawn get_IsColonist()");
        }

        private static bool PawnCheck(Pawn pawn)
        {
            return pawn.IsColonist || pawn.IsPrisonerOfColony;
        }
    }
}
