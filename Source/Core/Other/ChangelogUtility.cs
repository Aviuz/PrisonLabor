using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace PrisonLabor.Core.Other
{
    public class ChangelogUtility
    {
        public struct Patch
        {
            public string version;
            public IList<string> patchNotes;

            public override string ToString() => $"version {version}";
        }

        public static IEnumerable<Patch> GetPatchesFromText(string text)
        {
            var lines = Regex.Split(text, "\r\n|\r|\n");

            if (lines.Length < 2)
                yield break;

            string currentPatch = lines[0];
            List<string> currentPatchNotes = new List<string>();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i];

                var match = Regex.Match(line, "^\\*|-|=|\t|#");

                if (match.Success)
                {
                    currentPatchNotes.Add(line.Replace(match.Value, "").Trim());
                }
                else
                {
                    yield return new Patch() { version = currentPatch, patchNotes = currentPatchNotes };
                    currentPatch = line;
                    currentPatchNotes = new List<string>();
                }
            }
        }
    }
}