using HarmonyLib;
using PrisonLabor.Constants;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

//Faction newFaction, Pawn recruiter = null

namespace PrisonLabor.HarmonyPatches.Patches_WorkSettings
{
    [HarmonyPatch(typeof(InteractionWorker_RecruitAttempt))]
    [HarmonyPatch("DoRecruit")]
    [HarmonyPatch(new Type[] { typeof(Pawn), typeof(Pawn), typeof(float), typeof(string), typeof(string), typeof(bool), typeof(bool) },
        new ArgumentType[] { ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out, ArgumentType.Out, ArgumentType.Normal, ArgumentType.Normal })]
    class Patch_ResetWorktableWhenRecruited
    {
        static void Prefix(Pawn recruiter, Pawn recruitee)
        {            
            if(recruitee != null && recruitee.IsPrisonerOfColony && recruiter != null && recruiter.Faction == Faction.OfPlayer)
            {
                CleanPrisonersStatus.Clean(recruitee);
                Log.Message($"[PrisonLabor] Removed prisoners effects from {recruitee.LabelShort}");
            }
        }

    }
}
