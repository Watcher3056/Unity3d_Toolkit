using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    [Serializable]
    public class Stat : ICloneable
    {
        [HorizontalGroup(width: 0.7f, LabelWidth = 0.1f), ShowInInspector, ReadOnly, PropertyOrder(0), ShowIf("@linkedStat != null")]
        public string Name => LinkedStat?.Name;
        [HorizontalGroup, PropertyOrder(1)]
        public float value;
        public int ValueInt => (int)value;
        public DataStat LinkedStat => linkedStat;
        [SerializeField, HideIf("@$value != null"), PropertyOrder(0), Required]
        [ValueDropdown("@((StatsSet)$property.Parent.Parent.Parent.ValueEntry.WeakSmartValue)._EditorDropDownLinkedStat")]
        private DataStat linkedStat;


        public Stat()
        {

        }
        public Stat(DataStat linkedStat)
        {
            value = 0;
            this.linkedStat = linkedStat;
        }
        public Stat(DataStat linkedStat, float value)
        {
            this.value = value;
            this.linkedStat = linkedStat;
        }
        public static Stat operator +(Stat arg1, Stat arg2)
        {
            Stat result = new Stat(arg1.LinkedStat);
            result.value = arg1.value + arg2.value;

            return result;
        }
        public static Stat operator -(Stat arg1, Stat arg2)
        {
            Stat result = new Stat(arg1.LinkedStat);
            result.value = arg1.value - arg2.value;

            return result;
        }
        public static Stat operator *(Stat arg1, Stat arg2)
        {
            Stat result = new Stat(arg1.LinkedStat);
            result.value = arg1.value * arg2.value;

            return result;
        }
        public static Stat operator /(Stat arg1, Stat arg2)
        {
            Stat result = new Stat(arg1.LinkedStat);
            result.value = arg1.value / arg2.value;

            return result;
        }
        public static Stat operator /(Stat arg1, float arg2)
        {
            Stat result = new Stat(arg1.LinkedStat);
            result.value = arg1.value / arg2;

            return result;
        }
        public static Stat operator *(Stat arg1, float arg2)
        {
            Stat result = new Stat(arg1.LinkedStat);
            result.value = arg1.value * arg2;

            return result;
        }
        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
