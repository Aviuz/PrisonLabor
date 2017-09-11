using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class MotivationUtility
    {
        public static Dictionary<Map, Dictionary<Pawn, float>> calculatedValues = new Dictionary<Map, Dictionary<Pawn, float>>();

        public static float GetMotivationDif(Pawn pawn)
        {
            Map map = pawn.Map;
            if (!calculatedValues.ContainsKey(map) || !calculatedValues[map].ContainsKey(pawn))
                Calculate(map);
            float value = calculatedValues[map][pawn];
            calculatedValues[map].Remove(pawn);
            return value;
        }

        private static void Calculate(Map map)
        {
            List<Pawn> wardens = new List<Pawn>();
            wardens.AddRange(map.mapPawns.FreeColonists);
            List<Pawn> prisoners = new List<Pawn>();
            prisoners.AddRange(map.mapPawns.PrisonersOfColony);

            calculatedValues[map] = new Dictionary<Pawn, float>();
            foreach (var prisoner in prisoners)
                calculatedValues[map][prisoner] = 0f;

            List<Pawn> prisonersInRange = new List<Pawn>();
            foreach(var warden in wardens)
            {
                prisonersInRange.Clear();
                foreach (var prisoner in prisoners)
                    if (warden.Position.DistanceTo(prisoner.Position) < Need_Motivation.InpirationRange && prisoner.GetRoom() == warden.GetRoom())
                        prisonersInRange.Add(prisoner);

                float delta = Need_Motivation.InspireRate / prisonersInRange.Count;

                foreach (var prisoner in prisonersInRange)
                    calculatedValues[map][prisoner] += delta;
            }
        }
    }
}
