using System.Collections.Generic;
using Verse;

namespace PrisonLabor
{
    internal class InspirationUtility
    {
        public static Dictionary<Map, Dictionary<Pawn, float>> calculatedValues =
            new Dictionary<Map, Dictionary<Pawn, float>>();

        public static float GetInsiprationValue(Pawn pawn)
        {
            var map = pawn.Map;
            if (!calculatedValues.ContainsKey(map) || !calculatedValues[map].ContainsKey(pawn))
                Calculate(map);
            var value = calculatedValues[map][pawn];
            calculatedValues[map].Remove(pawn);
            return value;
        }

        private static void Calculate(Map map)
        {
            var wardens = new List<Pawn>();
            wardens.AddRange(map.mapPawns.FreeColonists);
            var prisoners = new List<Pawn>();
            prisoners.AddRange(map.mapPawns.PrisonersOfColony);

            calculatedValues[map] = new Dictionary<Pawn, float>();
            foreach (var prisoner in prisoners)
                calculatedValues[map][prisoner] = 0f;

            var prisonersInRange = new List<Pawn>();
            foreach (var warden in wardens)
            {
                prisonersInRange.Clear();
                foreach (var prisoner in prisoners)
                    if (warden.Position.DistanceTo(prisoner.Position) < Need_Motivation.InpirationRange &&
                        prisoner.GetRoom() == warden.GetRoom())
                        prisonersInRange.Add(prisoner);

                var delta = Need_Motivation.InspireRate / prisonersInRange.Count;

                foreach (var prisoner in prisonersInRange)
                    calculatedValues[map][prisoner] += delta;
            }
        }
    }
}