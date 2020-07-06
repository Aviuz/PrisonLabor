using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Instrumentation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch(typeof(Blueprint), "TryReplaceWithSolidThing")]
    class Patch_BlueprintsForPlayerFaction
    {

        public static void Postfix(Pawn workerPawn, Thing createdThing, bool __result)
        {
            
            if (__result && createdThing != null && createdThing.def.CanHaveFaction && workerPawn.IsPrisonerOfColony)
            {
#if DEBUG
                Log.Message($"Setting faction for: {createdThing}");
#endif
                createdThing.SetFactionDirect(Faction.OfPlayer);
            }
        
        }

    }
}
