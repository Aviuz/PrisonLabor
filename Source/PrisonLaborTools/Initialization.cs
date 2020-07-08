using System;
using System.Collections.Generic;
using System.Linq;
using PrisonLaborDebug.UI;
using RimWorld;
using Verse;

namespace PrisonLaborDebug
{
    [StaticConstructorOnStartup]
    public static class Initialization
    {
        private static IEnumerable<ThingDef> DefPawns;

        private static ThinkTreeDef DefStupidPawns;

        private static List<Type> tabs_types;

        static Initialization()
        {
            Initialization.Prepare();
            Initialization.InitializeUI();
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

        static IEnumerable<Type> GetSubTypes(Type type)
        {
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => type.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
                .Select(x => x);
        }

        static void InitializeUI()
        {
            PreInitializeTabs();
        }

        static void PreInitializeTabs()
        {
            tabs_types = GetSubTypes(typeof(ITabsPlus)).ToList();
            foreach (ThingDef t in DefPawns)
            {
                if (t.inspectorTabsResolved == null)
                    t.inspectorTabsResolved = new List<InspectTabBase>(1);

                foreach (Type tab_type in tabs_types)
                    t.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(tab_type));
            }

            InitializeTabs();
        }

        static void InitializeTabs()
        {
            var allThings = DefDatabase<ThingDef>.AllDefs;

            foreach (var def in allThings)
            {
                if (!(def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_Needs) ?? false) ||
                    (def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_Training) ?? false) ||
                    !(def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_Gear) ?? false) ||
                    (def.inspectorTabsResolved?.Any(tab => tab is ITab_Pawn_PrisonLabor) ?? false))
                    continue;

                if (def.inspectorTabsResolved == null)
                    def.inspectorTabsResolved = new List<InspectTabBase>();
                def.inspectorTabsResolved.Add(new ITab_Pawn_PrisonLabor());

                Log.Message("def:" + def.defName);

            }

        }
    }
}
