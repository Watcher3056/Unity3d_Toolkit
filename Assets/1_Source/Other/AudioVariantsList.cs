using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamAlpha.Source
{
    [InlineEditor]
    public class AudioVariantsList : ScriptableObject
    {
        [Required]
        public List<AudioPlayData> variants;
#if UNITY_EDITOR
        [ShowInInspector]
        private List<AudioClip> fastCreateList = new List<AudioClip>();
        [Button("Create")]
        private void _EditorFastCreate()
        {
            foreach (AudioClip clip in fastCreateList)
                variants.Add(new AudioPlayData { clip = clip });
            fastCreateList.Clear();
        }
#endif

        public AudioSource Play(ProcessorSoundPool.PoolLevel poolLevel, bool stopLast = false)
        {
            return variants.Random().Play(poolLevel, stopLast);
        }
    }
}