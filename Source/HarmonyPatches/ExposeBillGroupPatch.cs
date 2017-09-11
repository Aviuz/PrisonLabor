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
    [HarmonyPatch(typeof(Bill))]
    [HarmonyPatch("ExposeData")]
    [HarmonyPatch(new Type[] {})]
    class ExposeBillGroupPatch
    {
        private static void Postfix(Bill __instance)
        {
            BillUtility.GetData(__instance).ExposeData();
        }
    }
}
