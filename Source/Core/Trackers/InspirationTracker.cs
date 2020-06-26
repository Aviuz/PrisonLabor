using HarmonyLib;
using PrisonLabor.Constants;
<<<<<<< HEAD
using PrisonLabor.Core.AI.ThoughtWorkers;
using PrisonLabor.Core.Needs;
using RimWorld;
=======
>>>>>>> parent of c1f9f4e... Merge pull request #1 from kbatbouta/patch-2
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Verse;
using Verse.Noise;

namespace PrisonLabor.Core.Trackers
{
    public static class InspirationTracker
    {

<<<<<<< HEAD
        public static ThoughtDef defOfTreatedBadly;
        public static ThoughtDef defOfTreatedWell;
        public static ThoughtDef defOfLowMotivation;
        public static ThoughtDef defOfHorrer;

        public static Map map;

        private static List<Pawn> _prisoners;
        private static List<Pawn> _wardens;

        private static Dictionary<Pawn, float> _motivation = new Dictionary<Pawn, float>();
        private static Dictionary<Pawn, float> _dmotivation = new Dictionary<Pawn, float>();

        private static Dictionary<Pawn, bool> _watched = new Dictionary<Pawn, bool>();


        private static Dictionary<int, int[]> _memo;

        private static bool _currentIsWarden = false;

        private static int _currentPawn = 0;

        private static int _cycle = 0;


        public static bool Calculate()
=======
        /// <summary>
        /// Check if pawn is watched(supervised) by a Jailor
        /// </summary>
        public static bool IsWatched(this Pawn pawn)
>>>>>>> parent of c1f9f4e... Merge pull request #1 from kbatbouta/patch-2
        {
            if (!Gen.IsHashIntervalTick(map.Parent, 24))
            {
                return false;
            }

            if (_cycle == 0)
            {
                _prisoners = map.mapPawns.PrisonersOfColony;
                _wardens = map.mapPawns.FreeColonists;

                if (_prisoners.Count == 0 || _wardens.Count == 0)
                {
                    return false;
                }
                if (_memo == null)
                {
                    _memo = new Dictionary<int, int[]>();
                }

                _memo.Clear();

                _currentIsWarden = true;
                _currentPawn = 0;

                _cycle = 1;
            }
            else if (_cycle == 1)
            {
                if (_currentPawn >= _wardens.Count && _currentIsWarden)
                {
                    _currentIsWarden = false;
                    _currentPawn = 0;
                }

                Pawn p = null;

                if (_currentIsWarden)
                {
                    p = _wardens[_currentPawn];
                }
                else
                {
                    if (_currentPawn + 1 >= _prisoners.Count)
                    {
                        _cycle = 2;
                    }

                    p = _prisoners[_currentPawn];
                }

                _currentPawn += 1;
                if (p == null)
                {
                    return false;
                }

                UpdateRoom(p, p.GetRoom());
            }
            else if (_cycle == 2)
            {
                FinilizeUpdates();

                _currentPawn = 0;
                _cycle = 0;
            }

            return true;
        }

        public static void UpdateRoom(Pawn p, Room room)
        {
            if (room == null)
            {
                return;
            }

            if (room.CellCount > 40 * 40)
            {
                return;
            }

            var rId = room.ID;

            if (!_memo.ContainsKey(rId))
            {
                _memo.Add(rId, new int[2]);
            }

            if (p.IsColonist)
            {
                _memo[rId][0] += 1;
            }
            else if (p.IsPrisoner)
            {
                _memo[rId][1] += 1;
            }
        }

        public static void FinilizeUpdates()
        {
<<<<<<< HEAD
            foreach (Pawn p in _prisoners)
            {
                if (p.GetRoom() == null)
                {
                    _motivation[p] = -0.01f;
                    _dmotivation[p] = -0.0f;
                    _watched[p] = false;
                    continue;
                }

                var rId = p.GetRoom().ID;
                if (!_memo.ContainsKey(rId))
                {
                    _motivation[p] = -0.01f;
                    _dmotivation[p] = -0.0f;
                    _watched[p] = false;
                    continue;
                }

                if (!_motivation.ContainsKey(p))
                {
                    _motivation[p] = -0.01f;
                    _dmotivation[p] = -0.0f;
                    _watched[p] = false;
                }

                // TODO: update
                //var current = _memo[rId][0;

                if (_memo[rId][0] > 0)
                {
                    if (_memo[rId][1] < BGP.WardenCapacity)
                    {
                        //_motivation[p] += _memo[rId][0] * (BGP.InspireRate / _memo[rId][1]) + _dmotivation[p] * 5f * _prisoners.Count;

                        _dmotivation[p] = _dmotivation[p] * 0.75f;
                        _motivation[p] = (BGP.InspireRate / _memo[rId][1]) * _memo[rId][0] * 8f + _dmotivation[p] * 5.5f * _memo[rId][0];
                    }
                    else
                    {
                        //_motivation[p] -= (BGP.InspireRate / BGP.WardenCapacity) + _dmotivation[p] * 5f * BGP.WardenCapacity;

                        _dmotivation[p] = _dmotivation[p] * 0.75f;
                        _motivation[p] = (BGP.InspireRate / BGP.WardenCapacity) * _memo[rId][0] * 8f + _dmotivation[p] * 5.5f * _memo[rId][0];
=======
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
>>>>>>> parent of c1f9f4e... Merge pull request #1 from kbatbouta/patch-2
                    }
                    _watched[p] = true;
                }
                else
                {
                    _dmotivation[p] = _dmotivation[p] * 0.85f;
                    _motivation[p] = _memo[rId][1] * 25f * -0.0001f;
                    _watched[p] = false;
                }

<<<<<<< HEAD
#if TRACE
                Log.Message("1. if unmotivated of pawn " + p.Name.ToString() + " " + _memo[rId][1] * 25f * -0.0001f);
                Log.Message("2. _motivation of pawn " + p.Name.ToString() + " " + _motivation[p] + ", " + _dmotivation[p]);
                Log.Message("3. BGP capacity for room " + rId + " is, for" + p.Name.ToString() + ", " + (BGP.InspireRate / _memo[rId][1]));
#endif
            }
        }

        public static bool IsWatched(this Pawn pawn)
        {
            if (!_watched.ContainsKey(pawn))
            {
                _watched[pawn] = false;
                return false;
            }
            return _watched[pawn];
        }

        public static float GetInsiprationValue(Pawn pawn, bool refresh = false)
        {
            if (!_motivation.ContainsKey(pawn))
            {
                _motivation[pawn] = -0.001f;
                _dmotivation[pawn] = -0.0f;
                return 0.0f;
            }
            return _motivation[pawn];
        }

        public static void PrisonerToPrisoner(Pawn prisoner, Pawn other)
        {
            _prisoners = map.mapPawns.PrisonersOfColonySpawned;

#if TRACE
            Log.Message("Interation between " + prisoner.Name.ToString() + " " + other.Name.ToString());
#endif
            // Backstory
            // Been in solitary punishment...
            //
            if (defOfTreatedBadly.Worker.CurrentState(other).Active)
            {
                // TODO: headiff affraid...
                // fear of god!!
                // add fear headiff.
                _dmotivation[prisoner] = 0.0015f;

                //var memoryHandler = new MemoryThoughtHandler(prisoner);
                prisoner.needs.mood.thoughts.memories.TryGainMemory(defOfHorrer);

#if DEBUG
                Log.Message("Detected bad treatment!!");
#endif
            }
            else if (defOfTreatedWell.Worker.CurrentState(other).Active || defOfLowMotivation.Worker.CurrentState(other).Active)
            {
                // TODO: Motivation leak...
                _dmotivation[prisoner] = -0.0015f;
            }
#if TRACE
            Log.Message("o. Interation between " + prisoner.Name.ToString() + " " + other.Name.ToString() + " " + prisoner.needs.mood.CurLevelPercentage);
            Log.Message("o. Interation between changed _dmotivation to , for" + prisoner.Name.ToString() + ", " + _dmotivation[prisoner]);
#endif

        }

        public static void PrisonerToWarden(Pawn prisoner, Pawn warden)
        {
            _prisoners = map.mapPawns.PrisonersOfColonySpawned;

            if (!_watched.ContainsKey(prisoner))
            {
                _watched.Add(prisoner, true);
            }

            _watched[prisoner] = true;

            var opinion = prisoner.relations.OpinionOf(warden);

            if (opinion > 15)
            {
                // TODO: psyhc multipliers
                // Thought increase work productivity but increase escape change
                //
                // Do not motivate
                // 0.10 of motivation.
                _dmotivation[prisoner] = -0.0059f;

            }
            else if (opinion > 0)
            {
                // TODO: normal (legacy system)
                // minimal motivation
                // 0.50 motivation.             
                _dmotivation[prisoner] = -0.0015f;
            }
            else if (opinion < -40)
            {
                // TODO: headiff affraid...
                // fear of god!!
                // add fear headiff.
                _dmotivation[prisoner] = 0.0015f;
            }
#if TRACE
            Log.Message("o. Interation between " + prisoner.Name.ToString() + " " + warden.Name.ToString() + " / opinion " + opinion);
            Log.Message("o. Interation between changed _dmotivation to , for" + prisoner.Name.ToString() + ", " + _dmotivation[prisoner]);
#endif
=======
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
>>>>>>> parent of c1f9f4e... Merge pull request #1 from kbatbouta/patch-2
        }
    }
}