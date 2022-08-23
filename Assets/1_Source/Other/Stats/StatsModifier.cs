using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    [Serializable]
    public class StatsModifier
    {
        public StatsSet increment = new StatsSet(StatType.Main);
        public StatsSet multiply = new StatsSet(StatType.Main);
    }
    [Serializable]
    public class StatsModifierResources
    {
        public enum Mode { None, Modify, Override, Both }

        public Mode mode;
        [ShowIf("IsOverrideEnabled")]
        public StatsSet absoluteOverride = new StatsSet(StatType.Resource);
        [ShowIf("IsOverrideEnabled")]
        public StatsSet normalizedOverride = new StatsSet(StatType.Resource);
        [ShowIf("IsModifyEnabled")]
        public StatsSet increment = new StatsSet(StatType.Resource);
        [ShowIf("IsModifyEnabled")]
        [ValidateInput("_EditorValidateInputIncrementNormalized", "Only limited stats are allowed!")]
        public StatsSet incrementNormalized = new StatsSet(StatType.Resource);
        [ShowIf("IsModifyEnabled")]
        public StatsSet multiply = new StatsSet(StatType.Resource);

        public void Apply(StatsSet resources, StatsSet main)
        {
            if (IsOverrideEnabled)
            {
                foreach (Stat stat in absoluteOverride.list)
                    resources.TryFind(stat.LinkedStat, s => s.value = stat.value);
                foreach (Stat stat in normalizedOverride.list)
                    resources.TryFind(stat.LinkedStat, s =>
                    {
                        main.TryFind(stat.LinkedStat.StatMaxLimiter, sm => s.value = stat.value * sm.value);
                    });
            }
            if (IsModifyEnabled)
            {
                foreach (Stat stat in multiply.list)
                    resources.TryFind(stat.LinkedStat, s => s.value *= stat.value);
                foreach (Stat stat in increment.list)
                    resources.TryFind(stat.LinkedStat, s => s.value += stat.value);
                foreach (Stat stat in incrementNormalized.list)
                {
                    Stat statResource = resources.Get(stat.LinkedStat);

                    if (statResource == null)
                        continue;
                    Stat statMaxLimiter = main.Get(statResource.LinkedStat.StatMaxLimiter);

                    if (statMaxLimiter == null)
                    {
                        this.LogError("Cannot apply normalized value: Max Limiter not found!");
                        return;
                    }

                    statResource.value += statMaxLimiter.value * stat.value;
                }
            }
        }
        private bool IsOverrideEnabled => mode == Mode.Override || mode == Mode.Both;
        private bool IsModifyEnabled => mode == Mode.Modify || mode == Mode.Both;
#if UNITY_EDITOR
        private bool _EditorValidateInputIncrementNormalized(StatsSet value)
        {
            if (value.list.Exists(s => s.LinkedStat.StatMaxLimiter == null))
                return false;
            return true;
        }
#endif
    }
}
