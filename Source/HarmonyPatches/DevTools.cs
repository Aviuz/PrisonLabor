using HarmonyLib;
using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches
{
    /// <summary>
    /// This patch is adding Prison Labor dev tools
    /// </summary>
    public static class DevTools
    {
        private static bool logEscapeUtilityEnabled;

        [DebugAction("Prison Labor Tools", "Motivation +10%", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void MotivationUp(Pawn p)
        {
            if (p.needs.TryGetNeed<Need_Motivation>() != null)
            {
                OffsetNeed(p, Need_Motivation.Def, 0.1f);
            }
        }

        [DebugAction("Prison Labor Tools", "Motivation -10%", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void MotivationDown(Pawn p)
        {
            if (p.needs.TryGetNeed<Need_Motivation>() != null)
            {
                OffsetNeed(p, Need_Motivation.Def, -0.1f);
            }
        }

        [DebugAction("Prison Labor Tools", "Treatment +10%", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void Treatmentup(Pawn p)
        {
            if (p.needs.TryGetNeed<Need_Treatment>() != null)
            {
                OffsetNeed(p, Need_Treatment.Def, 0.1f);
            }
        }

        [DebugAction("Prison Labor Tools", "Treatment -10%", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void TreatmentDown(Pawn p)
        {
            if (p.needs.TryGetNeed<Need_Treatment>() != null)
            {
                OffsetNeed(p, Need_Treatment.Def, -0.1f);
            }
        }

        [DebugAction("Prison Labor Tools", "Turn into prisoner", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void MakePrisoner(Pawn p)
        {
            p.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Prisoner);
        }


        [DebugAction("Prison Labor Tools", "Set free", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        public static void SetFree(Pawn p)
        {
            p.guest.SetGuestStatus(null, GuestStatus.Guest);
        }


        [DebugAction("Prison Labor Tools", "Toggle logging escape utility", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.Playing)]
        public static void ToggleLogging()
        {
            logEscapeUtilityEnabled = !logEscapeUtilityEnabled;
        }

        #region Utilities
        static void OffsetNeed(Pawn pawn, NeedDef nd, float offsetPct)
        {
            if (pawn != null)
            {
                Need need = pawn.needs.TryGetNeed(nd);
                if (need != null)
                {
                    need.CurLevel += offsetPct * need.MaxLevel;
                    pawn.Drawer.Notify_DebugAffected();
                }
            }
        }
        #endregion
    }
}
