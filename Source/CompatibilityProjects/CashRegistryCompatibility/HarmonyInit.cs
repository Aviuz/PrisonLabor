using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace CashRegistryCompatibility
{
    [StaticConstructorOnStartup]
    public class HarmonyInit
    {
        static HarmonyInit()
        {
            var harmony = new Harmony("Harmony_PrisonLabor_CashRegistry");
            try
            {
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                Log.Message("[PL] CashRegistry patched");
            }
            catch (Exception e)
            {
                Log.Error($"[PL] Patches for CashRegistry failed: {e}");
            }
        }
    }
}
