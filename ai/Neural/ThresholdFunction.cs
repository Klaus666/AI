using System;
using System.Linq;

namespace Neural
{
    public static class ThresholdFunction
    {
        private static readonly Random random = new Random();
        private static readonly Function[] all = Enum.GetValues(typeof(Function)).Cast<Function>().ToArray();

        public enum Function : int
        {
            Linear = 0,
            Sign = 1,
            Sigma = 2
        }

        public static double[] GetDefaultParams(Function F)
        {
            switch (F)
            {
                case Function.Linear: return new double[] { 1, 0 };
                case Function.Sign: return new double[] { 0.0 };
                case Function.Sigma: return new double[] { 1.0, 1.0, 1.0 };
                default: throw new ArgumentException();
            }
        }

        public static double Calculate(double argument, Function F, params double[] Params)
        {
            switch (F)
            {
                case Function.Linear: return Params[0] * argument + Params[1];
                case Function.Sign: return argument > Params[0] ? 1.0 : 0.0;
                case Function.Sigma: return Params[0] / (Params[1] + Math.Exp(-argument * Params[2]));
                default: throw new ArgumentException();
            }
        }

        public static Function Random()
        {
            return all[random.Next(all.Length)];
        }
    }
}
