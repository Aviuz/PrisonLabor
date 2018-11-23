using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    [DefOf]
    public static class PrisonLaborDefOf
    {
        public static PrisonerInteractionModeDef PrisonLabor_workOption;
        public static PrisonerInteractionModeDef PrisonLabor_workAndRecruitOption;

        public static WorkTypeDef PrisonLabor_Jailor;

        public static NeedDef PrisonLabor_Motivation;
        public static NeedDef PrisonLabor_Treatment;
    }
}
