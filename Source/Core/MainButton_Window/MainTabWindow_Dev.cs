﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class MainTabWindow_Dev : CustomTabWindow
    {
        protected override PawnTableDef PawnTableDef => UIDefsOf.PL_DevTable;

        protected override IEnumerable<Pawn> Pawns => Find.CurrentMap.mapPawns.PrisonersOfColony;


    }
}
