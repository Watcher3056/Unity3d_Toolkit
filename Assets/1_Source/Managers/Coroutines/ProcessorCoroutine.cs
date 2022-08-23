using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class ProcessorCoroutine : MonoBehaviour
    {
        public static ProcessorCoroutine Default { get; private set; }


        private List<Coroutine> coroutines = new List<Coroutine>();
        public ProcessorCoroutine() => Default = this;
        private void Update()
        {
            for (int i = 0; i < coroutines.Count; i++)
            {
                Coroutine coroutine = coroutines[i];
                if (coroutine.IsPlaying && coroutine.UpdateMode == Coroutine.UpdateType.Update)
                    coroutine.Tick();
                else if (!coroutine.IsAlive)
                {
                    coroutines.RemoveAt(i);
                    i--;
                }
            }
        }
        private void FixedUpdate()
        {
            for (int i = 0; i < coroutines.Count; i++)
            {
                Coroutine coroutine = coroutines[i];
                if (coroutine.UpdateMode != Coroutine.UpdateType.FixedUpdate)
                    continue;
                if (coroutine.IsPlaying)
                    coroutine.Tick();
            }
        }
        private void LateUpdate()
        {
            for (int i = 0; i < coroutines.Count; i++)
            {
                Coroutine coroutine = coroutines[i];
                if (coroutine.UpdateMode != Coroutine.UpdateType.LateUpdate)
                    continue;
                if (coroutine.IsPlaying)
                    coroutine.Tick();
            }
        }
        public void Add(Coroutine coroutine)
        {
            coroutines.Add(coroutine);
        }
    }
}
