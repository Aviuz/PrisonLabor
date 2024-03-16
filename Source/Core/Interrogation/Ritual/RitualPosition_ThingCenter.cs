using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  public class RitualPosition_ThingCenter : RitualPosition
  {
    public override PawnStagePosition GetCell(IntVec3 spot, Pawn p, LordJob_Ritual ritual)
    {
      return new PawnStagePosition(spot, null, Rot4.Invalid, highlight);
    }
  }
}
