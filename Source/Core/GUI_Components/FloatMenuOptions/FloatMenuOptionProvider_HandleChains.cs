using System.Collections.Generic;
using PrisonLabor.Constants;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.GUI_Components.FloatMenuOptions
{
  public class FloatMenuOptionProvider_HandleChains : FloatMenuOptionProvider
  {
    protected override bool Drafted => true;

    protected override bool Undrafted => true;

    protected override bool Multiselect => false;

    protected override bool MechanoidCanDo => true;

    protected override bool RequiresManipulation => true;
    
    public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
    {
      return base.TargetPawnValid(pawn, context) && pawn.IsPrisonerOfColony && !pawn.InAggroMentalState && !pawn.Downed;
    }

    public override IEnumerable<FloatMenuOption> GetOptionsFor(Pawn clickedPawn, FloatMenuContext context)
    {
      var selectedPawn = context.FirstSelectedPawn;
      if ( selectedPawn.CanReach(clickedPawn, PathEndMode.ClosestTouch, Danger.Deadly))
      {
        yield return AddOption(selectedPawn,clickedPawn, 
          LabelSelect(clickedPawn, PL_DefOf.PrisonLabor_RemovedLegscuffs, "PrisonLabor_LegcuffsPut",
            "PrisonLabor_LegcuffsRemove"), PL_DefOf.PrisonLabor_HandlePrisonersLegChain);
        yield return AddOption(selectedPawn,clickedPawn, 
          LabelSelect(clickedPawn, PL_DefOf.PrisonLabor_RemovedHandscuffs, "PrisonLabor_HandcuffsPut",
            "PrisonLabor_HandcuffsRemove"), PL_DefOf.PrisonLabor_HandlePrisonersHandChain);
      }
    }
    private static string LabelSelect(LocalTargetInfo target, HediffDef hediffDef, string labelA, string labelB)
    {
      return target.Pawn.health.hediffSet.HasHediff(hediffDef, false) ? labelA : labelB;
    }


    private static FloatMenuOption AddOption(Pawn pawn, LocalTargetInfo target, string keyname, JobDef jobdef)
    {
      return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(keyname.Translate(), delegate ()
      {
        pawn.jobs.TryTakeOrderedJob(new Job(jobdef, target.Pawn));
      }, MenuOptionPriority.High), pawn, target);
    }
  }
}