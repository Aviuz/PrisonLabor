﻿using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public static class FoodUtility_Tweak
    {
        private static HashSet<Thing> filtered = new HashSet<Thing>();

        private static readonly SimpleCurve FoodOptimalityEffectFromMoodCurve = new SimpleCurve
        {
            {
                new CurvePoint(-100f, -600f),
                true
            },
            {
                new CurvePoint(-10f, -100f),
                true
            },
            {
                new CurvePoint(-5f, -70f),
                true
            },
            {
                new CurvePoint(-1f, -50f),
                true
            },
            {
                new CurvePoint(0f, 0f),
                true
            },
            {
                new CurvePoint(100f, 800f),
                true
            }
        };

        private static List<ThoughtDef> ingestThoughts = new List<ThoughtDef>();

        public static bool TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, out Thing foodSource, out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true, bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false)
        {
            bool flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            bool allowDrug = !eater.IsTeetotaler();
            Thing thing = null;
            if (canUseInventory)
            {
                if (flag)
                {
                    thing = FoodUtility_Tweak.BestFoodInInventory(getter, null, FoodPreferability.MealAwful, FoodPreferability.MealLavish, 0f, false);
                }
                if (thing != null)
                {
                    if (getter.Faction != Faction.OfPlayer)
                    {
                        foodSource = thing;
                        foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                    CompRottable compRottable = thing.TryGetComp<CompRottable>();
                    if (compRottable != null && compRottable.Stage == RotStage.Fresh && compRottable.TicksUntilRotAtCurrentTemp < 30000)
                    {
                        foodSource = thing;
                        foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                }
            }
            bool allowPlant = getter == eater;
            Thing thing2 = FoodUtility_Tweak.BestFoodSourceOnMap(getter, eater, desperate, FoodPreferability.MealLavish, allowPlant, allowDrug, allowCorpse, true, canRefillDispenser, allowForbidden, allowSociallyImproper);
            if (thing == null && thing2 == null)
            {
                if (canUseInventory && flag)
                {
                    thing = FoodUtility_Tweak.BestFoodInInventory(getter, null, FoodPreferability.DesperateOnly, FoodPreferability.MealLavish, 0f, allowDrug);
                    if (thing != null)
                    {
                        foodSource = thing;
                        foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                }
                if (thing2 == null && getter == eater && getter.RaceProps.predator)
                {
                    Pawn pawn = FoodUtility_Tweak.BestPawnToHuntForPredator(getter);
                    if (pawn != null)
                    {
                        foodSource = pawn;
                        foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                }
                foodSource = null;
                foodDef = null;
                return false;
            }
            if (thing == null && thing2 != null)
            {
                foodSource = thing2;
                foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                return true;
            }
            if (thing2 == null && thing != null)
            {
                foodSource = thing;
                foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                return true;
            }
            float num = FoodUtility_Tweak.FoodSourceOptimality(eater, thing2, (float)(getter.Position - thing2.Position).LengthManhattan, false);
            float num2 = FoodUtility_Tweak.FoodSourceOptimality(eater, thing, 0f, false);
            num2 -= 32f;
            if (num > num2)
            {
                foodSource = thing2;
                foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
                return true;
            }
            foodSource = thing;
            foodDef = FoodUtility_Tweak.GetFinalIngestibleDef(foodSource);
            return true;
        }

        public static ThingDef GetFinalIngestibleDef(Thing foodSource)
        {
            Building_NutrientPasteDispenser building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
            if (building_NutrientPasteDispenser != null)
            {
                return building_NutrientPasteDispenser.DispensableDef;
            }
            Pawn pawn = foodSource as Pawn;
            if (pawn != null)
            {
                return pawn.RaceProps.corpseDef;
            }
            return foodSource.def;
        }

        public static Thing BestFoodInInventory(Pawn holder, Pawn eater = null, FoodPreferability minFoodPref = FoodPreferability.NeverForNutrition, FoodPreferability maxFoodPref = FoodPreferability.MealLavish, float minStackNutrition = 0f, bool allowDrug = false)
        {
            if (holder.inventory == null)
            {
                return null;
            }
            if (eater == null)
            {
                eater = holder;
            }
            ThingOwner<Thing> innerContainer = holder.inventory.innerContainer;
            for (int i = 0; i < innerContainer.Count; i++)
            {
                Thing thing = innerContainer[i];
                if (thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && eater.RaceProps.CanEverEat(thing) && thing.def.ingestible.preferability >= minFoodPref && thing.def.ingestible.preferability <= maxFoodPref && (allowDrug || !thing.def.IsDrug))
                {
                    float num = thing.def.ingestible.nutrition * (float)thing.stackCount;
                    if (num >= minStackNutrition)
                    {
                        return thing;
                    }
                }
            }
            return null;
        }

        public static Thing BestFoodSourceOnMap(Pawn getter, Pawn eater, bool desperate, FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true, bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true, bool allowForbidden = false, bool allowSociallyImproper = false)
        {
            bool getterCanManipulate = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            if (!getterCanManipulate && getter != eater)
            {
                Log.Error(string.Concat(new object[]
                {
                    getter,
                    " tried to find food to bring to ",
                    eater,
                    " but ",
                    getter,
                    " is incapable of Manipulation."
                }));
                return null;
            }
            FoodPreferability minPref;
            if (!eater.RaceProps.Humanlike)
            {
                minPref = FoodPreferability.NeverForNutrition;
            }
            else if (desperate)
            {
                minPref = FoodPreferability.DesperateOnly;
            }
            else
            {
                minPref = ((eater.needs.food.CurCategory <= HungerCategory.UrgentlyHungry) ? FoodPreferability.RawBad : FoodPreferability.MealAwful);
            }
            Predicate<Thing> foodValidator = delegate (Thing t)
            {
                if (PrisonerFoodReservation.isReserved(t))
                {
                    return false;
                }
                if (!allowForbidden && t.IsForbidden(getter))
                {
                    return false;
                }
                Building_NutrientPasteDispenser building_NutrientPasteDispenser = t as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null)
                {
                    if (!allowDispenserFull || ThingDefOf.MealNutrientPaste.ingestible.preferability < minPref || ThingDefOf.MealNutrientPaste.ingestible.preferability > maxPref || !getterCanManipulate || (t.Faction != getter.Faction && t.Faction != getter.HostFaction) || (!building_NutrientPasteDispenser.powerComp.PowerOn || (!allowDispenserEmpty && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers())) || !FoodUtility_Tweak.IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || !t.InteractionCell.Standable(t.Map) || !getter.Map.reachability.CanReachNonLocal(getter.Position, new TargetInfo(t.InteractionCell, t.Map, false), PathEndMode.OnCell, TraverseParms.For(getter, Danger.Some, TraverseMode.ByPawn, false)))
                    {
                        return false;
                    }
                }
                else
                {
                    if (t.def.ingestible.preferability < minPref)
                    {
                        return false;
                    }
                    if (t.def.ingestible.preferability > maxPref)
                    {
                        return false;
                    }
                    if (!t.IngestibleNow || !t.def.IsNutritionGivingIngestible || (!allowCorpse && t is Corpse) || (!allowDrug && t.def.IsDrug) || (!desperate && t.IsNotFresh()) || t.IsDessicated() || !eater.RaceProps.WillAutomaticallyEat(t) || !FoodUtility_Tweak.IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) || !getter.AnimalAwareOf(t) || !getter.CanReserve(t, 1, -1, null, false))
                    {
                        return false;
                    }
                }
                return true;
            };
            ThingRequest thingRequest;
            if ((eater.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != FoodTypeFlags.None && allowPlant)
            {
                thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodSource);
            }
            else
            {
                thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            }
            Thing thing;
            if (getter.RaceProps.Humanlike)
            {
                Predicate<Thing> validator = foodValidator;
                thing = FoodUtility_Tweak.SpawnedFoodSearchInnerScan(eater, getter.Position, getter.Map.listerThings.ThingsMatching(thingRequest), PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator);
            }
            else
            {
                int searchRegionsMax = 30;
                if (getter.Faction == Faction.OfPlayer)
                {
                    searchRegionsMax = 100;
                }
                FoodUtility_Tweak.filtered.Clear();
                foreach (Thing current in GenRadial.RadialDistinctThingsAround(getter.Position, getter.Map, 2f, true))
                {
                    Pawn pawn = current as Pawn;
                    if (pawn != null && pawn != getter && pawn.RaceProps.Animal && pawn.CurJob != null && pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing)
                    {
                        FoodUtility_Tweak.filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing);
                    }
                }
                bool flag = !allowForbidden && ForbidUtility.CaresAboutForbidden(getter, true) && getter.playerSettings != null && getter.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap != null;
                Predicate<Thing> predicate = (Thing t) => foodValidator(t) && !FoodUtility_Tweak.filtered.Contains(t) && t.def.ingestible.preferability > FoodPreferability.DesperateOnly && !t.IsNotFresh();
                Predicate<Thing> validator = predicate;
                bool ignoreEntirelyForbiddenRegions = flag;
                thing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, searchRegionsMax, false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
                FoodUtility_Tweak.filtered.Clear();
                if (thing == null)
                {
                    desperate = true;
                    validator = foodValidator;
                    ignoreEntirelyForbiddenRegions = flag;
                    thing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest, PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator, null, 0, searchRegionsMax, false, RegionType.Set_Passable, ignoreEntirelyForbiddenRegions);
                }
            }
            return thing;
        }

        private static bool IsFoodSourceOnMapSociallyProper(Thing t, Pawn getter, Pawn eater, bool allowSociallyImproper)
        {
            if (!allowSociallyImproper)
            {
                bool animalsCare = !getter.RaceProps.Animal;
                if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, eater.IsPrisonerOfColony, animalsCare))
                {
                    return false;
                }
            }
            return true;
        }

        public static float FoodSourceOptimality(Pawn eater, Thing t, float dist, bool takingToInventory = false)
        {
            float num = 300f;
            num -= dist;
            ThingDef thingDef = (!(t is Building_NutrientPasteDispenser)) ? t.def : ThingDefOf.MealNutrientPaste;
            FoodPreferability preferability = thingDef.ingestible.preferability;
            if (preferability != FoodPreferability.NeverForNutrition)
            {
                if (preferability == FoodPreferability.DesperateOnly)
                {
                    num -= 150f;
                }
                CompRottable compRottable = t.TryGetComp<CompRottable>();
                if (compRottable != null)
                {
                    if (compRottable.Stage == RotStage.Dessicated)
                    {
                        return -9999999f;
                    }
                    if (!takingToInventory && compRottable.Stage == RotStage.Fresh && compRottable.TicksUntilRotAtCurrentTemp < 30000)
                    {
                        num += 12f;
                    }
                }
                if (eater.needs != null && eater.needs.mood != null)
                {
                    List<ThoughtDef> list = FoodUtility_Tweak.ThoughtsFromIngesting(eater, t);
                    for (int i = 0; i < list.Count; i++)
                    {
                        num += FoodUtility_Tweak.FoodOptimalityEffectFromMoodCurve.Evaluate(list[i].stages[0].baseMoodEffect);
                    }
                }
                if (thingDef.ingestible != null)
                {
                    num += thingDef.ingestible.optimalityOffset;
                }
                return num;
            }
            return -9999999f;
        }

        private static Thing SpawnedFoodSearchInnerScan(Pawn eater, IntVec3 root, List<Thing> searchSet, PathEndMode peMode, TraverseParms traverseParams, float maxDistance = 9999f, Predicate<Thing> validator = null)
        {
            if (searchSet == null)
            {
                return null;
            }
            Pawn pawn = traverseParams.pawn ?? eater;
            int num = 0;
            int num2 = 0;
            Thing result = null;
            float num3 = -3.40282347E+38f;
            for (int i = 0; i < searchSet.Count; i++)
            {
                Thing thing = searchSet[i];
                num2++;
                float num4 = (float)(root - thing.Position).LengthManhattan;
                if (num4 <= maxDistance)
                {
                    float num5 = FoodUtility_Tweak.FoodSourceOptimality(eater, thing, num4, false);
                    if (num5 >= num3)
                    {
                        if (pawn.Map.reachability.CanReach(root, thing, peMode, traverseParams))
                        {
                            if (thing.Spawned)
                            {
                                if (validator == null || validator(thing))
                                {
                                    result = thing;
                                    num3 = num5;
                                    num++;
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        public static void DebugFoodSearchFromMouse_Update()
        {
            IntVec3 root = UI.MouseCell();
            Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
            {
                return;
            }
            if (pawn.Map != Find.VisibleMap)
            {
                return;
            }
            Thing thing = FoodUtility_Tweak.SpawnedFoodSearchInnerScan(pawn, root, Find.VisibleMap.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree), PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false), 9999f, null);
            if (thing != null)
            {
                GenDraw.DrawLineBetween(root.ToVector3Shifted(), thing.Position.ToVector3Shifted());
            }
        }

        public static void DebugFoodSearchFromMouse_OnGUI()
        {
            IntVec3 a = UI.MouseCell();
            Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
            {
                return;
            }
            if (pawn.Map != Find.VisibleMap)
            {
                return;
            }
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Tiny;
            foreach (Thing current in Find.VisibleMap.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree))
            {
                float num = FoodUtility_Tweak.FoodSourceOptimality(pawn, current, (a - current.Position).LengthHorizontal, false);
                Vector2 vector = current.DrawPos.MapToUIPosition();
                Rect rect = new Rect(vector.x - 100f, vector.y - 100f, 200f, 200f);
                string text = num.ToString("F0");
                List<ThoughtDef> list = FoodUtility_Tweak.ThoughtsFromIngesting(pawn, current);
                for (int i = 0; i < list.Count; i++)
                {
                    string text2 = text;
                    text = string.Concat(new string[]
                    {
                        text2,
                        "\n",
                        list[i].defName,
                        "(",
                        FoodUtility_Tweak.FoodOptimalityEffectFromMoodCurve.Evaluate(list[i].stages[0].baseMoodEffect).ToString("F0"),
                        ")"
                    });
                }
                Widgets.Label(rect, text);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static Pawn BestPawnToHuntForPredator(Pawn predator)
        {
            if (predator.meleeVerbs.TryGetMeleeVerb() == null)
            {
                return null;
            }
            bool flag = false;
            float summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
            if (summaryHealthPercent < 0.25f)
            {
                flag = true;
            }
            List<Pawn> allPawnsSpawned = predator.Map.mapPawns.AllPawnsSpawned;
            Pawn pawn = null;
            float num = 0f;
            bool tutorialMode = TutorSystem.TutorialMode;
            for (int i = 0; i < allPawnsSpawned.Count; i++)
            {
                Pawn pawn2 = allPawnsSpawned[i];
                if (predator.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable))
                {
                    if (predator != pawn2)
                    {
                        if (!flag || pawn2.Downed)
                        {
                            if (FoodUtility_Tweak.IsAcceptablePreyFor(predator, pawn2))
                            {
                                if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false, TraverseMode.ByPawn))
                                {
                                    if (!pawn2.IsForbidden(predator))
                                    {
                                        if (!tutorialMode || pawn2.Faction != Faction.OfPlayer)
                                        {
                                            float preyScoreFor = FoodUtility_Tweak.GetPreyScoreFor(predator, pawn2);
                                            if (preyScoreFor > num || pawn == null)
                                            {
                                                num = preyScoreFor;
                                                pawn = pawn2;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return pawn;
        }

        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey)
        {
            if (!prey.RaceProps.canBePredatorPrey)
            {
                return false;
            }
            if (!prey.RaceProps.IsFlesh)
            {
                return false;
            }
            if (prey.BodySize > predator.RaceProps.maxPreyBodySize)
            {
                return false;
            }
            if (!prey.Downed)
            {
                if (prey.kindDef.combatPower > 2f * predator.kindDef.combatPower)
                {
                    return false;
                }
                float num = prey.kindDef.combatPower * prey.health.summaryHealth.SummaryHealthPercent * prey.ageTracker.CurLifeStage.bodySizeFactor;
                float num2 = predator.kindDef.combatPower * predator.health.summaryHealth.SummaryHealthPercent * predator.ageTracker.CurLifeStage.bodySizeFactor;
                if (num > 0.85f * num2)
                {
                    return false;
                }
            }
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) && (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) && (!predator.RaceProps.herdAnimal || predator.def != prey.def);
        }

        public static float GetPreyScoreFor(Pawn predator, Pawn prey)
        {
            float num = prey.kindDef.combatPower / predator.kindDef.combatPower;
            float num2 = prey.health.summaryHealth.SummaryHealthPercent;
            float bodySizeFactor = prey.ageTracker.CurLifeStage.bodySizeFactor;
            float lengthHorizontal = (predator.Position - prey.Position).LengthHorizontal;
            if (prey.Downed)
            {
                num2 = Mathf.Min(num2, 0.2f);
            }
            float num3 = -lengthHorizontal - 56f * num2 * num2 * num * bodySizeFactor;
            if (prey.RaceProps.Humanlike)
            {
                num3 -= 35f;
            }
            return num3;
        }

        public static void DebugDrawPredatorFoodSource()
        {
            Pawn pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
            {
                return;
            }
            Thing thing;
            ThingDef thingDef;
            if (FoodUtility_Tweak.TryFindBestFoodSourceFor(pawn, pawn, true, out thing, out thingDef, false, false, false, true, false))
            {
                GenDraw.DrawLineBetween(pawn.Position.ToVector3Shifted(), thing.Position.ToVector3Shifted());
                if (!(thing is Pawn))
                {
                    Pawn pawn2 = FoodUtility_Tweak.BestPawnToHuntForPredator(pawn);
                    if (pawn2 != null)
                    {
                        GenDraw.DrawLineBetween(pawn.Position.ToVector3Shifted(), pawn2.Position.ToVector3Shifted());
                    }
                }
            }
        }

        public static List<ThoughtDef> ThoughtsFromIngesting(Pawn ingester, Thing t)
        {
            FoodUtility_Tweak.ingestThoughts.Clear();
            if (ingester.needs == null || ingester.needs.mood == null)
            {
                return FoodUtility_Tweak.ingestThoughts;
            }
            ThingDef thingDef = t.def;
            if (thingDef == ThingDefOf.NutrientPasteDispenser)
            {
                thingDef = ThingDefOf.MealNutrientPaste;
            }
            if (!ingester.story.traits.HasTrait(TraitDefOf.Ascetic) && thingDef.ingestible.tasteThought != null)
            {
                FoodUtility_Tweak.ingestThoughts.Add(thingDef.ingestible.tasteThought);
            }
            CompIngredients compIngredients = t.TryGetComp<CompIngredients>();
            if (FoodUtility_Tweak.IsHumanlikeMeat(thingDef) && ingester.RaceProps.Humanlike)
            {
                FoodUtility_Tweak.ingestThoughts.Add((!ingester.story.traits.HasTrait(TraitDefOf.Cannibal)) ? ThoughtDefOf.AteHumanlikeMeatDirect : ThoughtDefOf.AteHumanlikeMeatDirectCannibal);
            }
            else if (compIngredients != null)
            {
                for (int i = 0; i < compIngredients.ingredients.Count; i++)
                {
                    ThingDef thingDef2 = compIngredients.ingredients[i];
                    if (thingDef2.ingestible != null)
                    {
                        if (ingester.RaceProps.Humanlike && FoodUtility_Tweak.IsHumanlikeMeat(thingDef2))
                        {
                            FoodUtility_Tweak.ingestThoughts.Add((!ingester.story.traits.HasTrait(TraitDefOf.Cannibal)) ? ThoughtDefOf.AteHumanlikeMeatAsIngredient : ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal);
                        }
                        else if (thingDef2.ingestible.specialThoughtAsIngredient != null)
                        {
                            FoodUtility_Tweak.ingestThoughts.Add(thingDef2.ingestible.specialThoughtAsIngredient);
                        }
                    }
                }
            }
            else if (thingDef.ingestible.specialThoughtDirect != null)
            {
                FoodUtility_Tweak.ingestThoughts.Add(thingDef.ingestible.specialThoughtDirect);
            }
            if (t.IsNotFresh())
            {
                FoodUtility_Tweak.ingestThoughts.Add(ThoughtDefOf.AteRottenFood);
            }
            return FoodUtility_Tweak.ingestThoughts;
        }

        public static bool IsHumanlikeMeat(ThingDef def)
        {
            return def.ingestible.sourceDef != null && def.ingestible.sourceDef.race != null && def.ingestible.sourceDef.race.Humanlike;
        }

        public static bool IsHumanlikeMeatOrHumanlikeCorpse(Thing thing)
        {
            if (FoodUtility_Tweak.IsHumanlikeMeat(thing.def))
            {
                return true;
            }
            Corpse corpse = thing as Corpse;
            return corpse != null && corpse.InnerPawn.RaceProps.Humanlike;
        }

        public static int WillIngestStackCountOf(Pawn ingester, ThingDef def)
        {
            int num = Mathf.Min(def.ingestible.maxNumToIngestAtOnce, FoodUtility_Tweak.StackCountForNutrition(def, ingester.needs.food.NutritionWanted));
            if (num < 1)
            {
                num = 1;
            }
            return num;
        }

        public static float GetBodyPartNutrition(Pawn pawn, BodyPartRecord part)
        {
            if (!pawn.RaceProps.IsFlesh)
            {
                return 0f;
            }
            return 5.2f * pawn.BodySize * pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(part);
        }

        public static int StackCountForNutrition(ThingDef def, float nutrition)
        {
            if (nutrition <= 0.0001f)
            {
                return 0;
            }
            return Mathf.Max(Mathf.RoundToInt(nutrition / def.ingestible.nutrition), 1);
        }

        public static bool ShouldBeFedBySomeone(Pawn pawn)
        {
            return FeedPatientUtility.ShouldBeFed(pawn) || WardenFeedUtility.ShouldBeFed(pawn);
        }

        public static void AddFoodPoisoningHediff(Pawn pawn, Thing ingestible)
        {
            pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.FoodPoisoning, pawn, null), null, null);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
            {
                Messages.Message("MessageFoodPoisoning".Translate(new object[]
                {
                    pawn.LabelShort,
                    ingestible.LabelCapNoCount
                }).CapitalizeFirst(), pawn, MessageSound.Negative);
            }
        }
    }
}
