using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class ValuePipeline<T>
    {
        public class Override
        {
            public WeakReference<object> source;
            public T lastValue;
            public Func<T, T> overrider;
        }

        public T Value
        {
            get
            {
                T result = ValueOrigin;
                foreach (Override _override in overrides)
                {
                    ICloneable cloneable = result as ICloneable;
                    if (cloneable != null)
                        _override.lastValue = _override.overrider.Invoke((T)(cloneable.Clone()));
                    else
                        _override.lastValue = _override.overrider.Invoke(result);
                    result = _override.lastValue;
                }

                return result;
            }
        }
        public T ValueOrigin => getter.Invoke();
        public event Action OnOverridesChanged = () => { };
        private List<Override> overrides = new List<Override>();
        private Func<T> getter;

        public ValuePipeline(Func<T> getter)
        {
            this.getter = getter;

            Type type = typeof(T);
            if (type.IsClass)
            {
                List<Type> interfaces = new List<Type>(type.GetInterfaces());
                if (interfaces.Exists(i => i.IsEquivalentTo(typeof(ICloneable))) == false)
                    throw new Exception("Non cloneable classes not allowed for value pipeline!");
            }
        }

        public void SetOverrideOnce(object source, Func<T, T> overrider)
        {
            RemoveOverridesFromNoEvent(source);
            AddOverrideNoEvent(source, overrider);
            OnOverridesChanged.Invoke();
        }
        public void AddOverride(object source, Func<T, T> overrider)
        {
            AddOverrideNoEvent(source, overrider);
            OnOverridesChanged.Invoke();
        }
        private void AddOverrideNoEvent(object source, Func<T, T> overrider)
        {
            Override _override = new Override();
            _override.source = new WeakReference<object>(source);
            _override.overrider = overrider;
            overrides.Add(_override);
        }
        public void RemoveOverridesAll()
        {
            RemoveOverridesAllNoEvent();
            OnOverridesChanged.Invoke();
        }
        private void RemoveOverridesAllNoEvent()
        {
            overrides.Clear();
        }
        public void RemoveOverridesFrom(object source)
        {
            RemoveOverridesFromNoEvent(source);
            OnOverridesChanged.Invoke();
        }
        private void RemoveOverridesFromNoEvent(object source)
        {
            overrides.RemoveAll(o => o.source.GetTarget() == source);
        }
    }
}