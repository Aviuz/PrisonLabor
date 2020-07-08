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

        private List<JobDef> jobHistory = new List<JobDef>();

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

            var yOffset = 10f;
            Widgets.Label(new Rect(10, yOffset, 200, 35), "Wardens in the room " + Tracked.Wardens[roomID].Count);
            yOffset += 35;

            Widgets.Label(new Rect(10, yOffset, 200, 35), "Prisoners in the room " + Tracked.Prisoners[roomID].Count);
            yOffset += 35;

            // A line divider
            Widgets.DrawLine(new Vector2(10, yOffset), new Vector2(200, yOffset), color: Color.white, 2f);
            yOffset += 5;

            if (jobHistory.Count != 0)
            {
                yOffset += 10;

                if (((Pawn)comp.parent).CurJob != null)
                {
                    if (((Pawn)comp.parent).CurJobDef != jobHistory.Last())
                        jobHistory.Add(((Pawn)comp.parent).CurJobDef);
                }

                Widgets.Label(new Rect(10, yOffset, 200, 20), "Pawn cur JobDef " + jobHistory.Last());
                yOffset += 20;

                for (int i = 0; i < jobHistory.Count - 1; i++)
                {
                    Widgets.Label(new Rect(10, yOffset, 200, 20), -(i + 1) + " " + jobHistory[jobHistory.Count - i - 2]);
                    yOffset += 20;
                }

                if (jobHistory.Count >= 5) { jobHistory.RemoveRange(5, jobHistory.Count - 5); }

                yOffset += 10;

                // A line divider
                Widgets.DrawLine(new Vector2(10, yOffset), new Vector2(200, yOffset), color: Color.white, 2f);
                yOffset += 5;
            }
            else
            {
                if (((Pawn)comp.parent).CurJob != null)
                    jobHistory.Add(((Pawn)comp.parent).CurJobDef);
            }
        }

        private static Vector2 nodeScrollPosition = Vector2.zero;
        private static readonly float compElementHeight = 30;
    }
}
