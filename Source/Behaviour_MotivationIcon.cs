using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    class Behaviour_MotivationIcon : MonoBehaviour
    {
        private static Texture2D inspiredTexture = ContentFinder<Texture2D>.Get("InspireIcon", false);
        private static Texture2D motivatedTexture = ContentFinder<Texture2D>.Get("MotivateIcon", false);
        private static Vector3 iconPos = new Vector3(0f, 0f, 1.3f);

        private float worldScale;

        public Behaviour_MotivationIcon() : base() { }

        private void DrawIcon(Texture2D texture, Vector3 pawnPos)
        {
            //TODO add iconSizeMult to prefs ?
            float iconSizeMult = 1.0f;
            //TODO add iconSize to prefs ?
            float iconSize = 2.0f;

            if (texture == null)
            {
                Log.Message("texture cant be found");
                return;
            }

            Vector2 scrPosVec = (pawnPos + iconPos).MapToUIPosition();
            float scrSize = worldScale * iconSizeMult * iconSize * 0.5f;
            Rect scrPos = new Rect(scrPosVec.x - scrSize * 0.5f, scrPosVec.y - scrSize * 0.5f, scrSize, scrSize);
            GUI.DrawTexture(scrPos, texture, ScaleMode.ScaleToFit, true);
        }

        public virtual void OnGUI()
        {
            //TODO add icons enabled
            bool iconsEnabled = true;
            bool inGame = Find.GameInfo != null;

            if (iconsEnabled && inGame)
            {
                foreach (Pawn pawn in Find.VisibleMap.mapPawns.AllPawns)
                {
                    if (pawn == null) continue;
                    if (pawn.RaceProps == null) continue;

                    if (pawn.IsPrisonerOfColony)
                    {
                        var need = pawn.needs.TryGetNeed<Need_Motivation>();
                        if (need != null && need.Motivated)
                        {
                            if (PrisonLaborUtility.WorkTime(pawn))
                                DrawIcon(inspiredTexture, pawn.DrawPos);
                            else
                                DrawIcon(motivatedTexture, pawn.DrawPos);
                        }
                    }
                }
            }
        }


        public virtual void Update()
        {
            worldScale = Screen.height / (2 * Camera.current.orthographicSize);
        }

        public static void Initialization()
        {
            GameObject iconModule = new GameObject("PrisonLabor_Initializer");
            iconModule.AddComponent<IconModuleInitializer>();
            DontDestroyOnLoad(iconModule);
        }
    }

    class IconModuleInitializer : MonoBehaviour
    {
        public void FixedUpdate()
        {
            GameObject iconModule = GameObject.Find("PrisonLabor_IconModule");
            if (iconModule == null)
            {
                iconModule = new GameObject("PrisonLabor_IconModule");
                iconModule.AddComponent<Behaviour_MotivationIcon>();
            }
        }
    }
}
