using PrisonLabor.Constants;
using PrisonLabor.Core.Trackers;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class ColumnWorker_HasHandscuffs : PawnColumnWorker_Checkbox
    {
        protected override bool GetValue(Pawn pawn)
        {
            CuffsTracker cuffsTracker = pawn.Map.GetComponent<CuffsTracker>();
            if (cuffsTracker != null && cuffsTracker.handscuffTracker.TryGetValue(pawn, out bool checkboxValue))
            {
                return checkboxValue;               
            }
            return pawn.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedHandscuffs, false) == null;
        }

        protected override void SetValue(Pawn pawn, bool value, PawnTable table)
        {
            CuffsTracker cuffsTracker = pawn.Map.GetComponent<CuffsTracker>();
            if(cuffsTracker != null)
            {
                if(value == (pawn.health.hediffSet.GetFirstHediffOfDef(PL_DefOf.PrisonLabor_RemovedHandscuffs, false) == null))
                {
                    cuffsTracker.handscuffTracker.Remove(pawn);                    
                }
                else
                {
                    cuffsTracker.handscuffTracker.Add(pawn, value);
                }
            }
            table.SetDirty();
        }
    }
}
