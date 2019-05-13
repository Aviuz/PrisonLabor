using Harmony;
using Harmony.ILCopying;
using RimWorld;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_PermissionFix.Jobs
{
    [HarmonyPatch(typeof(JobDriver_PlantWork))]
    [HarmonyPatch("MakeNewToils")]
    static class CutTree
    {
        static IEnumerable<CodeInstruction> Transpiler(ILGenerator gen, MethodBase mBase, IEnumerable<CodeInstruction> instructions)
        {
            foreach (var instr in instructions)
            {
                yield return instr;
            }
        }

        static void Postfix(JobDriver_PlantWork __instance, ref IEnumerable<Toil> __result)
        {
            foreach (var toil in __result)
            {
                if (toil.tickAction != null)
                    toil.tickAction = RemoveForbidFromAction(toil.tickAction);
            }
        }

        static Action RemoveForbidFromAction(Action action)
        {
            var method = new DynamicMethod(action.Method.Name, action.Method.ReturnType, action.Method.GetParameters().Select(p => p.ParameterType).ToArray());
            var gen = method.GetILGenerator();

            //var body = action.Method.GetMethodBody().GetILAsByteArray();


            Debug(MethodBodyReader.GetInstructions(gen, action.Method), "debugAfterUpgraded");

            return new Action(() => { });
        }

        [MethodImpl(MethodImplOptions.ForwardRef)]
        static extern void NewTickAction();

        static void EmitInstruction(ILGenerator ilGen, ILInstruction ilInstruction)
        {
            //    try
            //    {
            //        foreach (var label in ilInstruction.labels)
            //            ilGen.MarkLabel(label);
            //        switch (ilInstruction.opcode.OperandType)
            //        {
            //            case OperandType.InlineNone:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode);
            //                    break;
            //                }

            //            case OperandType.InlineSwitch:
            //                {
            //                    throw new NotSupportedException();
            //                    break;
            //                }

            //            case OperandType.ShortInlineBrTarget:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (sbyte)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineBrTarget:
            //                {
            //                    Log.Message(ilInstruction.operand.GetType().FullName);
            //                    ilGen.Emit(ilInstruction.opcode, (Int32)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.ShortInlineI:
            //                {
            //                    if (ilInstruction.opcode == OpCodes.Ldc_I4_S)
            //                    {
            //                        ilGen.Emit(ilInstruction.opcode, (sbyte)ilInstruction.operand);
            //                    }
            //                    else
            //                    {
            //                        ilGen.Emit(ilInstruction.opcode, (byte)ilInstruction.operand);
            //                    }
            //                    break;
            //                }

            //            case OperandType.InlineI:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (int)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.ShortInlineR:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (float)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineR:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (double)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineI8:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (long)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineSig:
            //                {
            //                    throw new NotSupportedException();
            //                    break;
            //                }

            //            case OperandType.InlineString:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (string)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineTok:
            //                {
            //                    throw new NotSupportedException();
            //                    //ilGen.Emit(ilInstruction.opcode, (MemberInfo)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineType:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (Type)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineMethod:
            //                {
            //                    if (ilInstruction.operand is ConstructorInfo)
            //                        ilGen.Emit(ilInstruction.opcode, (ConstructorInfo)ilInstruction.operand);
            //                    else
            //                        ilGen.Emit(ilInstruction.opcode, (MethodInfo)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.InlineField:
            //                {
            //                    ilGen.Emit(ilInstruction.opcode, (FieldInfo)ilInstruction.operand);
            //                    break;
            //                }

            //            case OperandType.ShortInlineVar:
            //                {
            //                    throw new NotSupportedException();
            //                    //var idx = ilBytes.ReadByte();
            //                    //if (TargetsLocalVariable(instruction.opcode))
            //                    //{
            //                    //    var lvi = GetLocalVariable(idx);
            //                    //    if (lvi == null)
            //                    //        ilInstruction.argument = idx;
            //                    //    else
            //                    //    {
            //                    //        ilInstruction.operand = lvi;
            //                    //        ilInstruction.argument = variables[lvi.LocalIndex];
            //                    //    }
            //                    //}
            //                    //else
            //                    //{
            //                    //    ilInstruction.operand = GetParameter(idx);
            //                    //    ilInstruction.argument = idx;
            //                    //}
            //                    break;
            //                }

            //            case OperandType.InlineVar:
            //                {
            //                    throw new NotSupportedException();
            //                    //var idx = ilBytes.ReadInt16();
            //                    //if (TargetsLocalVariable(instruction.opcode))
            //                    //{
            //                    //    var lvi = GetLocalVariable(idx);
            //                    //    if (lvi == null)
            //                    //        instruction.argument = idx;
            //                    //    else
            //                    //    {
            //                    //        instruction.operand = lvi;
            //                    //        instruction.argument = variables[lvi.LocalIndex];
            //                    //    }
            //                    //}
            //                    //else
            //                    //{
            //                    //    instruction.operand = GetParameter(idx);
            //                    //    instruction.argument = idx;
            //                    //}
            //                    break;
            //                }

            //            default:
            //                throw new NotSupportedException();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        if (ilInstruction == null)
            //        {
            //            throw new Exception("Error during emiting instruction: ilInstruction is null ", ex);
            //        }
            //        else
            //        {
            //            throw new Exception("Error during emiting instruction: case " + ilInstruction.opcode.OperandType.ToString(), ex);
            //        }
            //    }
        }

        static void Debug(IEnumerable<ILInstruction> instr, string fileName = "debug")
        {
            // Set a variable to the Desktop path.
            string myDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Write the string array to a new file.
            using (StreamWriter outputFile = new StreamWriter(myDesktopPath + @"\" + fileName + ".txt"))
            {
                outputFile.WriteLine($".method public void {fileName}(void) cil managed");
                outputFile.WriteLine("{");
                foreach (ILInstruction instruction in instr)
                {
                    //foreach (var label in instruction.labels)
                    //    outputFile.WriteLine($"\tLABEL_{label.GetHashCode()}:\n\n\n\n\n\n");

                    //var instructionString = instruction.opcode.ToString();
                    //var operandString = instruction.operand is ILInstruction ?
                    //                $"LABEL_{((ILInstruction)instruction.operand).GetHashCode()}" :
                    //                instruction.operand;
                    //outputFile.WriteLine($"\t{instructionString} {operandString}");
                    //outputFile.WriteLine($"\t\t{instruction.operand?.GetType()}");
                    outputFile.WriteLine(instruction.ToString());
                }
                outputFile.WriteLine("}");
            }
        }
    }
}
