using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Helpers
{
    /// <summary> 
    /// Emulation of a pointer, sans memory access (but with permanent access features)
    /// </summary>
    /// <usage>
    ///  - Can act as container for storing a value as a field in another class
    ///  - Can replace (albeit far more verbosely and dangerously)  the `ref` or `out` keywords.
    ///    should only be used in this context if the `ref` and `out` constraints are too restrictive
    ///  - Can serve as a method for moving structs around by reference
    /// </usage>
    public sealed class Pointer<T>
    {
        private Func<T> GetVal;
        private Action<T> SetVal;
        public Pointer(Func<T> getval, Action<T> setval)
        {
            GetVal = getval;
            SetVal = setval;
        }

        public T Value
        {
            get => GetVal();
            set => SetVal(value);
        }
    }
}
