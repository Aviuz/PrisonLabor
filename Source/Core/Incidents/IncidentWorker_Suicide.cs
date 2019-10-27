using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;

namespace PrisonLabor.Core.Incidents
{
    public class IncidentWorker_Suicide : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = parms.target as Map;

            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.IsColonist)
                    continue;

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
                if (pawn.IsColonist)
                    continue;

                var need = pawn.needs.TryGetNeed<Need_Treatment>();
                if (need == null)
                    continue;

                if (need.CurCategory <= TreatmentCategory.Bad)
                {
                    // If treatment is only bad reduce chance by 50%
                    if (need.CurCategory == TreatmentCategory.Bad && !parms.forced)
                    {
                        if (UnityEngine.Random.value < 0.5f)
                            continue;
                    }

                    SendStandardLetter(new TargetInfo(pawn.Position, pawn.Map), null, new string[] { pawn.Name.ToStringShort });
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