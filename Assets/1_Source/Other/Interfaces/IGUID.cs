using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public interface IGUID
    {
        public ComponentGUID GUID
        {
            get;
        }
    }
    public partial class Extensions
    {
        public static T Origin<T>(this IGUID guid) where T : Component
        {
            IGUID result = guid.Origin();

            return (result as Component).GetComponent<T>();
        }
        public static IGUID Origin(this IGUID guid) => DataGameMain.Default.DBAssetsIDs[guid.GUID.id];
        public static bool IsOrigin(this IGUID guid) => guid == guid.Origin();
        public static T InstantiateIfNeeded<T>(this T input, Transform parent = null) where T : Component, IGUID
        {
            T result = null;
            if (input.IsOrigin())
            {
                result = input.Instantiate(parent);
            }
            else
            {
                result = input;
                result.transform.SetParent(parent);
            }

            return result;
        }
    }
}
