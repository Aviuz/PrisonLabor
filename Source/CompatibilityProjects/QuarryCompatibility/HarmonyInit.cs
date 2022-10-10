using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace QuarryCompatibility
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Harmony_PrisonLabor_Quarry");
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[PL] Quarry patched");
            }
            catch (Exception e)
            {
                Log.Error($"[PL] Patches for Quarry failed: {e}");
            }
        }
    }
}
