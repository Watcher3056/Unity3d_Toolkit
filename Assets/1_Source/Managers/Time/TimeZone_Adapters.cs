using Animancer;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class TimeZoneAdapter
    {
        public virtual bool IsAlive(object _object) => throw new NotImplementedException();
        public virtual bool IsSuitable(object _object) => throw new NotImplementedException();
        // Do not use directly, unless you know what you do. Use TimeZone API instead
        public virtual void UpdateTimeScale(object _object, TimeZone timeZone) => throw new NotImplementedException();
        // Do not use directly, unless you know what you do. Use TimeZone API instead
        public virtual void RegisterObject(object _object, TimeZone timeZone) => throw new NotImplementedException();
        // Do not use directly, unless you know what you do. Use TimeZone API instead
        public virtual void RemoveObject(object _object) => throw new NotImplementedException();
        public virtual TimeZone GetTimeZoneByObject(object _object) => throw new NotImplementedException();
    }
    public class TimeZoneAdapter<T> : TimeZoneAdapter
    {
        private Dictionary<object, TimeZone> dicRegisteredObjects = new Dictionary<object, TimeZone>();

        public override bool IsAlive(object _object) => IsAlive((T)_object);
        public virtual bool IsAlive(T _object)
        {
            return (_object != null) && (_object.Equals(null) == false);
        }
        public void RegisterAdapter() => ProcessorTime.Default.RegisterAdapter(typeof(T), this);
        public override bool IsSuitable(object _object) => _object is T;
        public override void RegisterObject(object _object, TimeZone timeZone)
        {
            if (IsSuitable(_object))
            {
                dicRegisteredObjects.Add(_object, timeZone);
                UpdateTimeScale(_object, timeZone);
            }
            else
                this.LogError("Wrong input!");
        }
        public override void RemoveObject(object _object)
        {
            dicRegisteredObjects.Remove(_object);
        }
        public override void UpdateTimeScale(object _object, TimeZone timeZone) => UpdateTimeScale((T)_object, timeZone);
        public override TimeZone GetTimeZoneByObject(object _object)
        {
            if (dicRegisteredObjects.ContainsKey(_object))
                return dicRegisteredObjects[_object];
            else return null;
        }
        public virtual void UpdateTimeScale(T _object, TimeZone timeZone) => throw new NotImplementedException();
    }
    public class TimeZoneAdapterTimeScaled : TimeZoneAdapter<ITimeScaled>
    {
        public override void UpdateTimeScale(ITimeScaled _object, TimeZone timeZone)
        {
            _object.TimeScale = timeZone.TimeScale;
        }
        public override bool IsAlive(ITimeScaled _object)
        {
            return _object.IsAlive;
        }
    }
    public class TimeZoneAdapterTweener : TimeZoneAdapter<Tweener>
    {
        public override void UpdateTimeScale(Tweener _object, TimeZone timeZone)
        {
            _object.timeScale = timeZone.TimeScale;
        }
        public override bool IsAlive(Tweener _object) => _object.active;
    }
    public class TimeZoneAdapterAnimator : TimeZoneAdapter<Animator>
    {
        public override void UpdateTimeScale(Animator _object, TimeZone timeZone)
        {
            _object.speed = timeZone.TimeScale;
        }
    }
    public class TimeZoneAdapterAnimancer : TimeZoneAdapter<AnimancerComponent>
    {
        public override void UpdateTimeScale(AnimancerComponent _object, TimeZone timeZone)
        {
            _object.Playable.Speed = timeZone.TimeScale;
        }
    }
    public class TimeZoneAdapterAudioSource : TimeZoneAdapter<AudioSource>
    {
        public override void UpdateTimeScale(AudioSource _object, TimeZone timeZone)
        {
            _object.pitch = timeZone.TimeScale;
        }
    }
    public interface ITimeScaled
    {
        public float TimeScale { get; set; }
        public bool IsAlive { get; }
    }
}