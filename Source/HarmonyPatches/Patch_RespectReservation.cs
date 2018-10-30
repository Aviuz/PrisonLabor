using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(ReservationManager))]
    [HarmonyPatch("RespectsReservationsOf")]
    internal class Patch_RespectReservation
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            Label label = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(Patch_RespectReservation).GetMethod(nameof(RespectPrisoners)));
            yield return new CodeInstruction(OpCodes.Brfalse, label);
            yield return new CodeInstruction(OpCodes.Ldc_I4_1);
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach(var ci in instr)
            {
                if(first)
                {
                    ci.labels.Add(label);
                    first = false;
                }
                yield return ci;
            }
        }

        public static bool RespectPrisoners(Pawn newClaimant, Pawn oldClaimant)
        {
            if (newClaimant.Faction == Faction.OfPlayer && oldClaimant.IsPrisonerOfColony)
                return true;
            if (oldClaimant.Faction == Faction.OfPlayer && newClaimant.IsPrisonerOfColony)
                return true;
            return false;
        }
    }
}
