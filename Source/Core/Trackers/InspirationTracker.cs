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
            {
                isWatched[pawn] = 0.0f;
                return false;
            }

            if (wardensCount == 0)
            {
                isWatched[pawn] = -0.0085f;
                return false;
            }

            if (room.IsHuge || room.CellCount > 1600)
            {
                isWatched[pawn] = -0.01f;
                return false;
            }

            if (prisonersCount / wardensCount >= 3.5f)
            {
                isWatched[pawn] = -0.08f;
                return false;
            }

            isWatched[pawn] = (wardensCount * (wardensCount + 1)) / prisonersCount * 0.005f;
            return true;
        }

        public static float GetInsiprationValue(Pawn pawn, bool refresh = false)
        {
            if (!isWatched.ContainsKey(pawn))
                return 0;

            pawn.IsWatched();

            return isWatched[pawn];
        }
    }
}
