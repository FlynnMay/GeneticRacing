using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Evo
{
    public class EvoGeneticAlgorithm
    {
        /// <summary>
        /// All the Genomes (DNA), Currently control by the genetic algorithm.
        /// </summary>
        public List<Genome> Population { get; set; }

        /// <summary>
        /// The current generation the Genetetic Algorithm is on.
        /// </summary>
        public int Generation { get; private set; }

        /// <summary>
        /// The best fitness of the last generation
        /// </summary>
        public float BestFitness { get; private set; }

        /// <summary>
        /// The best genes of the last generation
        /// </summary>
        public object[] BestGenes { get; private set; }

        /// <summary>
        /// The chance of a singular Gene in a Genome mutating
        /// </summary>
        public float mutationRate;

        /// <summary>
        /// The provided random which is used in <see cref="ChooseParent"/>
        /// </summary>
        Random random;

        /// <summary>
        /// The total fitness value set in <see cref="CalculateFitness"/>
        /// </summary>
        float fitnessSum;

        /// <summary>
        /// The amount of elites in a population.
        /// Elites are the top genomes which persisit between generations.
        /// </summary>
        int eliteCount;

        public EvoGeneticAlgorithm(List<Genome> genomes, int genomeSize, Random _random, IEvolutionInstructions instructions, float _mutationRate = 0.01f, int _eliteCount = 2)
        {
            Generation = 1;
            mutationRate = _mutationRate;
            Population = genomes;
            random = _random;
            fitnessSum = 0;
            BestFitness = 0;
            BestGenes = new object[genomeSize];
            eliteCount = _eliteCount;
        }

        /// <summary>
        /// Calculates the fitness of the generation the uses the elites of the generation to build a new one.
        /// </summary>
        public void NewGeneration()
        {
            if (Population.Count <= 0)
                return;

            CalculateFitness();

            Population = Population.OrderByDescending(g => g.Fitness).ToList();
            EvolutionAgent[] evolutionAgents = Population.Select(g => g.agent).ToArray();
            List<Genome> newPopulation = new List<Genome>();

            for (int i = 0; i < Population.Count; i++)
            {
                if (i < eliteCount)
                {
                    Population[i].IsElite = true;
                    Population[i].IsKing = false;
                    Population[i].agent = evolutionAgents[i];
                    evolutionAgents[i].DNA = Population[i];
                    newPopulation.Add(Population[i]);
                    continue;
                }

                Genome parentA = ChooseParent();
                Genome parentB = ChooseParent();

                Genome child = parentA.Crossover(parentB);
                child.agent = evolutionAgents[i];
                evolutionAgents[i].DNA = child;

                child.IsElite = false;
                child.IsKing = false;
                child.Mutate(mutationRate);
                newPopulation.Add(child);
            }

            Population[0].IsKing = true;
            Population = newPopulation;
            Generation++;
        }

        /// <summary>
        /// Instructs each <see cref="Genome"/> in the population to calculate it's fitness, and adds each fitness value to the fitness sum
        /// </summary>
        void CalculateFitness()
        {
            fitnessSum = 0;
            Genome bestGenome = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalaculateFitness();

                if (Population[i].Fitness > bestGenome.Fitness)
                    bestGenome = Population[i];
            }

            BestFitness = bestGenome.Fitness;
            BestGenes = bestGenome.Genes;
        }

        /// <summary>
        /// Roulette wheel selection, has a weighted chance to pick the genomes most fit for the job as a parent.
        /// </summary>
        /// <returns>A random genome from the poulation, most likely one of the most fit genomes</returns>
        Genome ChooseParent()
        {
            // Roullete wheel
            if (fitnessSum <= 0)
                return Population[0];

            float[] fitnessProbabillity = Population.Select(g => g.Fitness / fitnessSum).ToArray();
            float[] cumaltiveProbabillity = new float[fitnessProbabillity.Length + 1];

            cumaltiveProbabillity[0] = 0.0f;
            for (int i = 1; i < cumaltiveProbabillity.Length; i++)
                cumaltiveProbabillity[i] = fitnessProbabillity[i - 1] + cumaltiveProbabillity[i - 1];

            float randNum = (float)random.NextDouble();

            for (int i = 0; i < cumaltiveProbabillity.Length; i++)
            {
                if (cumaltiveProbabillity[i] > randNum)
                    return Population[i - 1];
            }

            return null;
        }
    }
}