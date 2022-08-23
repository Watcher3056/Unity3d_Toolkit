using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public partial class Starter
    {
#if UNITY_EDITOR
        [Sirenix.OdinInspector.Button]
        private void CleanProgress()
        {
            ProcessorSaveLoad.CleanSaves();
        }
        private void Update()
        {

        }
#endif
    }
}
