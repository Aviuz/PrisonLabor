using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.AI.WorkGivers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_LaborArea
{
    [HarmonyPatch]
    class Patch_Scanner_HasJob
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            return Assembly.GetAssembly(typeof(WorkGiver_Scanner)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(WorkGiver_Scanner)) && myType != typeof(WorkGiver_Supervise) && myType != typeof(WorkGiver_Warden))
            .SelectMany(type => type.GetMethods())
                .Where(method => method.Name.Equals("HasJobOnThing"))
                .Cast<MethodBase>();
        }

        static bool Postfix(bool __result, WorkGiver_Scanner __instance, Pawn pawn, Thing t, bool forced = false)
        {
            if (__result && t != null )
            {
                return PrisonLaborUtility.canWorkHere(t.Position, pawn, __instance.def.workType);
            }
            return __result;
        }
    }
}
