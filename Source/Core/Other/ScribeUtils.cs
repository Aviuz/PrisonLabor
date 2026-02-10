using System.Collections.Generic;
using System.Linq;
using Verse;

namespace PrisonLabor.Core.Other
{
  public class ScribeUtils
  {
    private List<Pawn> tmpKeys;
    private List<bool> tmpVals;

    public void Scribe(ref Dictionary<Pawn, bool> dict, string name)
    {
      if (Verse.Scribe.mode == LoadSaveMode.Saving)
      {
        tmpKeys = new List<Pawn>(dict.Keys);
        tmpVals = new List<bool>(dict.Values);
      }

      Scribe_Collections.Look(ref tmpKeys, $"{name}.keys", LookMode.Reference);
      Scribe_Collections.Look(ref tmpVals, $"{name}.vals", LookMode.Deep);

      if (Verse.Scribe.mode == LoadSaveMode.PostLoadInit)
      {
        dict = new Dictionary<Pawn, bool>();
        for (var i = 0; i < tmpKeys.Count; i++)
        {
          if (tmpKeys[i] != null)
          {
            dict[tmpKeys[i]] = tmpVals.ElementAtOrDefault(i);
          }
        }
      }
    }
  }
}