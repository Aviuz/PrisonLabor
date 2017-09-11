using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    class Designator_AreaLaborClear : Designator_AreaLabor
    {
        public Designator_AreaLaborClear() : base(DesignateMode.Remove)
		{
            this.defaultLabel = "Clear Labor Area";
            this.defaultDesc = "work in progress";
            this.icon = ContentFinder<Texture2D>.Get("LaborAreaClear", true);
            //this.hotKey = KeyBindingDefOf.Misc6;
            this.soundDragSustain = SoundDefOf.DesignateDragAreaDelete;
            this.soundDragChanged = SoundDefOf.DesignateDragAreaDeleteChanged;
            this.soundSucceeded = SoundDefOf.DesignateAreaDelete;
        }
    }
}
