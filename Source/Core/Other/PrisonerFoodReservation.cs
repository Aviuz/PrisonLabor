using System.Collections.Generic;
using RimWorld;
using Verse;

namespace PrisonLabor.Core.Other
{
    internal class PrisonerFoodReservation
    {
        private static readonly Dictionary<Thing, Pawn> reservation = new Dictionary<Thing, Pawn>();

        public static bool IsReserved(Thing t)
        {
            Pawn p;
            reservation.TryGetValue(t, out p);
            if (p != null && t!= null && p.GetRoom() == t.GetRoom() && p.needs != null && p.needs.food != null && p.needs.food.CurCategory != HungerCategory.Fed)
                return true;
            return false;
        }

        public static void reserve(Thing t, Pawn p)
        {
            if (!reservation.ContainsKey(t))
                reservation.Add(t, p);
            else
                reservation[t] = p;

            if (reservation.Count > 50)
                clearEatenFood();
        }

        private static void clearEatenFood()
        {
            var removeList = new List<Thing>();
            foreach (var t in reservation.Keys)
                if (t == null || t.GetRoom() == null || !IsReserved(t))
                    removeList.Add(t);
            foreach (var t in removeList)
                reservation.Remove(t);
        }
    }
}