using HarmonyLib;
using PrisonLabor.Core.BillAssignation;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_BillAssignation
{
  [HarmonyPatch(typeof(Bill))]
  [HarmonyPatch("PawnAllowedToStartAnew")]
  public class Bill_StartAnew_Patch
  {
    static bool Postfix(bool __result, Bill __instance, Pawn p)
    {
      if (__result == false && __instance.PawnRestriction == null)
      {
        GroupMode group = BillAssignationUtility.IsFor(__instance);

        if (group == GroupMode.ColonyGroups)
        {
          DebugLogger.debug($"Skiping checking for {__instance.Label}. Pawn: {p.NameShortColored}");
          return __result;
        }

        if (group == GroupMode.ColonyOnly && __instance.SlavesOnly && p.IsSlave)
        {
          return true;
        }
        if (group == GroupMode.SlavesOnly && __instance.SlavesOnly && p.IsSlave)
        {
          return true;
        }

        if (group == GroupMode.CaptiveOnly && __instance.SlavesOnly && p.IsSlave)
        {
          return true;
        }
        if (__instance.recipe.workSkill != null && (p.skills != null || p.IsColonyMech))
        {
          int level = (p.skills != null) ? p.skills.GetSkill(__instance.recipe.workSkill).Level : p.RaceProps.mechFixedSkillLevel;
          if (level < __instance.allowedSkillRange.min)
          {
            JobFailReason.Is("UnderAllowedSkill".Translate(__instance.allowedSkillRange.min), __instance.Label);
            return false;
          }
          if (level > __instance.allowedSkillRange.max)
          {
            JobFailReason.Is("AboveAllowedSkill".Translate(__instance.allowedSkillRange.max), __instance.Label);
            return false;
          }

        }
        if (group == GroupMode.MechanitorOnly)
        {          
          return MechanitorUtility.IsMechanitor(p);
        }
        if (group == GroupMode.ColonyOnly || (group == GroupMode.CaptiveOnly && p.IsPrisoner))
        {
          return true;
        }

      }
      return __result;
    }
  }
}
