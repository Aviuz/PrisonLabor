using System.Collections.Generic;
using PrisonLabor.Core.Meta;
using RimWorld;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Other
{
    public static class Tutorials
    {
        private static readonly ConceptDef introductionDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Indroduction", true);

        private static readonly ConceptDef motivationDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Motivation", true);

        private static readonly ConceptDef growingDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Growing", true);

        private static readonly ConceptDef managementDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Management", true);

        private static readonly ConceptDef timetableDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Timetable", true);

        private static readonly ConceptDef treatmentDef = DefDatabase<ConceptDef>.GetNamed("PrisonLabor_Treatment", true);

        private static readonly List<ConceptDef> triggeredTutorials = new List<ConceptDef>();
        private static float lastTutorTime;

        public static void Apply()
        {
            if (PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.Introduction))
                PlayerKnowledgeDatabase.SetKnowledge(introductionDef, 1.0f);
            if (PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.Motivation))
                PlayerKnowledgeDatabase.SetKnowledge(motivationDef, 1.0f);
            if (PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.Growing))
                PlayerKnowledgeDatabase.SetKnowledge(growingDef, 1.0f);
            if (PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.Managment))
                PlayerKnowledgeDatabase.SetKnowledge(managementDef, 1.0f);
            if (PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.Timetable))
                PlayerKnowledgeDatabase.SetKnowledge(timetableDef, 1.0f);
            if (PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.Treatment))
                PlayerKnowledgeDatabase.SetKnowledge(treatmentDef, 1.0f);
        }

        public static void Introduction()
        {
            TryActivateTutorial(introductionDef, OpportunityType.Important);
        }

        public static void Motivation()
        {
            TryActivateTutorial(motivationDef, OpportunityType.Important);
        }

        public static void Management()
        {
            TryActivateTutorial(managementDef, OpportunityType.GoodToKnow);
        }

        public static void Timetable()
        {
            TryActivateTutorial(timetableDef, OpportunityType.GoodToKnow);
        }

        public static void Growing()
        {
            TryActivateTutorial(growingDef, OpportunityType.Important);
        }

        public static void Treatment()
        {
            TryActivateTutorial(treatmentDef, OpportunityType.Important);
        }

        public static void LaborAreaWarning()
        {
            if (!PrisonLaborPrefs.HasTutorialFlag(TutorialFlag.LaborAreaWarning))
                Find.WindowStack.Add(
                    new Dialog_MessageBox(
                        "PrisonLabor_LaborAreaWarning".Translate(),
                        "PrisonLabor_DontShowAgain".Translate(),
                        () => { PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.LaborAreaWarning); PrisonLaborPrefs.Save(); }
                        ));
        }

        private static void TryActivateTutorial(ConceptDef def, OpportunityType opr)
        {
            if (!triggeredTutorials.Contains(def))
                if (opr >= OpportunityType.Important || IsReady())
                {
                    lastTutorTime = Time.time;

                    LessonAutoActivator.TeachOpportunity(def, opr);
                    triggeredTutorials.Add(def);
                }
        }

        private static bool IsReady()
        {
            if (Time.time - lastTutorTime >= 60f)
                return true;
            return false;
        }

        public static void UpdateTutorialFlags()
        {
            if (PlayerKnowledgeDatabase.IsComplete(introductionDef))
                PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.Introduction);
            if (!PlayerKnowledgeDatabase.IsComplete(motivationDef))
                PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.Motivation);
            if (!PlayerKnowledgeDatabase.IsComplete(managementDef))
                PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.Managment);
            if (!PlayerKnowledgeDatabase.IsComplete(timetableDef))
                PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.Timetable);
            if (!PlayerKnowledgeDatabase.IsComplete(growingDef))
                PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.Growing);
            if (!PlayerKnowledgeDatabase.IsComplete(treatmentDef))
                PrisonLaborPrefs.AddTutorialFlag(TutorialFlag.Treatment);
        }

        public static void Reset()
        {
            PlayerKnowledgeDatabase.SetKnowledge(introductionDef, 0.0f);
            PlayerKnowledgeDatabase.SetKnowledge(motivationDef, 0.0f);
            PlayerKnowledgeDatabase.SetKnowledge(managementDef, 0.0f);
            PlayerKnowledgeDatabase.SetKnowledge(timetableDef, 0.0f);
            PlayerKnowledgeDatabase.SetKnowledge(growingDef, 0.0f);
            PlayerKnowledgeDatabase.SetKnowledge(treatmentDef, 0.0f);
        }
    }
}