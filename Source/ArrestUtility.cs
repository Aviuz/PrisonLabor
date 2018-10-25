using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public static class ArrestUtility
    {
        public static void AddArrestOrder(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
            //TODO this is some copy-pasted example, refactor it so it should add arrest option
            IntVec3 c = IntVec3.FromVector3(clickPos);
            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (LocalTargetInfo current in GenUI.TargetsAt(clickPos, ForArrest(pawn), true))
                {
                    LocalTargetInfo dest = current;
                    bool flag = dest.HasThing && dest.Thing is Pawn && ((Pawn)dest.Thing).IsWildMan();
                    if (!pawn.Drafted || flag)
                    {
                        if (!pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
                        {
                            opts.Add(new FloatMenuOption("CannotArrest".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                        else
                        {
                            Pawn pTarg = (Pawn)dest.Thing;
                            Action action = delegate
                            {
                                Building_Bed building_Bed = RestUtility.FindBedFor(pTarg, pawn, true, false, false);
                                if (building_Bed == null)
                                {
                                    building_Bed = RestUtility.FindBedFor(pTarg, pawn, true, false, true);
                                }
                                if (building_Bed == null)
                                {
                                    Messages.Message("CannotArrest".Translate() + ": " + "NoPrisonerBed".Translate(), pTarg, MessageTypeDefOf.RejectInput, false);
                                    return;
                                }
                                Job job = new Job(JobDefOf.Arrest, pTarg, building_Bed);
                                job.count = 1;
                                pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
                                if (pTarg.Faction != null && pTarg.Faction != Faction.OfPlayer && !pTarg.Faction.def.hidden)
                                {
                                    TutorUtility.DoModalDialogIfNotKnown(ConceptDefOf.ArrestingCreatesEnemies);
                                }
                            };
                            string label = "TryToArrest".Translate(dest.Thing.LabelCap, dest.Thing);
                            Action action2 = action;
                            MenuOptionPriority priority = MenuOptionPriority.High;
                            Thing thing = dest.Thing;
                            opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action2, priority, null, thing, 0f, null, null), pawn, pTarg, "ReservedBy"));
                        }
                    }
                }
            }
        }

        public static TargetingParameters ForArrest(Pawn arrester)
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetBuildings = false,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = delegate (TargetInfo targ)
                {
                    if (!targ.HasThing)
                    {
                        return false;
                    }
                    Pawn pawn = targ.Thing as Pawn;
                    return pawn != null && pawn != arrester && CanBeArrestedBy(pawn, arrester) && !pawn.Downed;
                }
            };
        }

        public static bool CanBeArrestedBy(Pawn pawn, Pawn arrester)
        {
            return pawn.RaceProps.Humanlike && pawn.HostileTo(arrester.Faction) && pawn.CurJob.def == JobDefOf.Flee && (!pawn.IsPrisonerOfColony || !pawn.Position.IsInPrisonCell(pawn.Map));
        }
    }
}
