using System;

namespace NeuronNetwork.Brain
{
    class Params
    {
        public const int NB_NEURON_PER_LAYER = 6;

        public const int NB_HIDDEN_LAYERS = 5;

        public const int OUTPUT_MULTIPLIER = 10;

        public const int MUTATION_FREQUENCY = 100;

        public const double BIAS_VALUE = 1;

        public static double getRandomWeight(Random randomizer)
        {
            return randomizer.NextDouble() * (randomizer.Next(-1, 2)); // Returns a double between -1 and 0 (1/3rd of the time), 0 (1/3rd of the time) or a double between 0 and 1 (1/3rd of the time)
            // return randomizer.NextDouble() * (randomizer.Next(0, 2)) % 2 == 0 ? -1 : 1); // Returns a double between -1 and 0 (half of the time) or a double between 0 and 1 (half of the time)
        }
    }
}
