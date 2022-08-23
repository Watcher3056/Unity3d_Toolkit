using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace TeamAlpha.Source
{
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {
        [Required]
        public AudioConfig audioOnClick;

        private Button button;
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => audioOnClick.Play(ProcessorSoundPool.PoolLevel.Global));
        }
    }
}
