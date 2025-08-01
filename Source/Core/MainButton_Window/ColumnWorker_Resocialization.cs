using Multiplayer.API;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class ColumnWorker_Resocialization : PawnColumnWorker
    {
        public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
        {
            var need = pawn.needs.TryGetNeed<Need_Treatment>();
            if (need?.ResocializationReady ?? false)
            {
                if(Widgets.ButtonText(rect, "PrisonLabor_RecruitButtonLabel".Translate()))
                {
                    ConvertPrisoner(pawn);
                    WindowNotifier.NotifyPLWindows();
                }

            }
        }
        public void ConvertPrisoner(Pawn pawn)
        {
            CleanPrisonersStatus.Clean(pawn);
            RecruitUtility.Recruit(pawn, Faction.OfPlayer);
        }
    }
}
