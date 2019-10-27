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
            public bool? silent;
            public string title;
            public string[] entries;
        }

        public static VersionNotes[] allVersionNotes;

        static NewsProvider()
        {
            try
            {
                allVersionNotes = GetVersionNotesFromChangelog(Properties.Resources.changelog).ToArray();

                int iterator = 0;
                foreach (var notes in GetVersionNotesFromNewsFeed(Properties.Resources.NewsFeed))
                {
                    while (allVersionNotes[iterator].version != notes.version && iterator < allVersionNotes.Length)
                        iterator++;

                    if (iterator < allVersionNotes.Length)
                    {
                        allVersionNotes[iterator] = Combine(allVersionNotes[iterator], notes);
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

        public static bool ShouldAutoShowChangelog(string lastVersion)
        {
            for (int i = 0; i < allVersionNotes.Length; i++)
            {
                if (allVersionNotes[i].version == lastVersion)
                    return false;
                else if (allVersionNotes[i].silent == false)
                    return true;
            }
            return false;
        }

        public static IEnumerable<VersionNotes> NewsAfterVersion(string versionString)
        {
            bool stop = false;
            for (int i = 0; i < allVersionNotes.Length && !stop; i++)
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
                        yield return new VersionNotes() 
                        { 
                            version = currentPatch,
                            silent = true,
                            title= $"[title]Prison Labor v{currentPatch}",
                            entries = currentPatchNotes.ToArray()
                        };
                    }
                    currentPatch = line;
                    currentPatchNotes = new List<string>();
                }
            }

            // Last iteration
            if (currentPatchNotes.Count > 0)
            {
                yield return new VersionNotes()
                {
                    version = currentPatch,
                    silent = true,
                    title = $"[title]Prison Labor v{currentPatch}",
                    entries = currentPatchNotes.ToArray()
                };
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
                    var versionNotes = new VersionNotes();

                    if(patch.Attributes["version"] != null)
                    {
                        versionNotes.version = patch.Attributes["version"].Value;
                    }
                    else
                    {
                        continue;
                    }

                    if (patch["title"] != null)
                    {
                        versionNotes.title = $"[title]{patch["title"].InnerXml}";
                    }
                    else
                    {
                        versionNotes.title = null;
                    }

                    if(patch.Attributes["silent"] != null)
                    {
                        versionNotes.silent = bool.Parse(patch.Attributes["silent"].Value);
                    }
                    else
                    {
                        versionNotes.silent = null;
                    }

                    if (patch["items"] != null)
                    {
                        var entries = new List<string>();
                        foreach (XmlNode item in patch["items"].ChildNodes)
                            entries.Add(item.InnerXml);
                        versionNotes.entries = entries.ToArray();
                    }

                    yield return versionNotes;
                }
            }
        }

        private static VersionNotes Combine(VersionNotes orginal, VersionNotes target)
        {
            if (target.version != orginal.version)
                throw new Exception("Bad news version");

            if (target.title != null)
                orginal.title = target.title;

            if (target.silent != null)
                orginal.silent = target.silent;

            if (target.entries != null)
                orginal.entries = target.entries;

            return orginal;
        }
    }
}
