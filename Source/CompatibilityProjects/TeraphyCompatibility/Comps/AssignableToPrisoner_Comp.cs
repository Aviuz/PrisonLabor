using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Therapy.Comps
{
    public class AssignableToPrisoner_Comp : CompAssignableToPawn
    {
        public override IEnumerable<Pawn> AssigningCandidates
        {
            get
            {
                if (!parent.Spawned)
                {
                    return Enumerable.Empty<Pawn>();
                }
                return parent.Map.mapPawns.PrisonersOfColonySpawned;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (parent.def.building.bed_humanlike && parent.Faction == Faction.OfPlayer)
            {
                Command_Action command_Action = new Command_Action
                {
                    defaultLabel = "CommandThingSetPrisonerPatientsLabel".Translate(),
                    icon = ContentFinder<Texture2D>.Get("UI/Commands/AssignOwner"),
                    defaultDesc = "CommandBedSetPrisonerPatientsDesc".Translate(),
                    action = delegate { Find.WindowStack.Add(new Dialog_AssignBuildingOwner(this)); }
                };
                yield return command_Action;
            }
        }


    }
}
