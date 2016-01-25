using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genetic
{
    public interface IChromosome<C> where C : IChromosome<C>
    {
        C[] Crossover(C anotherChromosome);

        C Mutate();
    }

    public interface IFitness<C, T>
        where C : IChromosome<C>
        where T : IComparable<T>
    {
        T Calculate(C chromosome);
    }

    public interface IIterationListener<C, T>
        where C : IChromosome<C>
        where T : IComparable<T>
    {
        void Update(GeneticAlgorithm<C, T> environment);
    }
}
