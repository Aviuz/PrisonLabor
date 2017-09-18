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
    public class SeedsPlease_WorkDriver_Patch
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            // Set a variable to the Desktop path.
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Write the string array to a new file.
            string fileName = "debug";
            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\" + fileName + ".txt"))
            {
                outputFile.WriteLine("==========");
                outputFile.WriteLine("Body of %s method", fileName);
                outputFile.WriteLine("==========");
                foreach (CodeInstruction instruction in instr)
                {
                    yield return instruction;
                    outputFile.WriteLine(String.Concat(instruction.opcode, " | ", instruction.operand, " | ", instruction.labels.Count > 0 ? instruction.labels[0].ToString() : "no labels"));
                }
            }
        }

        public static void Run()
        {
            var harmony = HarmonyInstance.Create("Harmony_PrisonLabor_SeedsPlease");
            var harvestDriverClass = DefDatabase<JobDef>.GetNamed("Harvest").driverClass;
            harmony.Patch(
                harvestDriverClass.GetMethod("HarvestSeedsToil",
                    new Type[0]),
                new HarmonyMethod(null), new HarmonyMethod(null), new HarmonyMethod(typeof(SeedsPlease_WorkDriver_Patch).GetMethod("Transpiler")));
        }
    }
}
