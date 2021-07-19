using PrisonLabor.Core.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Trackers
{
    public class CuffsTracker : MapComponent
    {
        public Dictionary<Pawn, bool> legscuffTracker = new Dictionary<Pawn, bool>();
        public Dictionary<Pawn, bool> handscuffTracker = new Dictionary<Pawn, bool>();
        private readonly ScribeUtils<Pawn, bool> scribeUtil = new ScribeUtils<Pawn, bool>();
        public CuffsTracker(Map map) : base(map)
        {
        }

        public override void ExposeData()
        {
            scribeUtil.Scribe(ref handscuffTracker, "handscuffTracker");
            scribeUtil.Scribe(ref legscuffTracker, "legscuffTracker");
        }
    }
}
