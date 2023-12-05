using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.AI;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  [StaticConstructorOnStartup]
  public class JobDriver_Interrogate : JobDriver
  {
    public static readonly Texture2D moteIcon = ContentFinder<Texture2D>.Get("Things/Mote/SpeechSymbols/Speech");
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
      return true;
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
      this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
      Toil toil = ToilMaker.MakeToil("MakeNewToils");
      toil.tickAction = delegate
      {
        pawn.GainComfortFromCellIfPossible();
        pawn.skills.Learn(SkillDefOf.Social, 0.01f);
        pawn.rotationTracker.FaceTarget(TargetB);
        Pawn prisoner = TargetThingB as Pawn;
        prisoner?.rotationTracker.FaceTarget(pawn.Position);
        MoteMaker.MakeSpeechBubble(pawn, moteIcon);
      };
      if (ModsConfig.IdeologyActive)
      {
        toil.PlaySustainerOrSound(() => (pawn.gender != Gender.Female) ? job.speechSoundMale : job.speechSoundFemale, pawn.story.VoicePitchFactor);
      }
      toil.defaultCompleteMode = ToilCompleteMode.Delay;
      toil.defaultDuration = 300;
      toil.handlingFacing = true;
      yield return toil;
    }
  }
}
