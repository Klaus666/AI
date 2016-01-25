using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    public class Population<C> : IEnumerable<C>
         where C : IChromosome<C>
    {
        private const int DEFAULT_NUMBER_OF_CHROMOSOMES = 32;
        private List<C> chromosomes = new List<C>(DEFAULT_NUMBER_OF_CHROMOSOMES);
        private static readonly Random random = new Random();

        public void addChromosome(C chromosome)
        {
            chromosomes.Add(chromosome);
        }

        public int GetSize()
        {
            return chromosomes.Count;
        }

        public C GetRandomChromosome()
        {
            int numOfChromosomes = chromosomes.Count;
            int indx = random.Next(numOfChromosomes);
            return chromosomes[indx];
        }

        public C GetChromosomeByIndex(int indx)
        {
            return chromosomes[indx];
        }

        public void SortPopulationByFitness(IComparer<C> chromosomesComparator)
        {
            chromosomes = chromosomes.OrderBy(w => random.Next()).ToList();
            chromosomes.Sort(chromosomesComparator);
        }

        public void Trim(int len)
        {
            chromosomes = chromosomes.Take(len).ToList();
        }

        public IEnumerator<C> GetEnumerator()
        {
            return chromosomes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return chromosomes.GetEnumerator();
        }
    }
}
