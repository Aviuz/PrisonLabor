using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    class HarmonyPatches
    {

        public static void run()
        {
            var harmony = HarmonyInstance.Create("Harmony_PrisonLabor");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            harmony.Patch(typeof(Pawn_CarryTracker).GetMethod("TryDropCarriedThing", new Type[] { typeof(IntVec3), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>) }),
                new HarmonyMethod(null), new HarmonyMethod(typeof(ForibiddenDropPatch).GetMethod("Postfix")));
            harmony.Patch(typeof(Pawn_CarryTracker).GetMethod("TryDropCarriedThing", new Type[] { typeof(IntVec3), typeof(int), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>) }),
                new HarmonyMethod(null), new HarmonyMethod(typeof(ForibiddenDropPatch).GetMethod("Postfix2")));
        }

    }

    [HarmonyPatch(typeof(PrisonerInteractionModeUtility))]
    [HarmonyPatch("GetLabel")]
    [HarmonyPatch(new Type[] { typeof(PrisonerInteractionModeDef) })]
    class PrisonInteractionPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            // create our WORK label
            Label jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Call, typeof(PrisonInteractionPatch).GetMethod("getLabelWork"));
            //yield return new CodeInstruction(OpCodes.Dup);
            yield return new CodeInstruction(OpCodes.Ldstr, "Work");
            yield return new CodeInstruction(OpCodes.Bne_Un, jumpTo);
            yield return new CodeInstruction(OpCodes.Ldstr, "PrisonLabor_PrisonerWork".Translate());
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (CodeInstruction ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                //debug
                //Log.Message("CODE: ToString():" + ci.ToString() + " || labels:" + ci.labels.Any());
                yield return ci;
            }
        }

        public static string getLabelWork(PrisonerInteractionModeDef def)
        {
            if (def == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                return "Work";
            return "";
        }
    }

    [HarmonyPatch(typeof(Pawn_NeedsTracker))]
    [HarmonyPatch("ShouldHaveNeed")]
    [HarmonyPatch(new Type[] { typeof(NeedDef) })]
    class NeedOnlyByPrisonersPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            //Searches for loadFieldPawn Instruction. Can't create this by generator (don't know why)
            CodeInstruction loadFieldPawn = null;
            foreach (CodeInstruction ci in instr)
            {
                if (ci.opcode.Value == OpCodes.Ldfld.Value)
                {
                    loadFieldPawn = ci;
                    break;
                }
            }
            
            // Define label to the begining of the original code
            Label jumpTo = gen.DefineLabel();
            //Load argument onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            //Load pawn onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return loadFieldPawn;
            //Call function
            yield return new CodeInstruction(OpCodes.Call, typeof(NeedOnlyByPrisonersPatch).GetMethod("ShouldHaveNeedPrisoner"));
            //If true continue
            yield return new CodeInstruction(OpCodes.Brtrue, jumpTo);
            //Load false to stack
            yield return new CodeInstruction(OpCodes.Ldc_I4_0);
            //Return
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (CodeInstruction ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                //debug
                //Log.Message("CODE: ToString():" + ci.ToString() + " || labels:" + ci.labels.Any());
                yield return ci;
            }
        }


        public static bool ShouldHaveNeedPrisoner(NeedDef nd, Pawn pawn)
        {
            //delete later
            if (nd.defName == "PrisonLabor_Laziness" || nd is Need_Laziness)
                return false;
            if (nd.defName == "PrisonLabor_Motivation" && !pawn.IsPrisoner)
            {
                return false;
            }
            return true;
        }
    }

    class ForibiddenDropPatch
    {
        public static void Postfix(Pawn_CarryTracker __instance, IntVec3 dropLoc, ThingPlaceMode mode, Thing resultingThing, Action<Thing, int> placedAction = null)
        {
            if (resultingThing.IsForbidden(Faction.OfPlayer) && __instance.pawn.IsPrisonerOfColony)
                resultingThing.SetForbidden(false);
        }

        public static void Postfix2(Pawn_CarryTracker __instance, int count, IntVec3 dropLoc, ThingPlaceMode mode, Thing resultingThing, Action<Thing, int> placedAction = null)
        {
            Postfix(__instance, dropLoc, mode, resultingThing, placedAction);
        }
    }

    [HarmonyPatch(typeof(Verse.AI.HaulAIUtility))]
    [HarmonyPatch("PawnCanAutomaticallyHaulBasicChecks")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
    class ReservedByPrisonerPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            //Load arguments onto stack
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            //Call function
            yield return new CodeInstruction(OpCodes.Call, typeof(ReservedByPrisonerPatch).GetMethod("CanHaulAndInPrisonCell"));
            //Return
            yield return new CodeInstruction(OpCodes.Ret);
        }


        public static bool CanHaulAndInPrisonCell(Pawn p, Thing t, bool forced)
        {
            UnfinishedThing unfinishedThing = t as UnfinishedThing;
            if (unfinishedThing != null && unfinishedThing.BoundBill != null)
            {
                return false;
            }
            if (!p.CanReach(t, PathEndMode.ClosestTouch, p.NormalMaxDanger(), false, TraverseMode.ByPawn))
            {
                return false;
            }
            if (!p.CanReserve(t, 1, -1, null, forced))
            {
                return false;
            }
            if (t.def.IsNutritionGivingIngestible && t.def.ingestible.HumanEdible && !t.IsSociallyProper(p, false, true))
            {
                if (PrisonerFoodReservation.isReserved(t))
                {
                    JobFailReason.Is("ReservedForPrisoners".Translate());
                    return false;
                }
            }
            if (t.IsBurning())
            {
                JobFailReason.Is("BurningLower".Translate());
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MainTabWindow_Work))]
    [HarmonyPatch("get_Pawns")]
    class WorkTabPatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Call, typeof(WorkTabPatch).GetMethod("Pawns"));
                    yield return new CodeInstruction(OpCodes.Ret);
        }

        public static IEnumerable<Pawn> Pawns(MainTabWindow mainTabWindow)
        {
            foreach (Pawn p in Find.VisibleMap.mapPawns.FreeColonists)
                yield return p;
            if (mainTabWindow is MainTabWindow_Work || mainTabWindow is MainTabWindow_Restrict || mainTabWindow.GetType().ToString().Contains("MainTabWindow_WorkTab"))
            {
                foreach (Pawn pawn in Find.VisibleMap.mapPawns.PrisonersOfColony)
                    if (pawn.guest.interactionMode == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                    {
                        PrisonLaborUtility.InitWorkSettings(pawn);
                        yield return pawn;
                    }
            }
        }
    }

    [HarmonyPatch(typeof(WidgetsWork))]
    [HarmonyPatch("DrawWorkBoxFor")]
    [HarmonyPatch(new Type[] { typeof(float), typeof(float), typeof(Pawn), typeof(WorkTypeDef), typeof(bool) })]
    class WorkDisablePatch
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            // Define label to the begining of the original code
            Label jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Ldarg_3);
            yield return new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod("Disabled", new Type[] { typeof(Pawn), typeof(WorkTypeDef) }));
            //If false continue
            yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
            //Return
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (CodeInstruction ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                yield return ci;
            }
        }
    }

    [HarmonyPatch(typeof(WidgetsWork))]
    [HarmonyPatch("TipForPawnWorker")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(WorkTypeDef), typeof(bool) })]
    class WorkDisablePatch2
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            // Define label to the begining of the original code
            Label jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Ldarg_1);
            yield return new CodeInstruction(OpCodes.Call, typeof(PrisonLaborUtility).GetMethod("Disabled", new Type[] { typeof(Pawn), typeof(WorkTypeDef) }));
            //If false continue
            yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
            //Load string TODO translate
            yield return new CodeInstruction(OpCodes.Ldstr, "Work type disabled by prisoners");
            //Return
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (CodeInstruction ci in instr)
            {
                if (first)
                {
                    first = false;
                    ci.labels.Add(jumpTo);
                }
                yield return ci;
            }
        }
    }

    [HarmonyPatch(typeof(PawnColumnWorker_Label))]
    [HarmonyPatch("DoCell")]
    [HarmonyPatch(new Type[] { typeof(Rect), typeof(Pawn), typeof(PawnTable) })]
    //(Rect rect, Pawn pawn, PawnTable table)
    class changeWorkTabPrisonerLabelColor
    {
        private static void Prefix(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.IsPrisonerOfColony)
            {
                // Log.Message("Pawn " + pawn.LabelCap + " detected as a prisoner");
                GUI.color = new Color32(0xB8, 0x9C, 0x73, 0xFF); // Color32(R,G,B,A), here is prisoner color
            }
        }
    }

    [HarmonyPatch(typeof(PawnColumnWorker_AllowedArea))]
    [HarmonyPatch("DoCell")]
    [HarmonyPatch(new Type[] { typeof(Rect), typeof(Pawn), typeof(PawnTable) })]
    //(Rect rect, Pawn pawn, PawnTable table)
    class disableAreaRestrictionsForPrisoners
    {
        private static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instr)
        {
            Label jumpTo = gen.DefineLabel();
            yield return new CodeInstruction(OpCodes.Ldarg_2);
            yield return new CodeInstruction(OpCodes.Call, typeof(disableAreaRestrictionsForPrisoners).GetMethod("isPrisoner"));
            yield return new CodeInstruction(OpCodes.Brfalse, jumpTo);
            yield return new CodeInstruction(OpCodes.Ret);

            bool first = true;
            foreach (CodeInstruction ci in instr)
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
    }
}