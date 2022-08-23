using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    public static class Random
    {
        public static bool YesOrNo(float chance = 0.5f)
        {
            return UnityEngine.Random.Range(0f, chance) <= chance;
        }
    }
}
