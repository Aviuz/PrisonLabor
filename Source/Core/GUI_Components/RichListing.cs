using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.GUI_Components
{
    public class RichListing
    {
        private Vector2 scrollPosition;
        private Rect windowRect;
        private Rect viewRect;
        private IEnumerable<string> entries;


        public float Spacing { get; set; }
        public float GapHeight { get; set; }
        public string MarginText { get; set; }
        public float MarginWidth { get; set; }
        public GameFont TitleFont { get; set; }
        public GameFont ItemFont { get; set; }

        public RichListing() // Defaults
        {
            TitleFont = GameFont.Medium;
            ItemFont = GameFont.Small;

            MarginText = " - ";
            MarginWidth = Text.fontStyles[1].CalcSize(new GUIContent(MarginText)).x;

            GapHeight = 12f;
            Spacing = 2f;
        }

        public void PreRender(Rect bounds, IEnumerable<string> entries)
        {
            var calculatedRect = new Rect(0, 0, bounds.width - 16f, CalculateHeight(bounds.width - 16f, entries));
            viewRect = calculatedRect;
            windowRect = bounds;
            this.entries = entries;
        }

        public void OnGui()
        {
            Start(windowRect, viewRect);
            foreach(var entry in entries)
                Append(entry);
            End();
        }

        public void OnGui(ref Vector2 scroller)
        {
            Start(windowRect, viewRect, ref scroller);
            foreach (var entry in entries)
                Append(entry);
            End();
        }

        public void Start(Rect windowRect, Rect viewRect)
        {
            this.viewRect = viewRect;
            Widgets.BeginScrollView(windowRect, ref scrollPosition, viewRect, true);
        }

        public void Start(Rect windowRect, Rect viewRect, ref Vector2 scroller)
        {
            this.viewRect = viewRect;
            Widgets.BeginScrollView(windowRect, ref scroller, viewRect, true);
        }

        public void Append(string item)
        {
            Text.Font = ItemFont;
            if (item.StartsWith("[title]"))
            {
                Text.Font = TitleFont;
                Widgets.Label(viewRect, item.Substring(7));
                viewRect.y += Text.CalcHeight(item, viewRect.width) + Spacing;

                // Draw line gap
                Color color = GUI.color;
                GUI.color = GUI.color * new Color(1f, 1f, 1f, 0.4f);
                Widgets.DrawLineHorizontal(viewRect.x, viewRect.y + +GapHeight * 0.5f, viewRect.width);
                GUI.color = color;
                viewRect.y += GapHeight;
            }
            // Draw Image with Text
            else if (item.StartsWith("[img]"))
            {
                int imgLength = item.IndexOf("[/img]");
                var imageString = item.Substring(5, imgLength - 5);
                var textToDraw = item.Substring(imgLength + 6);

                var content = new GUIContent();
                content.image = ContentFinder<Texture2D>.Get(imageString, false);
                content.text = textToDraw;
                Widgets.Label(viewRect, content);

                viewRect.y += GuiStyle(Text.Font).CalcHeight(content, viewRect.width);
            }
            // Draw Gap
            else if (item.StartsWith("[gap]"))
            {
                Color color = GUI.color;
                GUI.color = GUI.color * new Color(1f, 1f, 1f, 0.4f);
                Widgets.DrawLineHorizontal(viewRect.x, viewRect.y + +GapHeight * 0.5f, viewRect.width);
                GUI.color = color;
                viewRect.y += GapHeight;
            }
            // Draw Subtitle (without margin)
            else if (item.StartsWith("[subtitle]"))
            {
                Widgets.Label(viewRect, item.Substring(10));
                viewRect.y += Text.CalcHeight(item, viewRect.width) + Spacing;
            }
            // Draw Text
            else
            {
                viewRect.width -= MarginWidth;
                Widgets.Label(viewRect, MarginText);
                viewRect.x += MarginWidth;
                Widgets.Label(viewRect, item);
                viewRect.x -= MarginWidth;
                viewRect.y += Text.CalcHeight(item, viewRect.width) + Spacing;
                viewRect.width += MarginWidth;
            }
        }

        public void End()
        {
            Widgets.EndScrollView();
        }

        private float CalculateHeight(float width, IEnumerable<string> items)
        {
            float height = 0;
            foreach (var item in items)
            {
                if (item.StartsWith("[title]"))
                {
                    Text.Font = TitleFont;
                    height += Text.CalcHeight(item.Substring(7), width) + Spacing + GapHeight;
                }
                // Image with Text
                else if (item.StartsWith("[img]"))
                {
                    Text.Font = ItemFont;
                    int imgLength = item.IndexOf("[/img]");
                    var imageString = item.Substring(5, imgLength - 5);
                    var textToDraw = item.Substring(imgLength + 6);

                    var content = new GUIContent();
                    content.image = ContentFinder<Texture2D>.Get(imageString, false);
                    content.text = textToDraw;


                    height += GuiStyle(Text.Font).CalcHeight(content, width);
                }
                // Gap
                else if (item.StartsWith("[gap]"))
                {
                    height += GapHeight;
                }
                else if (item.StartsWith("[subtitle]"))
                {
                    Text.Font = ItemFont;
                    height += Text.CalcHeight(item.Substring(10), width) + Spacing;
                }
                // Only Text
                else
                {
                    Text.Font = ItemFont;
                    height += Text.CalcHeight(item, width - MarginWidth) + Spacing;
                }
            }
            return height;
        }

        private static GUIStyle GuiStyle(GameFont font)
        {
            GUIStyle gUIStyle;
            switch (font)
            {
                case GameFont.Tiny:
                    gUIStyle = Text.fontStyles[0];
                    break;
                case GameFont.Small:
                    gUIStyle = Text.fontStyles[1];
                    break;
                case GameFont.Medium:
                    gUIStyle = Text.fontStyles[2];
                    break;
                default:
                    return null;
            }
            gUIStyle.alignment = Text.Anchor;
            gUIStyle.wordWrap = Text.WordWrap;
            return gUIStyle;
        }
    }
}
