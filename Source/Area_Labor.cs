using UnityEngine;
using Verse;

namespace PrisonLabor
{
    internal class Area_Labor : Area
    {
        public Area_Labor()
        {
        }

        public Area_Labor(AreaManager areaManager) : base(areaManager)
        {
        }

        public override string Label => "Labor";

        public override Color Color => new Color(0.92f, 0.37f, 0.0f);

        public override int ListPriority => 8000;

        public override string GetUniqueLoadID()
        {
            return "Area_" + ID + "_Labor";
        }
    }
}