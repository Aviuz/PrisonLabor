using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;

namespace PrisonLabor.Core.Interrogation
{
  [DefOf]
  public static class InterrogationDefsOf
  {
    public static InterrogationQuestGenDef PL_GenQuest;
    public static ThoughtDef PL_BitMe;
    public static ThoughtDef PL_KindInterrogation;
    public static ThoughtDef PL_Interrogated;
    public static ThoughtDef PL_BrutallyInterrogated;
    public static JobDef PL_Interrogate;
    public static InteractionDef PL_InterrogateInteraction;
    public static InteractionDef PL_BeIntrrogatedInteraction;
  }
}
