using System;
using System.Reflection;
using Harmony;
using PrisonLabor.Harmony;
using Verse;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Diagnostics;

namespace PrisonLabor.HarmonyPatches
{
    internal static class HPatcher
    {
        // For logging purposes, it stores whenever each fragment was completed
        private static Dictionary<string, bool> fragments;

        public static void Init()
        {
            var harmony = HarmonyInstance.Create("Harmony_PrisonLabor");
            try
            {
                // Clear old data, to avoid misleading info
                fragments = new Dictionary<string, bool>();

                harmony.PatchAll(Assembly.GetExecutingAssembly());

                // TODO move it to seperate files. Why I did this ??? I have no clue ...
                harmony.Patch(
                    typeof(Pawn_CarryTracker).GetMethod("TryDropCarriedThing",
                        new[]
                        {
                        typeof(IntVec3), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(),
                        typeof(Action<Thing, int>)
                        }),
                    new HarmonyMethod(null), new HarmonyMethod(typeof(ForibiddenDropPatch).GetMethod("Postfix")));
                harmony.Patch(
                    typeof(Pawn_CarryTracker).GetMethod("TryDropCarriedThing",
                        new[]
                        {
                        typeof(IntVec3), typeof(int), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(),
                        typeof(Action<Thing, int>)
                        }),
                    new HarmonyMethod(null), new HarmonyMethod(typeof(ForibiddenDropPatch).GetMethod("Postfix2")));

                // Print out not completed methods
                foreach(var f in fragments.Keys)
                {
                    if(!fragments[f])
                        Log.Warning($"PrisonLaborWarning: Harmony patch failed to find \"{f}\" fragment.");
                }
            }
            catch (Exception e)
            {
                Log.Error($"PrisonLaborException: failed to proceed harmony patches: {e.Message}");
            }
        }

        /// <summary>
        /// CIL Debugging method. Creates debug file on desktop that list all CIL code instructions in the method.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="withReturn"></param>
        public static void CreateDebugFileOnDesktop(string fileName, IEnumerable<CodeInstruction> instr)
        {
            // Set a variable to the Desktop path.
            string myDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Write the string array to a new file.
            using (StreamWriter outputFile = new StreamWriter(myDesktopPath + @"\" + fileName + ".txt"))
            {
                outputFile.WriteLine("================");
                outputFile.WriteLine("Body of " + fileName + " method", fileName);
                outputFile.WriteLine("================");
                foreach (CodeInstruction instruction in instr)
                {
                    var instructionString = instruction.opcode.ToString();
                    instructionString += " | ";
                    instructionString += instruction.operand is Label ? $"Label {instruction.operand.GetHashCode()}" : instruction.operand;
                    instructionString += " | ";
                    if (instruction.labels.Count > 0)
                        foreach (var label in instruction.labels)
                            instructionString += $"Label {label.GetHashCode()}";
                    else
                        instructionString += "no labels";
                    outputFile.WriteLine(instructionString);
                }
            }
        }

        /// <summary>
        /// This method is used to add some CIL instructions after certain fragment in original code.
        /// It should be used inside foreach loop, and return true if particular iteration is the desired one.
        /// </summary>
        /// <param name="opCodes"></param>
        /// <param name="operands"></param>
        /// <param name="instr"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static bool IsFragment(OpCode[] opCodes, String[] operands, CodeInstruction instr, ref int step, string fragmentName)
        {
            if (opCodes.Length != operands.Length)
            {
                Log.Error("PrisonLaborException: IsFragment() arguments does not match requirments. Trace:" + new StackTrace());
                return false;
            }

            if (!fragments.ContainsKey(fragmentName))
                fragments.Add(fragmentName, false);
            if (step < 0 || step >= opCodes.Length)
                return false;

            var finalStep = opCodes.Length;

            if (instr.opcode == opCodes[step] && (instr.operand == null || instr.operand.ToString() == operands[step]))
                step++;
            else
                step = 0;

            if (step == finalStep)
            {
                step++;
                fragments[fragmentName] = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method is used to find particular label that is assigned to last instruction's operand 
        /// </summary>
        /// <param name="opCodes"></param>
        /// <param name="operands"></param>
        /// <param name="instr"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        public static object FindOperandAfter(OpCode[] opCodes, String[] operands, IEnumerable<CodeInstruction> instr)
        {
            if (opCodes.Length != operands.Length)
            {
                Log.Error("PrisonLaborException: FindOperandAfter() arguments does not match requirments. Trace:" + new StackTrace());
                return null;
            }

            var finalStep = opCodes.Length;

            int step = 0;
            foreach (var ci in instr)
            {
                if (ci.opcode == opCodes[step] && (ci.operand == null || ci.operand.ToString() == operands[step]))
                    step++;
                else
                    step = 0;

                if (step == finalStep)
                    return ci.operand;
            }

            Log.Error("PrisonLaborException: FindOperandAfter() didn't find any lines. Trace:" + new StackTrace());
            return null;
        }
    }
}
