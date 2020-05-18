using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_LaborArea
{
    [HarmonyPatch]
    internal class Patch_ProcessCell
    {
        public static MethodInfo TargetMethod()
        {
            Log.Error("Patch start");
            Type[] nested = typeof(JobGiver_Work).GetNestedTypes(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach(Type type in nested)
            {
                Log.Error($"Nested types \"{type}\"");
            }

            return null;
        }
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            HPatcher.CreateDebugFileOnDesktop("ProcessCell", instructions);
            return instructions;
        }
    }
}
