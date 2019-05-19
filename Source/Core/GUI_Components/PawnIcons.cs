using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
using System;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.GUI_Components
{
    [StaticConstructorOnStartup]
    public class PawnIcons : MapComponent
    {
        private static readonly Texture2D watchedTexture;
        private static readonly Texture2D lazyTexture;
        private static readonly Texture2D freezingTexture;
        private static readonly Vector3 iconPos;

        private static float worldScale;

        static PawnIcons()
        {
            watchedTexture = ContentFinder<Texture2D>.Get("InspireIcon", false);
            lazyTexture = ContentFinder<Texture2D>.Get("LazyIcon", false);
            freezingTexture = ContentFinder<Texture2D>.Get("FreezingIcon", false);
            iconPos = new Vector3(0.3f, 0f, 0.9f);
        }

        public PawnIcons(Map map) : base(map) { }

        private static void DrawIcon(Texture2D texture, Vector3 pawnPos)
        {
            //TODO add iconSizeMult to prefs ?
            var iconSizeMult = 1.0f;
            //TODO add iconSize to prefs ?
            var iconSize = 2.0f;

            if (texture == null)
            {
                Log.Message("texture cant be found");
                return;
            }

            var scrPosVec = (pawnPos + iconPos).MapToUIPosition();
            var scrSize = worldScale * iconSizeMult * iconSize * 0.5f;
            var scrPos = new Rect(scrPosVec.x - scrSize * 0.5f, scrPosVec.y - scrSize * 0.5f, scrSize, scrSize);
            GUI.DrawTexture(scrPos, texture, ScaleMode.ScaleToFit, true);
        }

        public override void MapComponentOnGUI()
        {
            try
            {
                if (!PrisonLaborPrefs.EnableMotivationIcons)
                    return;

                if (map.mapPawns == null)
                    return;

                foreach (var pawn in map.mapPawns.AllPawns)
                {
                    if (pawn == null) continue;
                    if (pawn.RaceProps == null) continue;

                    if (pawn.IsPrisonerOfColony && pawn.CarriedBy == null)
                    {
                        var need = pawn.needs.TryGetNeed<Need_Motivation>();
                        if (pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) && PrisonLaborUtility.WorkTime(pawn))
                        {
                            DrawIcon(freezingTexture, pawn.DrawPos);
                        }
                        else if (pawn.IsWatched())
                        {
                            DrawIcon(watchedTexture, pawn.DrawPos);
                        }
                        else if (need != null && need.IsLazy && PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn))
                        {
                            DrawIcon(lazyTexture, pawn.DrawPos);
                        }
                    }
                }
            }
            catch (NullReferenceException e)
            {
                Log.ErrorOnce("PrisonLaborError: null reference in OnGui() : " + e.Message + " trace: " + e.StackTrace, typeof(PawnIcons).GetHashCode());
            }
        }

        public override void MapComponentTick()
        {
            worldScale = Screen.height / (2 * Camera.current.orthographicSize);
        }
    }
}