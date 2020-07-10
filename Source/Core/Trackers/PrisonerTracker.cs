using System;
using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Core.Components;
using RimWorld;
using Verse;

namespace PrisonLabor.Core.Trackers
{
    public static class Tracked
    {
        public static readonly Object LOCK_WARDEN = new Object();

        public static Dictionary<int, List<int>> Wardens = new Dictionary<int, List<int>>();
        public static Dictionary<int, List<int>> Prisoners = new Dictionary<int, List<int>>();

        public static Dictionary<int, int> index = new Dictionary<int, int>();
        public static Dictionary<int, ThingComp> pawnComps = new Dictionary<int, ThingComp>();
    }

    // TODO Move
    // Used to inject prisonerComponent 
    [StaticConstructorOnStartup]
    public static class PrisonerRegistery
    {
        private static IEnumerable<ThingDef> DefPawns;

        private static ThinkTreeDef DefStupidPawns;

        static PrisonerRegistery()
        {
            PrisonerRegistery.Prepare();

            var allThings = DefDatabase<ThingDef>.AllDefs;

            // TODO Move 
            // Adding the PrisonerComp selectivly menu
            foreach (var def in allThings)
            {
                if (!(def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_Needs) ?? false) ||
                    (def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_Training) ?? false) ||
                    !(def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_Gear) ?? false) ||
                    (def.comps?.Any(comp => comp is PrisonerProperties) ?? false))
                    continue;

                if (def.comps == null)
                    def.comps = new List<CompProperties>();

                Log.Message("def:" + def.defName);
                def.comps.Add(new PrisonerProperties());
            }
        }

        static void Prepare()
        {
            DefStupidPawns = DefDatabase<ThinkTreeDef>.GetNamedSilentFail("Zombie");

            DefPawns = (
                    from def in DefDatabase<ThingDef>.AllDefs
                    where def.race?.intelligence == Intelligence.Humanlike
                    && !def.defName.Contains("AIPawn") && !def.defName.Contains("Robot")
                    && !def.defName.Contains("ChjDroid") && !def.defName.Contains("ChjBattleDroid")
                    && (DefStupidPawns == null || def.race.thinkTreeMain != DefStupidPawns)
                    select def
                );

            foreach (ThingDef def in DefPawns)
            {
                if (def.comps == null)
                    def.comps = new List<CompProperties>();
                def.comps.Add(new PrisonerProperties());
            }
        }
    }
}
