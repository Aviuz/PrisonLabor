using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  public class RitualSpectatorFilter_None : RitualSpectatorFilter
  {
    public override bool Allowed(Pawn p)
    {
      return false;
    }
  }
}
