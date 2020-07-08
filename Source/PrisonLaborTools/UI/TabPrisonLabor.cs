using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Trackers;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace PrisonLaborDebug.UI
{
    public class ITab_Pawn_PrisonLabor : ITabsPlus
    {
        public override bool IsVisible { get => (SelPawn.IsColonist || SelPawn.IsPrisonerOfColony) && Prefs.DevMode; }

        public ITab_Pawn_PrisonLabor()
        {
            this.size = new Vector2(500f, 470f);

            this.labelKey = "TabPrisonLabor";
            this.tutorTag = "PrisonLabor";
        }

        public override void TabUpdate()
        {
            base.TabUpdate();
        }

        public override void FillTab()
        {
            PrisonerComp comp = SelPawn.TryGetComp<PrisonerComp>();

            if (comp == null)
                return;

            var roomID = Tracked.index[comp.id];

            if (roomID == -1)
                return;

            Widgets.Label(new Rect(10, 10, 100, 35), "Wardens in the room " + Tracked.Wardens[roomID].Count);

            Widgets.Label(new Rect(10, 45, 100, 35), "Prisoners in the room " + Tracked.Prisoners[roomID].Count);
        }

        private static Vector2 nodeScrollPosition = Vector2.zero;
        private static readonly float compElementHeight = 30;
    }
}
