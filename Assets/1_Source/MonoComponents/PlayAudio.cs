using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class PlayAudio : MonoBehaviour
    {
        public AudioPlayData audioToPlay;

        private void Awake()
        {
            audioToPlay.Play(ProcessorSoundPool.PoolLevel.Global);
        }
    }
}
