using Animancer;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    [Serializable]
    public class TimeZone
    {
        [SerializeField]
        private float timeScaleOrigin = 1f;
        private ValuePipeline<float> timeScaleFactor;

        public float Delta => UnityEngine.Time.deltaTime * TimeScale;
        public float DeltaFixed => UnityEngine.Time.fixedDeltaTime * TimeScale;
        public float DeltaUnscaled => UnityEngine.Time.unscaledDeltaTime;
        // Used in calculation of delta
        public float TimeScale
        {
            get
            {
                if (needToUpdateCache)
                {
                    TimeZone parent = TimeZoneParent;
                    if (parent != null)
                        timeScaleCached = parent.TimeScale * TimeScaleLocal;
                    else
                        timeScaleCached = TimeScaleLocal;

                    needToUpdateCache = false;
                }
                return timeScaleCached;
            }
        }
        private float timeScaleCached;
        private bool needToUpdateCache = true;
        public float TimeScaleLocal => TimeScaleOrigin * TimeScaleFactor.Value;
        public TimeZone TimeZoneParent
        {
            get
            {
                if (timeZoneParent != null)
                    return timeZoneParent;
                else if (this == ProcessorTime.TimeZoneMain)
                    return null;
                else
                    return ProcessorTime.TimeZoneMain;
            }
            set
            {
                if (timeZoneParent != null)
                    timeZoneParent.timeZonesChilds.Remove(this);
                timeZoneParent = value;
                if (timeZoneParent != null)
                    timeZoneParent.timeZonesChilds.Add(this);
                HandleTimeScaleChanged();
            }
        }
        [NonSerialized]
        private TimeZone timeZoneParent;
        [NonSerialized]
        private List<TimeZone> timeZonesChilds = new List<TimeZone>();
        [NonSerialized]
        private List<object> registeredObjects = new List<object>();

        // Normally applied from runtime
        public ValuePipeline<float> TimeScaleFactor
        {
            get => timeScaleFactor;
        }
        // Normally applied from editor
        public float TimeScaleOrigin
        {
            get => timeScaleOrigin;
            private set
            {
                timeScaleOrigin = value;
                HandleTimeScaleChanged();
            }
        }

        public TimeZone()
        {
            Init(1f);
        }
        public TimeZone(float timeScaleOrigin = 1f)
        {
            Init(timeScaleOrigin);
        }
        private void Init(float timeScaleOrigin)
        {
            this.timeScaleOrigin = timeScaleOrigin;
            this.timeScaleFactor = new ValuePipeline<float>(() => 1f);
            this.timeScaleFactor.OnOverridesChanged += HandleTimeScaleChanged;
        }
        private void HandleTimeScaleChanged()
        {
            needToUpdateCache = true;
            for (int i = 0; i < registeredObjects.Count; i++)
            {
                object _object = registeredObjects[i];

                TimeZoneAdapter adapter = ProcessorTime.Default.GetAdapter(_object);
                bool remove = false;

                if (!adapter.IsAlive(_object))
                {
                    remove = true;
                }

                if (remove)
                {
                    RemoveObject(_object);
                    i--;
                }
                else
                {
                    adapter.UpdateTimeScale(_object, this);
                }
            }
            foreach (TimeZone timeZone in timeZonesChilds)
                timeZone.HandleTimeScaleChanged();
        }
        public void RegisterObject(object _object)
        {
            // Ensure that there is only one time zone applied to object in same time
            TimeZoneAdapter adapter = ProcessorTime.Default.GetAdapter(_object);
            TimeZone timeZoneOther = adapter.GetTimeZoneByObject(_object);
            if (timeZoneOther != null)
            {
                if (timeZoneOther == this)
                {
                    //this.LogWarning("Object already registered in this time zone.");
                    return;
                }
                // Unregister object from other timezone before registering here, to avoid collisions
                timeZoneOther.RemoveObject(_object);
            }
            registeredObjects.Add(_object);
            adapter.RegisterObject(_object, this);
        }
        public void RemoveObject(object _object)
        {
            registeredObjects.Remove(_object);
            TimeZoneAdapter adapter = ProcessorTime.Default.GetAdapter(_object);

            adapter.RemoveObject(_object);
        }
    }
    public static partial class Extensions
    {
        public static T TrySetTimeZone<T>(this T input, TimeZone timeZone)
        {
            if (timeZone != null)
                input.SetTimeZone(timeZone);

            return input;
        }
        public static T SetTimeZone<T>(this T input, TimeZone timeZone)
        {
            timeZone.RegisterObject(input);
            return input;
        }
        public static T SetTimeZoneMain<T>(this T input)
        {
            return input.SetTimeZone(ProcessorTime.TimeZoneMain);
        }
    }
}
