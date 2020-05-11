using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_PermissionFix
{
    [HarmonyPatch(typeof(ReservationManager))]
    [HarmonyPatch("RespectsReservationsOf")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(Pawn) })]
    internal class Patch_RespectReservation
    {        
        static bool Postfix(bool __result, Pawn newClaimant, Pawn oldClaimant)
        {
            if ((newClaimant.Faction == Faction.OfPlayer && oldClaimant.IsPrisonerOfColony)
                || (oldClaimant.Faction == Faction.OfPlayer && newClaimant.IsPrisonerOfColony)
                || (newClaimant.IsPrisonerOfColony && oldClaimant.IsPrisonerOfColony))
            {                
                return true;
            }
            return __result;
        }

    }
}
