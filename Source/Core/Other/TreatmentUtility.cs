using PrisonLabor.Core.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;

namespace PrisonLabor.Core.Other
{
    public static class TreatmentUtility
    {
        public static void OnApplyDamage(Pawn_HealthTracker instance, DamageInfo dinfo, bool absorbed)
        {
            if (dinfo.Instigator != null && !(dinfo.Instigator is Pawn))
                return;

            Pawn attacker = dinfo.Instigator as Pawn;
            Pawn victim = (Pawn)(typeof(Pawn_HealthTracker).GetField("pawn", BindingFlags.NonPublic | BindingFlags.Instance)?.GetValue(instance));

            if (attacker == null || victim == null)
                return;

            if (victim.IsPrisonerOfColony && attacker.IsColonist)
            {
                var need = victim.needs.TryGetNeed<Need_Treatment>();
                if (need != null)
                    need.NotifyPrisonerBeaten(dinfo, absorbed);
            }
        }
    }
}
