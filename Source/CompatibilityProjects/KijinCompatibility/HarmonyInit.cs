using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace KijinCompatibility
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Harmony_PrisonLabor_Kijin");
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[PL] Kijin Race 3.0 patched");
            }
            catch (Exception e)
            {
                Log.Error($"[PL] Patches for Kijin Race 3.0 failed: {e}");
            }
        }
    }
}
