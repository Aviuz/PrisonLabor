using System;
using PrisonLabor.Core.Other;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.GUI_Components.FloatMenuOptions
{
  public class FloatMenuOptionProvider_TakePrisonerToBed : FloatMenuOptionProvider
  {
    protected override bool Drafted => true;

    protected override bool Undrafted => true;

    protected override bool Multiselect => false;

    protected override bool MechanoidCanDo => true;

    protected override bool RequiresManipulation => true;

    public override bool TargetPawnValid(Pawn pawn, FloatMenuContext context)
    {
      var selectedPawn = context.FirstSelectedPawn;
      return base.TargetPawnValid(pawn, context) && ArrestUtility.CanBeTakenToBed(pawn, selectedPawn) && !pawn.Downed;
    }

    protected override FloatMenuOption GetSingleOptionFor(Pawn clickedPawn, FloatMenuContext context)
    {
      var selectedPawn = context.FirstSelectedPawn;
      if (!selectedPawn.CanReach(clickedPawn, PathEndMode.OnCell, Danger.Deadly))
        return new FloatMenuOption("PrisonLabor_CannotTakeToBed".Translate() + " (" + "NoPath".Translate() + ")", null);

      Action action = delegate { ArrestUtility.TakePrisonerToBed(clickedPawn, selectedPawn); };
      string label = "PrisonLabor_TakingToBed".Translate(clickedPawn.LabelCap);
      var priority = MenuOptionPriority.High;
      return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, priority, null, clickedPawn),
        selectedPawn, clickedPawn);
    }
  }
}