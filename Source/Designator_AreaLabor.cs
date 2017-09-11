using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    [StaticConstructorOnStartup]
    public abstract class Designator_AreaLabor : Designator
    {
        private DesignateMode mode;

        private static List<IntVec3> justRemovedCells = new List<IntVec3>();

        private static List<IntVec3> justAddedCells = new List<IntVec3>();

        private static List<Room> requestedRooms = new List<Room>();

        public override int DraggableDimensions
        {
            get
            {
                return 2;
            }
        }

        public override bool DragDrawMeasurements
        {
            get
            {
                return true;
            }
        }

        public Designator_AreaLabor(DesignateMode mode) : base()
        {
            this.mode = mode;
            this.soundDragSustain = SoundDefOf.DesignateDragStandard;
            this.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
            this.useMouseIcon = true;
            this.defaultLabel = "Prison Labor Area";
            this.icon = ContentFinder<Texture2D>.Get("extendLabor", true);
            //Initialization();
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(base.Map))
            {
                return false;
            }
            bool flag = Map.areaManager.Get<Area_Labor>()[c];
            if (mode == DesignateMode.Add)
            {
                return !flag;
            }
            return flag;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            if (this.mode == DesignateMode.Add)
            {
                Map.areaManager.Get<Area_Labor>()[c] = true;
                justAddedCells.Add(c);
            }
            else if (this.mode == DesignateMode.Remove)
            {
                Map.areaManager.Get<Area_Labor>()[c] = false;
                justRemovedCells.Add(c);
            }
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            if (mode == DesignateMode.Add)
            {
                for (int i = 0; i < justAddedCells.Count; i++)
                {
                    Map.areaManager.Get<Area_Labor>()[justAddedCells[i]] = true;
                }
                justAddedCells.Clear();
            }
            else if (mode == DesignateMode.Remove)
            {
                for (int j = 0; j < justRemovedCells.Count; j++)
                {
                    Map.areaManager.Get<Area_Labor>()[justRemovedCells[j]] = false;
                }
                justRemovedCells.Clear();
                requestedRooms.Clear();
            }
        }

        public override void SelectedUpdate()
        {
            GenUI.RenderMouseoverBracket();
            if (Map.areaManager.Get<Area_Labor>() == null)
                Map.areaManager.AllAreas.Add(new Area_Labor(this.Map.areaManager));
            base.Map.areaManager.Get<Area_Labor>().MarkForDraw();
        }

        public static void Initialization()
        {
            DefDatabase<DesignationCategoryDef>.GetNamed("Zone").AllResolvedDesignators.Add(new Designator_AreaLaborExpand());
            DefDatabase<DesignationCategoryDef>.GetNamed("Zone").AllResolvedDesignators.Add(new Designator_AreaLaborClear());
        }
    }
}
