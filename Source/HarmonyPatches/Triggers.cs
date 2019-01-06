using Harmony;
using PrisonLabor.Core.News;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using PrisonLabor.Tweaks;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches
{
    /// <summary>
    /// This is group of patch dedicated to attach execution of other methods similar to event handling.
    /// The difference between patches inside this class, and others is that
    /// this patches are executing single method of Prison Labor classes.
    /// </summary>
    static class Triggers
    {
        [HarmonyPatch(typeof(FloatMenuMakerMap))]
        [HarmonyPatch("AddHumanlikeOrders")]
        [HarmonyPatch(new[] { typeof(Vector3), typeof(Pawn), typeof(List<FloatMenuOption>) })]
        static class AddHumanlikeOrders
        {
            public static void Prefix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
            {
                ArrestUtility.AddArrestOrder(clickPos, pawn, opts);
            }
        }

        [HarmonyPatch(typeof(Pawn_HealthTracker), "PreApplyDamage")]
        static class Trigger_PreApplyDamage
        {
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instr)
            {
                foreach (var ci in instr)
                {
                    if (ci.opcode == OpCodes.Ret)
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return new CodeInstruction(OpCodes.Ldarg_1);
                        yield return new CodeInstruction(OpCodes.Ldarg_2);
                        yield return new CodeInstruction(OpCodes.Ldind_I1);
                        yield return new CodeInstruction(OpCodes.Call, typeof(TreatmentUtility).GetMethod(nameof(TreatmentUtility.OnApplyDamage)));
                    }

                    yield return ci;
                }
            }
        }

        static class UpgradeSave
        {

            [HarmonyPatch(typeof(Pawn_JobTracker))]
            [HarmonyPatch(nameof(Pawn_JobTracker.ExposeData))]
            static class Trigger_Pawn_JobTracker
            {
                static bool Prefix()
                {
                    SaveUpgrader.Pawn_JobTracker();
                    return true;
                }
            }


            [HarmonyPatch(typeof(Job))]
            [HarmonyPatch(nameof(Job.ExposeData))]
            static class Trigger_Job
            {
                static bool Prefix()
                {
                    SaveUpgrader.Job();
                    return true;
                }
            }
        }

        [HarmonyPatch(typeof(Map))]
        [HarmonyPatch(nameof(Map.MapPostTick))]
        static class InsiprationTracker
        {
            static bool Prefix(Map __instance)
            {
                InspirationTracker.Calculate(__instance);
                return true;
            }
        }

        [HarmonyPatch(typeof(Map))]
        [HarmonyPatch("FinalizeInit")]
        [HarmonyPatch(new Type[] { })]
        static class Patch_ShowNews
        {
            static void Postfix()
            {
                NewsDialog.TryShow();
            }
        }
    }
}
