using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.Helpers
{
    /// <summary> 
    /// Emulation of a pointer, sans memory access (but with permanent access features). Note that this is not type safe
    /// </summary>
    /// <usage>
    ///  - Can act as container for storing a value as a field in another class
    ///  - Can replace (albeit far more verbosely and dangerously)  the `ref` or `out` keywords.
    ///    should only be used in this context if the `ref` and `out` constraints are too restrictive
    ///  - Can serve as a method for moving structs around by reference
    /// </usage>
    public class SharpPointer<T> 
    {
        private Func<T> GetVal;
        private Action<T> SetVal;
        public SharpPointer(Func<T> getval, Action<T> setval)
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

    /// <summary> 
    /// Allows for the creation of an object field (or property) "pointer" by simply 
    /// passing the object and name of the property (as a string). EXTREMELY non type-safe, but enjoy it anyway
    /// </summary>
    public class FieldPointer<T> : SharpPointer<T>
    {
        /// <summary>
        /// Creates a pointer to an object field or property using the object and stringified field/property name
        /// </summary>
        public FieldPointer(object obj, string fieldName)
            :base
            (
                 () => (T)typeof(object).GetProperty(fieldName).GetValue(obj),
                 (T value) => typeof(object).GetProperty(fieldName).SetValue(obj, value)
            )
        {
        }
    }
}
