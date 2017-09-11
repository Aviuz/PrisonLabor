using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    class Designator_AreaLaborExpand : Designator_AreaLabor
    {
        public Designator_AreaLaborExpand() : base(DesignateMode.Add)
		{
            this.defaultLabel = "Expand Labor Area";
            this.defaultDesc = "work in progress";
            this.icon = ContentFinder<Texture2D>.Get("LaborAreaExpand", true);
            //this.hotKey = KeyBindingDefOf.Misc5;
            this.soundDragSustain = SoundDefOf.DesignateDragAreaAdd;
            this.soundDragChanged = SoundDefOf.DesignateDragAreaAddChanged;
            this.soundSucceeded = SoundDefOf.DesignateAreaAdd;
        }
    }
}
