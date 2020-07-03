using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core.Trackers;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Rooms
{
    [StaticConstructorOnStartup]
    public static class Patch_Room
    {
        static Patch_Room()
        {
            var harmony = new Harmony("Harmony_PrisonLabor");

            var mBase = AccessTools.Method(typeof(RoomGroup), nameof(RoomGroup.AddRoom));
            var mPost = AccessTools.Method(typeof(Patch_Room), nameof(Patch_Room.Postfix));

            harmony.Patch(mBase, new HarmonyMethod(mPost));
        }

        public static void Postfix(Room room)
        {
            lock (Tracked.LOCK_WARDEN)
            {
                if (!Tracked.Wardens.Keys.Contains(room.ID))
                    Tracked.Wardens[room.ID] = new List<int>();

                if (!Tracked.Prisoners.Keys.Contains(room.ID))
                    Tracked.Prisoners[room.ID] = new List<int>();
#if DEBUG
                Log.Message("Added Region " + room.ID);
#endif
            }
        }
    }

    [HarmonyPatch(typeof(Room))]
    [HarmonyPatch(nameof(Room.RemoveRegion))]
    public static class Patch_RoomDrefrenced
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = instructions.ToList();

            var mRemoved = typeof(Patch_RoomDrefrenced).GetMethod(nameof(Patch_RoomDrefrenced.Removed));

            for (int i = 0; i < codes.Count - 2; i++)
                yield return codes[i];

            yield return new CodeInstruction(OpCodes.Ldarg_0);
            yield return new CodeInstruction(OpCodes.Callvirt, mRemoved);

            yield return codes[codes.Count - 2];
            yield return codes[codes.Count - 1];
        }

        public static void Removed(Room __instance)
        {
            lock (Tracked.LOCK_WARDEN)
            {
                if (Tracked.Wardens.Keys.Contains(__instance.ID))
                {
                    for (int i = 0; i < Tracked.Wardens[__instance.ID].Count; i++)
                    {
                        var wID = Tracked.Wardens[__instance.ID][i];
                        Tracked.index[wID] = -1;
                    }

                    for (int i = 0; i < Tracked.Prisoners[__instance.ID].Count; i++)
                    {
                        var pID = Tracked.Prisoners[__instance.ID][i];
                        Tracked.index[pID] = -1;
                    }

                    Tracked.Wardens.Remove(__instance.ID);
                }
#if DEBUG
                Log.Message("Removed Region " + __instance.ID);
#endif
            }
        }
    }
}
