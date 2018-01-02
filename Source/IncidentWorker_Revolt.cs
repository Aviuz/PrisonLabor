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
        private const float MinMotivationToStart = 0.4f;

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

                var need = pawn.needs.TryGetNeed<Need_Motivation>();
                if (need == null)
                    continue;
                accumulatedMotivation += need.CurLevel;
                prisonersCount++;
            }

            if (accumulatedMotivation / prisonersCount > MinMotivationToStart)
                return false;

            return enemyFaction && PrisonLaborPrefs.EnableRevolts;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
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
            float points = parms.points;
            int prisonersLeft = affectedPawns.Count;
            foreach (Pawn pawn in affectedPawns)
            {
                pawn.ClearMind();
                pawn.guest.SetGuestStatus(null, false);
                pawn.SetFaction(parms.faction);

                ThingWithComps weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MeleeWeapon_Shiv"), ThingDefOf.WoodLog) as ThingWithComps;
                ThingWithComps ammo = null;
                int pointsToRemove = 0;

                if (parms.points >= 1000)
                    weapon = ThingMaker.MakeThing(DefDatabase<ThingDef>.GetNamed("MeleeWeapon_Shiv"), ThingDefOf.Steel) as ThingWithComps;

                if (points >= 1000)
                {
                    // If combat extended is enabled
                    if (DefDatabase<ThingDef>.GetNamed("Weapon_GrenadeStickBomb", false) != null)
                    {
                        if (UnityEngine.Random.value > 0.5f)
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
            LordMaker.MakeNewLord(parms.faction, (new RaidStrategyWorker_ImmediateAttackSmart()).MakeLordJob(parms, map), map, affectedPawns);
            base.SendStandardLetter(t, new string[] { t.NameStringShort, t.Faction.Name });
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }
    }
}