using System;
using System.Reflection;
using System.Collections.Generic;
namespace Evo
{
    [Serializable]
    public class Genome
    {
        /// <summary>
        /// The values being trained by the genetic algorithm
        /// </summary>
        public object[] Genes { get; set; }
        
        /// <summary>
        /// A value between 0 and 1, which determines how fit the genome is, 1 being the best
        /// </summary>
        public float Fitness { get; set; }

        /// <summary>
        /// The instructions passed to the genome, used to generate random gene values, as well as calculate the genomes fitness.
        /// </summary>
        public IEvolutionInstructions Instructions { get; private set; }

        /// <summary>
        /// Is true if the genome is one of the top <see cref="EvoGeneticAlgorithm.eliteCount"/> genomes in the <see cref="EvoGeneticAlgorithm.Population"/>
        /// </summary>
        public bool IsElite { get; set; }

        /// <summary>
        /// Is true if the genome is the top <see cref="EvoGeneticAlgorithm.eliteCount"/> genomes in the <see cref="EvoGeneticAlgorithm.Population"/>
        /// </summary>
        public bool IsKing { get; set; }

        /// <summary>
        /// Is true if the genome is currently apart of an <see cref="EvoGeneticAlgorithm.Population"/>
        /// </summary>
        public bool IsTraining { get; private set; }

        /// <summary>
        /// The <see cref="EvolutionAgent"/> currently using this <see cref="Genome"/>
        /// </summary>
        public EvolutionAgent agent;

        /// <summary>
        /// The random class used for calculation random values in <see cref="Mutate(float)"/> and <see cref="Crossover(Genome)"/>
        /// </summary>
        Random random;

        /// <summary>
        /// Training Constructor, used by the <see cref="EvolutionAgent"/> to create its <see cref="Genome"/>, during training to create a genome capable of random value generation
        /// </summary>
        /// <param name="size">Defines the genes array size</param>
        /// <param name="_random">used for getting random values in <see cref="Mutate(float)"/> and <see cref="Crossover(Genome)"/></param>
        /// <param name="instructions">The evolution instructions used, instructs and a genome how to create a random gene, as well as calculate its own fitness</param>
        /// <param name="_agent">the agent using this genome</param>
        public Genome(int size, Random _random, IEvolutionInstructions instructions, EvolutionAgent _agent = null)
        {
            agent = _agent;
            Instructions = instructions;
            Genes = new object[size];
            random = _random;

            for (int i = 0; i < Genes.Length; i++)
            {
                Genes[i] = Instructions.GetEvolutionRandomValue();
            }

            IsTraining = true;
        }

        /// <summary>
        /// Non-Training Constructor, used by the <see cref="EvolutionAgent"/> to create its <see cref="Genome"/> when it isn't training, 
        /// meaning it never needs access to random value generation, unlike the "Training Constructor"
        /// </summary>
        /// <param name="size">Defines the genes array size</param>
        /// <param name="_agent">the agent using this genome</param>
        public Genome(EvolutionAgent _agent, int size = 0)
        {
            agent = _agent;
            Genes = new object[size];
            IsTraining = false;
        }

        /// <summary>
        /// Generates a value between 0 and 1, which determines how great of a fit the genes are at solving the task
        /// </summary>
        /// <returns> A value between 0 and 1, which determines how great of a fit the genes are at solving the task</returns>
        public float CalaculateFitness()
        {
            if (!IsTraining)
                return 1.0f;

            Fitness = Instructions.EvolutionFitnessFunction(this);
            return Fitness;
        }

        /// <summary>
        /// Creates a child gene by combining the this current genomes' genes with the <paramref name="other"/> genomes' genes.
        /// </summary>
        /// <param name="other"></param>
        /// <returns>The created child dream</returns>
        public Genome Crossover(Genome other)
        {
            if (!IsTraining)
                return this;

            Genome child = new Genome(Genes.Length, random, Instructions);

            for (int i = 0; i < Genes.Length; i++)
            {
                // Coin flip to decide which parents genes to take
                child.Genes[i] = random.NextDouble() > 0.5 ? Genes[i] : other.Genes[i];
            }

            return child;
        }

        /// <summary>
        /// Can randomly change the some of the genes, the likeliness of this occuring is defined by <paramref name="mutationRate"/>
        /// </summary>
        /// <param name="mutationRate">A 0 to 1 value which defines the percentage chance of an individual gene mutating</param>
        public void Mutate(float mutationRate)
        {
            for (int i = 0; i < Genes.Length; i++)
            {
                if (random.NextDouble() < mutationRate)
                    Genes[i] = Instructions.GetEvolutionRandomValue();
            }
        }
    }
}