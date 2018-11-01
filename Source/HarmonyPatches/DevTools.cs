using Harmony;
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
    [HarmonyPatch(typeof(Dialog_DebugActionsMenu), "DoListingItems_MapTools")]
    static class DevTools
    {
        static void Postfix(Dialog_DebugActionsMenu __instance)
        {
            var menu = __instance;
            menu.DoLabel("Prison Labor Tools:");

            // Increase motivation
            menu.DebugToolMapForPawns("Tool: Motivation +10%", delegate (Pawn p)
            {
                if (p.needs.TryGetNeed<Need_Motivation>() != null)
                {
                    OffsetNeed(p, Need_Motivation.Def, 0.1f);
                }
            });
            // Decrease motivation
            menu.DebugToolMapForPawns("Tool: Motivation -10%", delegate (Pawn p)
            {
                if (p.needs.TryGetNeed<Need_Motivation>() != null)
                {
                    OffsetNeed(p, Need_Motivation.Def, -0.1f);
                }
            });
            // Increase treatment
            menu.DebugToolMapForPawns("Tool: Treatment +10%", delegate (Pawn p)
            {
                if (p.needs.TryGetNeed<Need_Treatment>() != null)
                {
                    OffsetNeed(p, Need_Treatment.Def, 0.1f);
                }
            });
            // Decrease treatment
            menu.DebugToolMapForPawns("Tool: Treatment -10%", delegate (Pawn p)
            {
                if (p.needs.TryGetNeed<Need_Treatment>() != null)
                {
                    OffsetNeed(p, Need_Treatment.Def, -0.1f);
                }
            });
            // Turn into prisoner
            menu.DebugToolMapForPawns("Tool: Turn into prisoner", delegate (Pawn p)
            {
                p.guest.SetGuestStatus(Faction.OfPlayer, true);
            });
            // Set free
            menu.DebugToolMapForPawns("Tool: Set free", delegate (Pawn p)
            {
                p.guest.SetGuestStatus(null, false);
            });
        }

        #region Utilities
        static void DoLabel(this Dialog_DebugActionsMenu instance, string label)
        {
            var method = typeof(Dialog_DebugActionsMenu).GetMethod("DoLabel", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { label });
        }

        static void DebugAction(this Dialog_DebugActionsMenu instance, string label, Action action)
        {
            var method = typeof(Dialog_DebugActionsMenu).GetMethod("DebugAction", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { label, action });
        }

        static void DebugToolMapForPawns(this Dialog_DebugActionsMenu instance, string label, Action<Pawn> pawnAction)
        {
            var method = typeof(Dialog_DebugActionsMenu).GetMethod("DebugToolMapForPawns", BindingFlags.Instance | BindingFlags.NonPublic);
            method.Invoke(instance, new object[] { label, pawnAction });
        }

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
