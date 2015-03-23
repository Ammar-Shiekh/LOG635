using System;

namespace NeuronNetwork.Util
{
    class Sigmoid
    {

        public static double sigmoid(double activation)
        {
            return (1 / (1 + Math.Exp(-activation)));
        }
    }
}
