using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_RenamingPrisoners
{
    /// <summary>
    /// This patch is enabling to temporary rename prisoners for a duration of improsiment
    /// </summary>
    static class Patch_RenamePrisoners
    {
        /// <summary>
        /// Storage for old names
        /// </summary>
        static Dictionary<Pawn, Name> oldNames = new Dictionary<Pawn, Name>();

        /// <summary>
        /// This patch is enabling renaming prisoner of colony
        /// </summary>
        [HarmonyPatch(typeof(CharacterCardUtility))]
        [HarmonyPatch(nameof(CharacterCardUtility.DrawCharacterCard))]
        static class EnableRenamingPrisoners
        {
            /* === Original code look-up===
             * if (pawn.IsColonist)
             * 
             * === CIL instructions===
             * ldloc.0 |  | Label 17Label 18
             * ldfld | Verse.Pawn pawn | no labels
             * callvirt | Boolean get_IsColonist() | no labels
             * 
             */

            static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
            {
                OpCode[] opCodes =
                {
                    OpCodes.Ldloc_0,
                    OpCodes.Ldfld,
                    OpCodes.Callvirt,
                };
                string[] operands =
                {
                    "",
                    "Verse.Pawn pawn",
                    "Boolean get_IsColonist()",
                };
                int step = 0;

                foreach (var instr in instructions)
                {
                    if (HPatcher.IsFragment(opCodes, operands, instr, ref step, nameof(Patch_RenamePrisoners) + nameof(EnableRenamingPrisoners), true))
                    {
                        yield return new CodeInstruction(OpCodes.Call, typeof(EnableRenamingPrisoners).GetMethod(nameof(IsColonistOrPrisonerOfColony)));
                    }
                    else
                    {
                        yield return instr;
                    }
                }
            }

            public static bool IsColonistOrPrisonerOfColony(Pawn pawn)
            {
                if (pawn.IsColonist || pawn.IsPrisonerOfColony)
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// This patch is for remebering old names
        /// </summary>
        [HarmonyPatch(typeof(Dialog_NamePawn))]
        [HarmonyPatch(nameof(Dialog_NamePawn.DoWindowContents))]
        static class StoreOldName
        {
            static bool Prefix(Dialog_NamePawn __instance, Rect inRect)
            {
                var pawn = typeof(Dialog_NamePawn).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as Pawn;
                if (pawn.IsPrisoner && !oldNames.ContainsKey(pawn))
                    oldNames.Add(pawn, pawn.Name);
                return true;
            }
        }

        /// <summary>
        /// This patch is for storing old names in save file
        /// </summary>
        [HarmonyPatch(typeof(Pawn))]
        [HarmonyPatch(nameof(Pawn.ExposeData))]
        static class SaveOldName
        {
            static void Postfix(Pawn __instance)
            {
                Name name = null;
                if (oldNames.ContainsKey(__instance))
                    name = oldNames[__instance];

                Scribe_Deep.Look<Name>(ref name, "oldName", new object[0]);

                if (name != null)
                    oldNames[__instance] = name;
            }
        }

        /// <summary>
        /// This patch is for restoring old names, after freeing prisoner
        /// </summary>
        [HarmonyPatch(typeof(Pawn_GuestTracker))]
        [HarmonyPatch(nameof(Pawn_GuestTracker.SetGuestStatus))]
        static class RestoreOldName
        {
            static bool Prefix(Pawn_GuestTracker __instance, Faction newHost, bool prisoner = false)
            {
                if (prisoner == false)
                {
                    var pawn = typeof(Pawn_GuestTracker).GetField("pawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(__instance) as Pawn;
                    if (oldNames.ContainsKey(pawn))
                        pawn.Name = oldNames[pawn];
                }
                return true;
            }
        }
    }
}
