using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace PrisonLabor.Core.Trackers
{
    internal static class InspirationTracker
    {
        private static Dictionary<Pawn, float> isWatched = new Dictionary<Pawn, float>();

        public static bool IsWatched(this Pawn pawn)
        {
            isWatched[pawn] = 0;

            var room = pawn.GetRoom();

            if (room == null)
                return false;

            var wardensCount = 0;
            var prisonersCount = 0;

            lock (Tracked.LOCK_WARDEN)
            {
                wardensCount = Tracked.Wardens[room.ID].Count;
                prisonersCount = Tracked.Prisoners[room.ID].Count;
            }

            if (prisonersCount == 0)
                return false;


            if (room.IsHuge)
            {
                if (room.CellCount > 1600)
                    return false;
            }

            if (prisonersCount >= 2 && wardensCount <= 1)
            {
                isWatched[pawn] = -prisonersCount / 100f;
                return false;
            }

            isWatched[pawn] = (wardensCount / prisonersCount) * 0.001f;
            return true;
        }

        public static float GetInsiprationValue(Pawn pawn, bool refresh = false)
        {
            if (!isWatched.ContainsKey(pawn))
                return 0;

            return isWatched[pawn];
        }
    }
}
