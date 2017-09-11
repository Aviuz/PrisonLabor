using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    internal class JobGiver_Joy : ThinkNode_JobGiver
    {
        private DefMap<JoyGiverDef, float> joyGiverChances;


        protected virtual bool CanDoDuringMedicalRest => false;

        public override float GetPriority(Pawn pawn)
        {
            if (pawn.timetable != null && pawn.timetable.CurrentAssignment == TimeAssignmentDefOf.Joy)
                return 10f;
            return 0f;
        }

        protected virtual bool JoyGiverAllowed(JoyGiverDef def)
        {
            return true;
        }

        protected virtual Job TryGiveJobFromJoyGiverDefDirect(JoyGiverDef def, Pawn pawn)
        {
            return def.Worker.TryGiveJob(pawn);
        }

        public override void ResolveReferences()
        {
            joyGiverChances = new DefMap<JoyGiverDef, float>();
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.timetable == null || pawn.timetable.CurrentAssignment != TimeAssignmentDefOf.Joy)
                return null;
            if (pawn.InBed() && HealthAIUtility.ShouldSeekMedicalRest(pawn))
                return null;
            var allDefsListForReading = DefDatabase<JoyGiverDef>.AllDefsListForReading;
            //JoyToleranceSet tolerances = pawn.needs.joy.tolerances;
            for (var i = 0; i < allDefsListForReading.Count; i++)
            {
                var joyGiverDef = allDefsListForReading[i];
                joyGiverChances[joyGiverDef] = 0f;
                if (JoyGiverAllowed(joyGiverDef))
                    if (joyGiverDef.Worker.MissingRequiredCapacity(pawn) == null)
                    {
                        if (joyGiverDef.pctPawnsEverDo < 1f)
                        {
                            Rand.PushState(pawn.thingIDNumber ^ 63216713);
                            if (Rand.Value >= joyGiverDef.pctPawnsEverDo)
                            {
                                Rand.PopState();
                                goto IL_FB;
                            }
                            Rand.PopState();
                        }
                        var num = joyGiverDef.Worker.GetChance(pawn);
                        //float num2 = 1f - tolerances[joyGiverDef.joyKind];
                        //num *= num2 * num2;
                        joyGiverChances[joyGiverDef] = num;
                    }
                IL_FB:
                ;
            }
            for (var j = 0; j < joyGiverChances.Count; j++)
            {
                JoyGiverDef def;
                if (!allDefsListForReading.TryRandomElementByWeight(d => joyGiverChances[d], out def))
                    break;
                var job = TryGiveJobFromJoyGiverDefDirect(def, pawn);
                if (job != null)
                    return job;
                joyGiverChances[def] = 0f;
            }
            return null;
        }
    }
}