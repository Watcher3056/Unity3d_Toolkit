using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class ProcessorObserver : MonoBehaviour
    {
        public class Observer
        {
            public WeakReference<object> target = new WeakReference<object>(null);
            public WeakReference<object> source = new WeakReference<object>(null);
            public Action<object> callback;
            public Func<bool> autoKillCondition;
            public Func<object> propertySelector;
            public object prevValue;
            public bool invokeOnceOnUpdate;
            public bool killed;

            public void Kill()
            {
                ProcessorObserver.Default.Kill(this);
            }
            public Observer SetAutoKill(Func<bool> autoKillCondition)
            {
                this.autoKillCondition = autoKillCondition;
                return this;
            }
            public Observer SetTarget(object target)
            {
                this.target = new WeakReference<object>(target);
                return this;
            }
            public Observer SetSource(object source)
            {
                this.source = new WeakReference<object>(source);
                return this;
            }
        }
        public static ProcessorObserver Default { private set; get; }

        public ProcessorObserver() => Default = this;

        private List<Observer> observers = new List<Observer>();
        private int curIndex = 0;
        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            curIndex = 0;
            while (curIndex < observers.Count)
            {
                Observer observer = observers[curIndex];

                if (observer.autoKillCondition != null && observer.autoKillCondition.Invoke())
                {
                    observer.Kill();
                    curIndex--;
                    continue;
                }
                object curValue = observer.propertySelector.Invoke();

                bool case1 = (curValue != null && !curValue.Equals(observer.prevValue));
                bool case2 = (curValue == null && observer.prevValue != curValue);
                if (case1 || case2 || observer.invokeOnceOnUpdate)
                {
                    observer.prevValue = curValue;
                    observer.callback.Invoke(curValue);
                    observer.invokeOnceOnUpdate = false;
                }
                curIndex++;
            }
        }
        public Observer Add<T>(Func<T> propertySelector, Action<T> callback, bool invokeOnceOnUpdate)
        {
            Observer observer = new Observer();
            observer.propertySelector = () => propertySelector();
            observer.prevValue = propertySelector.Invoke();
            observer.callback = arg => callback((T)arg);
            observer.invokeOnceOnUpdate = invokeOnceOnUpdate;
            observers.Add(observer);
            return observer;
        }
        public void Kill(Observer observer)
        {
            if (observer.killed)
                return;
            int indexOf = observers.IndexOf(observer);
            if (indexOf <= curIndex)
                curIndex--;
            observer.callback = null;
            observer.prevValue = null;
            observer.propertySelector = null;
            observers.Remove(observer);
            observer.killed = true;
        }
        public List<Observer> GetAllByTarget(object target)
        {
            return observers
                .FindAll(o =>
                {
                    object _target = null;
                    o.target.TryGetTarget(out _target);
                    return _target == target;
                });
        }
        public List<Observer> GetAllBySource(object source)
        {
            return observers
                .FindAll(o =>
                {
                    object _source = null;
                    o.source.TryGetTarget(out _source);
                    return _source == source;
                });
        }
        public List<Observer> GetAllBy(object source, object target)
        {
            return observers
                .FindAll(o =>
                {
                    object _target = null;
                    object _source = null;
                    o.target.TryGetTarget(out _target);
                    o.source.TryGetTarget(out _source);
                    return _source == source && _target == target;
                });
        }
        public void KillAllByTarget(object target)
        {
            GetAllByTarget(target)
                .ForEach(o =>
                {
                    o.Kill();
                });
        }
        public void KillAllBySource(object source)
        {
            GetAllBySource(source)
                .ForEach(o =>
                {
                    o.Kill();
                });
        }
        public void KillAllBy(object source, object target)
        {
            GetAllBy(source, target)
                .ForEach(o =>
                {
                    o.Kill();
                });
        }
    }
    public static partial class Extensions
    {
        public static void KillObserversOnMe(this object target) => 
            ProcessorObserver.Default.KillAllByTarget(target);
        public static void KillObserversFromMe(this object source) => 
            ProcessorObserver.Default.KillAllBySource(source);
        public static void KillObserversFromMeOnTarget(this object source, object target) => 
            ProcessorObserver.Default.KillAllBy(source, target);
        public static void KillObserversOnTargetFrom(this object target, object source) =>
            ProcessorObserver.Default.KillAllBy(source, target);
    }
}