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
    [MayRequireIdeology]
    public static InterrogationQuestGenDef PL_GenQuest;
    [MayRequireIdeology]
    public static ThoughtDef PL_BitMe;
    [MayRequireIdeology]
    public static ThoughtDef PL_KindInterrogation;
    [MayRequireIdeology]
    public static ThoughtDef PL_Interrogated;
    [MayRequireIdeology]
    public static ThoughtDef PL_BrutallyInterrogated;
    [MayRequireIdeology]
    public static JobDef PL_Interrogate;
    [MayRequireIdeology]
    public static InteractionDef PL_InterrogateInteraction;
    [MayRequireIdeology]
    public static InteractionDef PL_BeIntrrogatedInteraction;
  }
}
