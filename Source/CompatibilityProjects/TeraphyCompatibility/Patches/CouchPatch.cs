using HarmonyLib;
using PrisonLabor.Therapy.Comps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Therapy;
using Verse;

namespace PrisonLabor.Therapy.Patches
{
    [HarmonyPatch(typeof(Building_Couch), "ExplicitlyAssignedTo")]
    public class CouchPatch
    {
        public static bool Postfix(bool __result, Building_Couch __instance, Pawn pawn)
        {

            AssignableToPrisoner_Comp comp = __instance?.TryGetComp<AssignableToPrisoner_Comp>();
            if (comp != null)
            {
                if (pawn.IsPrisonerOfColony)
                {
                    return comp.AssignedAnything(pawn);
                }
                return __result && comp.AssignedPawnsForReading.Count == 0;
            }
            return __result;
        }

    }
}
