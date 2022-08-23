using DG.Tweening;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamAlpha.Source
{
    [Serializable, InlineDropdown]
    public class AudioPlayData
    {
        [InfoBox("Required!", InfoMessageType.Error, VisibleIf = "ParentHasRequiredAttribute")]
        public AudioClip clip;
        [Range(0f, 1f), SerializeField]
        private float volume = 1f;

        [FoldoutGroup("Advanced")]
        public float fadeInOutTime = 0.25f;
        [FoldoutGroup("Advanced")]
        public bool randomizePitch;
        [FoldoutGroup("Advanced")]
        [ShowIf("randomizePitch"), MinMaxSlider(-100f, 100f)]
        public Vector2 pitchPercentRandomization;
        [FoldoutGroup("Advanced")]
        [HideIf("randomizePitch")]
        public float pitchFactor = 1f;
        [FoldoutGroup("Advanced")]
        public bool randomizeStartTime;
        [FoldoutGroup("Advanced")]
        [ShowIf("randomizeStartTime")]
        public List<float> startTimePoints = new List<float>();

        [NonSerialized]
        private AudioSource lastSource;
        public float Volume
        {
            get => volume;
        }
        public bool Loop
        {
            get => loop;
        }
        [FoldoutGroup("Advanced")]
        [SerializeField]
        private bool loop;

#if UNITY_EDITOR
        private bool ParentHasRequiredAttribute(InspectorProperty property)
        {
            return property.Parent.Attributes.HasAttribute<RequiredAttribute>() && clip == null;
        }
#endif
        public AudioSource Play(ProcessorSoundPool.PoolLevel poolLevel, bool stopLast = false)
        {
            if (clip == null)
                return null;
            float pitch = 1f;
            if (randomizePitch)
            {
                float pitchRandom = UnityEngine.Random.Range(pitchPercentRandomization.x, pitchPercentRandomization.y);
                pitch *= 1f + pitchRandom / 100f;
            }
            else
                pitch *= this.pitchFactor;
            AudioSource audioSource = ProcessorSoundPool.PlaySound(clip, 0f, poolLevel, loop, pitch: pitch);

            if (fadeInOutTime > 0f)
                audioSource.DOVolume(volume, fadeInOutTime);
            else
                audioSource.volume = volume;
            if (randomizeStartTime)
            {
                if (startTimePoints.Count > 0)
                    audioSource.time = startTimePoints.Random();
                else
                    audioSource.time = UnityEngine.Random.Range(0f, audioSource.clip.length);
            }

            if (stopLast && lastSource != null)
                StopLast();
            lastSource = audioSource;
            return audioSource;
        }
        public void StopLast()
        {
            if (lastSource != null)
            {
                AudioSource _lastSource = lastSource;

                _lastSource
                    .DOVolume(0f, fadeInOutTime)
                    .OnComplete(() =>
                    {
                        _lastSource.Stop();
                        if (_lastSource == lastSource)
                            lastSource = null;
                    });
            }
        }
    }
}