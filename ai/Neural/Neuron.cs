using System;

namespace Neural
{
    public class Neuron
    {
        private double InputSignal;
        public double AfterActivationSignal { get; private set; }
        public ThresholdFunction.Function TransferFunction { get; private set; }
        private double[] Params;

        public Neuron(ThresholdFunction.Function Function, double[] Params) { SetFunctionAndParams(Function, Params); }

        public void SetFunctionAndParams(ThresholdFunction.Function Function, double[] Params)
        {
            if (Params.Length != Neural.ThresholdFunction.GetDefaultParams(Function).Length)
            {
                throw new ArgumentException("Function needs " + Neural.ThresholdFunction.GetDefaultParams(Function).Length
                        + " parameters. But params count is " + Params.Length);
            }

            TransferFunction = Function;
            this.Params = Params;
        }

        public void AddSignal(double value) => InputSignal += value;

        public void Activate()
        {
            AfterActivationSignal = Neural.ThresholdFunction.Calculate(InputSignal, TransferFunction, Params);
            InputSignal = 0;
        }

        public double[] GetParams()
        {
            var ret = new double[Params.Length];

            for (int i = 0; i < Params.Length; i++) ret[i] = Params[i];
            return ret;
        }

        public Neuron Clone()
        {
            double[] cloneParams = new double[Params.Length];
            for (int i = 0; i < Params.Length; i++) cloneParams[i] = Params[i];

            var clone = new Neuron(TransferFunction, cloneParams);
            clone.InputSignal = 0;
            clone.AfterActivationSignal = 0;
            return clone;
        }
    }
}

