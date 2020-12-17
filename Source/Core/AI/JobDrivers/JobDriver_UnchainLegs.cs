using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobDrivers
{
    class JobDriver_UnchainLegs : JobDriver_Unchain
	{     

        protected override IEnumerable<Toil> MakeNewToils()
        {
            return MakeNewToils(PL_DefOf.PrisonLabor_RemovedLegscuffs);
		}
    }
}
