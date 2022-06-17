using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Evo
{
    /// <summary>
    /// Useful for when Unity's regular Time class doesn't need to be scaled. WARNING: This can cause issues with physics!
    /// </summary>
    public class GeneticTime : MonoBehaviour
    {
        /// <summary>
        /// Used for singleton Implentation
        /// </summary>
        public static GeneticTime instance;

        /// <summary>
        /// The rate at which time passes
        /// </summary>
        public static float timeScale = 1.0f;

        /// <summary>
        /// The scaled time in seconds since the last frame
        /// </summary>
        public static float deltaTime;

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            deltaTime = Time.unscaledDeltaTime * timeScale;
        }
    }
}
