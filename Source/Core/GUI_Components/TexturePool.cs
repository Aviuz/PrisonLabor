using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.GUI_Components
{
    [StaticConstructorOnStartup]
    public class TexturePool
    {
        public static readonly Material watchedTexture = MaterialPool.MatFrom("InspireIcon", ShaderDatabase.MetaOverlay);
        public static readonly Material lazyTexture = MaterialPool.MatFrom("LazyIcon", ShaderDatabase.MetaOverlay);
        public static readonly Material freezingTexture = MaterialPool.MatFrom("FreezingIcon", ShaderDatabase.MetaOverlay);
    }
}
