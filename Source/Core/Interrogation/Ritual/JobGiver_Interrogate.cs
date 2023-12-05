using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using Verse;
using Verse.AI.Group;

namespace PrisonLabor.Core.Interrogation.Ritual
{
  public class JobGiver_Interrogate : ThinkNode_JobGiver
  {
    public SoundDef soundDefMale;

    public SoundDef soundDefFemale;

    public bool faceSpectatorsIfPossible;

    public bool showSpeechBubbles = true;

    protected override Job TryGiveJob(Pawn pawn)
    {
      PawnDuty duty = pawn.mindState.duty;
      if (duty == null)
      {
        return null;
      }
      IntVec3 result = pawn.Position;
      if (!pawn.CanReserve(pawn.Position))
      {
        CellFinder.TryRandomClosewalkCellNear(result, pawn.Map, 2, out result, (IntVec3 c) => pawn.CanReserveAndReach(c, PathEndMode.OnCell, pawn.NormalMaxDanger()));
      }
      LordJob_Ritual lordJob_Ritual = pawn.GetLord()?.LordJob as LordJob_Ritual;
      if (lordJob_Ritual == null)
      {
        return null;
      }
      Pawn prisoner = lordJob_Ritual.PawnWithRole("prisoner");
      Job job = JobMaker.MakeJob(InterrogationDefsOf.PL_Interrogate, result, prisoner);
      job.showSpeechBubbles = showSpeechBubbles;
      LordToil_Ritual lordToil_Ritual;
      if (lordJob_Ritual != null && (lordToil_Ritual = (lordJob_Ritual.lord.CurLordToil as LordToil_Ritual)) != null)
      {
        job.interaction = lordToil_Ritual.stage.BehaviorForRole(lordJob_Ritual.RoleFor(pawn).id).speakerInteraction;
      }
      job.speechSoundMale = (soundDefMale ?? SoundDefOf.Speech_Leader_Male);
      job.speechSoundFemale = (soundDefFemale ?? SoundDefOf.Speech_Leader_Female);
      job.speechFaceSpectatorsIfPossible = faceSpectatorsIfPossible;
      return job;
    }

    public override ThinkNode DeepCopy(bool resolve = true)
    {
      JobGiver_Interrogate obj = (JobGiver_Interrogate)base.DeepCopy(resolve);
      obj.soundDefMale = soundDefMale;
      obj.soundDefFemale = soundDefFemale;
      obj.showSpeechBubbles = showSpeechBubbles;
      obj.faceSpectatorsIfPossible = faceSpectatorsIfPossible;
      return obj;
    }
  }
}