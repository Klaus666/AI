using Genetic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neural
{

    public class OptimizableNeuralNetwork : NeuralNetwork, IChromosome<OptimizableNeuralNetwork>
    {
        private static class GaussianRandom
        {
            private static bool _hasDeviate = false;
            private static double _storedDeviate = .0;
            private static readonly Random _random = new Random();
            
            /// <summary>
            /// Obtains normally (Gaussian) distributed random numbers, using the Box-Muller
            /// transformation.  This transformation takes two uniformly distributed deviates
            /// within the unit circle, and transforms them into two independently
            /// distributed normal deviates.
            /// </summary>
            /// <param name="mu">The mean of the distribution.  Default is zero.</param>
            /// <param name="sigma">The standard deviation of the distribution.  Default is one.</param>
            /// <returns></returns>
            public static double NextGaussian(double mu = 0, double sigma = 1)
            {
                if (sigma <= 0)
                    throw new ArgumentOutOfRangeException("sigma", "Must be greater than zero.");

                if (_hasDeviate)
                {
                    _hasDeviate = false;
                    return _storedDeviate * sigma + mu;
                }

                double v1, v2, rSquared;
                do
                {
                    // two random values between -1.0 and 1.0
                    v1 = 2 * _random.NextDouble() - 1;
                    v2 = 2 * _random.NextDouble() - 1;
                    rSquared = v1 * v1 + v2 * v2;
                    // ensure within the unit circle
                } while (rSquared >= 1 || rSquared == 0);

                // calculate polar tranformation for each deviate
                var polar = Math.Sqrt(-2 * Math.Log(rSquared) / rSquared);
                // store first deviate
                _storedDeviate = v2 * polar;
                _hasDeviate = true;
                // return second deviate
                return v1 * polar * sigma + mu;
            }
        }

        private static Random random = new Random();
        private static double weightsMutationInterval = 1;
        private static double neuronParamsMutationInterval = 1;

        public OptimizableNeuralNetwork(int numberOfNeurons) : base(numberOfNeurons) { }

        public OptimizableNeuralNetwork(NeuralNetwork nn)
        {
            ActivationIterations = nn.ActivationIterations;
            Neurons = nn.CloneNeurons();
            NeuronsLinks = nn.GetNeuronsLinks();
        }

        public OptimizableNeuralNetwork[] Crossover(OptimizableNeuralNetwork anotherChromosome)
        {
            OptimizableNeuralNetwork anotherClone = anotherChromosome.Clone() as OptimizableNeuralNetwork;
            OptimizableNeuralNetwork thisClone = Clone() as OptimizableNeuralNetwork;

            switch (random.Next(4))
            {
                case 0:
                    {
                        var thisWeights = thisClone.NeuronsLinks.GetAllWeights();
                        var anotherWeights = anotherClone.NeuronsLinks.GetAllWeights();
                        TwoPointsWeightsCrossover(thisWeights, anotherWeights);
                        thisClone.NeuronsLinks.SetAllWeights(thisWeights);
                        anotherClone.NeuronsLinks.SetAllWeights(anotherWeights);
                    }
                    break;
                case 1:
                    {
                        var thisWeights = thisClone.NeuronsLinks.GetAllWeights();
                        var anotherWeights = anotherClone.NeuronsLinks.GetAllWeights();
                        UniformelyDistributedWeightsCrossover(thisWeights, anotherWeights);
                        thisClone.NeuronsLinks.SetAllWeights(thisWeights);
                        anotherClone.NeuronsLinks.SetAllWeights(anotherWeights);
                    }
                    break;
                case 2:
                    TwoPointsNeuronsCrossover(thisClone.Neurons, anotherClone.Neurons);
                    break;
                case 3:
                    UniformelyDistributedNeuronsCrossover(thisClone.Neurons, anotherClone.Neurons);
                    break;
            }

            return new OptimizableNeuralNetwork[] { anotherClone, thisClone, anotherClone.Mutate(), thisClone.Mutate() };
        }

        private static void TwoPointsWeightsCrossover(double[] thisWeights, double[] anotherWeights)
        {
            int left = random.Next(thisWeights.Length);
            int right = random.Next(thisWeights.Length);
            if (left > right)
            {
                int tmp = right;
                right = left;
                left = tmp;
            }
            for (int i = left; i < right; i++)
            {
                double thisWeight = anotherWeights[i];
                thisWeights[i] = anotherWeights[i];
                anotherWeights[i] = thisWeight;
            }
        }

        private static void UniformelyDistributedWeightsCrossover(double[] thisWeights, double[] anotherWeights)
        {
            int weightsSize = thisWeights.Length;
            int itersCount = random.Next(weightsSize);
            if (itersCount == 0)
                itersCount = 1;

            var used = new HashSet<int>();
            for (int iter = 0; iter < itersCount; iter++)
            {
                int i = random.Next(weightsSize);
                if (weightsSize > 1)
                    while (used.Contains(i)) i = random.Next(weightsSize);

                double thisWeight = thisWeights[i];
                double anotherWeight = anotherWeights[i];

                anotherWeights[i] = thisWeight;
                thisWeights[i] = anotherWeight;
                used.Add(i);
            }
        }

        private static void TwoPointsNeuronsCrossover(Neuron[] thisNeurons, Neuron[] anotherNeurons)
        {
            int left = random.Next(thisNeurons.Length);
            int right = random.Next(thisNeurons.Length);
            if (left > right)
            {
                int tmp = right;
                right = left;
                left = tmp;
            }
            for (int i = left; i < right; i++)
            {
                var thisNeuron = thisNeurons[i];
                thisNeurons[i] = anotherNeurons[i];
                anotherNeurons[i] = thisNeuron;
            }
        }

        private static void UniformelyDistributedNeuronsCrossover(Neuron[] thisNeurons, Neuron[] anotherNeurons)
        {
            int neuronsSize = thisNeurons.Length;
            int itersCount = random.Next(neuronsSize);
            if (itersCount == 0) itersCount = 1;

            var used = new HashSet<int>();
            for (int iter = 0; iter < itersCount; iter++)
            {
                int i = random.Next(neuronsSize);
                if (neuronsSize > 1)
                    while (used.Contains(i)) i = random.Next(neuronsSize);

                Neuron thisNeuron = thisNeurons[i];
                Neuron anotherNeuron = anotherNeurons[i];

                anotherNeurons[i] = thisNeuron;
                thisNeurons[i] = anotherNeuron;
                used.Add(i);
            }
        }

        public OptimizableNeuralNetwork Mutate()
        {
            var mutated = Clone() as OptimizableNeuralNetwork;

            switch (random.Next(4))
            {
                case 0:
                    {
                        var weights = mutated.NeuronsLinks.GetAllWeights();
                        MutateWeights(weights);
                        mutated.NeuronsLinks.SetAllWeights(weights);
                    }
                    break;
                case 1:
                    {
                        MutateNeuronsFunctionsParams(mutated.Neurons);
                    }
                    break;
                case 2:
                    {
                        MutateChangeNeuronsFunctions(mutated.Neurons);
                    }
                    break;
                case 3:
                    {
                        var weights = mutated.NeuronsLinks.GetAllWeights();
                        ShuffleWeightsOnSubinterval(weights);
                        mutated.NeuronsLinks.SetAllWeights(weights);
                    }
                    break;
            }

            return mutated;
        }

        private static void MutateWeights(double[] weights)
        {
            int weightsSize = weights.Length;
            int itersCount = random.Next(weightsSize);
            if (itersCount == 0) itersCount = 1;

            var used = new HashSet<int>();
            for (int iter = 0; iter < itersCount; iter++)
            {
                int i = random.Next(weightsSize);
                if (weightsSize > 1)
                    while (used.Contains(i)) i = random.Next(weightsSize);

                double w = weights[i];
                w += (GaussianRandom.NextGaussian() - GaussianRandom.NextGaussian()) * weightsMutationInterval;
                // w += (this.random.nextDouble() - this.random.nextDouble()) *
                // weightsMutationInterval;
                weights[i] = w;
                used.Add(i);
            }
        }

        private static void MutateNeuronsFunctionsParams(Neuron[] neurons)
        {
            int neuronsSize = neurons.Length;
            int itersCount = random.Next(neuronsSize);
            if (itersCount == 0) itersCount = 1;

            var used = new HashSet<int>();
            for (int iter = 0; iter < itersCount; iter++)
            {
                int i = random.Next(neuronsSize);
                if (neuronsSize > 1)
                    while (used.Contains(i)) i = random.Next(neuronsSize);

                var n = neurons[i];

                var Params = n.GetParams();
                for (int j = 0; j < Params.Length; j++)
                {
                    double param = Params[j];
                    param += (GaussianRandom.NextGaussian() - GaussianRandom.NextGaussian()) * neuronParamsMutationInterval;
                    // param += (this.random.nextDouble() -
                    // this.random.nextDouble()) * neuronParamsMutationInterval;
                    Params[j] = param;
                }
                n.SetFunctionAndParams(n.TransferFunction, Params);
                used.Add(i);
            }
        }

        private static void MutateChangeNeuronsFunctions(Neuron[] neurons)
        {
            int neuronsSize = neurons.Length;
            int itersCount = random.Next(neuronsSize);
            if (itersCount == 0) itersCount = 1;

            var used = new HashSet<int>();
            for (int iter = 0; iter < itersCount; iter++)
            {
                int i = random.Next(neuronsSize);
                if (neuronsSize > 1)
                    while (used.Contains(i)) i = random.Next(neuronsSize);
                    
                var n = neurons[i];
                var f = ThresholdFunction.Random();
                n.SetFunctionAndParams(f, ThresholdFunction.GetDefaultParams(f));
                used.Add(i);
            }
        }

        private static void ShuffleWeightsOnSubinterval(double[] weights)
        {
            int left = random.Next(weights.Length);
            int right = random.Next(weights.Length);
            if (left > right)
            {
                int tmp = right;
                right = left;
                left = tmp;
            }
            var subListOfWeights = new double[(right - left) + 1];
            for (int i = 0; i < ((right - left) + 1); i++)
            {
                subListOfWeights[i] = weights[left + i];
            }
            subListOfWeights = subListOfWeights.OrderBy(w => random.Next()).ToArray();
            for (int i = 0; i < ((right - left) + 1); i++)
                weights[left + i] = subListOfWeights[i];
        }

        public override NeuralNetwork Clone()
        {
            var clone = new OptimizableNeuralNetwork(Neurons.Length);
            CopyParameters(clone);
            return clone;
        }
    }
}
