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
        private readonly Dictionary<C, T> fitness_cache = new Dictionary<C, T>();



        private const int ALL_PARENTAL_CHROMOSOMES = int.MaxValue;

        private class ChromosomesComparator : IComparer<C>
        {
            private GeneticAlgorithm<C, T> context;

            public ChromosomesComparator(GeneticAlgorithm<C, T> algorithm)
            {
                context = algorithm;
            }

            public T Fit(C chr)
            {
                T fit = default(T);
                if (!context.fitness_cache.TryGetValue(chr, out fit))
                {
                    fit = context.fitnessFunc.Calculate(chr);
                    context.fitness_cache[chr] = fit;
                }
                return fit;
            }

            public void ClearCache()
            {
                context.fitness_cache.Clear();
            }

            public int Compare(C chr1, C chr2)
            {
                T fit1 = Fit(chr1);
                T fit2 = Fit(chr2);
                int ret = fit1.CompareTo(fit2);
                return ret;
            }
        }

        private readonly ChromosomesComparator chromosomesComparator;

        private readonly IFitness<C, T> fitnessFunc;

        private Population<C> population;

        private List<IIterationListener<C, T>> iterationListeners = new List<IIterationListener<C, T>>();

        private bool terminate = false;

        private int parentChromosomesSurviveCount = ALL_PARENTAL_CHROMOSOMES;

        private int iteration = 0;

        public GeneticAlgorithm(Population<C> population, IFitness<C, T> fitnessFunc)
        {
            this.population = population;
            this.fitnessFunc = fitnessFunc;
            chromosomesComparator = new ChromosomesComparator(this);
            this.population.SortPopulationByFitness(chromosomesComparator);
        }

        public void Evolve()
        {
            int parentPopulationSize = population.Count();

            var newPopulation = new Population<C>();

            var surviver_count = Math.Min(parentPopulationSize, parentChromosomesSurviveCount);

            for (int i = 0; i < surviver_count; i++)
                newPopulation.addChromosome(population.GetChromosomeByIndex(i));

            for (int i = 0; i < parentPopulationSize; i++)
            {
                var chromosome = population.GetChromosomeByIndex(i);
                var mutated = chromosome.Mutate();

                var otherChromosome = population.GetRandomChromosome();
                var crossovered = chromosome.Crossover(otherChromosome);

                newPopulation.addChromosome(mutated);
                foreach (var c in crossovered)
                    newPopulation.addChromosome(c);
            }

            newPopulation.SortPopulationByFitness(chromosomesComparator);
            newPopulation.Trim(parentPopulationSize);
            population = newPopulation;

            foreach (var corpse in fitness_cache.Keys.Except(new HashSet<C>(newPopulation)).ToArray()) fitness_cache.Remove(corpse);
        }

        public void Evolve(int count)
        {
            terminate = false;

            for (int i = 0; i < count; i++)
            {
                if (terminate) break;

                Evolve();
                iteration = i;
                foreach (var l in iterationListeners) l.Update(this);
            }
        }

        public int GetIteration()
        {
            return iteration;
        }

        public void Terminate()
        {
            terminate = true;
        }

        public Population<C> GetPopulation()
        {
            return population;
        }

        public C GetBest()
        {
            return population.GetChromosomeByIndex(0);
        }

        public C GetWorst()
        {
            return population.GetChromosomeByIndex(population.Count() - 1);
        }

        public void SetParentChromosomesSurviveCount(int parentChromosomesCount)
        {
            parentChromosomesSurviveCount = parentChromosomesCount;
        }

        public int GetParentChromosomesSurviveCount()
        {
            return parentChromosomesSurviveCount;
        }

        public void AddIterationListener(IIterationListener<C, T> listener)
        {
            iterationListeners.Add(listener);
        }

        public void removeIterationListener(IIterationListener<C, T> listener)
        {
            iterationListeners.Remove(listener);
        }

        public T Fitness(C chromosome)
        {
            return chromosomesComparator.Fit(chromosome);
        }

        public void ClearCache()
        {
            chromosomesComparator.ClearCache();
        }
    }
}
