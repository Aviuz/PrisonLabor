using System.Collections.Generic;
using PrisonLabor.Core.Other;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.LaborArea
{
    public abstract class Designator_AreaLabor : Designator_Cells
    {
        private static readonly List<IntVec3> JustRemovedCells = new List<IntVec3>();

        private static readonly List<IntVec3> JustAddedCells = new List<IntVec3>();

        private static readonly List<Room> RequestedRooms = new List<Room>();
        private readonly DesignateMode mode;

        public Designator_AreaLabor(DesignateMode mode)
        {
            this.mode = mode;
            soundDragSustain = SoundDefOf.Designate_DragStandard;
            soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
            useMouseIcon = true;
            defaultLabel = "PrisonLabor_LaborArea".Translate();
            //Initialization();
        }

        public override int DraggableDimensions => 2;

        public override bool DragDrawMeasurements => true;

        public override AcceptanceReport CanDesignateThing(Thing t)
        {
            return AcceptanceReport.WasRejected;
        }

        public override AcceptanceReport CanDesignateCell(IntVec3 c)
        {
            if (!c.InBounds(Map))
                return false;
            var flag = Map.areaManager.Get<Area_Labor>()[c];
            if (mode == DesignateMode.Add)
                return !flag;
            return flag;
        }

        public override void DesignateSingleCell(IntVec3 c)
        {
            if (mode == DesignateMode.Add)
            {
                Map.areaManager.Get<Area_Labor>()[c] = true;
                JustAddedCells.Add(c);
            }
            else if (mode == DesignateMode.Remove)
            {
                Map.areaManager.Get<Area_Labor>()[c] = false;
                JustRemovedCells.Add(c);
            }
        }

        protected override void FinalizeDesignationSucceeded()
        {
            base.FinalizeDesignationSucceeded();
            if (mode == DesignateMode.Add)
            {
                for (var i = 0; i < JustAddedCells.Count; i++)
                    Map.areaManager.Get<Area_Labor>()[JustAddedCells[i]] = true;
                JustAddedCells.Clear();
            }
            else if (mode == DesignateMode.Remove)
            {
                for (var j = 0; j < JustRemovedCells.Count; j++)
                    Map.areaManager.Get<Area_Labor>()[JustRemovedCells[j]] = false;
                JustRemovedCells.Clear();
                RequestedRooms.Clear();
            }
        }

        public override void SelectedUpdate()
        {
            Tutorials.LaborAreaWarning();
            GenUI.RenderMouseoverBracket();
            if (Map.areaManager.Get<Area_Labor>() == null)
                Map.areaManager.AllAreas.Add(new Area_Labor(Map.areaManager));
            Map.areaManager.Get<Area_Labor>().MarkForDraw();
        }
    }
}