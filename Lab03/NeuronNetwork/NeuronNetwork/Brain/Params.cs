using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork.Brain
{
    class Params
    {
        public const int NB_NEURON_PER_LAYER = 10;

        public const int NB_HIDDEN_LAYERS = 2;

        public const int OUTPUT_MULTIPLIER = 1;

        public const int MUTATION_FREQUENCY = 1;

        public const double BIAS_VALUE = -1;

        public const double ERROR_TRESHOLD = 0.2;

        public static double getRandomWeight(Random randomizer)
        {
            return randomizer.NextDouble() * (randomizer.Next(0, 2) % 2 == 0 ? -1 : 1); // Returns a double between -1 et 1
        }
    }
}
