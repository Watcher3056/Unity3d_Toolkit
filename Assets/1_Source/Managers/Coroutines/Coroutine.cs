using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class Coroutine : ITimeScaled
    {
        public enum UpdateType { Update, FixedUpdate, LateUpdate }
        public float TimeScale
        {
            get => timeScale;
            set => timeScale = value;
        }
        private float timeScale = 1f;
        public bool IsPlaying
        {
            get => isPlaying && IsAlive;
            private set => isPlaying = value;
        }
        private bool isPlaying;
        public UpdateType UpdateMode { get; set; }
        public bool IsAlive { get; private set; }
        public WaitFor WaitFor { get; set; }
        private IEnumerator enumerator;
        public Coroutine(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
            IsAlive = true;
        }
        public void Play()
        {
            if (!IsAlive)
            {
                this.LogError("Coroutine already killed and canot be played!");
                return;
            }
            IsPlaying = true;
        }
        public void Pause()
        {
            if (!IsAlive)
            {
                this.LogError("Coroutine already killed!");
                return;
            }
            IsPlaying = false;
        }
        public void Kill()
        {
            Pause();
            IsAlive = false;
            enumerator = null;
        }
        public void Tick()
        {
            if (WaitFor != null && WaitFor.Wait)
            {
                if (WaitFor.Time > 0f)
                {
                    WaitFor.Time -= UnityEngine.Time.deltaTime * TimeScale;
                    WaitFor.Time = Mathf.Clamp(WaitFor.Time, 0f, WaitFor.Time);
                }
                else if (WaitFor.TimeUnscaled > 0f)
                {
                    WaitFor.TimeUnscaled -= UnityEngine.Time.unscaledDeltaTime;
                    WaitFor.TimeUnscaled = Mathf.Clamp(WaitFor.TimeUnscaled, 0f, WaitFor.TimeUnscaled);
                }
            }
            else if (enumerator.MoveNext())
            {
                object _obj = enumerator.Current;
                //Skip this frame
                if (_obj == null)
                    return;
                else if (_obj is WaitFor)
                {
                    WaitFor = _obj as WaitFor;
                }
#if UNITY_EDITOR
                else if (_obj is YieldInstruction)
                    this.LogError("Detected Unity built-in yield instruction. Ignored...");
#endif
            }
            else
            {
                Kill();
            }
        }
        public static Coroutine Start(IEnumerator enumerator)
        {
            Coroutine result = new Coroutine(enumerator);
            ProcessorCoroutine.Default.Add(result);
            result.Play();

            return result;
        }
        public Coroutine SetUpdateType(UpdateType updateType)
        {
            UpdateMode = updateType;
            return this;
        }
    }
    public class WaitFor
    {
        public float Time { get; set; }
        public float TimeUnscaled { get; set; }
        public bool Wait => Time > 0f || TimeUnscaled > 0f;

        public WaitFor(float time = 0f, float timeUnscaled = 0f)
        {
            this.Time = time;
            this.TimeUnscaled = timeUnscaled;
        }
    }
}
