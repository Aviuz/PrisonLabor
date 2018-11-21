using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor.Tweaks
{
    static class SaveUpgrader
    {
        // Pawn_JobTracker.ExposeData()
        public static void Pawn_JobTracker()
        {
            if (VersionUtility.VersionOfSaveFile < Version.v0_10_0)
            {
                var classAttribute = Scribe.loader?.curXmlParent?["curDriver"]?.Attributes?["Class"];

                if (classAttribute != null && classAttribute.Value == "PrisonLabor.JobDriver_FoodDeliver_Tweak")
                {
                    classAttribute.Value = "JobDriver_FoodDeliver";
                }
            }
        }

        // Job.ExposeData()
        public static void Job()
        {
            if (VersionUtility.VersionOfSaveFile < Version.v0_10_0)
            {
                var def = Scribe.loader?.curXmlParent?["def"];

                if (def != null && def.InnerText == "PrisonLabor_DeliverFood_Tweak")
                {
                    def.InnerText = "DeliverFood";
                }
            }
        }
    }
}
