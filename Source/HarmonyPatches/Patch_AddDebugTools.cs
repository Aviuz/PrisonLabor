using Harmony;
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
    static class Patch_AddDebugTools
    {
        static void Postfix(Dialog_DebugActionsMenu __instance)
        {
            var menu = __instance;
            menu.DoLabel("Prison Labor Tools:");
            menu.DebugToolMapForPawns("Tool: Increase treatment by 10%", delegate (Pawn p)
            {
                if (p.needs.TryGetNeed<Need_Treatment>() != null)
                {
                    p.needs.TryGetNeed<Need_Treatment>().CurLevel += 0.1f;
                }
            });
            menu.DebugToolMapForPawns("Tool: Decrease treatment by 10%", delegate (Pawn p)
            {
                if (p.needs.TryGetNeed<Need_Treatment>() != null)
                {
                    p.needs.TryGetNeed<Need_Treatment>().CurLevel -= 0.1f;
                }
            });
        }

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
    }
}
