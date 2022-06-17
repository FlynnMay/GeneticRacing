using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Evo
{
    public class EvoGeneticAlgorithm
    {
        public List<Genome> Population { get; set; }
        public int Generation { get; private set; }
        public float BestFitness { get; private set; }
        public object[] BestGenes { get; private set; }

        public float mutationRate;
        Random random;
        float fitnessSum;
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

        void CalculateFitness()
        {
            fitnessSum = 0;
            Genome bestGenome = Population[0];

            for (int i = 0; i < Population.Count; i++)
            {
                fitnessSum += Population[i].CalaculateFitness(i);

                if (Population[i].Fitness > bestGenome.Fitness)
                    bestGenome = Population[i];
            }

            BestFitness = bestGenome.Fitness;
            BestGenes = bestGenome.Genes;
        }

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

/*
 *  START
 *  Generate the initial population
 *  Compute fitness
 *  REPEAT
 *      Selection
 *      Crossover
 *      Mutation
 *      Compute fitness
 *  UNTIL population has converged
 *  STOP
 */
