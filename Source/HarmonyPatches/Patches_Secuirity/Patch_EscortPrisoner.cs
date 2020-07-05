using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using PrisonLabor.Core.LaborArea;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Secuirity
{
    [HarmonyPatch(typeof(JobDriver_TakeToBed))]
    [HarmonyPatch("MakeNewToils")]
    public static class Patch_EscortPrisoner
    {
        public static bool Prefix(JobDriver_TakeToBed __instance, ref IEnumerable<Toil> __result)
        {
            var targetPawn = (Pawn)__instance.job.GetTarget(TargetIndex.A);

            if (!targetPawn.IsPrisonerOfColony || __instance.job.playerForced)
                return true;

            if (targetPawn.timetable.CurrentAssignment != TimeAssignmentDefOf.Work)
                return true;

            if (!targetPawn.Map.areaManager.Get<Area_Labor>().ActiveCells.Contains(targetPawn.Position))
                return true;

            var toils = new List<Toil>();

            toils.Add(MakeWatchToil(targetPawn));
            for (var i = 0; i < 40; i++)
                toils.Add(Toils_General.Wait(3));

            toils.Add(MakeWatchToil(targetPawn));
            for (var i = 0; i < 40; i++)
                toils.Add(Toils_General.Wait(3));

            __result = toils.AsEnumerable(); return false;
        }

        static Toil MakeWatchToil(Pawn prisoner)
        {
            var toil = new Toil();
            toil.initAction = delegate
            {
                var actor = toil.actor;
                var room = prisoner.GetRoom();

                if (room == null)
                    room = actor.GetRoom();

                actor.pather.StartPath(room.Cells.RandomElement(), PathEndMode.OnCell);
            };
            toil.defaultCompleteMode = ToilCompleteMode.PatherArrival;
            return toil;
        }
    }
}
