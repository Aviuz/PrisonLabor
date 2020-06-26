using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace PrisonLabor.Core.Trackers
{
    internal static class InspirationTracker
    {
        public static Dictionary<Map, Dictionary<Pawn, float>> inspirationValues = new Dictionary<Map, Dictionary<Pawn, float>>();

        private static UInt16 tickCounter = 32;
        private static UInt16 updateInterval = 32;

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
            if (InspirationTracker.tickCounter >= InspirationTracker.updateInterval)
            {
                if (map.mapPawns.PrisonersOfColonyCount == 0)
                {
                    return;
                }

                InspirationTracker.tickCounter = 0;

                lock (inspirationValues)
                {

                    var wardens = new List<Pawn>();
                    wardens.AddRange(map.mapPawns.FreeColonists);

                    var prisoners = new List<Pawn>();
                    prisoners.AddRange(map.mapPawns.PrisonersOfColony);

                    if (wardens.Count == 0 || prisoners.Count == 0)
                    {
                        return;
                    }
                    
                    Dictionary<Pawn, float> mapCalculations;
                    if (inspirationValues.TryGetValue(map, out mapCalculations))
                    {
                        mapCalculations.Clear();
                    }
                    else
                    {
                        inspirationValues[map] = mapCalculations = new Dictionary<Pawn, float>();
                    }

                    foreach (var prisoner in prisoners)
                    {
                        mapCalculations[prisoner] = 0f;
                    }

                    var roomsIdtoWardens = new Dictionary<int, List<Pawn>>();
                    var wardenCurrentPrisoners = new Dictionary<Pawn, List<Pawn>>();

                    foreach (var warden in wardens)
                    {
                        if(warden.GetRoom() == null)
                        {
                            continue;
                        }

                        var roomId = warden.GetRoom().ID;

                        if (!roomsIdtoWardens.ContainsKey(roomId))
                        {
                            roomsIdtoWardens[roomId] = new List<Pawn>();
                        }

                        roomsIdtoWardens[roomId].Add(warden);
                    }

                    foreach (var prisoner in prisoners)
                    {
                        if (prisoner.GetRoom() == null || !roomsIdtoWardens.ContainsKey(prisoner.GetRoom().ID))
                        {
                            continue;
                        }
                        var roomId = prisoner.GetRoom().ID;

                        foreach (var warden in roomsIdtoWardens[roomId])
                        {
                            float distance = warden.Position.DistanceTo(prisoner.Position);
                            if (distance < BGP.InpirationRange)
                            {
                                if (!wardenCurrentPrisoners.ContainsKey(warden))
                                {
                                    wardenCurrentPrisoners.Add(warden, new List<Pawn>());
                                }

                                wardenCurrentPrisoners[warden].Add(prisoner);
                            }
                        }
                    }

                    float points = 0.0f;
                    foreach (var warden in wardenCurrentPrisoners.Keys)
                    {
                        if (wardenCurrentPrisoners[warden].Count > BGP.WardenCapacity)
                        {
                            points = BGP.InspireRate / BGP.WardenCapacity;
                        }
                        else
                        {
                            points = BGP.InspireRate / wardenCurrentPrisoners[warden].Count;
                        }
                        foreach (var prisoner in wardenCurrentPrisoners[warden])
                        {                            
                            mapCalculations[prisoner] += points * updateInterval;
                        }
                    }
                }
            }
            else
            {
                InspirationTracker.tickCounter++;
            }
        }
    }
}
