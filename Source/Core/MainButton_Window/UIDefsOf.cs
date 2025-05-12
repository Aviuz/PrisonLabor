using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonLabor.Core.MainButton_Window
{
    [DefOf]
     public static class UIDefsOf
    {
        public static PawnTableDef PL_Assign;
        public static PawnTableDef PL_Overview;
        public static PawnTableDef PL_DevTable;
        static UIDefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(UIDefsOf));
        }
    }

    [DefOf]
    public static class MainButtonDefOf {
        public static MainButtonDef PL_Prisoners_Menu;
    }
}
