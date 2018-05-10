using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor_Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            var pawn1 = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist);
            var pawn2 = PawnGenerator.GeneratePawn(PawnKindDefOf.Colonist);
            var job = new Job(JobDefOf.PrisonerAttemptRecruit, new LocalTargetInfo(pawn1));
            var jobdriver = job.MakeDriver(pawn2);
            ToilLister.PrintToils(jobdriver);
        }
    }
}
