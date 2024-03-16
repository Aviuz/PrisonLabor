using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch(typeof(ChildcareUtility))]
  [HarmonyPatch(new[] { typeof(Pawn), typeof(Pawn) })]
  class Patch_BreastfeedCompatibleFactions
  {
    static readonly MethodInfo expectedMethod = AccessTools.PropertyGetter(typeof(Pawn), nameof(Pawn.Faction));

    [HarmonyPatch("HasBreastfeedCompatibleFactions")]
    static bool Postfix(bool __result, Pawn mom, Pawn baby)
    {
      if (mom.IsPrisonerOfColony || baby.IsPrisonerOfColony)
      {
        return ChildcareUtility.HasBreastfeedCompatibleFactions(PrisonLaborUtility.GetPawnFaction(mom), baby);
      }
      return __result;
    }
  }
}
