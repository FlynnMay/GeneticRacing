using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evo
{
    /// <summary>
    /// Basic float generator for values between 0 and 1
    /// </summary>
    [CreateAssetMenu(fileName = "Float01", menuName = "EvoDefaults/ValueGenerators/Float01")]
    public class DNAFloat01Generator : DNAValueGenerator<float>
    {
        /// <summary>
        /// Overrides Default GetValue()
        /// </summary>
        /// <returns>A float between 0 and 1</returns>
        public override float GetValue()
        {
            return Random.Range(0.0f, 1.0f);
        }
    }
}
