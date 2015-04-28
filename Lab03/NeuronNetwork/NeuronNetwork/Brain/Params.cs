using System;

namespace NeuronNetwork.Brain
{
    /// <summary>
    /// Provides parameters that affect neural nets and neuron layers computations.
    /// </summary>
    class Params
    {
        public const int NB_NEURON_PER_LAYER = 6;   // Nb of neuron per layers (except for output layer. Output layer always has only 1 neuron)

        public const int NB_HIDDEN_LAYERS = 2;      // Nb of hidden layers per neural net

        public const int OUTPUT_MULTIPLIER = 10;    // This number is multiplied with the output of each neuron to counter the lowering output variation caused by
                                                    // the number of layers. This effect is decribed with more details in the NeuronLayer.process method.

        public const int MUTATION_FREQUENCY = 100;  // This number is used to control the number of mutations between a neural layer and it's clone. The higher
                                                    // this number is, the less mutations there will be.
        
        public const double BIAS_VALUE = -1;        // Value of the bias on each neuron.

        /// <summary>
        /// Determinates the way random weights are generated.
        /// </summary>
        public static double getRandomWeight(Random randomizer)
        {
            // return (randomizer.Next(-1, 2)); // Returns -1 (1/3rd of the time), 0 (1/3rd of the time) or 1 (1/3rd of the time)

            return randomizer.NextDouble() * (randomizer.Next(-1, 2)); // Returns a double between -1 and 0 (1/3rd of the time), 0 (1/3rd of the time) or a double between 0 and 1 (1/3rd of the time)
            
            //return randomizer.NextDouble() * (randomizer.Next(0, 2) % 2 == 0 ? -1 : 1); // Returns a double between -1 and 0 (half of the time) or a double between 0 and 1 (half of the time)
        }
    }
}
