using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    public class Need_Laziness : Need
    {
        private const float LazyLevel = 0.2f;
        private const float NeedInspirationLevel = 0.5f;
        private const float LazyRate = 0.003f;
        private const float InspireRate = 0.015f;
        public const int WardenCapacity = (int)(InspireRate / LazyRate);

        private static PrisonerInteractionModeDef pimDef;
        private static NeedDef def;

        private bool enabled;
        private bool needToBeInspired;
        private bool isLazy;
        private int wardensCount;
        private int prisonersCount;

        private int slowDown;
        
        public Need_Laziness(Pawn pawn) : base(pawn)
        {

        }

        public override void NeedInterval()
        {
            
        }
    }
}
