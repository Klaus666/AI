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
}
