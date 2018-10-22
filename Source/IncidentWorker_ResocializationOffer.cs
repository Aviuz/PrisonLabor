﻿using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;

namespace PrisonLabor
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

                var need = pawn.needs.TryGetNeed<Need_Treatment>();
                if (need == null)
                    continue;

                if (need.CurLevel >= Need_Treatment.ResocializationLevel)
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

                var need = pawn.needs.TryGetNeed<Need_Treatment>();
                if (need == null)
                    continue;

                if (need.CurLevel >= Need_Treatment.ResocializationLevel)
                {
                    need.ResocializationReady = true;
                    parms.faction = pawn.Faction;
                    prisoner = pawn;
                    break;
                }
            }
            if (prisoner == null)
                return false;

            SendStandardLetter(prisoner, null, new string[] { prisoner.Name.ToStringShort, prisoner.Faction.Name });
            return true;
        }
    }
}