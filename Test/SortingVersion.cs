using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrisonLabor_Tests
{
    public static class SortingVersion
    {

        public static void Test()
        {
            var list = new List<string>();
            list.Add("1.12");
            list.Add("3.22");
            list.Add("3.22.13");
            list.Add("1.0.0");
            list.Add("0.9999");
            list.Add("9999");
            list.Add("123");
            list.Add("2");
            list.Add("3");
            list.Add("3.12.32.32.13");

            list.Sort(new Comparison<string>((x, y) => CompareVersion(x, y)));
            for (int i = 0; i < list.Count; i++)
            {
                System.Console.WriteLine(list[i]);
            }
        }

        private static int CompareVersion(string x, string y)
        {
            var xFragments = x.Split('.');
            var yFragments = y.Split('.');

            for (int i = 0; i < xFragments.Length || i < yFragments.Length; i++)
            {
                if (i == xFragments.Length)
                    return -1;
                if (i == yFragments.Length)
                    return 1;

                try
                {
                    int xValue = int.Parse(xFragments[i]);
                    int yValue = int.Parse(yFragments[i]);
                    if (xValue != yValue)
                        return xValue - yValue;
                }
                catch (FormatException e) { }
            }
            return 0;
        }
    }
}
