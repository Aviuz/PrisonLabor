using System;
using HarmonyLib;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace PrisonLabor.HarmonyPatches.Patches_FactionsAndLords
{
    [HarmonyPatch(typeof(Faction))]
    [HarmonyPatch(nameof(Faction.Notify_MemberCaptured))]
    public static class Patches_ColonistArrested
    {
        public static void Postfix(Pawn member)
        {
            if (member.IsPrisoner)
            {
                lock (Tracked.LOCK_WARDEN)
                {
                    var comp = member.TryGetComp<PrisonerComp>();

                    if (comp == null)
                        return;

                    if (!Tracked.index.ContainsKey(comp.id))
                        return;

                    if (Tracked.index[comp.id] == -1)
                    {
                        Tracked.CleanUp();
                    }
                    else if (Tracked.Wardens[Tracked.index[comp.id]].Contains(comp.id))
                    {
                        Tracked.Wardens[Tracked.index[comp.id]].Remove(comp.id);
                        Tracked.index[comp.id] = -1;
                    }
                }
            }

#if TRACE
            Log.Message("Pawn Arrest Attempted");
#endif


        }
    }
}