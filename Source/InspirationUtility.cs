using System;
using System.Collections.Generic;
using Verse;

namespace PrisonLabor
{
    internal static class InspirationUtility
    {
        public static Dictionary<Map, Dictionary<Pawn, float>> inspirationValues = new Dictionary<Map, Dictionary<Pawn, float>>();

        /// <summary>
        /// Check if pawn is watched(supervised) by a Jailor
        /// </summary>
        public static bool IsWatched(this Pawn pawn)
        {
            Dictionary<Pawn, float> iValues;
            float value;

            var map = pawn.Map;
            if (inspirationValues.TryGetValue(map, out iValues))
            {
                if (iValues.TryGetValue(pawn, out value))
                {
                    return value != 0;
                }
            }
            return false;
        }

        public static float GetInsiprationValue(Pawn pawn)
        {
            var map = pawn.Map;
            if (!inspirationValues.ContainsKey(map) || !inspirationValues[map].ContainsKey(pawn))
                Calculate(map);
            var value = inspirationValues[map][pawn];
            inspirationValues[map].Remove(pawn);
            return value;
        }

        private static void Calculate(Map map)
        {
            var wardens = new List<Pawn>();
            wardens.AddRange(map.mapPawns.FreeColonists);
            var prisoners = new List<Pawn>();
            prisoners.AddRange(map.mapPawns.PrisonersOfColony);

            inspirationValues[map] = new Dictionary<Pawn, float>();
            foreach (var prisoner in prisoners)
                inspirationValues[map][prisoner] = 0f;

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
                    inspirationValues[map][watchedPawns[i]] += points;
                }
            }
        }
    }
}