using System;

namespace Neural
{
    public class NeuralNetwork
    {
        protected Neuron[] Neurons;
        protected Links NeuronsLinks = new Links();
        public int ActivationIterations { get; set; } = 1;

        public NeuralNetwork() { }

        public NeuralNetwork(int numberOfNeurons)
        {
            Neurons = new Neuron[numberOfNeurons];
            for (int i = 0; i < numberOfNeurons; i++)
                Neurons[i] = new Neuron(ThresholdFunction.Function.Sign, ThresholdFunction.GetDefaultParams(ThresholdFunction.Function.Sign));
        }

        public void SetNeuronFunction(int neuronNumber, ThresholdFunction.Function function, double[] Params)
        {
            if (neuronNumber >= Neurons.Length)
                throw new Exception("Neural network has " + Neurons.Length
                        + " neurons. But there was trying to accsess neuron with index " + neuronNumber);

            Neurons[neuronNumber].SetFunctionAndParams(function, Params);
        }

        public void AddLink(int activatorNeuronNumber, int receiverNeuronNumber, double weight) => NeuronsLinks[activatorNeuronNumber, receiverNeuronNumber] = weight;

        public void PutSignalToNeuron(int neuronIndx, double signalValue)
        {
            if (neuronIndx < Neurons.Length)
                Neurons[neuronIndx].AddSignal(signalValue);
            else
                throw new ArgumentException();
        }

        public double GetAfterActivationSignal(int neuronIndx)
        {
            if (neuronIndx < Neurons.Length)
                return Neurons[neuronIndx].AfterActivationSignal;
            else
                throw new ArgumentException();
        }

        public void Activate()
        {
            for (int iter = 0; iter < ActivationIterations; iter++)
                for (int i = 0; i < Neurons.Length; i++)
                {
                    var activator = Neurons[i];
                    activator.Activate();
                    double activatorSignal = activator.AfterActivationSignal;

                    foreach (var receiverNum in NeuronsLinks.GetReceivers(i))
                    {
                        if (receiverNum >= Neurons.Length)
                            throw new Exception("Neural network has " + Neurons.Length
                                    + " neurons. But there was trying to accsess neuron with index " + receiverNum);

                        var receiver = Neurons[receiverNum];
                        double weight = NeuronsLinks[i, receiverNum];
                        receiver.AddSignal(activatorSignal * weight);
                    }
                }
        }

        public double[] GetWeightsOfLinks() => NeuronsLinks.GetAllWeights();

        public void SetWeightsOfLinks(double[] weights) => NeuronsLinks.SetAllWeights(weights);

        public Neuron[] CloneNeurons()
        {
            var ret = new Neuron[Neurons.Length];
            for (int n = 0; n < Neurons.Length; n++)
                ret[n] = Neurons[n].Clone();

            return ret;
        }

        public int NeuronsCount => Neurons.Length;

        public Links GetNeuronsLinks() => NeuronsLinks.Clone();

        public virtual NeuralNetwork Clone()
        {
            var clone = new NeuralNetwork(Neurons.Length);
            CopyParameters(clone);
            return clone;
        }

        protected void CopyParameters(NeuralNetwork clone)
        {
            clone.NeuronsLinks = NeuronsLinks.Clone();
            clone.ActivationIterations = ActivationIterations;

            clone.Neurons = new Neuron[Neurons.Length];
            for (int n = 0; n < Neurons.Length; n++)
                clone.Neurons[n] = Neurons[n].Clone();
        }
    }
}

