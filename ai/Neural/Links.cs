using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neural
{
    public class Links : ICloneable
    {
        private Dictionary<int, Dictionary<int, double>> links = new Dictionary<int, Dictionary<int, double>();
        private int totalLinksCount = 0;

        public IEnumerable<int> GetReceivers(int activatorNeuronNumber)
        {
            if (links.ContainsKey(activatorNeuronNumber))
                return links[activatorNeuronNumber].Keys;
            else return new int[0];
        }

        public double GetWeight(int activatorNumber, int receiverNumber)
        {
            if (links.ContainsKey(activatorNumber))
            {
                var receiverNumToWeight = links[activatorNumber];

                if (receiverNumToWeight.ContainsKey(receiverNumber))
                    return receiverNumToWeight[receiverNumber];
                else throw new ArgumentException();
            }
            else throw new ArgumentException();
        }

        public void AddWeight(int activatorNumber, int receiverNumber, double weight)
        {
            if (!links.ContainsKey(activatorNumber))
                links[activatorNumber] = new Dictionary<int, double>();
            links[activatorNumber][receiverNumber] = weight;
            totalLinksCount++;
        }

        public double[] GetAllWeights()
        {
            var weights = new List<double>(totalLinksCount);

            foreach (var activatorIndx in links.Keys)
            {
                var receiverIndxToWeight = links[activatorIndx];

                foreach (var receiverIndx in receiverIndxToWeight.Keys)
                    weights.Add(receiverIndxToWeight[receiverIndx]);
            }

            return weights.ToArray();
        }

        public void SetAllWeights(double[] weights)
        {
            if (weights.Length != totalLinksCount)
                throw new ArgumentException();

            int ind = 0;
            foreach (var activatorIndx in links.Keys)
            {
                var receiverIndxToWeight = links[activatorIndx];

                foreach (var receiverIndx in receiverIndxToWeight.Keys)
                {
                    receiverIndxToWeight[receiverIndx] = weights[ind];
                    ind++;
                }
            }
        }

        public object Clone()
        {
            var clone = new Links();
            clone.totalLinksCount = totalLinksCount;
            clone.links = new Dictionary<int, Dictionary<int, double>>();

            foreach (var key in links.Keys)
            {
                var val = new Dictionary<int, double>();

                foreach (var valKey in links[key].Keys)
                    val[valKey] = links[key][valKey];
            }
            return clone;
        }
    }
}
