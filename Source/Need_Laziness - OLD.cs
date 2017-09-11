using RimWorld;
using Verse;

namespace PrisonLabor
{
    public class Need_Laziness : Need
    {
        private const float LazyLevel = 0.2f;
        private const float NeedInspirationLevel = 0.5f;
        private const float LazyRate = 0.003f;
        private const float InspireRate = 0.015f;
        public const int WardenCapacity = (int) (InspireRate / LazyRate);

        private static PrisonerInteractionModeDef pimDef;
        private static NeedDef def;

        private bool enabled;
        private bool isLazy;
        private bool needToBeInspired;
        private int prisonersCount;

        private int slowDown;
        private int wardensCount;

        public Need_Laziness(Pawn pawn) : base(pawn)
        {
        }

        public override void NeedInterval()
        {
        }
    }
}