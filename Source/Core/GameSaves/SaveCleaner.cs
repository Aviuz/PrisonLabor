using PrisonLabor.Constants;
using PrisonLabor.Core.GUI_Components;
using PrisonLabor.Core.LaborArea;
using PrisonLabor.Core.Needs;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Verse;

namespace PrisonLabor.Core.GameSaves
{
    public static class SaveCleaner
    {
        public static void BackupSavegame(string fileName)
        {
            string savegamePath = GenFilePaths.FilePathForSavedGame(fileName);
            string backupPath = GetFilePathForBackup(savegamePath);

            File.Copy(savegamePath, backupPath, false);
            Log.Message($"Save copied to \"{backupPath}\"");
        }

        public static void RemoveFromSave(string fileName)
        {
            LongEventHandler.QueueLongEvent(
                () => UpdateFile(fileName),
                "Removing",
                false,
                (e) => OnError(e)
                );
        }

        private static void UpdateFile(string fileName)
        {
            string filePath = GenFilePaths.FilePathForSavedGame(fileName);

            XmlElement xmlNode;
            using (StreamReader streamReader = new StreamReader(filePath))
            {
                using (XmlTextReader xmlTextReader = new XmlTextReader(streamReader))
                {
                    var XmlDocument = new XmlDocument();
                    XmlDocument.Load(xmlTextReader);
                    xmlNode = XmlDocument.DocumentElement;
                }
            }

            UpdateData(xmlNode);

            using (FileStream saveStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                var xmlWriterSettings = new XmlWriterSettings();
                xmlWriterSettings.Indent = true;
                xmlWriterSettings.IndentChars = "\t";
                using (XmlWriter writer = XmlWriter.Create(saveStream, xmlWriterSettings))
                {
                    writer.WriteStartDocument();
                    writer.WriteNode(xmlNode.CreateNavigator(), false);
                }
            }

            Log.Message($"Save'{fileName}' converted successfuly");
        }

        private static void UpdateData(XmlElement xmlNode)
        {
            List<XmlNode> removalBuffer = new List<XmlNode>();
            XmlNode curNode = xmlNode;

            #region Meta
            var metaNode = xmlNode["meta"];

            // Meta.ModIds & Meta.ModNames
            XmlNode modIdsNode = metaNode["modIds"], modNamesNode = metaNode["modNames"];
            for (int i = 0; i < modIdsNode.ChildNodes.Count; i++)
            {
                var modNode = modIdsNode.ChildNodes[i];

                if (modNode.InnerText == "972057888" || modNode.InnerText == "PrisonLabor")
                {
                    modIdsNode.RemoveChild(modIdsNode.ChildNodes[i]);
                    modNamesNode.RemoveChild(modNamesNode.ChildNodes[i]);
                    break;
                }
            }

            // Remove version of PL
            var plVersionInfo = metaNode["PrisonLaborVersion"];
            if (plVersionInfo != null)
                metaNode.RemoveChild(plVersionInfo);
            #endregion

            #region Game
            var gameNode = xmlNode["game"];

            // Game.Tutor
            string[] conceptDefs = { "PrisonLabor_Indroduction", "PrisonLabor_Motivation", "PrisonLabor_Growing", "PrisonLabor_Management", "PrisonLabor_Timetable" };

            var tutorNode = gameNode["tutor"];
            //var activeLessonsNode = tutorNode["activeLesson"];
            var learningReadoutNode = tutorNode["learningReadout"];
            if (learningReadoutNode["activeConcepts"].HasChildNodes)
            {
                removalBuffer.Clear();
                foreach (XmlNode concept in learningReadoutNode["activeConcepts"].ChildNodes)
                {
                    foreach (string conceptDef in conceptDefs)
                    {
                        if (concept.InnerText == conceptDef)
                            removalBuffer.Add(concept);
                    }
                }
                foreach (var concept in removalBuffer)
                    learningReadoutNode["activeConcepts"].RemoveChild(concept);
            }
            if (learningReadoutNode["selectedConcept"] != null)
            {
                removalBuffer.Clear();
                foreach (string conceptDef in conceptDefs)
                {
                    if (learningReadoutNode["selectedConcept"].InnerText == conceptDef)
                        learningReadoutNode.RemoveChild(learningReadoutNode["selectedConcept"]);
                }
            }
            //var tutorialStateNode = tutorNode["tutorialState"];

            // Game.Maps
            foreach (XmlNode mapNode in gameNode["maps"].ChildNodes)
            {
                // Game.Maps.AreaManager
                var areaManagerNode = mapNode["areaManager"];

                if (areaManagerNode["areas"] != null && areaManagerNode["areas"].HasChildNodes)
                {
                    removalBuffer.Clear();
                    foreach (XmlNode areaNode in areaManagerNode["areas"].ChildNodes)
                    {
                        if (areaNode.Attributes["Class"].Value == typeof(Area_Labor).FullName)
                        {
                            removalBuffer.Add(areaNode);
                        }
                    }
                    foreach (var node in removalBuffer)
                        areaManagerNode["areas"].RemoveChild(node);
                }

                //Game.Maps.Components
                var components = mapNode["components"];
                removalBuffer.Clear();
                foreach (XmlNode component in components)
                {
                    if (component.Attributes["Class"].Value == "PrisonLabor.Core.GUI_Components.PawnIcons")
                    {
                        removalBuffer.Add(component);
                    }
                }
                foreach (var item in removalBuffer)
                    components.RemoveChild(item);
            }

            // TODO bills

            // Interaction Mode
            string[] interactions = { PL_DefOf.PrisonLabor_workOption.defName, PL_DefOf.PrisonLabor_workAndRecruitOption.defName, PL_DefOf.PrisonLabor_workAndConvertOption.defName, PL_DefOf.PrisonLabor_workAndEnslaveOption.defName };

            foreach (var guestTracker in gameNode.GetEveryNode("guest"))
            {
                var interactionMode = guestTracker["interactionMode"];
                if (interactionMode != null)
                {
                    foreach (string interaction in interactions)
                    {
                        if (interactionMode.InnerText == interaction)
                            interactionMode.InnerText = PrisonerInteractionModeDefOf.NoInteraction.defName;
                    }
                }
            }


            // Remove Heddifs
            foreach (var needTracker in gameNode.GetEveryNode("needs"))
            {
                var needs = needTracker["needs"];
                if (needs != null)
                {

                    if (needs != null && needs.HasChildNodes)
                    {
                        removalBuffer.Clear();
                        foreach (XmlNode need in needs.ChildNodes)
                        {
                            if (need.Attributes["Class"].Value == typeof(Need_Motivation).FullName)
                                removalBuffer.Add(need);
                            else if (need.Attributes["Class"].Value == typeof(Need_Treatment).FullName)
                                removalBuffer.Add(need);
                        }
                        foreach (var node in removalBuffer)
                            needs.RemoveChild(node);
                    }
                }
            }

            // Remove Heddifs
            foreach (var hediffSet in gameNode.GetEveryNode("hediffSet"))
            {
                var hediffs = hediffSet["hediffs"];

                if (hediffs != null && hediffs.HasChildNodes)
                {
                    removalBuffer.Clear();
                    foreach (XmlNode hediff in hediffs.ChildNodes)
                    {
                        if (hediff["def"].InnerText == "PrisonLabor_PrisonerChains")
                        {
                            removalBuffer.Add(hediff);
                        }
                    }
                    foreach (var node in removalBuffer)
                        hediffs.RemoveChild(node);
                }
            }
            #endregion
        }

        private static IEnumerable<XmlNode> GetEveryNode(this XmlNode rootElement, string nodeName)
        {
            foreach (XmlNode node in rootElement.ChildNodes)
            {
                if (node.Name.Equals(nodeName))
                    yield return node;
                if (node.HasChildNodes)
                {
                    foreach (XmlNode childNode in node.GetEveryNode(nodeName))
                        yield return childNode;
                }
            }
        }

        private static void OnError(Exception e)
        {
            Log.Error(e.ToString());
        }

        private static string GetFilePathForBackup(string filePath)
        {
            string originFilePathWithoutExtension = Path.GetDirectoryName(filePath) + @"\" + Path.GetFileNameWithoutExtension(filePath);

            string backupFileCoreString = originFilePathWithoutExtension + "_Backup";

            string backupFilePathFinal = backupFileCoreString + ".rws";

            if (!File.Exists(backupFilePathFinal))
                return backupFilePathFinal;

            for (int i = 1; i < int.MaxValue; i++)
            {
                backupFilePathFinal = backupFileCoreString + i.ToString() + ".rws";

                if (!File.Exists(backupFilePathFinal))
                    return backupFilePathFinal;
            }

            throw new IndexOutOfRangeException();
        }
    }
}
