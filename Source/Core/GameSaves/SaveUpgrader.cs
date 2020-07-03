using PrisonLabor.Core.GUI_Components;
using PrisonLabor.Core.Meta;
using PrisonLabor.Core.Needs;
using System;
using System.Xml;
using Verse;
using Version = PrisonLabor.Core.Meta.Version;

namespace PrisonLabor.Core.GameSaves
{
    static class SaveUpgrader
    {
        public static void Upgrade()
        {
            if (Scribe.mode == LoadSaveMode.LoadingVars || Scribe.mode == LoadSaveMode.ResolvingCrossRefs || Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (VersionUtility.VersionOfSaveFile == VersionUtility.versionNumber)
                    return;

                LongEventHandler.SetCurrentEventText("PrisonLabor_UpgradeSaveProcessMessage".Translate());

                var xmlNode = Scribe.loader.curXmlParent;

                if (VersionUtility.VersionOfSaveFile < Version.v0_10_0)
                    UpTo_0_10(xmlNode);
            }
        }

        private static void UpTo_0_10(XmlNode xml)
        {
            // Replace job trackers
            xml.InnerXml = xml.InnerXml.Replace("<curDriver Class=\"PrisonLabor.JobDriver_FoodDeliver_Tweak\">", "<curDriver Class=\"JobDriver_FoodDeliver\">");

            // Replace job defs
            xml.InnerXml = xml.InnerXml.Replace("<def>PrisonLabor_DeliverFood_Tweak</def>", "<def>DeliverFood</def>");

            // Replace need classes
            xml.InnerXml = xml.InnerXml.Replace("PrisonLabor.Need_Motivation", typeof(Need_Motivation).FullName);

            // Replace need classes
            xml.InnerXml = xml.InnerXml.Replace("PrisonLabor.Need_Treatment", typeof(Need_Treatment).FullName);

            // Replace need classes
            // xml.InnerXml = xml.InnerXml.Replace("PrisonLabor.MapComponent_Icons", typeof(PawnIcons).FullName);
        }
    }
}
