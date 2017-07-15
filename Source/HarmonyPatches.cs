using Harmony;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;

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
            if (nd.defName == "PrisonLabor_Laziness" && !pawn.IsPrisoner)
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
}
