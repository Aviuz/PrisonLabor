using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Sound;

namespace PrisonLabor.CompatibilityPatches
{
    internal static class SeedsPlease_WorkDriver_Patch
    {
        static MethodInfo method = null;

        public static void Run()
        {
            var harmony = HarmonyInstance.Create("Harmony_PrisonLabor_SeedsPlease");
            var harvestDriverClass = JobDefOf.Harvest.driverClass.BaseType;
            harmony.Patch(
                harvestDriverClass.GetMethod("HarvestSeedsToil", BindingFlags.NonPublic | BindingFlags.Instance),
                new HarmonyMethod(null), new HarmonyMethod(null), new HarmonyMethod(typeof(SeedsPlease_WorkDriver_Patch).GetMethod("MethodFinder")));
            harmony.Patch(
                method,
                new HarmonyMethod(null), new HarmonyMethod(null), new HarmonyMethod(typeof(SeedsPlease_WorkDriver_Patch).GetMethod("DelegateTranspiler")));
        }

        public static IEnumerable<CodeInstruction> MethodFinder(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            bool first = true;
            foreach (CodeInstruction instruction in instr)
            {
                if (first && instruction.opcode == OpCodes.Ldftn)
                {
                    method = instruction.operand as MethodInfo;
                    first = false;
                }
                yield return instruction;
            }
        }

        public static IEnumerable<CodeInstruction> DelegateTranspiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            int step = 0;
            int virtCall = 5;

            foreach (var ci in instr)
            {
                if (step == 0 && ci.opcode == OpCodes.Ldloc_0)
                {
                    step++;
                    yield return ci;
                }
                else if (step == 1)
                {
                    if (ci.opcode == OpCodes.Callvirt)
                    {
                        if (virtCall != 0)
                        {
                            yield return ci;
                            virtCall--;
                            step--;
                        }
                        else
                        {
                            step++;
                        }
                    }
                    else
                    {
                        step--;
                        yield return ci;
                    }
                }
                else if (step == 2)
                {
                    if (ci.opcode == OpCodes.Beq)
                    {
                        yield return new CodeInstruction(OpCodes.Call, typeof(SeedsPlease_WorkDriver_Patch).GetMethod(nameof(CorrectCondition)));
                        yield return new CodeInstruction(OpCodes.Brfalse, ci.operand);
                        step++;
                    }
                }
                else
                {
                    yield return ci;
                }
            }
        }

        public static bool CorrectCondition(Pawn pawn)
        {
            if (!pawn.IsColonist && !pawn.IsPrisonerOfColony)
                return true;
            return false;
        }
    }
}
