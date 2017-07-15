using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

//Work in progress

namespace RimWorldd
{
    [DefOf]
    public static class PrisonerInteractionModeDefOf
    {
        public static PrisonerInteractionModeDef NoInteraction;

        public static PrisonerInteractionModeDef Chat;

        //public static PrisonerInteractionModeDef Work;

        public static PrisonerInteractionModeDef AttemptRecruit;

        public static PrisonerInteractionModeDef Release;

        public static PrisonerInteractionModeDef Execution;
    }
}
