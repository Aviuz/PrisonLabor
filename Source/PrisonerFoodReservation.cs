using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{
    class PrisonerFoodReservation
    {
        private static List<Thing> reservation = new List<Thing>();

        public static bool isReserved(Thing t)
        {
            if (reservation.Contains(t))
                return true;
            else
                return false;
        }

        public static void reserve(Thing t)
        {
            reservation.Add(t);
        }
    }
}
