using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamAlpha.Source
{
    public interface IOwnedBy<T>
    {
        public T Owner { get; }
        public void SetOwner(T owner);
    }
    //public interface IOwner<T> : ICollection<T>
    //{

    //}
}
