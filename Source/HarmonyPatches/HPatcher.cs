using System;
using System.Reflection;
using HarmonyLib;
using Verse;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using System.Diagnostics;
using System.Linq;

namespace PrisonLabor.HarmonyPatches
{
  public static class HPatcher
  {

    static readonly MethodInfo getFactionMethod = AccessTools.PropertyGetter(typeof(Thing), nameof(Thing.Faction));
    // For logging purposes, it stores whenever each fragment was completed
    private static Dictionary<string, bool> fragments;

    public static void Init()
    {
      var harmony = new Harmony("Harmony_PrisonLabor");

      // SECTION - Classic patches
      try
      {
        // Clear old data, to avoid misleading info
        fragments = new Dictionary<string, bool>();

        harmony.PatchAll(Assembly.GetExecutingAssembly());

        // Print out not completed methods
        foreach (var f in fragments.Keys)
        {
          if (!fragments[f])
            Log.Error($"PrisonLaborWarning: Harmony patch failed to find \"{f}\" fragment.");
        }
      }
      catch (Exception e)
      {
        Log.Error($"PrisonLaborException: failed to proceed harmony patches: {e.InnerException.Message}");
        Log.Error(e.ToString());
      }
    }

    /// <summary>
    /// CIL Debugging method. Creates debug file on desktop that list all CIL code instructions in the method.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="withReturn"></param>
    public static void CreateDebugFileOnDesktop(string fileName, IEnumerable<CodeInstruction> instr)
    {
      // Set a variable to the Desktop path.
      string myDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

      // Write the string array to a new file.
      using (StreamWriter outputFile = new StreamWriter(myDesktopPath + @"\" + fileName + ".txt"))
      {
        outputFile.WriteLine("================");
        outputFile.WriteLine("Body of " + fileName + " method", fileName);
        outputFile.WriteLine("================");
        foreach (CodeInstruction instruction in instr)
        {
          var instructionString = instruction.opcode.ToString();
          instructionString += " | ";
          instructionString += instruction.operand is Label ? $"Label {instruction.operand.GetHashCode()}" : instruction.operand;
          instructionString += " | ";
          if (instruction.labels.Count > 0)
            foreach (var label in instruction.labels)
              instructionString += $"Label {label.GetHashCode()}";
          else
            instructionString += "no labels";
          outputFile.WriteLine(instructionString);
        }
      }
    }

    /// <summary>
    /// This method is used to add some CIL instructions after certain fragment in original code.
    /// It should be used inside foreach loop, and return true if particular iteration is the desired one.
    /// </summary>
    /// <param name="opCodes"></param>
    /// <param name="operands"></param>
    /// <param name="instr"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static bool IsFragment(OpCode[] opCodes, String[] operands, CodeInstruction instr, ref int step, string fragmentName, bool perfectMatch = true)
    {
      if (opCodes.Length != operands.Length)
      {
        Log.Error("PrisonLaborException: IsFragment() arguments does not match requirments. Trace:" + new StackTrace());
        return false;
      }

      if (!fragments.ContainsKey(fragmentName))
        fragments.Add(fragmentName, false);
      if (step < 0 || step >= opCodes.Length)
      {
        return false;
      }

      var finalStep = opCodes.Length;


      if (InstructionMatching(instr, opCodes[step], operands[step], perfectMatch))
        step++;
      else
        step = 0;

      if (step == finalStep)
      {
        step++;
        fragments[fragmentName] = true;
        return true;
      }
      return false;
    }

    /// <summary>
    /// This method is used to find particular label that is assigned to last instruction's operand 
    /// </summary>
    /// <param name="opCodes"></param>
    /// <param name="operands"></param>
    /// <param name="instr"></param>
    /// <param name="step"></param>
    /// <returns></returns>
    public static object FindOperandAfter(OpCode[] opCodes, String[] operands, IEnumerable<CodeInstruction> instr, bool perfectMatch = true)
    {
      if (opCodes.Length != operands.Length)
      {
        Log.Error("PrisonLaborException: FindOperandAfter() arguments does not match requirments. Trace:" + new StackTrace());
        return null;
      }

      var finalStep = opCodes.Length;

      int step = 0;
      foreach (var ci in instr)
      {
        if (InstructionMatching(ci, opCodes[step], operands[step], perfectMatch))
          step++;
        else
          step = 0;

        if (step == finalStep)
          return ci.operand;
      }

      Log.Error("PrisonLaborException: FindOperandAfter() didn't find any lines. Trace:" + new StackTrace());
      return null;
    }

    public static IEnumerable<CodeInstruction> ReplaceFragment(OpCode[] opCodes, String[] operands, IEnumerable<CodeInstruction> instr, IEnumerable<CodeInstruction> newFragment, string fragmentName, bool perfectMatch = true)
    {
      // Convert to list, to freely jump between lines
      var instructions = instr.ToList();

      // Find last index of fragment
      int index = -1;
      int step = 0;
      for (int i = 0; i < instructions.Count; i++)
      {
        if (HPatcher.IsFragment(opCodes, operands, instructions[i], ref step, fragmentName, perfectMatch))
        {
          index = i;
          break;
        }
      }

      // Jump back to begining of fragment
      index -= operands.Length - 1;

      // If no fragment is found throw exception (or somehow begining of fragment is lower than 0)
      if (index < 0)
      {
        throw new Exception($"Couldn't find fragment {fragmentName}");
      }

      // Remove fragment
      instructions.RemoveRange(index, operands.Length);

      // Add fragment
      instructions.InsertRange(index, newFragment);

      return instructions;
    }

    public static IEnumerable<CodeInstruction> InjectFragmentBefore(OpCode[] opCodes, String[] operands, IEnumerable<CodeInstruction> instr, IEnumerable<CodeInstruction> newFragment, string fragmentName, bool perfectMatch = true)
    {
      // Convert to list, to freely jump between lines
      var instructions = instr.ToList();

      // Find last index of fragment
      int index = -1;
      int step = 0;
      for (int i = 0; i < instructions.Count; i++)
      {
        if (HPatcher.IsFragment(opCodes, operands, instructions[i], ref step, fragmentName, perfectMatch))
        {
          index = i;
          break;
        }
      }

      // Jump back to begining of fragment
      index -= operands.Length;

      // If no fragment is found throw exception (or somehow begining of fragment is lower than 0)
      if (index < 0)
      {
        throw new Exception($"Couldn't find fragment {fragmentName}");
      }

      // Add fragment
      instructions.InsertRange(index + 1, newFragment);

      return instructions;
    }

    private static bool InstructionMatching(CodeInstruction instr, OpCode opCode, string operand, bool perfectMatch)
    {
      bool matchingOpCodes = instr.opcode == opCode;
      bool noOperands = instr.operand == null || string.IsNullOrEmpty(operand);
      bool matchingOperands;
      if (perfectMatch) matchingOperands = instr.operand != null && instr.operand.ToString() == operand;
      else matchingOperands = instr.operand != null && instr.operand.ToString().Contains(operand);

      return matchingOpCodes && (noOperands || matchingOperands);
    }

    public static bool IsGetFactionOperand(CodeInstruction inst)
    {
      return inst.opcode == OpCodes.Callvirt && inst.OperandIs(getFactionMethod);
    }
  }
}
