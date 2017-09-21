using System;
using System.Reflection;
using Harmony;
using PrisonLabor.Harmony;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    internal static class Initialization
    {
        public static void Run()
        {
            var harmony = HarmonyInstance.Create("Harmony_PrisonLabor");
            try
            {
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
            }
            catch(Exception e)
            {
                Log.Error($"Prison Labor Exception, failed to proceed harmony patches: {e.Message}");
            }
        }
    }
}

/* CIL Debugging method
===============
// Set a variable to the Desktop path.
string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

// Write the string array to a new file.
string fileName = "debug";
using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\" + fileName + ".txt"))
{
    outputFile.WriteLine("==========");
    outputFile.WriteLine("Body of " + fileName + " method", fileName);
    outputFile.WriteLine("==========");
    foreach (CodeInstruction instruction in instr)
    {
        yield return instruction;
        outputFile.WriteLine(String.Concat(instruction.opcode, " | ", instruction.operand, " | ", instruction.labels.Count > 0 ? instruction.labels[0].ToString() : "no labels"));
    }
}
===============
*/
