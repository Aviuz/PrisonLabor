using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonLabor.Therapy.Comps
{
    public class CompProperties_AssingPatients : CompProperties_AssignableToPawn
    {
        public CompProperties_AssingPatients()
        {
            compClass = typeof(AssignableToPrisoner_Comp);
        }
    }
}
