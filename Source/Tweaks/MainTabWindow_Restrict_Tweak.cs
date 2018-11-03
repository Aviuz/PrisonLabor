using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor.Tweaks
{
    public class MainTabWindow_Restrict_Tweak : MainTabWindow_Dual
    {
        public static Type MainTabWindowType { get; set; }

        protected override Type InnerTabType => MainTabWindowType;
    }
}
