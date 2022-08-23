using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    public class Blocker
    {
        public bool IsBlocked => blockers.Count > 0;
        private List<object> blockers = new List<object>();
        public void SetBlock(object blocker, bool arg)
        {
            if (arg && !blockers.Contains(blocker))
                blockers.Add(blocker);
            else if (!arg)
                blockers.Remove(blocker);
        }
        public void RemoveAllBlockers()
        {
            blockers.Clear();
        }
    }
}
