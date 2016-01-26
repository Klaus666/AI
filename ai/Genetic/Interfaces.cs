namespace Genetic
{
    public interface IChromosome<C> where C : IChromosome<C>
    {
        C[] Crossover(C anotherChromosome);

        C Mutate();
    }
}
