using HarmonyLib;
using Multiplayer.API;
using PrisonLabor.Core.BaseClasses;
using PrisonLabor.Core.BillAssignation;
using PrisonLabor.Core.MainButton_Window;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Verse;

namespace PrisonLabor.CompatibilityPatches
{
    [StaticConstructorOnStartup]
    public static class Multiplayer
    {
        static Multiplayer()
        {
            if (!MP.enabled) return;
            
            var harmony = new Harmony("Harmony_PrisonLabor_MPCompat");


            MP.RegisterSyncMethod(typeof(SimpleTimer), nameof(SimpleTimer.Start));
            MP.RegisterSyncMethod(typeof(SimpleTimer), nameof(SimpleTimer.Stop));
            MP.RegisterSyncMethod(typeof(SimpleTimer), nameof(SimpleTimer.Reset));
            MP.RegisterSyncMethod(typeof(SimpleTimer), nameof(SimpleTimer.Tick));
            MP.RegisterSyncMethod(typeof(SimpleTimer), nameof(SimpleTimer.ResetAndStop));

            MP.RegisterSyncMethod(typeof(BillAssignationUtility), nameof(BillAssignationUtility.SetFor));
            //Remove is synced

            MP.RegisterSyncMethod(typeof(ColumnWorker_HasHandscuffs), nameof(ColumnWorker_HasHandscuffs.UpdateTracker));
            MP.RegisterSyncMethod(typeof(ColumnWorker_HasLegcuffs), nameof(ColumnWorker_HasHandscuffs.UpdateTracker));

            //Unable to sync Manual Priority Checkbox in Prisoner Tab, though changes to the one in Work Tab is synced to Prisoner Tab
            
            //MP.RegisterSyncField(AccessTools.Field(typeof(CuffsTracker), "legscuffTracker"));
            //harmony.Patch(AccessTools.Method(typeof(MainTabWindow_Labor), "DoManualPrioritiesCheckbox"), 
            //prefix: new HarmonyMethod(typeof(Multiplayer), "ManualPriorityPrefix"),
            //postfix: new HarmonyMethod(typeof(Multiplayer), "ManualPriorityPostfix"));


            MP.RegisterSyncMethod(typeof(ColumnWorker_Interaction), nameof(ColumnWorker_Interaction.SetInteractionMode)).CancelIfAnyArgNull();
            MP.RegisterSyncMethod(typeof(ColumnWorker_Resocialization), nameof(ColumnWorker_Resocialization.ConvertPrisoner)).CancelIfAnyArgNull();

            MP.RegisterSyncMethod(typeof(ArrestUtility), nameof(ArrestUtility.ArrestPrisoner)).SetContext(SyncContext.MapSelected);
            MP.RegisterSyncMethod(typeof(ArrestUtility), nameof(ArrestUtility.TakePrisonerToBed)).SetContext(SyncContext.MapSelected);
        }
    
        /*
        private static void ManualPriorityPrefix(Rect globalRect)
        {
            if (MP.IsInMultiplayer)
            {
                MP.WatchBegin();
                MP.Watch("Verse.Current/Game/playSettings/useWorkPriorities");
            }
        }

        private static void ManualPriorityPostfix()
        {
            MP.WatchEnd();
        }
        */
    }
}
