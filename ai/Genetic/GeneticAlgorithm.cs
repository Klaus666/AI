using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Genetic
{
    public class GeneticAlgorithm<C, T>
        where C : IChromosome<C>
        where T : IComparable<T>
    {
        public delegate void OnIterationComplete(GeneticAlgorithm<C, T> environment);

        #region ChromosomesComparator
        private readonly Dictionary<C, T> fitness_cache = new Dictionary<C, T>();

        private class ChromosomesComparator : IComparer<C>
        {
            private readonly GeneticAlgorithm<C, T> context;
            private readonly IFitness<C, T> fitnessFunc;

            public ChromosomesComparator(GeneticAlgorithm<C, T> algorithm, IFitness<C, T> fitness)
            {
                context = algorithm;
                fitnessFunc = fitness;
            }
            
            public T Fit(C chr)
            {
                T fit = default(T);
                if (!context.fitness_cache.TryGetValue(chr, out fit))
                {
                    fit = fitnessFunc.Calculate(chr);
                    context.fitness_cache[chr] = fit;
                }
                return fit;
            }

            public int Compare(C chr1, C chr2)
            {
                T fit1 = Fit(chr1);
                T fit2 = Fit(chr2);
                return fit1.CompareTo(fit2);
            }
        }

        private readonly ChromosomesComparator chromosomesComparator;
        #endregion


        public Population<C> Population { get; private set; }

        public event OnIterationComplete IterationComplete;

        private bool terminate = false;

        public int ParentChromosomesSurviveCount { get; set; } = int.MaxValue;

        public int Iteration { get; private set; } = 0;

        public GeneticAlgorithm(Population<C> population, IFitness<C, T> fitnessFunc)
        {
            Population = population;
            chromosomesComparator = new ChromosomesComparator(this, fitnessFunc);
            Population.SortPopulationByFitness(chromosomesComparator);
        }

        public void Evolve()
        {
            var newPopulation = new Population<C>();

            var surviver_count = Math.Min(Population.Size, ParentChromosomesSurviveCount);

            for (int i = 0; i < surviver_count; i++)
                newPopulation.addChromosome(Population.GetChromosomeByIndex(i));

            for (int i = 0; i < Population.Size; i++)
            {
                var chromosome = Population.GetChromosomeByIndex(i);
                var mutated = chromosome.Mutate();

                var otherChromosome = Population.GetRandomChromosome();
                var crossovered = chromosome.Crossover(otherChromosome);

                newPopulation.addChromosome(mutated);
                foreach (var c in crossovered)
                    newPopulation.addChromosome(c);
            }

            newPopulation.SortPopulationByFitness(chromosomesComparator);
            newPopulation.Trim(Population.Size);
            Population = newPopulation;

            foreach (var corpse in fitness_cache.Keys.Except(new HashSet<C>(newPopulation)).ToArray()) fitness_cache.Remove(corpse);
        }

        public void Evolve(int count)
        {
            terminate = false;

            for (int i = 0; i < count; i++)
            {
                if (terminate) break;

                Evolve();
                Iteration = i;
                if (IterationComplete != null) IterationComplete(this);
            }
        }

        public void Terminate() => terminate = true;

        public C GetBest() => Population.GetChromosomeByIndex(0);
    }
}
