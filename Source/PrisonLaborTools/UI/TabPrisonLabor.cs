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
            var tyOffset = yOffset;

            Widgets.Label(new Rect(10, yOffset, 200, 35), "Wardens in the room " + Tracked.Wardens[roomID].Count);
            yOffset += 35;

            Widgets.Label(new Rect(10, yOffset, 200, 35), "Prisoners in the room " + Tracked.Prisoners[roomID].Count);
            yOffset += 35;

            // A line divider
            Widgets.DrawLine(new Vector2(10, yOffset), new Vector2(200, yOffset), color: Color.white, 2f);
            yOffset += 5;

            tyOffset = yOffset;
            if (jobHistory.Count != 0)
            {
                Text.Font = GameFont.Tiny;

                yOffset += 10;
                if (((Pawn)comp.parent).CurJob != null)
                {
                    if (((Pawn)comp.parent).CurJobDef != jobHistory.Last())
                        jobHistory.Add(((Pawn)comp.parent).CurJobDef);
                }

                try
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur JobDef " + jobHistory.Last());
                    yOffset += 15;
                }
                catch
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur JobDef None");
                    yOffset += 15;
                }

                try
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur job Lord " + ((Pawn)comp.parent).jobs.curJob.lord.ToString());
                    yOffset += 15;
                }
                catch
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur job Lord None");
                    yOffset += 15;
                }

                try
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur Think tree name" + ((Pawn)comp.parent).jobs.curJob.jobGiverThinkTree);
                    yOffset += 15;

                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur Think tree root" + ((Pawn)comp.parent).jobs.curJob.jobGiverThinkTree.thinkRoot);
                    yOffset += 15;
                }
                catch
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur Think tree name None");
                    yOffset += 15;
                }

                try
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur Toil" + ((Pawn)comp.parent).jobs.curDriver.CurToil.ToString());
                    yOffset += 15;
                }
                catch
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn cur Toil None");
                    yOffset += 15;
                }

                var tdriver = ((Pawn)comp.parent).jobs.curDriver;
                if (tdriver != null)
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), "Pawn JobDriver" + tdriver.ToString());
                    yOffset += 15;
                }

                yOffset += 25;

                for (int i = 0; i < jobHistory.Count - 1; i++)
                {
                    Widgets.Label(new Rect(10, yOffset, 400, 20), -(i + 1) + " " + jobHistory[jobHistory.Count - i - 2]);
                    yOffset += 20;
                }

                if (jobHistory.Count >= 5) { jobHistory.RemoveRange(0, jobHistory.Count - 5); }
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

            Text.Font = GameFont.Small;
        }

        private static Vector2 nodeScrollPosition = Vector2.zero;
        private static readonly float compElementHeight = 30;
    }
}
