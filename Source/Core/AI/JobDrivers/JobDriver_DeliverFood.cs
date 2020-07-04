using System;
using System.Collections.Generic;
using RimWorld;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    public class JobDriver_JailorDeliverFood : JobDriver_FoodDeliver
    {
        public override RandomSocialMode DesiredSocialMode()
        {
            return base.DesiredSocialMode();
        }

        public override string GetReport()
        {
            return base.GetReport();
        }

        public override bool IsContinuation(Job j)
        {
            return base.IsContinuation(j);
        }

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            return base.MakeNewToils();
        }
    }
}
