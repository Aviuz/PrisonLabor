using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse.AI;
using System.Reflection.Emit;
using System.Reflection;

namespace PrisonLabor_Tests
{
    static class ToilLister
    {
        public static IEnumerable<Toil> PickToils(JobDriver toilMaker)
        {
            var method = toilMaker.GetType().GetMethod("MakeNewToils", BindingFlags.NonPublic | BindingFlags.Instance);
            return (IEnumerable<Toil>)method.Invoke(toilMaker, new object[] { });
        }

        public static void PrintToils(JobDriver toilMaker)
        {
            var toils = PickToils(toilMaker);
            if (toils != null)
                foreach (var toil in toils)
                {
                    Console.WriteLine($"{toil.GetType()}");
                    foreach (var ci in MethodBodyReader.GetInstructions(toil.initAction.Method))
                    {
                        var operand = ci.operand is Label ? ("Label " + ci.operand.GetHashCode()).ToString() : ci.operand;
                        string labels = "";
                        if (ci.labels.Count > 0)
                            foreach (var label in ci.labels)
                                labels += $"Label {label.GetHashCode()}";
                        else
                            labels += "no labels";
                        Console.WriteLine($"    {ci.opcode} | {operand} | {labels}");
                    }
                }
            else
                Console.WriteLine("Null");
        }

    }
}
