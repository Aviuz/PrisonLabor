using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace PrisonLabor
{
    class Laziness
    {
        internal const double LAZY_LEVEL = 0.8;
        internal const double NEED_INSPIRATION_LEVEL = 0.5;
        internal const double LAZY_RATE = 0.03;
        internal const double INSPIRE_RATE = 0.15;
        public const int WARDEN_CAPACITY = (int)(INSPIRE_RATE/LAZY_RATE);

        private static Dictionary<Pawn, Laziness> map = new Dictionary<Pawn, Laziness>();
        private static PrisonerInteractionModeDef pimDef;

        private double level;
        private bool needToBeInspired;
        private bool isLazy;

        public static PrisonerInteractionModeDef PimDef
        {
            get
            {
                if (pimDef == null)
                    pimDef = DefDatabase<PrisonerInteractionModeDef>.GetNamed("PrisonLabor_workOption");
                return pimDef;
            }
        }

        private Laziness()
        {
            level = 0;
            needToBeInspired = false;
        }

        public void change(double value)
        {
            level += value;
        }

        public bool IsLazy
        {
            get
            {
                return isLazy;
            }
        }

        public bool NeedToBeInspired
        {
            get
            {
                return needToBeInspired;
            }
        }

        public static Laziness pawn(Pawn pawn)
        {
            if (map.ContainsKey(pawn))
            {
                return map[pawn];
            }
            else
            {
                Laziness laz = new Laziness();
                map.Add(pawn, laz);
                return laz;
            }

        }

        public static void tick(Pawn pawn)
        {
            Laziness laz = Laziness.pawn(pawn);
            double oldValue = laz.level;

            List<Pawn> pawnsInRoom = new List<Pawn>();
            int prisonersCount = 0;
            int wardensCount = 0;
            foreach (IntVec3 cell in pawn.GetRoomGroup().Cells)
            {
                foreach (Thing thing in cell.GetThingList(pawn.Map))
                {
                    if (thing is Pawn)
                        pawnsInRoom.Add((Pawn)thing);
                }
            }
            foreach (Pawn p in pawnsInRoom)
            {
                // colonist nearby
                if (p.IsFreeColonist)
                    wardensCount++;
                if (p.IsPrisoner && p.guest.interactionMode == PimDef)
                    prisonersCount++;
            }

            laz.level = laz.level + LAZY_RATE < 1 ? laz.level + LAZY_RATE : 1;
            double insipre = (wardensCount * INSPIRE_RATE) / prisonersCount;
            laz.level = laz.level > insipre ? laz.level - insipre : 0;

            if (laz.level == 0)
                laz.needToBeInspired = false;
            if (laz.level >= NEED_INSPIRATION_LEVEL && !laz.needToBeInspired)
                laz.needToBeInspired = true;
            if (laz.level >= LAZY_LEVEL && !laz.isLazy && wardensCount == 0)
            {
                laz.isLazy = true;
                Messages.Message("Your prioner got lazy!", pawn, MessageSound.Standard);
                Tutorials.LazyPrisoner();
            }
            else if (laz.isLazy && wardensCount > 0)
            {
                laz.isLazy = false;
            }

            // For Denugging
            //Log.Message("Laziness of " + pawn.Name + " changed from " + oldValue + " to " + laz.level);
        }
    }
}
