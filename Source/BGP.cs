using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrisonLabor
{
    /// <summary>
    /// ||Balanced Gameplay Parameters||
    /// Defined constants for balananced gameplay,
    /// stored in one place for optimized re-balancing
    /// </summary>
    public static class BGP
    {
        #region Insipiration
        public const float InspireRate = 0.015f;
        public const int WardenCapacity = (int)(InspireRate / Laziness_LazyRate);
        public const float InpirationRange = 10.0f;
        #endregion

        #region Laziness
        public const float Laziness_LazyRate = 0.002f;
        public const float Laziness_HungryRate = 0.006f;
        public const float Laziness_TiredRate = 0.006f;
        public const float Laziness_HealthRate = 0.006f;
        public const float Laziness_JoyRate = 0.001f;
        #endregion

        #region Escape
        // Escape time = ax + b (x -- treatment level)
        public const int Escape_MinLevel = 100;
        public const int Escape_MaxLevel = 5000;
        public const float Escape_LevelTreatmentMultiplier = 7000;
        public const int Escape_LevelBase = -950;
        #endregion

        #region Treatment
        public const float ResocializationLevel = 0.1f;

        // 10% every 12 days
        public const float LaborRate = 1f / (120f * GenDate.TicksPerDay / 150f);
        // 1% every 12 days for every point of status
        public const float StatusMultiplier = 1f / (1200f * GenDate.TicksPerDay / 150f);
        // 10% every 6 days
        public const float JoyRate = 1f / (60f * GenDate.TicksPerDay / 150f);

        public const float BeatenHit = -0.1f;
        #endregion
    }
}
