using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Other
{
    public class ScribeUtils<T, S>
    {
        List<T> tmpKeys;
        List<S> tmpVals;

        public void Scribe(ref Dictionary<T, S> dict, string name)
        {
            if (Verse.Scribe.mode == LoadSaveMode.Saving)
            {
                tmpKeys = new List<T>(dict.Keys);
                tmpVals = new List<S>(dict.Values);
            }

            Scribe_Collections.Look(ref tmpKeys, $"{name}.keys", LookMode.Reference);
            Scribe_Collections.Look(ref tmpVals, $"{name}.vals", LookMode.Deep);

            if (Verse.Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                dict = new Dictionary<T, S>();
                for (var i = 0; i < tmpKeys.Count; i++)
                    if (tmpKeys[i] != null && tmpVals[i] != null)
                        dict[tmpKeys[i]] = tmpVals[i];
            }
        }
    }
}
