using PrisonLabor.Core.Components;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.Other
{
  public static class ArrestUtility
  {
    //For Multiplayer Compatibility
    public static void ArrestPrisoner(Pawn prisoner, Pawn pawn)
    {
      var building_Bed = FindBed(prisoner, pawn);
      if (building_Bed == null)
      {
        Messages.Message("CannotArrest".Translate() + ": " + "NoPrisonerBed".Translate(), prisoner,
          MessageTypeDefOf.RejectInput, false);
        return;
      }

      var job = new Job(JobDefOf.Arrest, prisoner, building_Bed)
      {
        count = 1
      };
      pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
      if (prisoner.Faction != null &&
          ((prisoner.Faction != Faction.OfPlayer && !prisoner.Faction.Hidden) || prisoner.IsQuestLodger()))
      {
        TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.ArrestingCreatesEnemies);
      }
    }

    public static void TakePrisonerToBed(Pawn prisoner, Pawn pawn)
    {
      var building_Bed = FindBed(prisoner, pawn);
      if (building_Bed == null)
      {
        Messages.Message("PrisonLabor_CannotTakeToBed".Translate() + ": " + "NoPrisonerBed".Translate(), prisoner,
          MessageTypeDefOf.RejectInput, false);
        return;
      }

      var job = new Job(JobDefOf.EscortPrisonerToBed, prisoner, building_Bed)
      {
        count = 1
      };
      pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
    }

    private static Building_Bed FindBed(Pawn pTarg, Pawn pawn)
    {
      return RestUtility.FindBedFor(pTarg, pawn, false, false, GuestStatus.Prisoner) ?? 
             RestUtility.FindBedFor(pTarg, pawn, false, true, GuestStatus.Prisoner);
    }

    public static bool CanBeTakenToBed(Pawn pawn, Pawn arrester)
    {
      return (!pawn.InAggroMentalState || !pawn.HostileTo(arrester)) && !IsPawnFleeing(pawn) && pawn.IsPrisonerOfColony;
    }

    public static bool IsPawnFleeing(Pawn pawn)
    {
      var prisonerComp = pawn.TryGetComp<PrisonerComp>();
      return (prisonerComp?.EscapeTracker.CanEscape ?? false) && pawn.CurJob?.def == JobDefOf.Goto;
    }
  }
}