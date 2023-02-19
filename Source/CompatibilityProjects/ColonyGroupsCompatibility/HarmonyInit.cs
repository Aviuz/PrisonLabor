using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ColonyGroupsCompatibility
{
  [StaticConstructorOnStartup]
  public class HarmonyInit
  {
    static HarmonyInit()
    {
      var harmony = new Harmony("Harmony_PrisonLabor_ColonyGroups");
      try
      {
        harmony.PatchAll(Assembly.GetExecutingAssembly());
        Log.Message("[PL] ColonyGroups patched");
      }
      catch (Exception e)
      {
        Log.Error($"[PL] Patches for ColonyGroups failed: {e}");
      }
    }
  }
}
