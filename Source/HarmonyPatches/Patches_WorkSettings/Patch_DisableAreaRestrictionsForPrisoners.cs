using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PrisonLabor.HarmonyPatches.Patches_WorkSettings
{
    /// <summary>
    /// This patch will remove prisoners in "Restrict" tab.
    /// They are there in first place, because of adding them to PawnTable in another patch.
    /// </summary>
    /*  [HarmonyPatch(typeof(PawnColumnWorker_AllowedArea))]
      [HarmonyPatch("DoCell")]
      [HarmonyPatch(new[] {typeof(Rect), typeof(Pawn), typeof(PawnTable)})]
      internal class DisableAreaRestrictionsForPrisoners
      {
          private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase,
              IEnumerable<CodeInstruction> instr)
          {
              var jumpTo = gen.DefineLabel();
              yield return new CodeInstruction(OpCodes.Ldarg_2);
              yield return new CodeInstruction(OpCodes.Call,
                  typeof(DisableAreaRestrictionsForPrisoners).GetMethod(nameof(isPrisoner)));
              yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
              yield return new CodeInstruction(OpCodes.Ret);

              var first = true;
              foreach (var ci in instr)
              {
                  if (first)
                  {
                      first = false;
                      ci.labels.Add(jumpTo);
                  }
                  yield return ci;
              }
          }

          public static bool isPrisoner(Pawn pawn)
          {
              if (pawn.IsPrisoner)
                  return true;
              return false;
          }
      }*/

    [HarmonyPatch(typeof(PawnColumnWorker_AllowedArea))]
    [HarmonyPatch("DoCell")]
    class EnableAreaRestrictionsForPrisoners
    {
        static void Postfix(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.IsPrisonerOfColony)
            {
                //Log.Message($"Pawn {pawn.LabelShort} area: {pawn.playerSettings.AreaRestriction}, is MouseBlocked: {Mouse.IsInputBlockedNow}");
                AreaAllowedGUI.DoAllowedAreaSelectors(rect, pawn);
            }
        }
    }

}