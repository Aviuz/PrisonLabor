using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
    [HarmonyPatch(typeof(JobDriver_Mine))]
    [HarmonyPatch("MakeNewToils")]
    class Patch_JobDriver_Mine
    {
        static IEnumerable<Toil> Postfix(IEnumerable<Toil> toilList, JobDriver_Mine __instance)
        {
			Log.Message("Postfix launch");
			Log.Message($"Toil size {toilList.Count()}");
			int counter = 1;
			int count = toilList.Count(); 
			foreach (var toil in toilList)
			{
				if (counter == count)
				{					
					toil.AddFinishAction(createDelegate(__instance));
				}
				counter++;
				yield return toil;
			}
			
		}

		static private Action createDelegate(JobDriver_Mine __instance)
		{
			return delegate
			{
				Log.Message("Toil entered");
				Thing mineTarget = __instance.job.GetTarget(TargetIndex.A).Thing;
				Pawn pawn = __instance.pawn;
				Log.Message("Toil before if");
				if (pawn != null && mineTarget != null && mineTarget.Destroyed)
				{
					Log.Message("After toil");
					if (pawn.IsPrisonerOfColony)
					{
						IntVec3 position = mineTarget.Position;
						List<Thing> thingList = position.GetThingList(pawn.Map);
						for (int i = 0; i < thingList.Count; i++)
						{
							Log.Message($"Setting forbid false for: {thingList[i]}");
							thingList[i].SetForbidden(value: false, warnOnFail: false);
						}

						if (MineStrikeManager.MineableIsVeryValuable(mineTarget.def))
						{
							TaleRecorder.RecordTale(TaleDefOf.MinedValuable, pawn, mineTarget.def.building.mineableThing);
						}
						if (MineStrikeManager.MineableIsValuable(mineTarget.def) && !pawn.Map.IsPlayerHome)
						{
							TaleRecorder.RecordTale(TaleDefOf.CaravanRemoteMining, pawn, mineTarget.def.building.mineableThing);
						}
					}
				}

			};
		}

    }
}
