using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using Verse;

namespace PrisonLabor.Core.Other
{
    public static class NewsProvider
    {
        public struct VersionNotes
        {
            public string version;
            public string[] entries;
        }

        public static VersionNotes[] allVersionNotes;

        static NewsProvider()
        {
            try
            {

                Log.Message("news initialized");

                allVersionNotes = GetVersionNotesFromChangelog(Properties.Resources.changelog).ToArray();

                int iterator = 0;
                foreach (var notes in GetVersionNotesFromNewsFeed(Properties.Resources.NewsFeed))
                {
                    while (allVersionNotes[iterator].version != notes.version && iterator < allVersionNotes.Length)
                        iterator++;

                    if (iterator < allVersionNotes.Length)
                    {
                        allVersionNotes[iterator] = notes;
                    }
                    else
                    {
                        Log.Warning($"PrisonLabor: couldn't find version {notes.version} to alter");
                        iterator = 0;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("PrisonLabor: Failed to initialize news" + e.ToString());
            }
        }

        public static IEnumerable<VersionNotes> NewsAfterVersion(string versionString)
        {
            bool stop = false;
            for (int i = 0; i < allVersionNotes.Length && stop; i++)
            {
                if (allVersionNotes[i].version == versionString)
                    stop = true;
                else
                    yield return allVersionNotes[i];
            }
        }

        private static IEnumerable<VersionNotes> GetVersionNotesFromChangelog(string text)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n");

            if (lines.Length < 2)
                yield break;

            string currentPatch = lines[0];
            List<string> currentPatchNotes = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                var match = Regex.Match(line, "^\\*|-|\t");

                if (match.Success)
                {
                    currentPatchNotes.Add("[-]" + line.Replace(match.Value, "").Trim());
                }
                else
                {
                    if (currentPatchNotes.Count > 0)
                    {
                        currentPatchNotes.Insert(0, $"[title]Prison Labor v{currentPatch}");
                        yield return new VersionNotes() { version = currentPatch, entries = currentPatchNotes.ToArray() };
                    }
                    currentPatch = line;
                    currentPatchNotes = new List<string>();
                }
            }
        }

        private static IEnumerable<VersionNotes> GetVersionNotesFromNewsFeed(string xml)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            foreach (XmlNode patch in xmlDocument.DocumentElement["patches"].ChildNodes)
            {
                if (patch.Name == "patch")
                {
                    var entries = new List<string>();
                    entries.Add($"[title]{patch["title"].InnerXml}");
                    foreach (XmlNode item in patch["items"].ChildNodes)
                        entries.Add(item.InnerXml);
                    yield return new VersionNotes()
                    {
                        version = patch.Attributes["version"].Value,
                        entries = entries.ToArray(),
                    };
                }
            }
        }
    }
}
