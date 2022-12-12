using HarmonyLib;
using PrisonLabor.Core;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{

  [HarmonyPatch(typeof(RestUtility), "FindBedFor", new Type[] { typeof(Pawn), typeof(Pawn), typeof(bool), typeof(bool), typeof(GuestStatus) })]
  class Patch_RestUtility
  {
    //Don't try to take wounded to unreachable bed
    static Building_Bed Postfix(Building_Bed __result, Pawn sleeper, Pawn traveler, bool checkSocialProperness, bool ignoreOtherReservations, GuestStatus? guestStatus)
    {
      if (__result != null && traveler.IsPrisonerOfColony && !traveler.CanReach(__result, PathEndMode.ClosestTouch, traveler.NormalMaxDanger()))
      {
        return null;
      }
      return __result;
    }
  }
}
