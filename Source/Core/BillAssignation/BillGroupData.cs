using Verse;

namespace PrisonLabor.Core.BillAssignation
{
    public enum GroupMode
    {
        ColonyOnly,
        PrisonersOnly,
        ColonistsOnly
    }

    internal class BillGroupData : IExposable
    {
        public GroupMode Mode;

        public BillGroupData()
        {
            Mode = GroupMode.ColonyOnly;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref Mode, "PrisonLabor_BillGroup", GroupMode.ColonyOnly, false);
        }
    }
}