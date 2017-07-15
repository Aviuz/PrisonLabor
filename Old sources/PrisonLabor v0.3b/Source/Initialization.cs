using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using System.Reflection;
using System.Reflection.Emit;
using Verse.AI;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    class Initialization
    {
        public static int version = 3;
        public static bool oldPlayerNotification = false;

        static Initialization()
        {
            var harmony = HarmonyInstance.Create("Harmony_PrisonLabor");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            harmony.Patch(typeof(Pawn_CarryTracker).GetMethod("TryDropCarriedThing", new Type[] { typeof(IntVec3), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>) }),
                new HarmonyMethod(null), new HarmonyMethod(typeof(ForibiddenDropPatch).GetMethod("Postfix")));
            harmony.Patch(typeof(Pawn_CarryTracker).GetMethod("TryDropCarriedThing", new Type[] { typeof(IntVec3), typeof(int), typeof(ThingPlaceMode), typeof(Thing).MakeByRefType(), typeof(Action<Thing, int>) }),
                new HarmonyMethod(null), new HarmonyMethod(typeof(ForibiddenDropPatch).GetMethod("Postfix2")));

            PrisonLaborPrefs.Init();
            checkVersion();
        }

        private static void checkVersion()
        {
            if(PrisonLaborPrefs.Version < 3)
            {
                // only way to check if mod was installed before
                if (PlayerKnowledgeDatabase.IsComplete(DefDatabase<ConceptDef>.GetNamed("PrisonLabor")))
                {
                    Log.Message("Detected older version of PrisonLabor");
                    oldPlayerNotification = true;
                }
            }
            else
            {
                Log.Message("Detected PrisonLabor v" + PrisonLaborPrefs.Version);
            }
            PrisonLaborPrefs.Version = 3;
            PrisonLaborPrefs.Save();
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
            yield return new CodeInstruction(OpCodes.Ldstr, "Work");
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
                //Log.Message("CODE: ToString():" + ci.ToString() + " || labels:" + ci.labels + " || opcode:" + ci.opcode.ToString());
                yield return ci;
            }
        }

        public static string getLabelWork(PrisonerInteractionModeDef def)
        {
            if(def == DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption"))
                return "Work";
            return "";
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
