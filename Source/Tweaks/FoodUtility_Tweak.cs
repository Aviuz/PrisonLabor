using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor
{
    public static class FoodUtility_Tweak
    {
        private static readonly HashSet<Thing> filtered = new HashSet<Thing>();

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

        private static readonly List<ThoughtDef> ingestThoughts = new List<ThoughtDef>();

        public static bool TryFindBestFoodSourceFor(Pawn getter, Pawn eater, bool desperate, out Thing foodSource,
            out ThingDef foodDef, bool canRefillDispenser = true, bool canUseInventory = true,
            bool allowForbidden = false, bool allowCorpse = true, bool allowSociallyImproper = false)
        {
            var flag = getter.RaceProps.ToolUser && getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            var allowDrug = !eater.IsTeetotaler();
            Thing thing = null;
            if (canUseInventory)
            {
                if (flag)
                    thing = BestFoodInInventory(getter, null, FoodPreferability.MealAwful, FoodPreferability.MealLavish,
                        0f, false);
                if (thing != null)
                {
                    if (getter.Faction != Faction.OfPlayer)
                    {
                        foodSource = thing;
                        foodDef = GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                    var compRottable = thing.TryGetComp<CompRottable>();
                    if (compRottable != null && compRottable.Stage == RotStage.Fresh &&
                        compRottable.TicksUntilRotAtCurrentTemp < 30000)
                    {
                        foodSource = thing;
                        foodDef = GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                }
            }
            var allowPlant = getter == eater;
            var thing2 = BestFoodSourceOnMap(getter, eater, desperate, FoodPreferability.MealLavish, allowPlant,
                allowDrug, allowCorpse, true, canRefillDispenser, allowForbidden, allowSociallyImproper);
            if (thing == null && thing2 == null)
            {
                if (canUseInventory && flag)
                {
                    thing = BestFoodInInventory(getter, null, FoodPreferability.DesperateOnly,
                        FoodPreferability.MealLavish, 0f, allowDrug);
                    if (thing != null)
                    {
                        foodSource = thing;
                        foodDef = GetFinalIngestibleDef(foodSource);
                        return true;
                    }
                }
                if (thing2 == null && getter == eater && getter.RaceProps.predator)
                {
                    var pawn = BestPawnToHuntForPredator(getter);
                    if (pawn != null)
                    {
                        foodSource = pawn;
                        foodDef = GetFinalIngestibleDef(foodSource);
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
                foodDef = GetFinalIngestibleDef(foodSource);
                return true;
            }
            if (thing2 == null && thing != null)
            {
                foodSource = thing;
                foodDef = GetFinalIngestibleDef(foodSource);
                return true;
            }
            var num = FoodSourceOptimality(eater, thing2, (getter.Position - thing2.Position).LengthManhattan, false);
            var num2 = FoodSourceOptimality(eater, thing, 0f, false);
            num2 -= 32f;
            if (num > num2)
            {
                foodSource = thing2;
                foodDef = GetFinalIngestibleDef(foodSource);
                return true;
            }
            foodSource = thing;
            foodDef = GetFinalIngestibleDef(foodSource);
            return true;
        }

        public static ThingDef GetFinalIngestibleDef(Thing foodSource)
        {
            var building_NutrientPasteDispenser = foodSource as Building_NutrientPasteDispenser;
            if (building_NutrientPasteDispenser != null)
                return building_NutrientPasteDispenser.DispensableDef;
            var pawn = foodSource as Pawn;
            if (pawn != null)
                return pawn.RaceProps.corpseDef;
            return foodSource.def;
        }

        public static Thing BestFoodInInventory(Pawn holder, Pawn eater = null,
            FoodPreferability minFoodPref = FoodPreferability.NeverForNutrition,
            FoodPreferability maxFoodPref = FoodPreferability.MealLavish, float minStackNutrition = 0f,
            bool allowDrug = false)
        {
            if (holder.inventory == null)
                return null;
            if (eater == null)
                eater = holder;
            var innerContainer = holder.inventory.innerContainer;
            for (var i = 0; i < innerContainer.Count; i++)
            {
                var thing = innerContainer[i];
                if (thing.def.IsNutritionGivingIngestible && thing.IngestibleNow && eater.RaceProps.CanEverEat(thing) &&
                    thing.def.ingestible.preferability >= minFoodPref &&
                    thing.def.ingestible.preferability <= maxFoodPref && (allowDrug || !thing.def.IsDrug))
                {
                    var num = thing.def.ingestible.CachedNutrition * thing.stackCount;
                    if (num >= minStackNutrition)
                        return thing;
                }
            }
            return null;
        }

        public static Thing BestFoodSourceOnMap(Pawn getter, Pawn eater, bool desperate,
            FoodPreferability maxPref = FoodPreferability.MealLavish, bool allowPlant = true, bool allowDrug = true,
            bool allowCorpse = true, bool allowDispenserFull = true, bool allowDispenserEmpty = true,
            bool allowForbidden = false, bool allowSociallyImproper = false)
        {
            var getterCanManipulate = getter.RaceProps.ToolUser &&
                                      getter.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation);
            if (!getterCanManipulate && getter != eater)
            {
                Log.Error(string.Concat(getter, " tried to find food to bring to ", eater, " but ", getter,
                    " is incapable of Manipulation."));
                return null;
            }
            FoodPreferability minPref;
            if (!eater.RaceProps.Humanlike || eater == getter && eater.IsPrisoner)
                minPref = FoodPreferability.NeverForNutrition;
            else if (desperate)
                minPref = FoodPreferability.DesperateOnly;
            else
                minPref = eater.needs.food.CurCategory <= HungerCategory.UrgentlyHungry
                    ? FoodPreferability.RawBad
                    : FoodPreferability.MealAwful;
            Predicate<Thing> foodValidator = delegate(Thing t)
            {
                if (PrisonerFoodReservation.IsReserved(t) && (eater != getter || !eater.IsPrisoner) && !desperate)
                    return false;
                if (!allowForbidden && t.IsForbidden(getter))
                    return false;
                var building_NutrientPasteDispenser = t as Building_NutrientPasteDispenser;
                if (building_NutrientPasteDispenser != null)
                {
                    if (!allowDispenserFull || ThingDefOf.MealNutrientPaste.ingestible.preferability < minPref ||
                        ThingDefOf.MealNutrientPaste.ingestible.preferability > maxPref || !getterCanManipulate ||
                        t.Faction != getter.Faction && t.Faction != getter.HostFaction ||
                        !building_NutrientPasteDispenser.powerComp.PowerOn ||
                        !allowDispenserEmpty && !building_NutrientPasteDispenser.HasEnoughFeedstockInHoppers() ||
                        !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) ||
                        !t.InteractionCell.Standable(t.Map) || !getter.Map.reachability.CanReachNonLocal(
                            getter.Position, new TargetInfo(t.InteractionCell, t.Map, false), PathEndMode.OnCell,
                            TraverseParms.For(getter, Danger.Some, TraverseMode.ByPawn, false)))
                        return false;
                }
                else
                {
                    if (t.def.ingestible.preferability < minPref)
                        return false;
                    if (t.def.ingestible.preferability > maxPref)
                        return false;
                    if (!t.IngestibleNow || !t.def.IsNutritionGivingIngestible || !allowCorpse && t is Corpse ||
                        !allowDrug && t.def.IsDrug || !desperate && t.IsNotFresh() || t.IsDessicated() ||
                        !eater.RaceProps.WillAutomaticallyEat(t) ||
                        !IsFoodSourceOnMapSociallyProper(t, getter, eater, allowSociallyImproper) ||
                        !getter.AnimalAwareOf(t) || !getter.CanReserve(t, 1, -1, null, false))
                        return false;
                }
                return true;
            };
            ThingRequest thingRequest;
            if ((eater.RaceProps.foodType & (FoodTypeFlags.Plant | FoodTypeFlags.Tree)) != FoodTypeFlags.None &&
                allowPlant)
                thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodSource);
            else
                thingRequest = ThingRequest.ForGroup(ThingRequestGroup.FoodSourceNotPlantOrTree);
            Thing thing;
            if (getter.RaceProps.Humanlike)
            {
                var validator = foodValidator;
                thing = SpawnedFoodSearchInnerScan(eater, getter.Position,
                    getter.Map.listerThings.ThingsMatching(thingRequest), PathEndMode.ClosestTouch,
                    TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, validator);
            }
            else
            {
                var searchRegionsMax = 30;
                if (getter.Faction == Faction.OfPlayer)
                    searchRegionsMax = 100;
                filtered.Clear();
                foreach (var current in GenRadial.RadialDistinctThingsAround(getter.Position, getter.Map, 2f, true))
                {
                    var pawn = current as Pawn;
                    if (pawn != null && pawn != getter && pawn.RaceProps.Animal && pawn.CurJob != null &&
                        pawn.CurJob.def == JobDefOf.Ingest && pawn.CurJob.GetTarget(TargetIndex.A).HasThing)
                        filtered.Add(pawn.CurJob.GetTarget(TargetIndex.A).Thing);
                }
                var flag = !allowForbidden && ForbidUtility.CaresAboutForbidden(getter, true) &&
                           getter.playerSettings != null &&
                           getter.playerSettings.EffectiveAreaRestrictionInPawnCurrentMap != null;
                Predicate<Thing> predicate = t =>
                    foodValidator(t) && !filtered.Contains(t) &&
                    t.def.ingestible.preferability > FoodPreferability.DesperateOnly && !t.IsNotFresh();
                var validator = predicate;
                var ignoreEntirelyForbiddenRegions = flag;
                thing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest,
                    PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false),
                    9999f, validator, null, 0, searchRegionsMax, false, RegionType.Set_Passable,
                    ignoreEntirelyForbiddenRegions);
                filtered.Clear();
                if (thing == null)
                {
                    desperate = true;
                    validator = foodValidator;
                    ignoreEntirelyForbiddenRegions = flag;
                    thing = GenClosest.ClosestThingReachable(getter.Position, getter.Map, thingRequest,
                        PathEndMode.ClosestTouch, TraverseParms.For(getter, Danger.Deadly, TraverseMode.ByPawn, false),
                        9999f, validator, null, 0, searchRegionsMax, false, RegionType.Set_Passable,
                        ignoreEntirelyForbiddenRegions);
                }
            }
            return thing;
        }

        private static bool IsFoodSourceOnMapSociallyProper(Thing t, Pawn getter, Pawn eater,
            bool allowSociallyImproper)
        {
            if (!allowSociallyImproper)
            {
                var animalsCare = !getter.RaceProps.Animal;
                if (!t.IsSociallyProper(getter) && !t.IsSociallyProper(eater, false, animalsCare))
                    return false;
            }
            return true;
        }

        public static float FoodSourceOptimality(Pawn eater, Thing t, float dist, bool takingToInventory = false)
        {
            var num = 300f;
            num -= dist;
            var thingDef = !(t is Building_NutrientPasteDispenser) ? t.def : ThingDefOf.MealNutrientPaste;
            var preferability = thingDef.ingestible.preferability;
            if (preferability != FoodPreferability.NeverForNutrition)
            {
                if (preferability == FoodPreferability.DesperateOnly)
                    num -= 150f;
                var compRottable = t.TryGetComp<CompRottable>();
                if (compRottable != null)
                {
                    if (compRottable.Stage == RotStage.Dessicated)
                        return -9999999f;
                    if (!takingToInventory && compRottable.Stage == RotStage.Fresh &&
                        compRottable.TicksUntilRotAtCurrentTemp < 30000)
                        num += 12f;
                }
                if (eater.needs != null && eater.needs.mood != null)
                {
                    var list = ThoughtsFromIngesting(eater, t);
                    for (var i = 0; i < list.Count; i++)
                        num += FoodOptimalityEffectFromMoodCurve.Evaluate(list[i].stages[0].baseMoodEffect);
                }
                if (thingDef.ingestible != null)
                    if (eater.RaceProps.Humanlike)
                    {
                        num += thingDef.ingestible.optimalityOffsetHumanlikes;
                    }
                    else if (eater.RaceProps.Animal)
                    {
                        num += thingDef.ingestible.optimalityOffsetFeedingAnimals;
                    }
                return num;
            }
            return -9999999f;
        }

        private static Thing SpawnedFoodSearchInnerScan(Pawn eater, IntVec3 root, List<Thing> searchSet,
            PathEndMode peMode, TraverseParms traverseParams, float maxDistance = 9999f,
            Predicate<Thing> validator = null)
        {
            if (searchSet == null)
                return null;
            var pawn = traverseParams.pawn ?? eater;
            var num = 0;
            var num2 = 0;
            Thing result = null;
            var num3 = -3.40282347E+38f;
            for (var i = 0; i < searchSet.Count; i++)
            {
                var thing = searchSet[i];
                num2++;
                float num4 = (root - thing.Position).LengthManhattan;
                if (num4 <= maxDistance)
                {
                    var num5 = FoodSourceOptimality(eater, thing, num4, false);
                    if (num5 >= num3)
                        if (pawn.Map.reachability.CanReach(root, thing, peMode, traverseParams))
                            if (thing.Spawned)
                                if (validator == null || validator(thing))
                                {
                                    result = thing;
                                    num3 = num5;
                                    num++;
                                }
                }
            }
            return result;
        }

        public static void DebugFoodSearchFromMouse_Update()
        {
            var root = UI.MouseCell();
            var pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
                return;
            if (pawn.Map != Find.CurrentMap)
                return;
            var thing = SpawnedFoodSearchInnerScan(pawn, root,
                Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup.FoodSourceNotPlantOrTree),
                PathEndMode.ClosestTouch, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false), 9999f, null);
            if (thing != null)
                GenDraw.DrawLineBetween(root.ToVector3Shifted(), thing.Position.ToVector3Shifted());
        }

        public static void DebugFoodSearchFromMouse_OnGUI()
        {
            var a = UI.MouseCell();
            var pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
                return;
            if (pawn.Map != Find.CurrentMap)
                return;
            Text.Anchor = TextAnchor.MiddleCenter;
            Text.Font = GameFont.Tiny;
            foreach (var current in Find.CurrentMap.listerThings.ThingsInGroup(ThingRequestGroup
                .FoodSourceNotPlantOrTree))
            {
                var num = FoodSourceOptimality(pawn, current, (a - current.Position).LengthHorizontal, false);
                var vector = current.DrawPos.MapToUIPosition();
                var rect = new Rect(vector.x - 100f, vector.y - 100f, 200f, 200f);
                var text = num.ToString("F0");
                var list = ThoughtsFromIngesting(pawn, current);
                for (var i = 0; i < list.Count; i++)
                {
                    var text2 = text;
                    text = string.Concat(text2, "\n", list[i].defName, "(",
                        FoodOptimalityEffectFromMoodCurve.Evaluate(list[i].stages[0].baseMoodEffect).ToString("F0"),
                        ")");
                }
                Widgets.Label(rect, text);
            }
            Text.Anchor = TextAnchor.UpperLeft;
        }

        private static Pawn BestPawnToHuntForPredator(Pawn predator)
        {
            if (predator.meleeVerbs.TryGetMeleeVerb(null) == null)
                return null;
            var flag = false;
            var summaryHealthPercent = predator.health.summaryHealth.SummaryHealthPercent;
            if (summaryHealthPercent < 0.25f)
                flag = true;
            var allPawnsSpawned = predator.Map.mapPawns.AllPawnsSpawned;
            Pawn pawn = null;
            var num = 0f;
            var tutorialMode = TutorSystem.TutorialMode;
            for (var i = 0; i < allPawnsSpawned.Count; i++)
            {
                var pawn2 = allPawnsSpawned[i];
                if (predator.GetRoom(RegionType.Set_Passable) == pawn2.GetRoom(RegionType.Set_Passable))
                    if (predator != pawn2)
                        if (!flag || pawn2.Downed)
                            if (IsAcceptablePreyFor(predator, pawn2))
                                if (predator.CanReach(pawn2, PathEndMode.ClosestTouch, Danger.Deadly, false,
                                    TraverseMode.ByPawn))
                                    if (!pawn2.IsForbidden(predator))
                                        if (!tutorialMode || pawn2.Faction != Faction.OfPlayer)
                                        {
                                            var preyScoreFor = GetPreyScoreFor(predator, pawn2);
                                            if (preyScoreFor > num || pawn == null)
                                            {
                                                num = preyScoreFor;
                                                pawn = pawn2;
                                            }
                                        }
            }
            return pawn;
        }

        public static bool IsAcceptablePreyFor(Pawn predator, Pawn prey)
        {
            if (!prey.RaceProps.canBePredatorPrey)
                return false;
            if (!prey.RaceProps.IsFlesh)
                return false;
            if (prey.BodySize > predator.RaceProps.maxPreyBodySize)
                return false;
            if (!prey.Downed)
            {
                if (prey.kindDef.combatPower > 2f * predator.kindDef.combatPower)
                    return false;
                var num = prey.kindDef.combatPower * prey.health.summaryHealth.SummaryHealthPercent *
                          prey.ageTracker.CurLifeStage.bodySizeFactor;
                var num2 = predator.kindDef.combatPower * predator.health.summaryHealth.SummaryHealthPercent *
                           predator.ageTracker.CurLifeStage.bodySizeFactor;
                if (num > 0.85f * num2)
                    return false;
            }
            return (predator.Faction == null || prey.Faction == null || predator.HostileTo(prey)) &&
                   (predator.Faction != Faction.OfPlayer || prey.Faction != Faction.OfPlayer) &&
                   (!predator.RaceProps.herdAnimal || predator.def != prey.def);
        }

        public static float GetPreyScoreFor(Pawn predator, Pawn prey)
        {
            var num = prey.kindDef.combatPower / predator.kindDef.combatPower;
            var num2 = prey.health.summaryHealth.SummaryHealthPercent;
            var bodySizeFactor = prey.ageTracker.CurLifeStage.bodySizeFactor;
            var lengthHorizontal = (predator.Position - prey.Position).LengthHorizontal;
            if (prey.Downed)
                num2 = Mathf.Min(num2, 0.2f);
            var num3 = -lengthHorizontal - 56f * num2 * num2 * num * bodySizeFactor;
            if (prey.RaceProps.Humanlike)
                num3 -= 35f;
            return num3;
        }

        public static void DebugDrawPredatorFoodSource()
        {
            var pawn = Find.Selector.SingleSelectedThing as Pawn;
            if (pawn == null)
                return;
            Thing thing;
            ThingDef thingDef;
            if (TryFindBestFoodSourceFor(pawn, pawn, true, out thing, out thingDef, false, false, false, true, false))
            {
                GenDraw.DrawLineBetween(pawn.Position.ToVector3Shifted(), thing.Position.ToVector3Shifted());
                if (!(thing is Pawn))
                {
                    var pawn2 = BestPawnToHuntForPredator(pawn);
                    if (pawn2 != null)
                        GenDraw.DrawLineBetween(pawn.Position.ToVector3Shifted(), pawn2.Position.ToVector3Shifted());
                }
            }
        }

        public static List<ThoughtDef> ThoughtsFromIngesting(Pawn ingester, Thing t)
        {
            ingestThoughts.Clear();
            if (ingester.needs == null || ingester.needs.mood == null)
                return ingestThoughts;
            var thingDef = t.def;
            if (thingDef == ThingDefOf.NutrientPasteDispenser)
                thingDef = ThingDefOf.MealNutrientPaste;
            if (!ingester.story.traits.HasTrait(TraitDefOf.Ascetic) && thingDef.ingestible.tasteThought != null)
                ingestThoughts.Add(thingDef.ingestible.tasteThought);
            var compIngredients = t.TryGetComp<CompIngredients>();
            if (IsHumanlikeMeat(thingDef) && ingester.RaceProps.Humanlike)
                ingestThoughts.Add(!ingester.story.traits.HasTrait(TraitDefOf.Cannibal)
                    ? ThoughtDefOf.AteHumanlikeMeatDirect
                    : ThoughtDefOf.AteHumanlikeMeatDirectCannibal);
            else if (compIngredients != null)
                for (var i = 0; i < compIngredients.ingredients.Count; i++)
                {
                    var thingDef2 = compIngredients.ingredients[i];
                    if (thingDef2.ingestible != null)
                        if (ingester.RaceProps.Humanlike && IsHumanlikeMeat(thingDef2))
                            ingestThoughts.Add(!ingester.story.traits.HasTrait(TraitDefOf.Cannibal)
                                ? ThoughtDefOf.AteHumanlikeMeatAsIngredient
                                : ThoughtDefOf.AteHumanlikeMeatAsIngredientCannibal);
                        else if (thingDef2.ingestible.specialThoughtAsIngredient != null)
                            ingestThoughts.Add(thingDef2.ingestible.specialThoughtAsIngredient);
                }
            else if (thingDef.ingestible.specialThoughtDirect != null)
                ingestThoughts.Add(thingDef.ingestible.specialThoughtDirect);
            if (t.IsNotFresh())
                ingestThoughts.Add(ThoughtDefOf.AteRottenFood);
            return ingestThoughts;
        }

        public static bool IsHumanlikeMeat(ThingDef def)
        {
            return def.ingestible.sourceDef != null && def.ingestible.sourceDef.race != null &&
                   def.ingestible.sourceDef.race.Humanlike;
        }

        public static bool IsHumanlikeMeatOrHumanlikeCorpse(Thing thing)
        {
            if (IsHumanlikeMeat(thing.def))
                return true;
            var corpse = thing as Corpse;
            return corpse != null && corpse.InnerPawn.RaceProps.Humanlike;
        }

        public static int WillIngestStackCountOf(Pawn ingester, ThingDef def)
        {
            var num = Mathf.Min(def.ingestible.maxNumToIngestAtOnce,
                StackCountForNutrition(def, ingester.needs.food.NutritionWanted));
            if (num < 1)
                num = 1;
            return num;
        }

        public static float GetBodyPartNutrition(Pawn pawn, BodyPartRecord part)
        {
            if (!pawn.RaceProps.IsFlesh)
                return 0f;
            return 5.2f * pawn.BodySize * pawn.health.hediffSet.GetCoverageOfNotMissingNaturalParts(part);
        }

        public static int StackCountForNutrition(ThingDef def, float nutrition)
        {
            if (nutrition <= 0.0001f)
                return 0;
            return Mathf.Max(Mathf.RoundToInt(nutrition / def.ingestible.CachedNutrition), 1);
        }

        public static bool ShouldBeFedBySomeone(Pawn pawn)
        {
            return FeedPatientUtility.ShouldBeFed(pawn) || WardenFeedUtility.ShouldBeFed(pawn);
        }

        public static void AddFoodPoisoningHediff(Pawn pawn, Thing ingestible)
        {
            pawn.health.AddHediff(HediffMaker.MakeHediff(HediffDefOf.FoodPoisoning, pawn, null), null, null);
            if (PawnUtility.ShouldSendNotificationAbout(pawn))
                Messages.Message(
                    "MessageFoodPoisoning".Translate(pawn.LabelShort, ingestible.LabelCapNoCount).CapitalizeFirst(),
                    pawn, MessageTypeDefOf.NegativeEvent);
        }
    }
}