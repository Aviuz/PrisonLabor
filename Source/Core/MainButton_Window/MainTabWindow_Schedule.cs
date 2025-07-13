using PrisonLabor.Core.LaborWorkSettings;
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
  public class MainTabWindow_Schedule : CustomTabWindow
  {
    private const int TimeAssignmentSelectorWidth = 191;

    private const int TimeAssignmentSelectorHeight = 65;

    protected override PawnTableDef PawnTableDef => PawnTableDefOf.Restrict;

    protected override IEnumerable<Pawn> Pawns
    {
      get
      {
        foreach (var pawn in base.Pawns.Where(p => p.LaborEnabled()))
        {
          WorkSettings.InitWorkSettings(pawn);
          yield return pawn;
        }
      }
    }

    public override void DoWindowContents(Rect fillRect)
    {
      base.DoWindowContents(fillRect);
      TimeAssignmentSelector.DrawTimeAssignmentSelectorGrid(new Rect(fillRect.x, fillRect.y,
        TimeAssignmentSelectorWidth, TimeAssignmentSelectorHeight));
    }
  }
}