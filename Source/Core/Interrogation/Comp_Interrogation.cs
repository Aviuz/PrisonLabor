using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation
{
  public class CompInterrogation : ThingComp
  {
  }

  public class CompPropertiesInterrogation : CompProperties
  {
    public CompPropertiesInterrogation()
    {
      compClass = typeof(CompInterrogation);
    }
  }
}
