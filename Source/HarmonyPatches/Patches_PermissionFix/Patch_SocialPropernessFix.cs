using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_PermissionFix
{
    /// <summary>
    /// This patch will fix conditions that allows prisoners only to use some activities inside room where he is.
    /// Now prisoners can do those activities if he can reach those objects from his bed.
    /// </summary>
    [HarmonyPatch(typeof(SocialProperness), "IsSociallyProper")]
    [HarmonyPatch(new Type[] { typeof(Thing), typeof(Pawn), typeof(bool), typeof(bool) })]
    static class Patch_SocialPropernessFix
    {
        static private IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, IEnumerable<CodeInstruction> instr)
        {
            OpCode[] opCodes =
            {
                OpCodes.Ldarg_2,
                OpCodes.Brfalse,
            };
            string[] operands =
            {
                "",
                "System.Reflection.Emit.Label",
            };
            int step = 0;

            foreach (var ci in instr)
            {
                yield return ci;

                if (HPatcher.IsFragment(opCodes, operands, ci, ref step, "Patch_SocialPropernessFix"))
                {
                    yield return new CodeInstruction(OpCodes.Ldc_I4_1);
                    yield return new CodeInstruction(OpCodes.Ret);
                }
            }
        }
    }
}

/*
ldarg.2 |  | no labels
brfalse | Label 8 | no labels
 */

/*
 *
// RimWorld.SocialProperness
public static bool IsSociallyProper(this Thing t, Pawn p, bool forPrisoner, bool animalsCare = false)
{
	if (!animalsCare && !p.RaceProps.Humanlike)
	{
		return true;
	}
	if (!t.def.socialPropernessMatters)
	{
		return true;
	}
	if (!t.Spawned)
	{
		return true;
	}
	IntVec3 intVec = (!t.def.hasInteractionCell) ? t.Position : t.InteractionCell;
	if (forPrisoner)
	{
		return intVec.GetRoom(t.Map, RegionType.Set_Passable) == p.GetRoom(RegionType.Set_Passable);
	}
	return !intVec.IsInPrisonCell(t.Map);
}
 * 
 * 
 */
