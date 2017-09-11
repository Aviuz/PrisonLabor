using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Harmony;
using UnityEngine;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;

namespace PrisonLabor.HarmonyPatches
{
    [HarmonyPatch(typeof(Map))]
    [HarmonyPatch("FinalizeInit")]
    [HarmonyPatch(new Type[] {})]
    class ShowNewsPatch
    {
        private static void Postfix()
        {
            NewsDialog.TryShow();
        }
    }
}
