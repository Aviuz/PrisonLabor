using System;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.AI.Group;
using System.Collections.Generic;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Other;

namespace PrisonLabor.Core.Incidents
{
    public class IncidentWorker_Revolt : IncidentWorker
    {
        private const float MaxMotivationToStart = 0.4f;
        private const float MaxTreatmentToStart = 3.5f;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = parms.target as Map;

            bool enemyFaction = false;
            float accumulatedMotivation = 0.0f;
            float accumulatedTreatment = 0.0f;
            int prisonersCount = 0;

            // Calculate values
            foreach (var pawn in map.mapPawns.PrisonersOfColony)
            {
                if (pawn.Faction.HostileTo(Faction.OfPlayer))
                    enemyFaction = true;

                var need = pawn.needs.TryGetNeed<Need_Motivation>();
                if (need == null)
                    continue;
                accumulatedMotivation += need.CurLevel;
                prisonersCount++;
            }

            // If motivation is too high
            if (accumulatedMotivation / prisonersCount >= MaxMotivationToStart)
                return false;
            // If treatment is too good
            if (accumulatedTreatment / prisonersCount >= MaxTreatmentToStart)
                return false;

            return enemyFaction && PrisonLaborPrefs.EnableRevolts;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            try
            {
                Map map = (Map)parms.target;
                Pawn t = null;
                var affectedPawns = new List<Pawn>(map.mapPawns.PrisonersOfColony);

                // Calculate chance for blocking incident if prisoners are treated good
                float treatment = 0f;
                float chance = 0f;
                foreach (Pawn pawn in affectedPawns)
                    if (pawn.needs.TryGetNeed<Need_Treatment>() != null)
                        treatment += (float)pawn.needs.TryGetNeed<Need_Treatment>().CurCategory;
                treatment = treatment / affectedPawns.Count;
                if (treatment < 0.5)
                    chance = 1f;
                else if (treatment < 1.5)
                    chance = 0.95f;
                else if (treatment < 2.5)
                    chance = 0.5f;
                else if (treatment < 3.5)
                    chance = 0.1f;

                // When incident is forced, log instead of blocking
                if (!parms.forced)
                {
                    if (Prefs.DevMode)
                    {
                        string msg = $"Prison Labor: Revolt blocking chance is currently equal to {chance * 100}% (overall treatment = {treatment}). Rolling ...";
                        Log.Message(msg);
                    }
                    if (Verse.Rand.Value > chance)
                        return false;
                }


                foreach (Pawn pawn in affectedPawns)
                {
                    if (pawn.Faction.HostileTo(Faction.OfPlayer))
                    {
                        parms.faction = pawn.Faction;
                        t = pawn;
                        break;
                    }
                }
                float points = parms.points;
                int prisonersLeft = affectedPawns.Count;
                foreach (Pawn pawn in affectedPawns)
                {
                    pawn.ClearMind();
                    pawn.guest.SetGuestStatus(null, GuestStatus.Guest);
                    pawn.SetFaction(parms.faction);

                    ThingWithComps weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MeleeWeapon_Knife"), ThingDefOf.WoodLog) as ThingWithComps;
                    ThingWithComps ammo = null;
                    int pointsToRemove = 0;

                    if (parms.points >= 1000)
                        weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MeleeWeapon_Knife"), ThingDefOf.Steel) as ThingWithComps;

                    if (points >= 1000)
                    {
                        // If combat extended is enabled
                        if (DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeStickBomb", false) != null)
                        {
                            if (Verse.Rand.Value > 0.5f)
                            {
                                weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeStickBomb")) as ThingWithComps;
                                ammo = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeStickBomb")) as ThingWithComps;
                                ammo.stackCount = 6;
                            }
                            else
                            {
                                weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeMolotov")) as ThingWithComps;
                                ammo = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeMolotov")) as ThingWithComps;
                                ammo.stackCount = 6;
                            }
                        }
                        else
                        {
                            weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeMolotov")) as ThingWithComps;
                        }

                        pointsToRemove = 500;
                    }
                    else if (points >= 500)
                    {
                        weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Bow_Short")) as ThingWithComps;

                        if (DefDatabase<ThingDef>.GetNamed("Ammo_Arrow_Stone", false) != null)
                        {
                            ammo = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("Ammo_Arrow_Stone")) as ThingWithComps;
                            ammo.stackCount = 30;
                        }

                        pointsToRemove = 100;
                    }
                    else if (points >= 300)
                    {
                        weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MeleeWeapon_Club"), ThingDefOf.Granite) as ThingWithComps;
                        pointsToRemove = 100;
                    }

                    if (pawn.equipment.Primary == null)
                    {
                        pawn.equipment.AddEquipment(weapon);
                        if (ammo != null)
                            pawn.inventory.innerContainer.TryAdd(ammo);
                        points -= pointsToRemove;
                    }
                }
                var lordJob = new LordJob_AssaultColony(parms.faction, true, true, false, true, true);
                //TODO old code:
                LordMaker.MakeNewLord(parms.faction, lordJob/*(new RaidStrategyWorker_ImmediateAttackSmart()).MakeLordJob(parms, map)*/, map, affectedPawns);
                SendStandardLetter(parms, t, t.Name.ToStringShort, t.Faction.Name);
                Find.TickManager.slower.SignalForceNormalSpeedShort();

                Tutorials.Treatment();

                return true;
            }
            catch(Exception e)
            {
                Log.Error($"PrisonLabor: Erron on executing Revolt Incident: {e.ToString()}");
                return false;
            }
        }
    }
}