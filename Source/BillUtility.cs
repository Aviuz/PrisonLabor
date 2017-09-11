using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace PrisonLabor
{

    class BillUtility
    {
        private static Dictionary<Bill, BillGroupData> map = new Dictionary<Bill, BillGroupData>();

        public static GroupMode IsFor(Bill key)
        {
            if (!map.ContainsKey(key))
                map[key] = new BillGroupData();
            return map[key].mode;
        }

        public static void SetFor(Bill key, GroupMode value)
        {
            map[key].mode = value;

        }

        public static BillGroupData GetData(Bill key)
        {
            if (!map.ContainsKey(key))
                map[key] = new BillGroupData();
            return map[key];
        }

        public static void Remove(Bill bill)
        {
            map.Remove(bill);
        }
    }
}
