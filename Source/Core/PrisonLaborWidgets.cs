using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core
{
    public static class PrisonLaborWidgets
    {
        public const int TabHeight = 20;
        public const int HorizontalSpacing = 10;
        public const int HLineHeight = 2;

        private static Stack<int> TabbedViewStackIndex = new Stack<int>();

        public static void BeginTabbedView(Rect rect, string[] tabs, ref int activeTabIndex)
        {
            GUI.BeginGroup(rect);

            float occupatedSpace = 0;
            for (int i = 0; i < tabs.Length; i++)
            {
                float width = HorizontalSpacing + Text.CalcSize(tabs[i]).x + HorizontalSpacing;
                var tabRect = new Rect(occupatedSpace, 0, width, TabHeight);
                occupatedSpace += width;

                if (Widgets.ButtonInvisible(tabRect, true))
                    activeTabIndex = i;
                if (activeTabIndex == i)
                {
                    Widgets.DrawHighlightSelected(tabRect);
                }
                else
                {
                    Widgets.DrawHighlightIfMouseover(tabRect);
                }

                tabRect.x += HorizontalSpacing;
                Widgets.Label(tabRect, tabs[i]);
            }

            var highlitedLine = new Rect(0, TabHeight, rect.width, 2);
            Widgets.DrawHighlightSelected(highlitedLine);

            var contentRect = new Rect(0, TabHeight+ HLineHeight, rect.width, rect.height - TabHeight- HLineHeight);
            GUI.BeginGroup(contentRect);
            TabbedViewStackIndex.Push(activeTabIndex);
        }

        public static void EndTabbedView()
        {
            TabbedViewStackIndex.Pop();
            GUI.EndGroup();
            GUI.EndGroup();
        }

        public static bool IsTabActive(int tabIndex)
        {
            int active = TabbedViewStackIndex.Peek();
            return active == tabIndex;
        }
    }
}
