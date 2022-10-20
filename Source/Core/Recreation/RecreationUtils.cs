﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using PrisonLabor.Core.Needs;

namespace PrisonLabor.Core.Recreation
{
    public class RecreationUtils
    {
        public static bool PrisonerJoyTickCheckEnd(Pawn pawn, JoyTickFullJoyAction fullJoyAction = JoyTickFullJoyAction.EndJob, float extraJoyGainFactor = 1f, Building joySource = null)
        {
            Job curJob = pawn.CurJob;
            if (curJob.def.joyKind == null)
            {
                Log.Warning("This method can only be called for jobs with joyKind.");
                return false;
            }
            if (joySource != null)
            {
                if (joySource.def.building.joyKind != null && pawn.CurJob.def.joyKind != joySource.def.building.joyKind)
                {
                    Log.ErrorOnce("Joy source joyKind and jobDef.joyKind are not the same. building=" + joySource.ToStringSafe() + ", jobDef=" + pawn.CurJob.def.ToStringSafe(), joySource.thingIDNumber ^ 0x343FD5CC);
                }
                extraJoyGainFactor *= joySource.GetStatValue(StatDefOf.JoyGainFactor);
            }
            Need need = GetPrisonerNeed(pawn);
            if (need == null && !curJob.doUntilGatheringEnded)
            {
                pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);
                return false;
            }

            if (pawn.needs.joy != null && curJob.doUntilGatheringEnded)
            {
                //if somehow prisoner has joy need by other mods.
                pawn.needs.joy?.GainJoy(extraJoyGainFactor * curJob.def.joyGainRate * 0.36f / 2500f, curJob.def.joyKind);
            }


            if (curJob.def.joySkill != null)
            {
                pawn.skills.GetSkill(curJob.def.joySkill).Learn(curJob.def.joyXpPerTick);
            }
            if (!curJob.ignoreJoyTimeAssignment && !pawn.GetTimeAssignment().allowJoy && !curJob.doUntilGatheringEnded)
            {
                pawn.jobs.curDriver.EndJobWith(JobCondition.InterruptForced);
                return true;
            }
            if (need.CurLevel > 0.9999f && !curJob.doUntilGatheringEnded)
            {
                switch (fullJoyAction)
                {
                    case JoyTickFullJoyAction.EndJob:
                        pawn.jobs.curDriver.EndJobWith(JobCondition.Succeeded);
                        return true;
                    case JoyTickFullJoyAction.GoToNextToil:
                        pawn.jobs.curDriver.ReadyForNextToil();
                        return true;
                }
            }
            return false;
        }

        private static Need GetPrisonerNeed(Pawn pawn)
        {
            Need need = pawn.needs.TryGetNeed<Need_Treatment>();
            if (need == null)
            {
                need = pawn.needs.TryGetNeed<Need_Joy>();
            }
            return need;
        }
    }
}