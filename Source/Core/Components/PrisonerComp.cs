using PrisonLabor.Core.GUI_Components;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Components
{

   public class PrisonerComp : ThingComp
    {
        private bool Active
        {
            get
            {
                Pawn pawn = this.parent as Pawn;
                return pawn != null && pawn.IsPrisonerOfColony && !pawn.Dead && pawn.Spawned && pawn.CarriedBy == null;
            }
        }

        public override void PostDraw()
        {
            if (Active && PrisonLaborPrefs.EnableMotivationIcons)
            {
                Pawn pawn = this.parent as Pawn;
                var need = pawn.needs.TryGetNeed<Need_Motivation>();
                if (pawn.health.hediffSet.HasTemperatureInjury(TemperatureInjuryStage.Serious) && PrisonLaborUtility.WorkTime(pawn))
                {
                    DrawIcon(TexturePool.freezingTexture);
                }
                else if (pawn.IsWatched())
                {
                    DrawIcon(TexturePool.watchedTexture);
                }
                else if (need != null && need.IsLazy && PrisonLaborUtility.LaborEnabled(pawn) && PrisonLaborUtility.WorkTime(pawn))
                {
                    DrawIcon(TexturePool.lazyTexture);
                }
            }
            
        }


        private void DrawIcon(Material drawIcon)
        {
            var drawPos = parent.DrawPos;
            drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.28125f;

            drawPos.x += parent.def.size.x - 0.52f;
            drawPos.z += parent.def.size.z - 0.45f;


            var num = (Time.realtimeSinceStartup + (397f * (parent.thingIDNumber % 571))) * 4f;
            var num2 = ((float)Math.Sin(num) + 1f) * 0.5f;
            num2 = 0.3f + (num2 * 0.7f);
            var material = FadedMaterialPool.FadedVersionOf(drawIcon, num2);
            Graphics.DrawMesh(MeshPool.plane05, drawPos, Quaternion.identity, material, 0);
        }
    }
}
