using RimWorld.Planet;
using System;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    internal class Behaviour_MotivationIcon : MonoBehaviour
    {
        // TODO delete later
        private static bool displayedError = false;

        private static readonly Texture2D inspiredTexture;
        private static readonly Texture2D motivatedTexture;
        private static readonly Vector3 iconPos;

        private float worldScale;

        static Behaviour_MotivationIcon()
        {
            inspiredTexture = ContentFinder<Texture2D>.Get("InspireIcon", false);
            motivatedTexture = ContentFinder<Texture2D>.Get("MotivateIcon", false);
            iconPos = new Vector3(0f, 0f, 1.3f);
        }

        private void DrawIcon(Texture2D texture, Vector3 pawnPos)
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

        public virtual void OnGUI()
        {
            try
            {
                var iconsEnabled = PrisonLaborPrefs.EnableMotivationIcons && !PrisonLaborPrefs.DisableMod;
                var inGame = Find.VisibleMap != null && Find.VisibleMap.mapPawns != null && !WorldRendererUtility.WorldRenderedNow;

                if (iconsEnabled && inGame)
                    foreach (var pawn in Find.VisibleMap.mapPawns.AllPawns)
                    {
                        if (pawn == null) continue;
                        if (pawn.RaceProps == null) continue;

                        if (pawn.IsPrisonerOfColony)
                        {
                            var need = pawn.needs.TryGetNeed<Need_Motivation>();
                            if (need != null && need.Motivated)
                                if (need.Insipred)
                                    DrawIcon(inspiredTexture, pawn.DrawPos);
                                else
                                    DrawIcon(motivatedTexture, pawn.DrawPos);
                        }
                    }
            }
            catch (NullReferenceException e)
            {
                if (!displayedError)
                {
                    Log.ErrorOnce("PrisonLaborError: null reference in OnGui() : " + e.Message + " trace: " + e.StackTrace, typeof(Behaviour_MotivationIcon).GetHashCode());
                    displayedError = true;
                }
            }
        }


        public virtual void Update()
        {
            worldScale = Screen.height / (2 * Camera.current.orthographicSize);
        }

        public static void Initialization()
        {
            var iconModule = new GameObject("PrisonLabor_Initializer");
            iconModule.AddComponent<IconModuleInitializer>();
            DontDestroyOnLoad(iconModule);
        }
    }

    internal class IconModuleInitializer : MonoBehaviour
    {
        public void FixedUpdate()
        {
            var iconModule = GameObject.Find("PrisonLabor_IconModule");
            if (iconModule == null)
            {
                iconModule = new GameObject("PrisonLabor_IconModule");
                iconModule.AddComponent<Behaviour_MotivationIcon>();
            }
        }
    }
}