using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Harmony
{
    [HarmonyPatch(typeof(PawnColumnWorker_Label))]
    [HarmonyPatch("DoCell")]
    [HarmonyPatch(new[] {typeof(Rect), typeof(Pawn), typeof(PawnTable)})]
    //(Rect rect, Pawn pawn, PawnTable table)
    internal class ChangeWorkTabPrisonerLabelColor
    {
        private static void Prefix(Rect rect, Pawn pawn, PawnTable table)
        {
            if (pawn.IsPrisonerOfColony)
                GUI.color = new Color32(0xB8, 0x9C, 0x73, 0xFF); // Color32(R,G,B,A), here is prisoner color
        }
    }
}