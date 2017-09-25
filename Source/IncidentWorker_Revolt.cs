using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;

namespace PrisonLabor
{
    public class IncidentWorker_Revolt : IncidentWorker
    {
        private const float HivePoints = 400f;

        protected override bool CanFireNowSub(IIncidentTarget target)
        {
            Map map = (Map)target;

            bool enemyFaction = false;
            float accumulatedMotivation = 0.0f;
            int prisonersCount = 0;

            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.Faction.HostileTo(Faction.OfPlayer))
                    enemyFaction = true;

                accumulatedMotivation += pawn.needs.TryGetNeed<Need_Motivation>().CurLevel;
                prisonersCount++;
            }

            //TODO balance numbers
            if (accumulatedMotivation / prisonersCount > 0.5f)
                return false;

            return enemyFaction;
        }

        public override bool TryExecute(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            Pawn t = null;
            var affectedPawns = new List<Pawn>(map.mapPawns.PrisonersOfColony);
            foreach (Pawn pawn in affectedPawns)
            {
                if (pawn.Faction.HostileTo(Faction.OfPlayer))
                {
                    parms.faction = pawn.Faction;
                    t = pawn;
                    break;
                }
            }
            foreach (Pawn pawn in affectedPawns)
            {
                pawn.ClearMind();
                pawn.guest.SetGuestStatus(null, false);
                pawn.SetFaction(parms.faction);
                pawn.equipment.AddEquipment(ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MeleeWeapon_Shiv"), ThingDefOf.WoodLog) as ThingWithComps);
            }
            LordMaker.MakeNewLord(parms.faction, (new RaidStrategyWorker_ImmediateAttackSmart()).MakeLordJob(parms, map), map, affectedPawns);
            base.SendStandardLetter(t, new string[] { t.NameStringShort, t.Faction.Name });
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}