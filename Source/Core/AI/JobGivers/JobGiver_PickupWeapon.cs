using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.JobGivers
{
    public class JobGiver_PickupWeapon : ThinkNode_JobGiver
    {
        private static TreatmentCategory maxCategory = TreatmentCategory.Bad;

        private bool preferBuildingDestroyers;

        public override float GetPriority(Pawn pawn) => 12f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (pawn.equipment == null)
            {
                return null;
            }
            if (AlreadySatisfiedWithCurrentWeapon(pawn))
            {
                return null;
            }
            var treatmentNeed = pawn.needs?.TryGetNeed<Need_Treatment>();
            if(treatmentNeed == null || treatmentNeed.CurCategory > maxCategory)
            {
                return null;
            }
            if (pawn.RaceProps.Humanlike && pawn.story.WorkTagIsDisabled(WorkTags.Violent))
            {
                return null;
            }
            if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                return null;
            }
            if (pawn.GetRegion(RegionType.Set_Passable) == null)
            {
                return null;
            }
            Thing thing = GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForGroup(ThingRequestGroup.Weapon), PathEndMode.OnCell, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false), 8f, (Thing x) => pawn.CanReserve(x, 1, -1, null, false) && ShouldEquip(x, pawn), null, 0, 15, false, RegionType.Set_Passable, false);
            if (thing != null)
            {
                return new Job(JobDefOf.Equip, thing);
            }
            return null;
        }

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_PickupWeapon jobGiver = (JobGiver_PickupWeapon)base.DeepCopy(resolve);
            jobGiver.preferBuildingDestroyers = preferBuildingDestroyers;
            return jobGiver;
        }

        private bool ShouldEquip(Thing newWep, Pawn pawn)
        {
            return GetWeaponScore(newWep) > GetWeaponScore(pawn.equipment.Primary);
        }

        private int GetWeaponScore(Thing wep)
        {
            if (wep == null)
            {
                return 0;
            }
            if (wep.def.IsMeleeWeapon && wep.GetStatValue(StatDefOf.MeleeWeapon_AverageDPS, true) < MinMeleeWeaponDPSThreshold)
            {
                return 0;
            }
            if (preferBuildingDestroyers && wep.TryGetComp<CompEquippable>().PrimaryVerb.verbProps.ai_IsBuildingDestroyer)
            {
                return 3;
            }
            if (wep.def.IsRangedWeapon)
            {
                return 2;
            }
            return 1;
        }

        private bool AlreadySatisfiedWithCurrentWeapon(Pawn pawn)
        {
            ThingWithComps primary = pawn.equipment.Primary;
            if (primary == null)
            {
                return false;
            }
            if (preferBuildingDestroyers)
            {
                if (!pawn.equipment.PrimaryEq.PrimaryVerb.verbProps.ai_IsBuildingDestroyer)
                {
                    return false;
                }
            }
            else if (!primary.def.IsRangedWeapon)
            {
                return false;
            }
            return true;
        }

        private float MinMeleeWeaponDPSThreshold
        {
            get
            {
                List<Tool> tools = ThingDefOf.Human.tools;
                float num = 0f;
                for (int i = 0; i < tools.Count; i++)
                {
                    if (tools[i].linkedBodyPartsGroup == BodyPartGroupDefOf.LeftHand || tools[i].linkedBodyPartsGroup == BodyPartGroupDefOf.RightHand)
                    {
                        num = tools[i].power / tools[i].cooldownTime;
                        break;
                    }
                }
                return num + 2f;
            }
        }
    }
}
