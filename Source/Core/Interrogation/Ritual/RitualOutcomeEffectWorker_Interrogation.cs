using Mono.Cecil;
using PrisonLabor.Core.Components;
using PrisonLabor.Core.Other;
using RimWorld;
using RimWorld.QuestGen;
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
  public class RitualOutcomeEffectWorker_Interrogation : RitualOutcomeEffectWorker
  {
    public RitualOutcomeEffectWorker_Interrogation()
    {
    }

    public RitualOutcomeEffectWorker_Interrogation(RitualOutcomeEffectDef def)
      : base(def)
    {
    }

    private readonly float BaseChanceToGetIntel = 0.3f;
    public override void Apply(float progress, Dictionary<Pawn, int> totalPresence, LordJob_Ritual jobRitual)
    {
      Pawn warden = jobRitual.PawnWithRole("warden");
      Pawn prisoner = jobRitual.PawnWithRole("prisoner");
      InterrogationType interrogationType;
      float successChances = BaseChanceToGetIntel + CalculateChances(warden, prisoner, out interrogationType);
      int hits = 0;

      if (interrogationType == InterrogationType.Brutal || interrogationType == InterrogationType.Psycho)
      {
        hits += PunchPrisoner(warden, prisoner);
      }
      if (interrogationType == InterrogationType.Psycho)
      {
        hits += BitePrisoner(warden, prisoner);
      }
      if (interrogationType == InterrogationType.Kind)
      {
        BeNice(warden, prisoner);
      }
      if (hits > 0 && warden.Faction != null && prisoner.Faction != null && warden.Faction.IsPlayer && warden.IsFreeColonist && !Faction.OfPlayer.HostileTo(prisoner.Faction))
      {
        int goodwillChange = (int)(-1.3f * hits);
        Faction.OfPlayer.TryAffectGoodwillWith(prisoner.Faction, goodwillChange, canSendMessage: true, false, HistoryEventDefOf.AttackedMember, prisoner);
      }

      PrisonerComp prisonerComp = prisoner.GetComp<PrisonerComp>();
      if (Rand.Chance(successChances))
      {
        QuestScriptDef questDef = InterrogationDefsOf.PL_GenQuest.questScriptDefs.RandomElement();
        float points = StorytellerUtility.DefaultThreatPointsNow(jobRitual.Map);
        Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(questDef, points);
        QuestUtility.SendLetterQuestAvailable(quest);
        prisonerComp.HasIntel = false;
      }
      prisonerComp.LastInteractionTick = Find.TickManager.TicksGame;

    }

    private void BeNice(Pawn warden, Pawn prisoner)
    {
      if (HasTrait(warden, TraitDefOf.Kind))
      {
        TryGainMemory(prisoner, warden, InterrogationDefsOf.PL_KindInterrogation);
      }
      TryGainMemory(prisoner, warden, InterrogationDefsOf.PL_Interrogated);
    }

    private void TryGainMemory(Pawn pawn, Pawn otherPawn, ThoughtDef thoughtDef)
    {
      pawn.needs?.mood?.thoughts?.memories?.TryGainMemory(thoughtDef, otherPawn: otherPawn);
    }

    private int BitePrisoner(Pawn warden, Pawn prisoner)
    {
      DoDamage(warden, prisoner, false, DamageDefOf.Bite, InterrogationDefsOf.PL_BitMe, out bool hitDone);
      return hitDone ? 1 : 0;
    }

    private float CalculateChances(Pawn warden, Pawn prisoner, out InterrogationType iterrogationType)
    {
      if (HasTrait(warden, TraitDefOf.Kind) || warden.WorkTagIsDisabled(WorkTags.Violent))
      {
        iterrogationType = InterrogationType.Kind;
        return GoEasy(warden, prisoner);
      }
      if (HasTrait(warden, TraitDefOf.Bloodlust))
      {
        iterrogationType = InterrogationType.Brutal;
        return GoBrutal(warden, prisoner);
      }
      if (HasTrait(warden, TraitDefOf.Psychopath))
      {
        iterrogationType = InterrogationType.Psycho;
        return GoBrutal(warden, prisoner);
      }
      if (Rand.Chance(0.5f + warden.relations.OpinionOf(prisoner) * 0.005f))
      {
        iterrogationType = InterrogationType.Kind;
        return GoEasy(warden, prisoner);
      }
      else
      {
        iterrogationType = InterrogationType.Brutal;
        return GoBrutal(warden, prisoner);
      }
    }

    private float GoBrutal(Pawn warden, Pawn prisoner)
    {
      float actualChance = 0f;
      //Knows how to hit
      actualChance += warden.skills.GetSkill(SkillDefOf.Social).Level * 0.01f;
      //Knows where to hit
      actualChance += warden.skills.GetSkill(SkillDefOf.Medicine).Level * 0.01f;
      //Prisoner will is still important
      actualChance += Mathf.Clamp(prisoner.guest.will * 0.01f, 0, 1f);
      if (HasTrait(warden, TraitDefOf.Brawler))
      {
        actualChance += 0.1f;
      }
      if (HasTrait(prisoner, InterrogationDefsOf.Tough))
      {
        actualChance -= 0.3f;
      }
      if (HasTrait(prisoner, InterrogationDefsOf.Nerves))
      {
        Trait nerves = GetTrait(prisoner, InterrogationDefsOf.Nerves);
        actualChance -= nerves.Degree * 0.1f;
      }
      if (HasTrait(prisoner, TraitDefOf.Wimp))
      {
        //Wimp can't handle beating
        actualChance = 1f;
      }
      if (HasTrait(prisoner, InterrogationDefsOf.Masochist))
      {
        //Prisoner likes pain...
        actualChance = 0f;
      }
      DebugLogger.debug($"Base chances for successful Brutal Interrogation: {actualChance}");
      return actualChance;
    }

    private float GoEasy(Pawn warden, Pawn prisoner)
    {
      float actualChance = 0f;
      if (HasTrait(warden, TraitDefOf.AnnoyingVoice))
      {
        actualChance -= 0.2f;
      }
      if (HasTrait(warden, TraitDefOf.CreepyBreathing))
      {
        actualChance -= 0.1f;
      }
      if (HasTrait(warden, InterrogationDefsOf.TooSmart))
      {
        actualChance -= 0.1f;
      }
      if (HasTrait(prisoner, InterrogationDefsOf.Nerves))
      {
        Trait nerves = GetTrait(prisoner, InterrogationDefsOf.Nerves);
        actualChance -= nerves.Degree * 0.1f;
      }
      actualChance += prisoner.relations.OpinionOf(warden) * 0.002f;
      actualChance += warden.skills.GetSkill(SkillDefOf.Social).Level * 0.01f;

      actualChance += Mathf.Clamp(prisoner.guest.will * 0.01f, 0, 1f);
      DebugLogger.debug($"Base chances for successful Easy Interrogation: {actualChance}");
      return actualChance;
    }

    private bool HasTrait(Pawn pawn, TraitDef trait)
    {
      return pawn?.story?.traits != null && pawn.story.traits.HasTrait(trait);
    }

    private Trait GetTrait(Pawn pawn, TraitDef trait)
    {
      return pawn.story.traits.GetTrait(trait);
    }

    private int PunchPrisoner(Pawn warden, Pawn prisoner)
    {
      bool done = false;
      int successful = 0;
      int amountOfHits = Rand.RangeInclusive(1, warden.skills.GetSkill(SkillDefOf.Melee).Level);
      for (int i = 0; i < amountOfHits && !prisoner.Downed; i++)
      {
        done = DoDamage(warden, prisoner, done, DamageDefOf.Blunt, InterrogationDefsOf.PL_BrutallyInterrogated, out bool hitDone);
        successful += hitDone ? 1 : 0;
      }
      return successful;
    }

    private bool DoDamage(Pawn warden, Pawn prisoner, bool done, DamageDef damageDef, ThoughtDef interrogationThoughDef, out bool hitDone)
    {
      hitDone = false;
      IEnumerable<VerbEntry> verbs = warden.meleeVerbs.GetUpdatedAvailableVerbsList(false).Where(ve => ve.verb.GetDamageDef() == damageDef);
      if (verbs.Any())
      {
        Verb verb = verbs.RandomElement().verb;
        float damage = verb.verbProps.AdjustedMeleeDamageAmount(verb, warden);
        DamageInfo damageInfo = new DamageInfo(def: verb.verbProps.meleeDamageDef, amount: damage, instigatorGuilty: false);
        prisoner.TakeDamage(damageInfo);
        prisoner.pather?.StopDead();
        hitDone = true;
        if (!done)
        {
          done = true;
          TryGainMemory(prisoner, warden, interrogationThoughDef);
        }

      }
      return done;
    }
  }
  internal enum InterrogationType
  {
    Kind,
    Brutal,
    Psycho
  }
}
