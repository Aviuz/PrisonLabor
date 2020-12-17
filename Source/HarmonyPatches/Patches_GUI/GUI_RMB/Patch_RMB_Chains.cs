using HarmonyLib;
using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_GUI.GUI_RMB
{
    [HarmonyPatch(typeof(FloatMenuMakerMap))]
    [HarmonyPatch("AddHumanlikeOrders")]
    class Patch_RMB_Chains
    {
		static void Postfix(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
        {
			TargetingParameters targetParams = new TargetingParameters()
			{
				canTargetHumans = true,
				mapObjectTargetsMustBeAutoAttackable = false,
			};

			var validtargets = GenUI.TargetsAt(clickPos, targetParams);

            foreach (LocalTargetInfo target in validtargets)
            {
                if(target.Pawn != null && target.Pawn.IsPrisonerOfColony && pawn.CanReach(target, PathEndMode.ClosestTouch, Danger.Deadly))
                {
                    
                    opts.AddDistinct(AddOption(pawn, target, "PrisonLabor_ToggleLegscuffs", PL_DefOf.PrisonLabor_HandlePrisonersLegChain));
                    opts.AddDistinct(AddOption(pawn, target, "PrisonLabor_ToggleHandscuffs", PL_DefOf.PrisonLabor_HandlePrisonersHandChain));
                }
            }
        }


        private static FloatMenuOption AddOption(Pawn pawn, LocalTargetInfo target, string keyname, JobDef jobdef)
        {
           return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(keyname.Translate(), delegate ()
            {
                pawn.jobs.TryTakeOrderedJob(new Job(jobdef, target.Pawn));
            }, MenuOptionPriority.High), pawn, target);
        }
    }
}
