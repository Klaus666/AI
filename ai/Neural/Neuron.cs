<<<<<<< HEAD
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural
{
    public class Neuron : ICloneable
    {
        private double InputSignal;
        private double AfterActivationSignal;
        private ThresholdFunction.Function ThresholdFunction;
        private double[] Params;

        public Neuron(ThresholdFunction.Function Function, double[] Params)
        {
            SetFunctionAndParams(Function, Params);
        }

        public void SetFunctionAndParams(ThresholdFunction.Function Function, double[] Params)
        {
            if (Params.Length != Neural.ThresholdFunction.GetDefaultParams(ThresholdFunction).Length)
            {
                throw new ArgumentException("Function needs " + Neural.ThresholdFunction.GetDefaultParams(ThresholdFunction).Length
                        + " parameters. But params count is " + Params.Length);
            }

            ThresholdFunction = Function;
            this.Params = Params;
        }

        public void AddSignal(double value)
        {
            InputSignal += value;
        }

        public void Activate()
        {
            AfterActivationSignal = Neural.ThresholdFunction.Calculate(InputSignal, ThresholdFunction, Params);
            InputSignal = 0;
        }

        public double GetAfterActivationSignal()
        {
            return AfterActivationSignal;
        }

        public ThresholdFunction.Function GetFunction()
        {
            return ThresholdFunction;
        }

        public double[] GetParams()
        {
            var ret = new double[Params.Length];

            for (int i = 0; i < Params.Length; i++) ret[i] = Params[i];
            return ret;
        }

        public object Clone()
        {
            double[] cloneParams = new double[Params.Length];
            for (int i = 0; i < Params.Length; i++) cloneParams[i] = Params[i];

            var clone = new Neuron(ThresholdFunction, cloneParams);
            clone.InputSignal = 0;
            clone.AfterActivationSignal = 0;
            return clone;
        }
    }
}
=======
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural
{
    public class Neuron : ICloneable
    {
        private double InputSignal;
        private double AfterActivationSignal;
        private ThresholdFunction.Function ThresholdFunction;
        private double[] Params;

        public Neuron(ThresholdFunction.Function Function, double[] Params)
        {
            SetFunctionAndParams(Function, Params);
        }

        public void SetFunctionAndParams(ThresholdFunction.Function Function, double[] Params)
        {
            if (Params.Length != Neural.ThresholdFunction.GetDefaultParams(ThresholdFunction).Length)
            {
                throw new ArgumentException("Function needs " + Neural.ThresholdFunction.GetDefaultParams(ThresholdFunction).Length
                        + " parameters. But params count is " + Params.Length);
            }

            ThresholdFunction = Function;
            this.Params = Params;
        }

        public void AddSignal(double value)
        {
            InputSignal += value;
        }

        public void Activate()
        {
            AfterActivationSignal = Neural.ThresholdFunction.Calculate(InputSignal, ThresholdFunction, Params);
            InputSignal = 0;
        }

        public double GetAfterActivationSignal()
        {
            return AfterActivationSignal;
        }

        public ThresholdFunction.Function GetFunction()
        {
            return ThresholdFunction;
        }

        public double[] GetParams()
        {
            var ret = new double[Params.Length];

            for (int i = 0; i < Params.Length; i++) ret[i] = Params[i];
            return ret;
        }
        
        object ICloneable.Clone()
        {
            double[] cloneParams = new double[Params.Length];
            for (int i = 0; i < Params.Length; i++) cloneParams[i] = Params[i];

            var clone = new Neuron(ThresholdFunction, cloneParams);
            clone.InputSignal = 0;
            clone.AfterActivationSignal = 0;
            return clone;
        }
    }
}
>>>>>>> refs/remotes/origin/master
