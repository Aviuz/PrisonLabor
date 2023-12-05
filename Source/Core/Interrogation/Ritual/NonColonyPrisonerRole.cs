using PrisonLabor.Core.Components;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  public class NonColonyPrisonerRole : RitualRole
  {
    public override bool AppliesToPawn(Pawn p, out string reason, TargetInfo selectedTarget, LordJob_Ritual ritual = null, RitualRoleAssignments assignments = null, Precept_Ritual precept = null, bool skipReason = false)
    {
      if (!AppliesIfChild(p, out reason, skipReason))
      {
        return false;
      }
      if (p.IsPrisonerOfColony && !p.Faction.IsPlayer)
      {
        PrisonerComp prisonerComp = p.TryGetComp<PrisonerComp>();
        if (prisonerComp == null)
        {
          PrepareReason("PrisonLabor_MissingComp".Translate(), skipReason, out reason);
          return false;
        }

        if (!prisonerComp.HasIntel)
        {
          PrepareReason("PrisonLabor_NoIntel".Translate(base.LabelCap), skipReason, out reason);
          return false;
        }
        if (!ReadyForInterrogation(prisonerComp))
        {
          PrepareReason("PrisonLabor_TooSoonInterrogation".Translate(base.LabelCap), skipReason, out reason);
          return false;
        }
        return true;
      }

      PrepareReason("PrisonLabor_MustBePrisoner".Translate(base.LabelCap), skipReason, out reason);
      return false;
    }

    private void PrepareReason(string baseReason, bool skipReason, out string reason)
    {
      reason = skipReason ? null : baseReason;
    }

    private bool ReadyForInterrogation(PrisonerComp prisonerComp)
    {
      return prisonerComp.LastInteractionTick == 0 || Find.TickManager.TicksGame - prisonerComp.LastInteractionTick > 60_000;
    }

    public override bool AppliesToRole(Precept_Role role, out string reason, Precept_Ritual ritual = null, Pawn p = null, bool skipReason = false)
    {
      reason = null;
      return false;
    }
  }
}
