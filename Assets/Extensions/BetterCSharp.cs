using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
    public static class BetterCSharp
    {
        public static ICollection<T> RemoveCollection<T>(this ICollection<T> current, ICollection<T> other)
        {
            ICollection<T> collection = current;
            for (int i = 0; i < other.Count(); i++)
            {
                collection.Remove(other.ElementAt(i));
            }
            return collection;
        }
        
        public static IEnumerable<T> Add<T>(this IEnumerable<T> current, T value)
        {
            foreach (var item in current)
                yield return item;
            
            yield return value;
        }
    }
}
