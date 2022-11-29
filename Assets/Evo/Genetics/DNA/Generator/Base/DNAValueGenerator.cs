using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evo
{
    public class DNAValueGenerator : ScriptableObject
    {
    }

    /// <summary>
    /// Extend this class to create custom DNA Value Generators. 
    /// Check the example class <see cref="DNAFloat01Generator"/> for more info.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DNAValueGenerator<T> : DNAValueGenerator
    {
        /// <summary>
        /// Override to return a custom value, Random values are recommended
        /// </summary>
        /// <returns>Returns default value, override for random value</returns>
        public virtual T GetValue() { return default(T); }
    }
}
