using PrisonLabor.Core.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.ThinkNodes
{
    class ThinkNode_IsMotivated : ThinkNode_Conditional
    {
        protected override bool Satisfied(Pawn pawn)
        {
            bool motivatedPrisoner = pawn.IsPrisonerOfColony && pawn.IsMotivated();
            if (motivatedPrisoner && pawn.drugs == null)
            {
                //Migration from older version of PL
                pawn.drugs = new Pawn_DrugPolicyTracker(pawn);
            }
            return motivatedPrisoner;
        }
    }
}
