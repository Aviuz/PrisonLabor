using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    public enum GroupMode
    {
        ColonyOnly,
        PrisonersOnly,
        ColonistsOnly
    }

    class BillGroupData : IExposable
    {
        public GroupMode mode;

        public BillGroupData()
        {
            mode = GroupMode.ColonyOnly;
        }

        public void ExposeData()
        {
            Scribe_Values.Look<GroupMode>(ref this.mode, "PrisonLabor_BillGroup", GroupMode.ColonyOnly, false);
        }
    }
}
