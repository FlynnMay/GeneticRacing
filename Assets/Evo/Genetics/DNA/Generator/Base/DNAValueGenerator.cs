using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evo
{
    public class DNAValueGenerator : ScriptableObject
    {
    }

    public class DNAValueGenerator<T> : DNAValueGenerator
    {
        /// <summary>
        /// Override to return a custom value, Random values are recommended
        /// </summary>
        /// <returns>Returns default value, override for random value</returns>
        public virtual T GetValue() { return default(T); }
    }
}
