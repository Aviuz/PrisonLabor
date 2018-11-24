using System;
using System.Collections.Generic;
using Verse;

namespace PrisonLabor
{
    internal static class InspirationTracker
    {
        public static Dictionary<Map, Dictionary<Pawn, float>> inspirationValues = new Dictionary<Map, Dictionary<Pawn, float>>();

        /// <summary>
        /// Check if pawn is watched(supervised) by a Jailor
        /// </summary>
        public static bool IsWatched(this Pawn pawn)
        {
            Dictionary<Pawn, float> iValues;
            float value;

            var map = pawn?.Map;
            lock (inspirationValues)
            {
                if (map != null && inspirationValues.TryGetValue(map, out iValues))
                {
                    if (iValues.TryGetValue(pawn, out value))
                    {
                        return value != 0;
                    }
                }
            }
            return false;
        }

        public static float GetInsiprationValue(Pawn pawn, bool refresh = false)
        {
            var map = pawn.Map;
            if (!inspirationValues.ContainsKey(map) || !inspirationValues[map].ContainsKey(pawn))
            {
                Log.Message("PrisonLabor Warning: InspirationTracker.GetInsipirationValue() didn't find map or pawn. Maybe it was called before calculating values.");
                return 0;
            }
            lock (inspirationValues)
            {
                return inspirationValues[map][pawn];
            }
        }

        public static void Calculate(Map map)
        {
            lock (inspirationValues)
            {
                var wardens = new List<Pawn>();
                wardens.AddRange(map.mapPawns.FreeColonists);
                var prisoners = new List<Pawn>();
                prisoners.AddRange(map.mapPawns.PrisonersOfColony);

                Dictionary<Pawn, float> mapCalculations;
                if (inspirationValues.TryGetValue(map, out mapCalculations))
                    mapCalculations.Clear();
                else
                    inspirationValues[map] = mapCalculations = new Dictionary<Pawn, float>();

                foreach (var prisoner in prisoners)
                {
                    mapCalculations[prisoner] = 0f;
                }

                var inRange = new Dictionary<Pawn, float>();
                foreach (var warden in wardens)
                {
                    inRange.Clear();
                    foreach (var prisoner in prisoners)
                    {
                        float distance = warden.Position.DistanceTo(prisoner.Position);
                        if (distance < BGP.InpirationRange && prisoner.GetRoom() == warden.GetRoom())
                            inRange.Add(prisoner, distance);
                    }

                    var watchedPawns = new List<Pawn>(inRange.Keys);
                    float points;
                    if (inRange.Count > BGP.WardenCapacity)
                    {
                        watchedPawns.Sort(new Comparison<Pawn>((x, y) => inRange[x].CompareTo(inRange[y])));
                        points = BGP.InspireRate / BGP.WardenCapacity;
                    }
                    else
                    {
                        points = BGP.InspireRate / inRange.Count;
                    }

                    for (int i = 0; i < watchedPawns.Count && i < BGP.WardenCapacity; i++)
                    {
                        mapCalculations[watchedPawns[i]] += points;
                    }
                }
            }
        }
    }
}