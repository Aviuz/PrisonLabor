using System;
using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Core.Components;
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

        public static void CleanUp(Pawn pawn)
        {
            var comp = pawn.TryGetComp<PrisonerComp>();

            if (comp == null)
                return;

            if (Tracked.index.ContainsKey(comp.id))
                Tracked.index[comp.id] = -1;

            foreach (int roomId in Tracked.Wardens.Keys)
                if (Tracked.Wardens[roomId].Contains(comp.id))
                    Tracked.Wardens[roomId].Remove(comp.id);

            foreach (int roomId in Tracked.Prisoners.Keys)
                if (Tracked.Prisoners[roomId].Contains(comp.id))
                    Tracked.Prisoners[roomId].Remove(comp.id);
        }
    }


    [StaticConstructorOnStartup]
    public static class PrisonerRegistery
    {
        private static IEnumerable<ThingDef> DefPawns;

        private static ThinkTreeDef DefStupidPawns;

        static PrisonerRegistery()
        {
            PrisonerRegistery.Prepare();
            foreach (ThingDef defOfPawn in DefPawns)
            {
                if (defOfPawn.comps == null)
                    defOfPawn.comps = new List<CompProperties>();
                defOfPawn.comps.Add(new PrisonerProperties());
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
        }
    }
}
