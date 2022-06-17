using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Evo
{
    /// <summary>
    /// Custom Manhattan Distance Fitness Function, Target must be set
    /// </summary>
    [CreateAssetMenu(fileName = "Manhattan Distance Function", menuName = "EvoDefaults/Fitness/ManhattanDistanceFunction")]
    public class ManhattanDistanceFitnessFunction : FitnessFunction
    {
        public Transform target;
        
        /// <summary>
        /// Sets the target transform
        /// </summary>
        /// <param name="target"></param>
        public void SetTarget(Transform target)
        {
            this.target = target;
        }

        /// <summary>
        /// Overrides the default fittness function
        /// </summary>
        /// <returns>The manhattan distance between the agent and the target scaled between 0 and 1</returns>
        /// <param name="agent"></param>
        public override float GetValue(EvolutionAgent agent)
        {
            Vector3 pos = agent.transform.position;
            float dist = Mathf.Abs(pos.x - target.position.x) + Mathf.Abs(pos.y - target.position.y) + Mathf.Abs(pos.z - target.position.z);
            return 1.0f / dist;
        }
    }
}

