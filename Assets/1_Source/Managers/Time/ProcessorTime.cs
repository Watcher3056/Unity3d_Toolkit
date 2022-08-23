using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class ProcessorTime : MonoBehaviour
    {
        public static ProcessorTime Default { get; private set; }

        public static TimeZone TimeZoneMain => Default.timeZoneMain;
        private TimeZone timeZoneMain = new TimeZone();

        private Dictionary<Type, TimeZoneAdapter> dicAdapters = new Dictionary<Type, TimeZoneAdapter>();
        public ProcessorTime()
        {
            Default = this;
        }
        public void Awake()
        {
            new TimeZoneAdapterTimeScaled().RegisterAdapter();
            new TimeZoneAdapterTweener().RegisterAdapter();
            new TimeZoneAdapterAnimator().RegisterAdapter();
            new TimeZoneAdapterAnimancer().RegisterAdapter();
            new TimeZoneAdapterAudioSource().RegisterAdapter();
        }
        public TimeZoneAdapter GetAdapter(object _object) => GetAdapter(_object.GetType());
        public TimeZoneAdapter GetAdapter(Type type)
        {
            // Fast search
            TimeZoneAdapter result = null;
            if (dicAdapters.TryGetValue(type, out result))
                return result;

            // Deep search
            foreach (Type keyType in dicAdapters.Keys)
            {
                if (keyType.IsAssignableFrom(type))
                {
                    // Link this type to adapter, so next search will be fast!
                    result = dicAdapters[keyType];
                    RegisterAdapter(type, result);

                    return result;
                }
            }

            return null;
        }
        public void RegisterAdapter(Type type, TimeZoneAdapter adapter) => dicAdapters.Add(type, adapter);
    }
}
