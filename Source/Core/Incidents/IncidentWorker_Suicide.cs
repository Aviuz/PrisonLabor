using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;
using PrisonLabor.Core.Meta;

namespace PrisonLabor.Core.Incidents
{
    public class IncidentWorker_Suicide : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!PrisonLaborPrefs.EnableSuicide)
                return false;

            Map map = parms.target as Map;

            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.IsColonist || !pawn.Spawned || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) == 0f || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) == 0f)
                {
                    continue;
                }
                var need = pawn.needs.TryGetNeed<Need_Treatment>();
                if (need == null)
                    continue;

                if (need.CurCategory <= TreatmentCategory.Bad)
                    return true;
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            var affectedPawns = new List<Pawn>(map.mapPawns.PrisonersOfColony);
            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.IsColonist || !pawn.Spawned || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Manipulation) == 0f || pawn.health.capacities.GetLevel(PawnCapacityDefOf.Consciousness) == 0f)
                    continue;

                var need = pawn.needs.TryGetNeed<Need_Treatment>();
                if (need == null)
                    continue;

                if (need.CurCategory <= TreatmentCategory.Bad)
                {
                    // If treatment is only bad reduce chance by 50%
                    if (need.CurCategory == TreatmentCategory.Bad && !parms.forced)
                    {
                        if (Verse.Rand.Value < 0.5f)
                            continue;
                    }

                    SendStandardLetter(parms, new TargetInfo(pawn.Position, pawn.Map), pawn.Name.ToStringShort);
                    parms.faction = pawn.Faction;

                    DamageInfo dinfo = new DamageInfo(DamageDefOf.Cut, 29, 0, 0, pawn, pawn.RaceProps.body.AllParts.Find(p => p.def == BodyPartDefOf.Neck));
                    while (!pawn.Dead)
                        pawn.TakeDamage(dinfo);

                    Tutorials.Treatment();

                    return true;
                }
            }
            return false;
        }
    }
}