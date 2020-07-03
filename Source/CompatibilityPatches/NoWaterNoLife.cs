using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.CompatibilityPatches
{
    // TODO: PrisonLabor.CompatibilityPatches Remove
    //internal static class NoWaterNoLife
    //{
    //    private const string DirectoryName = "PrisonLaborPatch";
    //    private const string FileName = "Patch.xml";

    //    internal static void Init()
    //    {
    //        if (Check())
    //            Work();
    //    }

    //    internal static bool Check()
    //    {
    //        if (DefDatabase<ThinkTreeDef>.GetNamed("Mizu_LeaveIfDehydration", false) != null)
    //            return true;
    //        else
    //            return false;
    //    }

    //    internal static void Work()
    //    {
    //        var mod = LoadedModManager.RunningMods.First(m => m.Name == "No Water, No Life.");
    //        CreateCompatibilityXml(mod);

    //        var list = DirectXmlLoader.XmlAssetsInModFolder(mod, DirectoryName + @"\").ToList<LoadableXmlAsset>();
    //        foreach (var item in list)
    //        {
    //            if (item == null || item.xmlDoc == null || item.xmlDoc.DocumentElement == null)
    //            {
    //                Log.Error(string.Format("{0}: unknown parse failure", item.fullFolderPath + @"\" + item.name));
    //            }
    //            else if (item.xmlDoc.DocumentElement.Name != "Defs")
    //            {
    //                Log.Error(string.Format("{0}: root element named {1}; should be named Defs", item.fullFolderPath + @"\" + item.name, item.xmlDoc.DocumentElement.Name));
    //            }
    //            XmlInheritance.TryRegisterAllFrom(item, mod);
    //        }
    //        XmlInheritance.Resolve();
    //        
    //        foreach (var item in list)
    //        {
    //            
    //            //foreach (Def def in item.defPackage)
    //            //{
    //            //    Log.Message($"Added def {def.defName}");
    //            //    DefDatabase<ThinkTreeDef>.Add(def as ThinkTreeDef);
    //            //}
    //        }
    //        foreach(var def in DefDatabase<ThinkTreeDef>.AllDefs.Where(d => d.insertTag == "Humanlike_PostDuty").OrderByDescending(d => d.insertPriority))
    //            Log.Message($"  ThinkTree {def.defName}");

    //    }

    //    internal static void CreateCompatibilityXml(ModContentPack mod)
    //    {
    //        var dir = mod.RootDir;

    //        if (!Directory.Exists(dir + @"\" + DirectoryName))
    //        {
    //            // Create directory for patch xml
    //            Directory.CreateDirectory(dir + @"\" + DirectoryName);

    //            // Write the string array to a new file.
    //            using (StreamWriter outputFile = new StreamWriter(dir + @"\" + DirectoryName + @"\" + FileName))
    //            {
    //                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
    //                outputFile.WriteLine("<Defs>");
    //                outputFile.WriteLine("<ThinkTreeDef>");
    //                outputFile.WriteLine("<defName>PrisonLabor_NoWaterNoLife_CompatibilityThinkTree</defName>");
    //                outputFile.WriteLine("<insertTag>Humanlike_PostDuty</insertTag>");
    //                outputFile.WriteLine("<insertPriority>110</insertPriority>");
    //                outputFile.WriteLine("<thinkRoot Class=\"ThinkNode_ConditionalPrisoner\">");
    //                outputFile.WriteLine("<subNodes>");
    //                outputFile.WriteLine("<li Class=\"ThinkNode_PrioritySorter\">");
    //                outputFile.WriteLine("<subNodes>");
    //                outputFile.WriteLine("<li Class=\"MizuMod.JobGiver_GetWater\"/>");
    //                outputFile.WriteLine("</subNodes>");
    //                outputFile.WriteLine("</li>");
    //                outputFile.WriteLine("</subNodes>");
    //                outputFile.WriteLine("</thinkRoot>");
    //                outputFile.WriteLine("</ThinkTreeDef>");
    //                outputFile.Write("</Defs>");
    //            }
    //        }
    //    }

    //}
}
