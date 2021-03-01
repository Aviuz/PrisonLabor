using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_BillAssignation
{
    [HarmonyPatch(typeof(Bill_ProductionWithUft), "get_BoundWorker")]
    class Patch_Bill_ProductionWithUft
    {

        static void Prefix(Bill_ProductionWithUft __instance, out UnfinishedThing __state)
        {
            UnfinishedThing unfinishedThing = Traverse.Create(__instance).Field("boundUftInt").GetValue<UnfinishedThing>();

            if(unfinishedThing != null && unfinishedThing.Creator != null && unfinishedThing.Creator.IsPrisonerOfColony)
            {
                DebugLogger.debug($"[PL] Saving unfinished thing to state pawn: {unfinishedThing.Creator.LabelShort}, bill: {unfinishedThing.LabelShort}");
                __state = unfinishedThing;
            }
            else
            {
                __state = null;
            }

        }

        static Pawn Postfix(Pawn __result, Bill_ProductionWithUft __instance, UnfinishedThing __state)
        {
            
            if (__result == null && __state != null)
            {
                Pawn creator = __state.Creator;
                if (creator == null || creator.Downed  || creator.Destroyed || !creator.Spawned)
                {
                    return __result;
                }
                Thing thing = __instance.billStack.billGiver as Thing;
                if (thing != null)
                {
                    WorkTypeDef workTypeDef = null;
                    List<WorkGiverDef> allDefsListForReading = DefDatabase<WorkGiverDef>.AllDefsListForReading;
                    for (int i = 0; i < allDefsListForReading.Count; i++)
                    {
                        if (allDefsListForReading[i].fixedBillGiverDefs != null && allDefsListForReading[i].fixedBillGiverDefs.Contains(thing.def))
                        {
                            workTypeDef = allDefsListForReading[i].workType;
                            break;
                        }
                    }
                    if (workTypeDef != null && !creator.workSettings.WorkIsActive(workTypeDef))
                    {						
                        return __result;
                    }
                }
                Traverse.Create(__instance).Field("boundUftInt").SetValue(__state);
                DebugLogger.debug($"Returning creator {creator.LabelShort}, value override");
                return creator;
            }

            return __result;
        }
    }
}
