using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.Components
{
    public class PrisonerCompProperties : CompProperties
    {
        public PrisonerCompProperties()
        {
            this.compClass = typeof(PrisonerComp);
        }

    }
}
