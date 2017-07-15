using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

//Work in progress 

namespace RimWorldd
{
    public static class PrisonerInteractionModeUtility
    {
        public static string GetLabel(this PrisonerInteractionModeDef mode)
        {
            if (mode == PrisonerInteractionModeDefOf.NoInteraction)
            {
                return "PrisonerNoInteraction".Translate();
            }
            if (mode == PrisonerInteractionModeDefOf.Chat)
            {
                return "PrisonerFriendlyChat".Translate();
            }
            /*
            if (mode == PrisonerInteractionModeDefOf.Work)
            {
                return "Work";
            }
            */
            if (mode == PrisonerInteractionModeDefOf.AttemptRecruit)
            {
                return "PrisonerAttemptRecruit".Translate();
            }
            if (mode == PrisonerInteractionModeDefOf.Release)
            {
                return "PrisonerRelease".Translate();
            }
            if (mode == PrisonerInteractionModeDefOf.Execution)
            {
                return "PrisonerExecution".Translate();
            }
            return "Mode needs label";
        }
    }
}
