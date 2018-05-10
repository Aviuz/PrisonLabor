using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage")]
    public static class Trigger_PreApplyDamage
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
        {
            foreach(var ci in instr)
            {
                if(ci.opcode == OpCodes.Ret)
                {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Ldarg_2);
                    yield return new CodeInstruction(OpCodes.Ldind_I1);
                    yield return new CodeInstruction(OpCodes.Call, typeof(Trigger_PreApplyDamage).GetMethod("Trigger"));
                }

                yield return ci;
            }
        }

        public static void Trigger(Pawn_HealthTracker instance, DamageInfo dinfo, bool absorbed)
        {
            if (dinfo.Instigator != null && !(dinfo.Instigator is Pawn))
                return;

            Pawn attacker = dinfo.Instigator as Pawn;
            Pawn victim = (Pawn)(typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(instance));

            if (victim.IsPrisonerOfColony && attacker.IsColonist)
            {
                var need = victim.needs.TryGetNeed<Need_Treatment>();
                if (need != null)
                    need.NotifyPrisonerBeaten(dinfo, absorbed);
            }
        }
    }
}
