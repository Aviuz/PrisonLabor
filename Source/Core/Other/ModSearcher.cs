using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Other
{
    public class ModSearcher
    {
        private readonly string modId;
        public ModSearcher(string modName)
        {
            this.modId = modName;
        }

        public bool LookForMod()
        {
            bool modFound = false;
            try
            {
                var mod = LoadedModManager.RunningMods.First(m => m.Name == modId);
                modFound = mod != null;
            }
            catch
            {
                //No needed to handle
            }

            Verse.Log.Message($"[PL] Trying to find: {modId}, Result: {modFound}");
            return modFound;
        }
    }
}
