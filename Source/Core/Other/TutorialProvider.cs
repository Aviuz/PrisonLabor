using System;
using System.Collections.Generic;
using System.Xml;
using Verse;

namespace PrisonLabor.Core.Other
{
    public static class TutorialProvider
    {
        public struct TutorialNode
        {
            public string[] entries;
        }

        public static Dictionary<string, TutorialNode> TutorialNodes = new Dictionary<string, TutorialNode>();

        static TutorialProvider()
        {
            try
            {
                Log.Message("PrisonLabor: tutorials initialized");

                GetVersionNotesFromTutorialFeed(Properties.Resources.TutorialFeed);
            }
            catch (Exception e)
            {
                Log.Error("PrisonLabor: Failed to initialize tutorials" + e.ToString());
            }
        }

        private static void GetVersionNotesFromTutorialFeed(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            foreach (XmlNode tutorial in xmlDocument.DocumentElement["tutorials"].ChildNodes)
            {
                if (tutorial.Name == "tutorial")
                {
                    var entries = new List<string>();
                    foreach (XmlNode item in tutorial["items"].ChildNodes)
                        entries.Add(item.InnerXml);

                    TutorialNodes.Add(
                        key: tutorial.Attributes["key"].Value,
                        value: new TutorialNode() { entries = entries.ToArray(), }
                    );
                }
            }
        }
    }
}
