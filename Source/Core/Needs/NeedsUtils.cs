using PrisonLabor.Core.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Needs
{
    public static class NeedsUtils
    {
        public static bool IsMotivated(this Pawn pawn)
        {
            var need = pawn.needs.TryGetNeed<Need_Motivation>();
            return !PrisonLaborPrefs.EnableMotivationMechanics || ( need != null && !need.IsLazy);
        }
    }
}
