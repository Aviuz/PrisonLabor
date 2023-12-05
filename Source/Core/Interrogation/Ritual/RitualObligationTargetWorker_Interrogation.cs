using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  public class RitualObligationTargetWorker_Interrogation : RitualObligationTargetFilter
  {
    public RitualObligationTargetWorker_Interrogation()
    {
    }

    public RitualObligationTargetWorker_Interrogation(RitualObligationTargetFilterDef def)
      : base(def)
    {
    }

    public override IEnumerable<string> GetTargetInfos(RitualObligation obligation)
    {
      return Enumerable.Empty<string>();
    }

    public override IEnumerable<TargetInfo> GetTargets(RitualObligation obligation, Map map)
    {
      return Enumerable.Empty<TargetInfo>();
    }

    protected override RitualTargetUseReport CanUseTargetInternal(TargetInfo target, RitualObligation obligation)
    {
      return target.HasThing && target.Thing.Faction != null && target.Thing.Faction.IsPlayer && target.Thing.TryGetComp<CompInterrogation>() != null;
    }
  }
}
