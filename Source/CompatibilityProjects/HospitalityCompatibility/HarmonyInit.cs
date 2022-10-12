using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Hospitality
{
    [StaticConstructorOnStartup]
    public static class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Harmony_PrisonLaborHospitality");
            try
            {
                Log.Message($"[PL] Found hospitality mod");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[PL] Hospitality mod patched");
            }
            catch (Exception e)
            {
                Log.Error($"[PL] Hospitality harmony patches failed {e}");
            }
        }
    }
}
