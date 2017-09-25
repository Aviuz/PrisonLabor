using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    internal class Designator_AreaLaborClear : Designator_AreaLabor
    {
        public Designator_AreaLaborClear() : base(DesignateMode.Remove)
        {
            defaultLabel = "PrisonLabor_ClearLaborArea".Translate();
            defaultDesc = "PrisonLabor_LaborAreaDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("LaborAreaClear", true);
            //this.hotKey = KeyBindingDefOf.Misc6;
            soundDragSustain = SoundDefOf.DesignateDragAreaDelete;
            soundDragChanged = SoundDefOf.DesignateDragAreaDeleteChanged;
            soundSucceeded = SoundDefOf.DesignateAreaDelete;
        }
    }
}