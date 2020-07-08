using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace PrisonLabor.Core.Trackers
{
    internal static class InspirationTracker
    {
        private static Dictionary<Pawn, bool> isWatched = new Dictionary<Pawn, bool>();
        private static Dictionary<Pawn, float> inspirationValue = new Dictionary<Pawn, float>();

        public static void UpdateInspiration(this Pawn pawn)
        {
            inspirationValue[pawn] = 0;

            var room = pawn.GetRoom();

            if (room == null)
            {
                isWatched[pawn] = false; return;
            }

            var wardensCount = 0;
            var prisonersCount = 0;

            lock (Tracked.LOCK_WARDEN)
            {
                wardensCount = Tracked.Wardens[room.ID].Count;
                prisonersCount = Tracked.Prisoners[room.ID].Count;
            }

            if (prisonersCount == 0)
            {
                inspirationValue[pawn] = 0.0f;
                isWatched[pawn] = false; return;
            }

            if (wardensCount == 0)
            {
                inspirationValue[pawn] = -0.00425f;
                isWatched[pawn] = false; return;
            }

            if (room.IsHuge)
            {
                inspirationValue[pawn] = -0.005f;
                isWatched[pawn] = false; return;
            }

            if (prisonersCount / wardensCount > Prefs.MaxNumberOfPlayerSettlements)
            {
                inspirationValue[pawn] = -0.04f;
                isWatched[pawn] = false; return;
            }

            isWatched[pawn] = true;
            inspirationValue[pawn] = (wardensCount * (wardensCount + 1)) / prisonersCount * 0.005f;
        }

        public static bool IsWatched(this Pawn pawn, bool refresh = false)
        {
            if (refresh) { UpdateInspiration(pawn); }

            if (!inspirationValue.ContainsKey(pawn))
            {
                isWatched[pawn] = false; inspirationValue[pawn] = 0f; return false;
            }

            return isWatched[pawn];
        }

        public static float GetInsiprationValue(Pawn pawn, bool refresh = false)
        {
            if (refresh) { UpdateInspiration(pawn); }

            if (!inspirationValue.ContainsKey(pawn))
            {
                inspirationValue[pawn] = 0f; isWatched[pawn] = false; return 0f;
            }

            return inspirationValue[pawn];
        }
    }
}
