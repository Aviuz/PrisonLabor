using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  public class RitualBehaviorWorker_Interrogation : RitualBehaviorWorker
  {
    private int ticksSinceLastInteraction = -1;

    public const int SocialInteractionIntervalTicks = 700;
    private const string PrisonerRoleId = "prisoner";
    private const string WardenRoleId = "warden";

    public RitualBehaviorWorker_Interrogation()
    {
    }

    public RitualBehaviorWorker_Interrogation(RitualBehaviorDef def)
      : base(def)
    {
    }

    public override void Cleanup(LordJob_Ritual ritual)
    {
      Pawn pawn = ritual.PawnWithRole(PrisonerRoleId);
      if (pawn.IsPrisonerOfColony)
      {
        pawn.guest.WaitInsteadOfEscapingFor(2500);
      }
    }

    public override void PostCleanup(LordJob_Ritual ritual)
    {
      Pawn warden = ritual.PawnWithRole(WardenRoleId);
      Pawn prisoner = ritual.PawnWithRole(PrisonerRoleId);
      if (prisoner.IsPrisonerOfColony)
      {
        ArrestUtility.TakePrisonerToBed(prisoner, warden);
        prisoner.guest.WaitInsteadOfEscapingFor(1250);
      }
    }

    public override void Tick(LordJob_Ritual ritual)
    {
      base.Tick(ritual);
      if (ritual.StageIndex == 0)
      {
        return;
      }
      if (ticksSinceLastInteraction == -1 || ticksSinceLastInteraction > SocialInteractionIntervalTicks)
      {
        ticksSinceLastInteraction = 0;
        Pawn warden = ritual.PawnWithRole(WardenRoleId);
        Pawn prisoner = ritual.PawnWithRole(PrisonerRoleId);
        if (Rand.Bool)
        {
          warden.interactions.TryInteractWith(prisoner, InterrogationDefsOf.PL_InterrogateInteraction);
        }
        else
        {
          prisoner.interactions.TryInteractWith(warden, InterrogationDefsOf.PL_BeIntrrogatedInteraction);
        }
      }
      else
      {
        ticksSinceLastInteraction++;
      }
    }

    public override void ExposeData()
    {
      base.ExposeData();
      Scribe_Values.Look(ref ticksSinceLastInteraction, "ticksSinceLastInteraction", -1);
    }
  }
}
