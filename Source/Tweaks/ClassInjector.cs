using Harmony.ILCopying;
using PrisonLabor.Constants;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using Verse;
using Verse.AI;

namespace PrisonLabor.Tweaks
{
    public static class ClassInjector
    {
        public static void Init()
        {
            UITweaks();
            JobTweaks();
            SplitWardenType();
        }

        private static void UITweaks()
        {
            // Replace work tab with custom one
            var workTab = DefDatabase<MainButtonDef>.GetNamed("Work");
            MainTabWindow_Work_Tweak.MainTabWindowType = workTab.tabWindowClass;
            workTab.tabWindowClass = typeof(MainTabWindow_Work_Tweak);

            // Replace assign tab with custom one
            var assignTab = DefDatabase<MainButtonDef>.GetNamed("Restrict");
            MainTabWindow_Restrict_Tweak.MainTabWindowType = assignTab.tabWindowClass;
            assignTab.tabWindowClass = typeof(MainTabWindow_Restrict_Tweak);
        }

        private static void JobTweaks()
        {
            // Mine
            var minerJob = JobDefOf.Mine;
            minerJob.driverClass = typeof(JobDriver_Mine_Tweak);

            // Cut plant
            var cutPlantJob = JobDefOf.CutPlant;
            cutPlantJob.driverClass = typeof(JobDriver_PlantCut_Tweak);

            // Harvest designated
            {
                var instance = new JobDriver_PlantHarvest_Designated();
                var enumerator = typeof(JobDriver_PlantHarvest_Designated).GetMethod("MakeNewToils", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(instance, null) as IEnumerable<Toil>;
                if (enumerator != null)
                {
                    string myDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    using (StreamWriter outputFile = new StreamWriter(myDesktopPath + @"\" + "toils" + ".txt"))
                    {
                        foreach (var toil in enumerator)
                        {
                            outputFile.WriteLine($"{toil.GetType()}");
                            outputFile.WriteLine($"CompleteMode: {toil.defaultCompleteMode.ToString()}");
                            outputFile.WriteLine($"SocialMode: {toil.socialMode}");
                            if (toil.initAction != null && toil.initAction.Method != null)
                            {
                                outputFile.WriteLine($"  InitAction:");
                                var dynMethod = new DynamicMethod(toil.initAction.Method.Name, toil.initAction.Method.ReturnType, null);
                                foreach (ILInstruction ci in MethodBodyReader.GetInstructions(dynMethod.GetILGenerator(), toil.initAction.Method))
                                {
                                    var operand = ci.operand is Label ? ("Label " + ci.operand.GetHashCode()).ToString() : ci.operand;
                                    string labels = "";
                                    if (ci.labels.Count > 0)
                                        foreach (var label in ci.labels)
                                            labels += $"Label {label.GetHashCode()}";
                                    else
                                        labels += "no labels";
                                    outputFile.WriteLine($"    {ci.opcode} | {operand} | {labels}");
                                }
                            }

                            if (toil.tickAction != null && toil.tickAction.Method != null)
                            {
                                outputFile.WriteLine($"  TickAction:");
                                var dynMethod = new DynamicMethod(toil.tickAction.Method.Name, toil.tickAction.Method.ReturnType, null);
                                foreach (ILInstruction ci in MethodBodyReader.GetInstructions(dynMethod.GetILGenerator(), toil.tickAction.Method))
                                {
                                    var operand = ci.operand is Label ? ("Label " + ci.operand.GetHashCode()).ToString() : ci.operand;
                                    string labels = "";
                                    if (ci.labels.Count > 0)
                                        foreach (var label in ci.labels)
                                            labels += $"Label {label.GetHashCode()}";
                                    else
                                        labels += "no labels";
                                    outputFile.WriteLine($"    {ci.opcode} | {operand} | {labels}");
                                }
                            }

                            if (toil.finishActions != null)
                            {
                                outputFile.WriteLine($"  FinishActions:");
                                foreach (var action in toil.finishActions)
                                {
                                    outputFile.WriteLine($"  Action:");
                                    var dynMethod = new DynamicMethod(action.Method.Name, action.Method.ReturnType, null);
                                    foreach (ILInstruction ci in MethodBodyReader.GetInstructions(dynMethod.GetILGenerator(), action.Method))
                                    {
                                        var operand = ci.operand is Label ? ("Label " + ci.operand.GetHashCode()).ToString() : ci.operand;
                                        string labels = "";
                                        if (ci.labels.Count > 0)
                                            foreach (var label in ci.labels)
                                                labels += $"Label {label.GetHashCode()}";
                                        else
                                            labels += "no labels";
                                        outputFile.WriteLine($"    {ci.opcode} | {operand} | {labels}");
                                    }
                                }
                            }

                            if (toil.preInitActions != null)
                            {
                                outputFile.WriteLine($"  PreInitActions:");
                                foreach (var action in toil.preInitActions)
                                {
                                    outputFile.WriteLine($"  Action:");
                                    var dynMethod = new DynamicMethod(action.Method.Name, action.Method.ReturnType, null);
                                    foreach (ILInstruction ci in MethodBodyReader.GetInstructions(dynMethod.GetILGenerator(), action.Method))
                                    {
                                        var operand = ci.operand is Label ? ("Label " + ci.operand.GetHashCode()).ToString() : ci.operand;
                                        string labels = "";
                                        if (ci.labels.Count > 0)
                                            foreach (var label in ci.labels)
                                                labels += $"Label {label.GetHashCode()}";
                                        else
                                            labels += "no labels";
                                        outputFile.WriteLine($"    {ci.opcode} | {operand} | {labels}");
                                    }
                                }
                            }

                            if (toil.preTickActions != null)
                            {
                                outputFile.WriteLine($"  PreTickActions:");
                                foreach (var action in toil.preTickActions)
                                {
                                    outputFile.WriteLine($"  Action:");
                                    var dynMethod = new DynamicMethod(action.Method.Name, action.Method.ReturnType, null);
                                    foreach (ILInstruction ci in MethodBodyReader.GetInstructions(dynMethod.GetILGenerator(), action.Method))
                                    {
                                        var operand = ci.operand is Label ? ("Label " + ci.operand.GetHashCode()).ToString() : ci.operand;
                                        string labels = "";
                                        if (ci.labels.Count > 0)
                                            foreach (var label in ci.labels)
                                                labels += $"Label {label.GetHashCode()}";
                                        else
                                            labels += "no labels";
                                        outputFile.WriteLine($"    {ci.opcode} | {operand} | {labels}");
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log.Error("enumerator is null");
                }
            }

            // Harvest
            if (CompatibilityPatches.SeedsPlease.CanOverrideHarvest())
            {
                var harvestJob = JobDefOf.Harvest;
                harvestJob.driverClass = typeof(JobDriver_PlantHarvest_Tweak);
            }

            // Grow
            var growWorkGiver = DefDatabase<WorkGiverDef>.GetNamed("GrowerSow");
            growWorkGiver.giverClass = typeof(WorkGiver_GrowerSow_Tweak);

            // Clean
            var cleanWorkGiver = DefDatabase<WorkGiverDef>.GetNamed("CleanFilth");
            cleanWorkGiver.giverClass = typeof(WorkGiver_CleanFilth_Tweak);
        }

        private static void SplitWardenType()
        {
            DefDatabase<WorkGiverDef>.GetNamed("DoExecution").workType = PL_DefOf.PrisonLabor_Jailor;
            DefDatabase<WorkGiverDef>.GetNamed("ReleasePrisoner").workType = PL_DefOf.PrisonLabor_Jailor;
            DefDatabase<WorkGiverDef>.GetNamed("TakePrisonerToBed").workType = PL_DefOf.PrisonLabor_Jailor;
            //DefDatabase<WorkGiverDef>.GetNamed("FeedPrisoner").workType = PrisonLaborDefOf.PrisonLabor_Jailor;
            //DefDatabase<WorkGiverDef>.GetNamed("DeliverFoodToPrisoner").workType = PrisonLaborDefOf.PrisonLabor_Jailor;
            WorkTypeDefOf.Warden.workGiversByPriority.Clear();
            WorkTypeDefOf.Warden.ResolveReferences();
            PL_DefOf.PrisonLabor_Jailor.workGiversByPriority.Clear();
            PL_DefOf.PrisonLabor_Jailor.ResolveReferences();
        }
    }
}
