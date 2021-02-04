using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;

namespace PrisonLabor.Core.Other
{
	public static class CustomForbidenUtil
    {
		public static bool PrisonerCaresAboutForbidden(Pawn pawn, bool cellTarget)
		{
			if (!pawn.Spawned || !pawn.IsPrisonerOfColony && pawn.timetable.CurrentAssignment != TimeAssignmentDefOf.Work)
			{
				return false;
			}
			if (pawn.InMentalState)
			{
				return false;
			}
			if (cellTarget && ThinkNode_ConditionalShouldFollowMaster.ShouldFollowMaster(pawn))
			{
				return false;
			}
			return true;
		}

		public static bool IsFoodForbiden(this Thing t, Pawn pawn)
        {
			if (pawn.IsPrisonerOfColony)
			{
				DebugLogger.debug($"[PL] Pawn {pawn.LabelShort} checking null object");
			}
			return t != null && PrisonerFoodReservation.IsReserved(t) && !pawn.IsPrisoner;
		}

		public static bool IsForbiddenForPrisoner(this Thing t, Pawn pawn)
		{

			if (pawn.IsWatched() && ForbidUtility.IsForbidden(t, Faction.OfPlayer))
			{
				return true;
			}
			if (!PrisonerCaresAboutForbidden(pawn, cellTarget: false))
			{
				return false;
			}
			if (t != null && t.Spawned && t.Position.IsForbiddenForPrisoner(pawn))
			{
				return true;
			}
			Lord lord = pawn.GetLord();
			if (lord != null && lord.extraForbiddenThings.Contains(t))
			{
				return true;
			}
			return false;
		}

		public static bool IsForbiddenForPrisoner(this IntVec3 c, Pawn pawn)
		{
			if (!PrisonerCaresAboutForbidden(pawn, cellTarget: true))
			{
				return false;
			}
			if (!c.InAllowedArea(pawn))
			{
				return true;
			}
			if (pawn.mindState.maxDistToSquadFlag > 0f && !c.InHorDistOf(pawn.DutyLocation(), pawn.mindState.maxDistToSquadFlag))
			{
				return true;
			}
			return false;
		}
	}
}


