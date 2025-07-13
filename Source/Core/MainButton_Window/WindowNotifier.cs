using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.Core.MainButton_Window
{
    public class WindowNotifier
    {
        public static void NotifyPLWindows()
        {
            if (Find.WindowStack != null)
            {
                WindowStack windowStack = Find.WindowStack;
                for (int i = 0; i < windowStack.Count; i++)
                {
                    (windowStack[i] as CustomTabWindow)?.Notify_PawnsChanged();
                    (windowStack[i] as PrisonerButtonWindow)?.Notify_PawnsChanged();
                }
            }
        }
    }
}
