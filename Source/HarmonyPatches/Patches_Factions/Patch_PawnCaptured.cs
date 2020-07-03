using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;
using System;
using System.IO;
using PrisonLabor.Core.Trackers;
using PrisonLabor.Core.Components;

namespace PrisonLabor.HarmonyPatches.Patches_Factions
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(nameof(Faction.Notify_MemberCaptured))]
    public static class Patches_PawnCaptured
    {
        public static void Postfix(Pawn member, Faction violator)
        {
            if (!violator.IsPlayer)
                return;

            lock (Tracked.LOCK_WARDEN)
            {
                var comp = member.TryGetComp<PrisonerComp>();
                if (comp == null && member.IsPrisonerOfColony && !member.Dead)
                {

                    member.AllComps.Add(new PrisonerComp());
                }
                else if (comp == null)
                    return;

                var id = comp.id;

                if (!Tracked.index.ContainsKey(id))
                    Tracked.index[id] = -1;
            }
        }
    }
}
