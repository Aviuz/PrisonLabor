using System;
using HarmonyLib;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Trackers;
using Verse;
using Verse.AI.Group;

namespace PrisonLabor.HarmonyPatches.Patches_FactionsAndLords
{
    [HarmonyPatch(typeof(Lord))]
    [HarmonyPatch(nameof(Lord.Notify_PawnAttemptArrested))]
    public static class Patches_ColonistArrested
    {
        public static void Postfix(Pawn victim)
        {
            if (victim.IsPrisonerOfColony)
            {
                lock (Tracked.LOCK_WARDEN)
                {
                    var comp = victim.TryGetComp<PrisonerComp>();

                    if (comp == null)
                        return;

                    if (!Tracked.index.ContainsKey(comp.id))
                        return;

                    if (Tracked.index[comp.id] == -1)
                        return;

                    if (Tracked.Wardens[Tracked.index[comp.id]].Contains(comp.id))
                    {
                        Tracked.Wardens[Tracked.index[comp.id]].Remove(comp.id);
                        Tracked.index[comp.id] = -1;
                    }
                }
            }
        }
    }
}
