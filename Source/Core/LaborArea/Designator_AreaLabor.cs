using PrisonLabor.Core.Other;
using RimWorld;
using Verse;

namespace PrisonLabor.Core.LaborArea
{
  public abstract class Designator_AreaLabor : Designator_Cells
  {
    private readonly DesignateMode mode;

    public Designator_AreaLabor(DesignateMode mode)
    {
      this.mode = mode;
      soundDragSustain = SoundDefOf.Designate_DragStandard;
      soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
      useMouseIcon = true;
      defaultLabel = "PrisonLabor_LaborArea".Translate();
    }

    public override DrawStyleCategoryDef DrawStyleCategory => DrawStyleCategoryDefOf.Areas;

    public override bool DragDrawMeasurements => true;

    public override AcceptanceReport CanDesignateThing(Thing t)
    {
      return AcceptanceReport.WasRejected;
    }

    public override AcceptanceReport CanDesignateCell(IntVec3 c)
    {
      if (!c.InBounds(Map))
      {
        return false;
      }

      var flag = Map.areaManager.Get<Area_Labor>()[c];
      if (mode == DesignateMode.Add)
      {
        return !flag;
      }

      return flag;
    }

    public override void DesignateSingleCell(IntVec3 c)
    {
      switch (mode)
      {
        case DesignateMode.Add:
          Map.areaManager.Get<Area_Labor>()[c] = true;
          break;
        case DesignateMode.Remove:
          Map.areaManager.Get<Area_Labor>()[c] = false;
          break;
      }
    }

    public override void SelectedUpdate()
    {
      Tutorials.LaborAreaWarning();
      GenUI.RenderMouseoverBracket();
      if (Map.areaManager.Get<Area_Labor>() == null)
      {
        Map.areaManager.AllAreas.Add(new Area_Labor(Map.areaManager));
      }

      Map.areaManager.Get<Area_Labor>().MarkForDraw();
    }
  }
}