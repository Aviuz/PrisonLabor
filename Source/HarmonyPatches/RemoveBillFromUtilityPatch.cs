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

    [HarmonyPatch(typeof(BillStack))]
    [HarmonyPatch("Delete")]
    [HarmonyPatch(new Type[] { typeof(Bill) })]
    class RemoveBillFromUtilityPatch
    {
        public static void Postfix(Bill bill)
        {
            BillUtility.Remove(bill);
        }
    }
}
