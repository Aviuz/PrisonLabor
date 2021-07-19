using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class PrisonersTabDef : Def
    {
        public Type workerClass;
        public int order;
        public bool dev = false;
    }
}
