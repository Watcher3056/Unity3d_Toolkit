using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace TeamAlpha.Source
{
    [Serializable]
    public class AudioConfig
    {
        public enum Mode { None, Single, Variants }
        public Mode mode;
        [ShowIf("mode", Mode.Single)]
        [InfoBox("Required!", InfoMessageType.Error, "@$value.clip == null && mode == Mode.Single")]
        public AudioPlayData audio;
        [ShowIf("mode", Mode.Variants)]
        [InfoBox("Required!", InfoMessageType.Error, "@$value == null && mode == Mode.Variants")]
        public AudioVariantsList variants;


        public AudioSource Play(ProcessorSoundPool.PoolLevel poolLevel, bool stopLast = false)
        {
            if (mode == Mode.Single)
                return audio.Play(poolLevel, stopLast);
            else if (mode == Mode.Variants)
                return variants.Play(poolLevel, stopLast);
            else
                return null;
        }
    }
}