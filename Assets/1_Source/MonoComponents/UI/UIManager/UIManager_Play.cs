using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    public partial class UIManager
    {
        private void SetupStatePlay()
        {
            statesMap.AddState((int)State.Play, StatePlayOnStart, StatePlayOnEnd);
        }
        private void StatePlayOnStart()
        {

        }
        private void StatePlayOnEnd(int stateTo)
        {

        }
    }
}
