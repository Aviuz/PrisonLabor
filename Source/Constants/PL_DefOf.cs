using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Constants
{
    [DefOf]
    public static class PL_DefOf
    {
        public static PrisonerInteractionModeDef PrisonLabor_workOption;
        public static PrisonerInteractionModeDef PrisonLabor_workAndRecruitOption;

        [MayRequireIdeology]
        public static PrisonerInteractionModeDef PrisonLabor_workAndConvertOption;

        [MayRequireIdeology]
        public static PrisonerInteractionModeDef PrisonLabor_workAndEnslaveOption;

        [MayRequireBiotech]
        public static PrisonerInteractionModeDef PrisonLabor_workAndBloodfeedOption;

        [MayRequireBiotech]
        public static PrisonerInteractionModeDef PrisonLabor_workAndHemogenFarmOption;

        public static WorkTypeDef PrisonLabor_Jailor;

        public static NeedDef PrisonLabor_Motivation;
        public static NeedDef PrisonLabor_Treatment;

        public static HediffDef PrisonLabor_RemovedHandscuffs;
        public static HediffDef PrisonLabor_RemovedLegscuffs;

        public static JobDef PrisonLabor_HandlePrisonersHandChain;
        public static JobDef PrisonLabor_HandlePrisonersLegChain;
    }
}
