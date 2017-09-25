using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    internal class Designator_AreaLaborExpand : Designator_AreaLabor
    {
        public Designator_AreaLaborExpand() : base(DesignateMode.Add)
        {
            defaultLabel = "PrisonLabor_ExpandLaborArea".Translate();
            defaultDesc = "PrisonLabor_LaborAreaDesc".Translate();
            icon = ContentFinder<Texture2D>.Get("LaborAreaExpand", true);
            //this.hotKey = KeyBindingDefOf.Misc5;
            soundDragSustain = SoundDefOf.DesignateDragAreaAdd;
            soundDragChanged = SoundDefOf.DesignateDragAreaAddChanged;
            soundSucceeded = SoundDefOf.DesignateAreaAdd;
        }
    }
}