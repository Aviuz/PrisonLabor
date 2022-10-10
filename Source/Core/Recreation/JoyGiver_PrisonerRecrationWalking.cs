using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.Recreation
{
    public class JoyGiver_PrisonerRecrationWalking : JoyGiver
    {
        protected float wanderRadius;

        protected Func<Pawn, IntVec3, IntVec3, bool> wanderDestValidator;

        protected IntRange ticksBetweenWandersRange = new IntRange(20, 100);

        protected LocomotionUrgency locomotionUrgency = LocomotionUrgency.Walk;

        protected LocomotionUrgency? locomotionUrgencyOutsideRadius;

        protected Danger maxDanger = Danger.None;

        protected int expiryInterval = -1;


        public JoyGiver_PrisonerRecrationWalking()
        {
            wanderRadius = 5f;
            locomotionUrgency = LocomotionUrgency.Walk;
            ticksBetweenWandersRange = new IntRange(125, 200);
            wanderDestValidator = (Pawn pawn, IntVec3 loc, IntVec3 root) => pawn.CanReach(loc, PathEndMode.OnCell, maxDanger);
        }

        public override Job TryGiveJob(Pawn pawn)
        {
            if (!pawn.IsPrisonerOfColony)
            {
                DebugLogger.info($"[PL] Pawn {pawn.NameShortColored} is not prisoner {typeof(JoyGiver_PrisonerRecrationWalking).Name}. Return null");
                return null;
            }

            bool flag = pawn.CurJob != null && pawn.CurJob.def.defName == def.defName;
            bool nextMoveOrderIsWait = pawn.mindState.nextMoveOrderIsWait;
            if (!flag)
            {
                pawn.mindState.nextMoveOrderIsWait = !pawn.mindState.nextMoveOrderIsWait;
            }
            if (nextMoveOrderIsWait && !flag)
            {
                Job job = JobMaker.MakeJob(JobDefOf.Wait_Wander);
                job.expiryInterval = ticksBetweenWandersRange.RandomInRange;
                return job;
            }
            IntVec3 exactWanderDest = GetExactWanderDest(pawn);
            if (!exactWanderDest.IsValid)
            {
                pawn.mindState.nextMoveOrderIsWait = false;
                DebugLogger.info($"[PL] Pawn {pawn.NameShortColored} has not valid dest in {typeof(JoyGiver_PrisonerRecrationWalking).Name}. Return null");
                return null;
            }
            LocomotionUrgency value = locomotionUrgency;
            if (locomotionUrgencyOutsideRadius.HasValue && !pawn.Position.InHorDistOf(GetWanderRoot(pawn), wanderRadius))
            {
                value = locomotionUrgencyOutsideRadius.Value;
            }
            Job job2 = JobMaker.MakeJob(def.jobDef, exactWanderDest);
            job2.locomotionUrgency = value;
            job2.expiryInterval = expiryInterval;
            job2.checkOverrideOnExpire = true;
            return job2;
        }


        private IntVec3 GetExactWanderDest(Pawn pawn)
        {
            IntVec3 wanderRoot = GetWanderRoot(pawn);
            float value = wanderRadius;
            PawnDuty duty = pawn.mindState.duty;
            if (duty != null && duty.wanderRadius.HasValue)
            {
                value = duty.wanderRadius.Value;
            }
            return RCellFinder.RandomWanderDestFor(pawn, wanderRoot, value, wanderDestValidator, PawnUtility.ResolveMaxDanger(pawn, maxDanger));
        }

        public override bool CanBeGivenTo(Pawn pawn)
        {
            return pawn.IsPrisonerOfColony && base.CanBeGivenTo(pawn);
        }

        private IntVec3 GetWanderRoot(Pawn pawn)
        {
            return pawn.Position;
        }
    }
}
