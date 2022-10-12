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
    public class JobDriver_PrisonerRecrationWalking : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return true;
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Toil goToil = Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.OnCell);
            goToil.tickAction = delegate
            {
                if (Find.TickManager.TicksGame > startTick + job.def.joyDuration)
                {
                    EndJobWith(JobCondition.Succeeded);
                }
                else
                {
                    JoyUtility.JoyTickCheckEnd(pawn);
                }
            };
            yield return goToil;
        }
    }
}
