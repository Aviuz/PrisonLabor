using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.LaborArea
{
    internal class Designator_AreaLaborClear : Designator_AreaLabor
    {
        public Designator_AreaLaborClear() : base(DesignateMode.Remove)
        {
            defaultLabel = "PrisonLabor_ClearLaborArea".Translate();
            defaultDesc = "PrisonLabor_LaborAreaDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("LaborAreaClear");
            soundDragSustain = SoundDefOf.Designate_DragAreaDelete;
            soundDragChanged = null;
            soundSucceeded = SoundDefOf.Designate_ZoneDelete;
        }
    }
}