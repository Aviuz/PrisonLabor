using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Therapy
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Harmony_PrisonLabor_Therapy");
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[PL] Therapy patched");
            }
            catch (Exception e)
            {
                Log.Error($"[PL] Patches for Therapy failed: {e}");
            }
        }
    }
}
