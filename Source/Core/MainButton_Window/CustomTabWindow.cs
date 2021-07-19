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
	public abstract class CustomTabWindow : Window
    {
		private PawnTable table;

		protected virtual float ExtraBottomSpace => 53f;

		protected virtual float ExtraTopSpace => 0f;

		protected abstract PawnTableDef PawnTableDef
		{
			get;
		}

		protected override float Margin => 6f;

		public virtual Vector2 RequestedTabSize
		{
			get
			{
				if (table == null)
				{
					return Vector2.zero;
				}
				return new Vector2(table.Size.x + Margin * 2f, table.Size.y + ExtraBottomSpace + ExtraTopSpace + Margin * 2f);
			}
		}

		public virtual MainTabWindowAnchor Anchor => MainTabWindowAnchor.Left;

		public override Vector2 InitialSize
		{
			get
			{
				Vector2 requestedTabSize = RequestedTabSize;
				if (requestedTabSize.y > (float)(UI.screenHeight - 35))
				{
					requestedTabSize.y = UI.screenHeight - 35;
				}
				if (requestedTabSize.x > (float)UI.screenWidth)
				{
					requestedTabSize.x = UI.screenWidth;
				}
				return requestedTabSize;
			}
		}
		protected virtual IEnumerable<Pawn> Pawns => Find.CurrentMap.mapPawns.FreeColonists_NoHusks;

		public override void PostOpen()
		{
			base.PostOpen();
			if (table == null)
			{
				table = CreateTable();
			}
			SetDirty();
		}

		public override void DoWindowContents(Rect rect)
		{
			table.PawnTableOnGUI(new Vector2(rect.x, rect.y + ExtraTopSpace));
		}

		public void Notify_PawnsChanged()
		{
			SetDirty();
		}

		public override void Notify_ResolutionChanged()
		{
			table = CreateTable();
			base.Notify_ResolutionChanged();
		}

		private PawnTable CreateTable()
		{
			return (PawnTable)Activator.CreateInstance(PawnTableDef.workerClass, PawnTableDef, (Func<IEnumerable<Pawn>>)(() => Pawns), UI.screenWidth - (int)(Margin * 2f), (int)((float)(UI.screenHeight - 35) - ExtraBottomSpace - ExtraTopSpace - Margin * 2f));
		}

		protected void SetDirty()
		{
			table.SetDirty();
			SetInitialSizeAndPosition();
		}
		public CustomTabWindow()
		{
			layer = WindowLayer.GameUI;
			soundAppear = null;
			soundClose = SoundDefOf.TabClose;
			doCloseButton = false;
			doCloseX = false;
			preventCameraMotion = false;
		}

		protected override void SetInitialSizeAndPosition()
		{
			base.SetInitialSizeAndPosition();
			if (Anchor == MainTabWindowAnchor.Left)
			{
				windowRect.x = 0f;
			}
			else
			{
				windowRect.x = (float)UI.screenWidth - windowRect.width;
			}
			windowRect.y = (float)(UI.screenHeight - 35) - windowRect.height;
		}

	}
}
