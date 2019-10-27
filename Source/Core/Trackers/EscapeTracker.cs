using PrisonLabor.Constants;
using PrisonLabor.Core.BaseClasses;
using PrisonLabor.Core.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Core.Trackers
{
    public class EscapeTracker : IExposable
    {
        #region Object Access
        /**
         * Object Access region:
         *  This region is for ensuring that for every pawn there will be only one escape tracker.
         *  It is constructed in this way to prevent heavy modification of Pawn class (on external library).
         */
        private static Dictionary<Pawn, EscapeTracker> escapeTimers = new Dictionary<Pawn, EscapeTracker>();

        /// <summary>
        /// Access EscapeTracker of Pawn
        /// </summary>
        public static EscapeTracker Of(Pawn pawn, bool forced = false)
        {
            if(!pawn.IsPrisoner && !forced)
            {
                return null;
            }
            if (!escapeTimers.ContainsKey(pawn))
            {
                escapeTimers.Add(pawn, new EscapeTracker(pawn));
            }
            return escapeTimers[pawn];
        }

        public static void Save(Pawn pawn, EscapeTracker tracker)
        {
            if(pawn != null && tracker != null)
            {
                escapeTimers[pawn] = tracker;
            }
        }
        #endregion

        private SimpleTimer timer = new SimpleTimer();

        /// <summary>
        /// Debug information
        /// </summary>
        public int ReadyToRunPercentage => timer.Ticks * 100 / EscapeLevel;

        /// <summary>
        /// Attached pawn
        /// </summary>
        public Pawn Pawn { get; private set; }

        /// <summary>
        /// Flag that indicates whenever pawn should escape when JobGiver is trying to assign him a job
        /// </summary>
        public bool ReadyToEscape { get; set; }

        /// <summary>
        /// This value is tracking whenever pawn is ready to run,
        /// in order to count time of this state
        /// </summary>
        public bool CanEscape
        {
            get => timer.IsActive;

            set
            {
                if (value == true)
                    timer.Start();
                else
                    timer.Stop();
            }
        }

        /// <summary>
        /// This value represent how long pawn will wait until he decides to escape.
        /// Measured in game ticks.
        /// </summary>
        public int EscapeLevel
        {
            get
            {
                var treatment = Pawn.needs?.TryGetNeed<Need_Treatment>();
                if (treatment == null || treatment.CurCategory <= TreatmentCategory.Bad)
                    return BGP.Escape_MinLevel;

                int level = (int)(treatment.CurLevel * BGP.Escape_LevelTreatmentMultiplier) + BGP.Escape_LevelBase;

                if (level < BGP.Escape_MinLevel)
                    return BGP.Escape_MinLevel;
                else if (level > BGP.Escape_MaxLevel)
                    return BGP.Escape_MaxLevel;
                else
                    return level;
            }
        }

        /// <summary>
        /// Creates new escape tracker attached to pawn
        /// </summary>
        /// <param name="pawn"></param>
        public EscapeTracker(Pawn pawn)
        {
            Pawn = pawn;
        }

        public void Tick()
        {
            // Reset all
            if (Pawn.IsWatched())
            {
                // Check if state isn't reseted already
                if (timer.Ticks != 0)
                {
                    timer.ResetAndStop();
                    ReadyToEscape = false;
                }
            }
            // Check if timer should trigger escape
            else if (timer.Ticks >= EscapeLevel)
            {
                ReadyToEscape = true;
            }

            // Tick timer
            timer.Tick();
        }

        public void ExposeData()
        {
            Scribe_Deep.Look(ref timer, "Timer");
        }
    }
}
