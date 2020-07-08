using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using HarmonyLib;
using Verse;
using Verse.AI.Group;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.CompatibilityPatches;

namespace PrisonLabor.HarmonyPatches.PathFinding
{
    public static class DoorRegistery
    {
        public static Object LOCK = new Object();

        public static readonly HashSet<int> Controls = new HashSet<int>();

        public static void Update(Building_Door door)
        {
            if (door.Map.areaManager.Get<Area_Labor>().ActiveCells.Contains(door.Position))
                DoorRegistery.Controls.Add(door.thingIDNumber);
            else
                DoorRegistery.Controls.RemoveWhere(x => x == door.thingIDNumber);
        }
    }

    [HarmonyPatch(typeof(Building_Door))]
    [HarmonyPatch(nameof(Building_Door.PawnCanOpen))]
    public static class Patch_InjectLockCheck
    {
        public static bool Prefix(Building_Door __instance, Pawn p, ref bool __result)
        {
            if (Locks.Found || LocksDoorExpanded.Found)
                return true;

            if (p.IsPrisoner)
            {
                __result = DoorRegistery.Controls.Contains(__instance.thingIDNumber); return false;
            }
            else
            {
                return true;
            }
        }
    }

    [HarmonyPatch(typeof(Building_Door))]
    [HarmonyPatch(nameof(Building_Door.Tick))]
    public static class Patch_InjectUpdate
    {
        public static void Postfix(Building_Door __instance)
        {
            if (!Gen.IsHashIntervalTick(__instance, 512)) { return; }

            if (Locks.Found || LocksDoorExpanded.Found) { return; }

            DoorRegistery.Update(__instance);
        }
    }
}
