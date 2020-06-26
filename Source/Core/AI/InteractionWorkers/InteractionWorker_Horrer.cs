using System.Collections.Generic;
using System.Linq;
using PrisonLabor.Constants;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
using RimWorld;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.AI.InteractionWorkers
{
    public class InteractionWorker_Horrer : InteractionWorker
    {

        public override float RandomSelectionWeight(Pawn initiator, Pawn recipient)
        {
            if (!initiator.IsPrisoner || !recipient.IsPrisoner)
                return 0.0f;

            return 0.5f;
        }

        public override void Interacted(Pawn initiator, Pawn recipient, List<RulePackDef> extraSentencePacks, out string letterText, out string letterLabel, out LetterDef letterDef, out LookTargets lookTargets)
        {

            base.Interacted(initiator, recipient, extraSentencePacks, out letterText, out letterLabel, out letterDef, out lookTargets);
            initiator.needs.mood.thoughts.memories.TryGainMemory(ThoughtDefOf.CrashedTogether, recipient);
        }
    }
}
