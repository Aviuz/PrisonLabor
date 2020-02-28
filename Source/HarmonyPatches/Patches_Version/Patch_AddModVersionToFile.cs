using HarmonyLib;
using PrisonLabor.Core.Meta;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Version = PrisonLabor.Core.Meta.Version;

namespace PrisonLabor.HarmonyPatches.Patches_Version
{
    static class Patch_AddModVersionToFile
    {
        private const string ParameterName = "PrisonLaborVersion";

        [HarmonyPatch(typeof(ScribeMetaHeaderUtility))]
        [HarmonyPatch(nameof(ScribeMetaHeaderUtility.WriteMetaHeader))]
        static class Patch_WriteVersion
        {
            /*  === Orignal code Look-up===
             *  
             * catch
             * {
             *      (...)
             *      <---
             * }     
             * finally
             * {
             * 		(...)
             * }
             *  
             *  === CIL Instructions ===
             *  
             *  leave | label 4 | no labels
             *  
             */

            static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
            {
                OpCode[] opCodes1 =
                {
                    OpCodes.Leave,
                };
                string[] operands1 =
                {
                    "System.Reflection.Emit.Label",
                };
                int step1 = 0;

                foreach (var instr in instructions)
                {
                    if (HPatcher.IsFragment(opCodes1, operands1, instr, ref step1, nameof(Patch_WriteVersion), true))
                        yield return new CodeInstruction(OpCodes.Call, typeof(Patch_WriteVersion).GetMethod(nameof(AddMetaData)));
                    yield return instr;
                }
            }

            public static void AddMetaData()
            {
                var currentVersionString = VersionUtility.versionNumber;
                Scribe_Values.Look(ref currentVersionString, ParameterName, default(Version), true);
            }
        }


        [HarmonyPatch(typeof(ScribeMetaHeaderUtility))]
        [HarmonyPatch(nameof(ScribeMetaHeaderUtility.LoadGameDataHeader))]
        static class Patch_LoadVersion
        {
            /*  === Orignal code Look-up===
             *  
             * catch
             * {
             *      (...)
             *      <---
             * }     
             * finally
             * {
             * 		(...)
             * }
             *  
             *  === CIL Instructions ===
             *  
             *  leave | label x | no labels
             *  
             */

            static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
            {
                OpCode[] opCodes1 =
                {
                    OpCodes.Leave,
                };
                string[] operands1 =
                {
                    "System.Reflection.Emit.Label",
                };
                int step1 = 0;

                foreach (var instr in instructions)
                {
                    if (HPatcher.IsFragment(opCodes1, operands1, instr, ref step1, nameof(Patch_LoadVersion), true))
                        yield return new CodeInstruction(OpCodes.Call, typeof(Patch_LoadVersion).GetMethod(nameof(ReadMetaData)));
                    yield return instr;
                }
            }

            public static void ReadMetaData()
            {
                var currentVersionString = VersionUtility.versionNumber;
                // TODO change version to 0.9.6 later, on next major update from RimWorld 1.0
                Scribe_Values.Look(ref currentVersionString, ParameterName, Version.v0_9_6, true);
                VersionUtility.VersionOfSaveFile = currentVersionString;
            }
        }

    }
}
