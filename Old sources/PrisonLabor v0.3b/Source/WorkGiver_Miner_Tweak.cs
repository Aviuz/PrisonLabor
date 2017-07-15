﻿using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public class WorkGiver_Miner_Tweak : WorkGiver_Scanner
    {
        public override PathEndMode PathEndMode
        {
            get
            {
                return PathEndMode.Touch;
            }
        }

        [DebuggerHidden]
        public override IEnumerable<Thing> PotentialWorkThingsGlobal(Pawn pawn)
        {
            foreach (Designation des in pawn.Map.designationManager.SpawnedDesignationsOfDef(DesignationDefOf.Mine))
            {
                bool mayBeAccessible = false;
                for (int i = 0; i < 8; i++)
                {
                    IntVec3 c = des.target.Cell + GenAdj.AdjacentCells[i];
                    if (c.InBounds(pawn.Map) && c.Walkable(pawn.Map))
                    {
                        mayBeAccessible = true;
                        break;
                    }
                }
                if (mayBeAccessible)
                {
                    Thing j = MineUtility.MineableInCell(des.target.Cell, pawn.Map);
                    if (j != null)
                    {
                        yield return j;
                    }
                }
            }
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (!pawn.IsPrisoner)
            {
                return null;
            }
            if (!t.def.mineable)
            {
                return null;
            }
            if (pawn.Map.designationManager.DesignationAt(t.Position, DesignationDefOf.Mine) == null)
            {
                return null;
            }
            if (!pawn.CanReserve(t, 1, -1, null, false))
            {
                return null;
            }
            bool flag = false;
            for (int i = 0; i < 8; i++)
            {
                IntVec3 intVec = t.Position + GenAdj.AdjacentCells[i];
                if (intVec.InBounds(pawn.Map) && intVec.Standable(pawn.Map) && ReachabilityImmediate.CanReachImmediate(intVec, t, pawn.Map, PathEndMode.Touch, pawn))
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                for (int j = 0; j < 8; j++)
                {
                    IntVec3 intVec2 = t.Position + GenAdj.AdjacentCells[j];
                    if (intVec2.InBounds(t.Map))
                    {
                        if (ReachabilityImmediate.CanReachImmediate(intVec2, t, pawn.Map, PathEndMode.Touch, pawn))
                        {
                            if (intVec2.Walkable(t.Map) && !intVec2.Standable(t.Map))
                            {
                                Thing firstHaulable = intVec2.GetFirstHaulable(t.Map);
                                if (firstHaulable != null && firstHaulable.def.passability == Traversability.PassThroughOnly)
                                {
                                    return HaulAIUtility.HaulAsideJobFor(pawn, firstHaulable);
                                }
                            }
                        }
                    }
                }
                return null;
            }
            return new Job(DefDatabase<JobDef>.GetNamed("PrisonLabor.Mine_Tweak"), t, 1500, true);
        }
    }
}
