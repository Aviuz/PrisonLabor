using HarmonyLib;
using RimWorld;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
    //The simplest way to unforbidden harvested thing
    [HarmonyPatch(typeof(QuestManager))]
    [HarmonyPatch("Notify_PlantHarvested")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing) })]
    class Patch_PlantWork
    {
        static void Postfix(Pawn worker, Thing harvested)
        {
            if(worker != null && harvested != null && worker.IsPrisonerOfColony)
            {
                harvested.SetForbidden(false);
            }
        }
    }
}
