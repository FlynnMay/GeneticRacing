using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evo
{
    /// <summary>
    /// Inherit from this to create a custom scriptable object class for getting algorithm fitness
    /// </summary>
    public class FitnessFunction : ScriptableObject
    {
        /// <summary>
        /// Used to create custom fitness functions, return value should be the fitness of an agent.
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>Agents calculate fitness</returns>
        public virtual float GetValue(EvolutionAgent agent)
        {
            return float.PositiveInfinity;
        }
    }
}