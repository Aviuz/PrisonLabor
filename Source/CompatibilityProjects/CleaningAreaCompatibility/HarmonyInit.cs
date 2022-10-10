using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CleaningAreaCompatibility
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Harmony_PrisonLabor_CleaningArea");
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[PL] CleaningArea patched");
            }
            catch (Exception e)
            {
                Log.Error($"[PL] Patches for CleaningArea failed: {e}");
            }
        }
    }
}
