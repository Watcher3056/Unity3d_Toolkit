using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    public class FieldInfoContainer
    {
        public FieldInfo fieldInfo;
        public object owner;

        public object Value 
        {
            get => fieldInfo.GetValue(owner);
            set => fieldInfo.SetValue(owner, value);
        }
    }
    public static partial class Extensions
    {
        public static List<FieldInfoContainer> GetFields(this object obj, Func<FieldInfoContainer, bool> filter)
        {
            List<FieldInfoContainer> result = GetFields(obj);
            for (int i = 0; i < result.Count; i++)
                if (filter(result[i]))
                {
                    result.RemoveAt(i);
                    i--;
                }
            return result;
        }
        public static List<FieldInfoContainer> GetFields(this object obj)
        {
            List<FieldInfoContainer> result = new List<FieldInfoContainer>();
            List<FieldInfo> fields = new List<FieldInfo>(obj.GetType().GetFields());

            foreach (FieldInfo field in fields)
                result.Add(new FieldInfoContainer
                {
                    owner = obj,
                    fieldInfo = field
                });
            return result;
        }
    }
}
