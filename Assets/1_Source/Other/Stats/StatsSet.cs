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
    // Runtime
    [Serializable]
    public partial class StatsSet : ICloneable
    {
        [HideLabel, ListDrawerSettings(Expanded = true, DraggableItems = false), HideInInspector]
        public List<Stat> list = new List<Stat>();
        [SerializeField, HideInInspector]
        private StatType _statType;


        public StatsSet()
        {
            list = new List<Stat>();
        }
        public StatsSet(StatType statType)
        {
            list = new List<Stat>();
            _statType = statType;
        }
        public void SetType(StatType statType, bool fill)
        {
            if (statType == _statType)
                return;
            _statType = statType;
            if (fill)
                list = GetStatList(statType);
        }
        public void UpdateStatsList()
        {
            // Remove deprecated or disabled stats
            for (int i = 0; i < list.Count; i++)
            {
                Stat stat = list[i];
                if (stat.LinkedStat == null ||
                    (stat.LinkedStat != null && !DataGameMain.Default.statsList.Contains(stat.LinkedStat)))
                {
                    list.RemoveAt(i);
                    i--;
                }
            }

            // Add missing stats
            //List<Stat> _listActual = GetStatList(_statType);
            //foreach (Stat stat in _listActual)
            //{
            //    if (!list.Exists(s => s.LinkedStat.Equals(stat.LinkedStat)))
            //        list.Add(stat);
            //}

            // Sort same as in settings
            list.Sort((s1, s2) => s1.LinkedStat.SortOrder.CompareTo(s2.LinkedStat.SortOrder));
        }
        public static StatsSet Create(StatType type, bool fill)
        {
            StatsSet result = new StatsSet();
            result.SetType(type, fill);

            return result;
        }
        public static List<Stat> GetStatList(StatType statType)
        {
            List<DataStat> dataStats = new List<DataStat>();
            if (statType == StatType.All)
                dataStats.AddRange(DataGameMain.Default.statsList.ToArray());
            else
                dataStats = DataGameMain.Default.statsList.FindAll(s => s.StatType == statType);
            List<Stat> result = new List<Stat>();

            foreach (DataStat dataStat in dataStats)
                result.Add(new Stat(dataStat));

            return result;
        }

        public object Clone()
        {
            StatsSet result = (StatsSet)this.MemberwiseClone();
            result.list = new List<Stat>();
            foreach (Stat stat in this.list)
                result.list.Add((Stat)stat.Clone());
#if UNITY_EDITOR
            result._editorList = new List<Stat>(_editorList.ToArray());
#endif
            return result;
        }

        public static StatsSet operator +(StatsSet arg1, StatsSet arg2)
        {
            StatsSet result = new StatsSet();
            result.list = arg1.list.CloneAndMerge(arg2.list, StatsExtensions.Operation.Plus);

            return result;
        }
        public static StatsSet operator -(StatsSet arg1, StatsSet arg2)
        {
            StatsSet result = new StatsSet();
            result.list = arg1.list.CloneAndMerge(arg2.list, StatsExtensions.Operation.Minus);

            return result;
        }
        public static StatsSet operator *(StatsSet arg1, StatsSet arg2)
        {
            StatsSet result = new StatsSet();
            result.list = arg1.list.CloneAndMerge(arg2.list, StatsExtensions.Operation.Multiply);

            return result;
        }
        public static StatsSet operator /(StatsSet arg1, StatsSet arg2)
        {
            StatsSet result = new StatsSet();
            result.list = arg1.list.CloneAndMerge(arg2.list, StatsExtensions.Operation.Divide);

            return result;
        }
        public static StatsSet operator *(StatsSet arg1, float arg2)
        {
            StatsSet result = arg1.Clone() as StatsSet;

            for (int i = 0; i < result.list.Count; i++)
                result.list[i].value *= arg2;

            return result;
        }
        public static StatsSet operator /(StatsSet arg1, float arg2)
        {
            StatsSet result = arg1.Clone() as StatsSet;

            for (int i = 0; i < result.list.Count; i++)
                result.list[i].value /= arg2;

            return result;
        }
    }
    public partial class Extensions
    {
        public static void TryFind(this StatsSet stats, DataStat dataStat, Action<Stat> onApply)
        {
            Stat stat = stats.list.Find(s => s.LinkedStat == dataStat);

            if (stat != null)
                onApply.Invoke(stat);
        }
    }
#if UNITY_EDITOR
    // Editor
    public partial class StatsSet
    {
        [ListDrawerSettings(Expanded = true, AlwaysAddDefaultValue = true, 
            CustomAddFunction = "_EditorListCustomAddFunction", DraggableItems = false),
            ShowInInspector, HideLabel,
            OnValueChanged("_EditorOnValueChanged_EditorList", includeChildren: true), HideReferenceObjectPicker]
        [OnInspectorGUI(Append = "_EditorOnInspectorGUI")]
        private List<Stat> _editorList = new List<Stat>();

        private void _EditorListCustomAddFunction()
        {
            _editorList.Add(new Stat());
            _EditorOnValueChanged_EditorList();
        }
        [OnInspectorInit]
        private void _EditorOnInspectorInit()
        {
            UpdateStatsList();
            _editorList = new List<Stat>(list.ToArray());
            _EditorOnValueChanged_EditorList();
        }
        private void _EditorOnInspectorGUI()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                List<Stat> statsMissingInEditorList = list.FindAll(s => !_editorList.Exists(_s => _s.LinkedStat.Equals(s.LinkedStat)));
                _editorList.AddRange(statsMissingInEditorList.ToArray());
                _EditorOnValueChanged_EditorList();
            };
        }
        private void _EditorOnValueChanged_EditorList()
        {
            List<Stat> updatedList = _editorList.ToList();
            updatedList.RemoveAll(s => s.LinkedStat == null);
            list = updatedList;

            //_EditorUpdateListDrawerSettings();
        }
        private IEnumerable _EditorDropDownLinkedStat
        {
            get
            {
                ValueDropdownList<DataStat> stats = new ValueDropdownList<DataStat>();
                foreach (DataStat stat in DataGameMain.Default.statsList)
                {
                    if (list.Exists(s => s.LinkedStat.Equals(stat)))
                        continue;
                    if (DataStat.TypeFilter(_statType, stat))
                        stats.Add(new ValueDropdownItem<DataStat>
                        {
                            Text = stat.Name,
                            Value = stat
                        });
                }
                return stats;
            }
        }
        //private void _EditorUpdateListDrawerSettings()
        //{
        //    InspectorProperty property = 
        //    var listDrawerSettings = property.GetAttribute<ListDrawerSettingsAttribute>();
        //    listDrawerSettings.Expanded = true;
        //    if (!_EditorDropDownLinkedStat.GetEnumerator().MoveNext())
        //        listDrawerSettings.HideAddButton = true;
        //    else
        //        listDrawerSettings.HideAddButton = false;
        //    property.RefreshSetup();
        //}
    }
#endif
}
