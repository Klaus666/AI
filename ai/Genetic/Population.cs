using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Genetic
{
    public sealed class Population<C> : IEnumerable<C>
         where C : IChromosome<C>
    {
        private static readonly Random random = new Random();
        private List<C> chromosomes = new List<C>(22);

        public void Add(C chromosome) => chromosomes.Add(chromosome);

        public int Size => chromosomes.Count;

        public C GetRandomChromosome()
        {
            int numOfChromosomes = chromosomes.Count;
            int indx = random.Next(numOfChromosomes);
            return chromosomes[indx];
        }

        public C this[int indx] => chromosomes[indx];

        public void SortPopulationByFitness(IComparer<C> chromosomesComparator)
        {
            chromosomes = chromosomes.OrderBy(w => random.Next()).ToList();
            chromosomes.Sort(chromosomesComparator);
        }

        public void Trim(int len) => chromosomes = chromosomes.Take(len).ToList();

        public IEnumerator<C> GetEnumerator() => chromosomes.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => chromosomes.GetEnumerator();
    }
}
