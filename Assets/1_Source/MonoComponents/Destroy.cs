using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TeamAlpha.Source
{
    public class Destroy : MonoBehaviour
    {
        public enum Condition { Awake, ReleaseBuild }

        public Condition condition;

        private void Awake()
        {
            if (condition == Condition.Awake)
                Destroy(gameObject);
            else if (condition == Condition.ReleaseBuild)
            {
                if (!Debug.isDebugBuild)
                    Destroy(gameObject);
            }
        }
    }
}
