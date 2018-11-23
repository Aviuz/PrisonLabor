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
    }
}
