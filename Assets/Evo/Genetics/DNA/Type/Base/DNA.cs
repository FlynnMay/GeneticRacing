using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Evo
{
    /// <summary>
    /// Empty holder class for DNA<T>
    /// </summary>
    public class DNA : ScriptableObject
    {
    }

    /// <summary>
    /// Generic Type class to be overwritten for custom DNA Types
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DNA<T> : DNA
    {
        public T[] genes;

        public void SetGenes(T[] genes)
        {
            this.genes = genes;
        }

        protected void SetGenesFromObject(object[] genes)
        {
            this.genes = new T[genes.Length];
            for (int i = 0; i < genes.Length; i++)
            {
                this.genes[i] = (T)genes[i];
            }
        }
    }
}

