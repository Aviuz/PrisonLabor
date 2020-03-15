using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using PrisonLabor.Constants;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Other;

namespace PrisonLabor.Core.Incidents
{
    public class IncidentWorker_ResocializationOffer : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.IsColonist)
                    continue;

                var treatment = pawn.needs.TryGetNeed<Need_Treatment>();
                if (treatment == null)
                    continue;

                if (treatment.CurLevel >= BGP.ResocializationLevel)
                    return true;
            }

            return false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn prisoner = null;
            var affectedPawns = new List<Pawn>(map.mapPawns.PrisonersOfColony);
            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.IsColonist)
                    continue;

                var treatment = pawn.needs.TryGetNeed<Need_Treatment>();
                if (treatment == null)
                    continue;

                if (treatment.CurLevel >= BGP.ResocializationLevel)
                {
                    treatment.ResocializationReady = true;
                    parms.faction = pawn.Faction;
                    prisoner = pawn;
                    break;
                }
            }
            if (prisoner == null)
                return false;

            Tutorials.Treatment();

            SendStandardLetter(parms, prisoner, prisoner.Name.ToStringShort, prisoner.Faction.Name);
            return true;
        }
    }
}