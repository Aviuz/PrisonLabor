using Multiplayer.API;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.Other
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
                        if (dest.Thing is Pawn && (pawn.InSameExtraFaction((Pawn)dest.Thing, ExtraFactionType.HomeFaction, (Quest)null) || pawn.InSameExtraFaction((Pawn)dest.Thing, ExtraFactionType.MiniFaction, (Quest)null)))
                        {
                            opts.Add(new FloatMenuOption((string)("CannotArrest".Translate() + ": " + "SameFaction".Translate((NamedArgument)dest.Thing)), (Action)null, MenuOptionPriority.Default, (Action<Rect>)null, (Thing)null, 0.0f, (Func<Rect, bool>)null, (WorldObject)null, true, 0));
                        }
                        else if (!pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
                        {
                            opts.Add(new FloatMenuOption("CannotArrest".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                        }
                        else
                        {
                            Pawn pTarg = (Pawn)dest.Thing;
                            Action action = delegate
                            {
                                ArrestUtility.ArrestPrisoner(pTarg, pawn);
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
        public static void AddTakeToBedOrder(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {

            if (pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                foreach (LocalTargetInfo current in GenUI.TargetsAt(clickPos, ForEscortToBed(pawn), true))
                {
                    LocalTargetInfo dest = current;


                    if (!pawn.CanReach(dest, PathEndMode.OnCell, Danger.Deadly, false, false, TraverseMode.ByPawn))
                    {
                        opts.Add(new FloatMenuOption("PrisonLabor_CannotTakeToBed".Translate() + " (" + "NoPath".Translate() + ")", null, MenuOptionPriority.Default, null, null, 0f, null, null));
                    }
                    else
                    {
                        Pawn pTarg = (Pawn)dest.Thing;
                        Action action = delegate
                        {
                            ArrestUtility.TakePrisonerToBed(pTarg, pawn);
                        };
                        string label = "PrisonLabor_TakingToBed".Translate(dest.Thing.LabelCap);
                        MenuOptionPriority priority = MenuOptionPriority.High;
                        Thing thing = dest.Thing;
                        opts.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(label, action, priority, null, thing, 0f, null, null), pawn, pTarg, "ReservedBy"));
                    }

                }
            }
        }

        //For Multiplayer Compatibility
        public static void ArrestPrisoner(Pawn pTarg, Pawn pawn)
        {
            Building_Bed building_Bed = FindBed(pTarg, pawn);
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
        }

        public static void TakePrisonerToBed(Pawn pTarg, Pawn pawn)
        {
            Building_Bed building_Bed = FindBed(pTarg, pawn);
            if (building_Bed == null)
            {
                Messages.Message("PrisonLabor_CannotTakeToBed".Translate() + ": " + "NoPrisonerBed".Translate(), pTarg, MessageTypeDefOf.RejectInput, false);
                return;
            }
            Job job = new Job(JobDefOf.EscortPrisonerToBed, pTarg, building_Bed)
            {
                count = 1
            };
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

        private static Building_Bed FindBed(Pawn pTarg, Pawn pawn)
        {
            Building_Bed building_Bed = RestUtility.FindBedFor(pTarg, pawn, false, false, GuestStatus.Prisoner);
            if (building_Bed == null)
            {
                building_Bed = RestUtility.FindBedFor(pTarg, pawn, false, true, GuestStatus.Prisoner);
            }

            return building_Bed;
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

        public static TargetingParameters ForEscortToBed(Pawn arrester)
        {
            return new TargetingParameters
            {
                canTargetPawns = true,
                canTargetHumans = true,
                canTargetBuildings = false,
                mapObjectTargetsMustBeAutoAttackable = false,
                validator = delegate (TargetInfo targ)
                {
                    if (!targ.HasThing)
                    {
                        return false;
                    }
                    return targ.Thing is Pawn pawn && pawn != arrester && CanBeTakenToBed(pawn, arrester) && !pawn.Downed;
                }
            };
        }
        private static bool CanBeTakenToBed(Pawn pawn, Pawn arrester)
        {
            return !IsPawnFleeing(pawn) && pawn.IsPrisonerOfColony && !pawn.Position.IsInPrisonCell(pawn.Map);
        }

        public static bool CanBeArrestedBy(Pawn pawn, Pawn arrester)
        {
            return pawn.RaceProps.Humanlike && pawn.HostileTo(arrester.Faction) && IsPawnFleeing(pawn) && (!pawn.IsPrisonerOfColony || !pawn.Position.IsInPrisonCell(pawn.Map));
        }

        private static bool IsPawnFleeing(Pawn pawn)
        {
            return pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Flee;
        }
    }
}
