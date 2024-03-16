using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation
{
  public class RoomRoleWorker_InterrogationRoom : RoomRoleWorker
  {
    public override float GetScore(Room room)
    {
      List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
      foreach (Thing thing in containedAndAdjacentThings)
      {
        if (thing.TryGetComp<CompInterrogation>() != null)
        {
          return 100000f;
        }
      }
      return 0f;
    }
  }
}
