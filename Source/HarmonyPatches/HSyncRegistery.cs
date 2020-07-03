using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using HarmonyLib;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;


namespace PrisonLabor.HarmonyPatches
{
    public static class Patch_Sync
    {
        #region Baisc

        // TODO: usage
        // Called at the start of the game loop.
        public static void OnStart()
        {

        }

        // TODO: usage
        // Called at the end of the game loop.
        public static void OnEnd()
        {
            if (Tracked.remove.Count > 0)
            {
                for (int i = 0; i < Tracked.remove.Count; i++)
                {
                    var comp = Tracked.remove[i].GetComp<PrisonerComp>();
                    if (comp != null)
                        Tracked.remove[i].AllComps.Remove(comp);
                }
                Tracked.remove.Clear();
            }
        }

        #endregion Baisc 
    }
}
