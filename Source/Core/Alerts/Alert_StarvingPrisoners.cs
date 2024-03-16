using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.Alerts
{
  internal class Alert_StarvingPrisoners : Alert
  {
    public Alert_StarvingPrisoners()
    {
      defaultLabel = "PrisonLabor_StarvingPrisonerAlert".Translate();
      defaultExplanation = "PrisonLabor_StarvingPrisonerExplanationDef".Translate();
    }

    private IEnumerable<Pawn> StarvingPrisoners
    {
      get
      {
        return PawnsFinder.AllMaps_PrisonersOfColonySpawned.Where(prisoner => IsStarvingPrisoner(prisoner));
      }
    }

    private bool IsStarvingPrisoner(Pawn pawn)
    {
      return PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn) &&
                            (!PrisonLaborPrefs.EnableMotivationMechanics || !pawn.needs.TryGetNeed<Need_Motivation>().IsLazy) && pawn.timetable != null &&
                            pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Anything &&
                            pawn.needs.food.Starving;
    }

    public override TaggedString GetExplanation()
    {
      var stringBuilder = new StringBuilder();
      foreach (var current in StarvingPrisoners)
        stringBuilder.AppendLine("    " + current.Name.ToStringShort);
      return string.Format("PrisonLabor_StarvingPrisonerExplanation".Translate(), stringBuilder);
    }

    public override AlertReport GetReport()
    {
      return AlertReport.CulpritIs(StarvingPrisoners.FirstOrDefault());
    }
  }
}