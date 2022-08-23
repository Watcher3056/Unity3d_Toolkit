using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public enum StatType { None, Main, Resource, Any = 254, All = 255 }
    [RequireComponent(typeof(GUID))]
    public class DataStat : ScriptableObject, IGUID
    {
        [SerializeField, Required]
        private ComponentGUID _guid;
        [SerializeField]
        private StatType _statType;
        [SerializeField, Required]
        private string _name;
        [SerializeField, ShowIf("_statType", StatType.Main)]
        private float _minValue;
        [SerializeField]
        private DataStat _statMaxLimiter;


        public string Name => _name;
        public StatType StatType => _statType;
        public ComponentGUID GUID => _guid;
        public DataStat StatMaxLimiter => _statMaxLimiter;
        public float MinValue => _minValue;
        public int SortOrder => DataGameMain.Default.statsList.IndexOf(this);
        public static bool TypeFilter(StatType filter, DataStat input)
        {
            return TypeFilter(filter, input.StatType);
        }
        public static bool TypeFilter(StatType filter, StatType input)
        {
            return filter == StatType.Any || input == filter;
        }
    }
}
