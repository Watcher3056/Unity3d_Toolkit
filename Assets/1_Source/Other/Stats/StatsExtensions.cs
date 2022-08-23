using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    public static class StatsExtensions
    {
        public static bool ContainsStat(this StatsSet stats, DataStat stat)
        {
            return stats.list.ContainsStat(stat);
        }
        public static bool ContainsStat(this List<Stat> stats, DataStat stat)
        {
            return stats.Exists(s => s.LinkedStat.Equals(stat));
        }
        public static Stat Get(this StatsSet stats, DataStat statToGet)
        {
            return stats.list.Get(statToGet);
        }
        public static Stat Get(this List<Stat> stats, DataStat statToGet)
        {
            return stats.Find(s => s.LinkedStat.Equals(statToGet));
        }
        public enum Operation { Plus, Minus, Multiply, Divide }
        public static List<Stat> CloneAndMerge(this List<Stat> arg1, List<Stat> arg2, Operation operation)
        {
            List<Stat> result = new List<Stat>(arg1.Count);
            foreach (Stat stat in arg1)
                result.Add((Stat)stat.Clone());

            for (int i = 0; i < arg2.Count; i++)
            {
                Stat stat = arg2[i];
                int index = result.FindIndex(s => s.LinkedStat.Equals(stat.LinkedStat));
                if (index == -1)
                    result.Add((Stat)stat.Clone());
                else
                {
                    if (operation == Operation.Plus)
                        result[index] += stat;
                    else if (operation == Operation.Minus)
                        result[index] -= stat;
                    else if (operation == Operation.Multiply)
                        result[index] *= stat;
                    else if (operation == Operation.Divide)
                        result[index] /= stat;
                }
            }

            return result;
        }
        public static StatsSet ModifyIncludeOrigin(this StatsSet origin, StatsModifier modifier)
        {
            return origin * modifier.multiply + modifier.increment;
        }
        public static StatsSet ModifyExcludeOrigin(this StatsSet origin, StatsModifier modifier)
        {
            return (origin * modifier.multiply - origin) + modifier.increment;
        }
    }
}
