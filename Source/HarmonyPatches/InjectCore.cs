using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    //[HarmonyPatch(typeof(Root_Play))]
    //[HarmonyPatch(nameof(Root_Play.Update))]
    //public static class Pathc_InjectSyncCore
    //{
    //    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //    {
    //        var codes = new List<CodeInstruction>(instructions);
    //
    //        var mStart = typeof(Patch_Sync).GetMethod(nameof(Patch_Sync.OnStart));
    //        var mEnd = typeof(Patch_Sync).GetMethod(nameof(Patch_Sync.OnEnd));
    //
    //        var mShip = typeof(ShipCountdown).GetMethod(nameof(ShipCountdown.ShipCountdownUpdate));
    //        var mMuisc = typeof(MusicManagerPlay).GetMethod(nameof(MusicManagerPlay.MusicUpdate));
    //
    //        for (int i = 0; i < codes.Count; i++)
    //        {
    //            //if(mCount == codes)
    //            if (codes[i].opcode == OpCodes.Call)
    //            {
    //                if (codes[i].operand as MethodInfo == mShip)
    //                    yield return new CodeInstruction(OpCodes.Call, mStart);
    //
    //                yield return codes[i];
    //            }
    //            else if (codes[i].opcode == OpCodes.Callvirt)
    //            {
    //                yield return codes[i];
    //
    //                if (codes[i].operand as MethodInfo == mMuisc)
    //                    yield return new CodeInstruction(OpCodes.Call, mEnd);
    //            }
    //            else
    //            {
    //                yield return codes[i];
    //            }
    //        }
    //    }
    //}
}
