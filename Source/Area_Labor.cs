using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace PrisonLabor
{
    class Area_Labor : Area
    {
        public override string Label
        {
            get
            {
                return "Labor";
            }
        }

        public override Color Color
        {
            get
            {
                return new Color(0.92f, 0.37f, 0.0f);
            }
        }

        public override int ListPriority
        {
            get
            {
                return 8000;
            }
        }

        public Area_Labor()
        {
        }

        public Area_Labor(AreaManager areaManager) : base(areaManager)
		{
        }

        public override string GetUniqueLoadID()
        {
            return "Area_" + this.ID + "_Labor";
        }
    }
}
