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

        public static MainButtonDef PL_Prisoners_Menu;

        static UIDefsOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(UIDefsOf));
        }
    }
}
