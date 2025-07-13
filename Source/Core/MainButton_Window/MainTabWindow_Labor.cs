using HarmonyLib;
using Multiplayer.API;
using PrisonLabor.Core.LaborWorkSettings;
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
  public class MainTabWindow_Labor : CustomTabWindow
  {

    protected override float ExtraTopSpace => 40f;
    protected override PawnTableDef PawnTableDef => PawnTableDefOf.Work;

    protected override IEnumerable<Pawn> Pawns
    {
      get
      {
        foreach (var pawn in base.Pawns)
        {
          if (pawn.LaborEnabled())
          {
            WorkSettings.InitWorkSettings(pawn);
            yield return pawn;
          }
        }
      }
    }
    public override void DoWindowContents(Rect rect)
    {
      base.DoWindowContents(rect);
      if (Event.current.type != EventType.Layout)
      {
        DoManualPrioritiesCheckbox(rect);
        GUI.color = new Color(1f, 1f, 1f, 0.5f);
        Text.Anchor = TextAnchor.UpperCenter;
        Text.Font = GameFont.Tiny;
        Widgets.Label(new Rect(rect.x + 370f, rect.y + 5f, 160f, 30f), "<= " + "HigherPriority".Translate());
        Widgets.Label(new Rect(rect.x + 630f, rect.y + 5f, 160f, 30f), "LowerPriority".Translate() + " =>");
        GUI.color = Color.white;
        Text.Font = GameFont.Small;
        Text.Anchor = TextAnchor.UpperLeft;
      }

    }

    private void DoManualPrioritiesCheckbox(Rect globalRect)
    {
      Text.Font = GameFont.Small;
      GUI.color = Color.white;
      Text.Anchor = TextAnchor.UpperLeft;
      Rect rect = new Rect(globalRect.x + 5f, globalRect.y + 5f, 140f, 30f);
      bool useWorkPriorities = Current.Game.playSettings.useWorkPriorities;
      Widgets.CheckboxLabeled(rect, "ManualPriorities".Translate(), ref Current.Game.playSettings.useWorkPriorities);
      if (useWorkPriorities != Current.Game.playSettings.useWorkPriorities)
      {
        foreach (Pawn item in PawnsFinder.AllMapsWorldAndTemporary_Alive)
        {
          if (item.IsPrisonerOfColony && item.workSettings != null)
          {
            item.workSettings.Notify_UseWorkPrioritiesChanged();
          }
        }
      }
      if (Current.Game.playSettings.useWorkPriorities)
      {
        using (new TextBlock(new Color(1f, 1f, 1f, 0.5f)))
        {
          Widgets.Label(new Rect(rect.x, rect.yMax - 6f, rect.width, 60f), "PriorityOneDoneFirst".Translate());
        }
      }
      if (Current.Game.playSettings.useWorkPriorities)
      {
        return;
      }
      UIHighlighter.HighlightOpportunity(rect, "ManualPriorities-Off");
    }
    public override void PostOpen()
    {

      base.PostOpen();
      DebugLogger.debug("Called PostOpen");
    }
  }
}
