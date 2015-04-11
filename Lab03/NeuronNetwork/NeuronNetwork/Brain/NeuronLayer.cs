using System;

namespace NeuronNetwork.Brain
{
    /// <summary>
    /// Represents a neuron layer composed of weights for each neurons' input. Each neuron has different weights on each of it's input.
    /// </summary>
    class NeuronLayer
    {
        private static int someInt = 1; // For better random

        private double[] output; // One output for each neuron

        private double[][] inputWeights; // Array of weights for each input of each neuron
        
        private int nbNeurons; // Nb neurons in this layer

        // Use static factory methods below
        private NeuronLayer(int nbNeurons, double[][] inputWeights) 
        {
            this.nbNeurons = nbNeurons;
            this.inputWeights = inputWeights;
            this.output = new double[nbNeurons];
        }

        /// <summary>
        /// Factory method : Builds a new layer with completely random weights
        /// </summary>
        public static NeuronLayer buildNewLayer(int nbNeurons, int nbInput)
        {
            double[][] weights = new double[nbNeurons][];

            Random weightRandomizer = new Random((++someInt + Environment.TickCount) * 2);

            // For each neuron
            for (int i = 0; i < nbNeurons; i++)
            {
                weights[i] = new double[nbInput + 1]; // +1 for the bias

                // Generate a random weight for each of its input (and the bias)
                for (int j = 0; j < nbInput + 1; j++)
                {
                    weights[i][j] = Params.getRandomWeight(weightRandomizer);
                }
            }

            return new NeuronLayer(nbNeurons, weights);
        }

        /// <summary>
        ///  Factory method : Creates a genetic clone of a layer with mutations
        /// </summary>
        public static NeuronLayer copyLayer(int nbNeurons, int nbInputs, NeuronLayer original)
        {
            double[][] weights = new double[nbNeurons][];
            Random geneticRandomizer = new Random((++someInt + Environment.TickCount) * 2);
            Random weightRandomizer = new Random((++someInt + Environment.TickCount) * 2);

            for (int i = 0; i < nbNeurons; i++)
            {
                weights[i] = new double[nbInputs + 1]; // +1 for the bias

                for (int j = 0; j < nbInputs + 1; j++)
                {
                    if (geneticRandomizer.Next(0, Params.MUTATION_FREQUENCY) % Params.MUTATION_FREQUENCY == 0) // Mutation
                    {
                        weights[i][j] = Params.getRandomWeight(weightRandomizer);
                    }
                    else // no mutation
                    {
                        weights[i][j] = original.InputWeights[i][j];
                    }
                }
            }

            return new NeuronLayer(nbNeurons, weights);
        }

        /// <summary>
        /// Computes the output of each neurons
        /// </summary>
        public void process(double[] input)
        {
            // For each neuron
            for (int i = 0; i < nbNeurons; i++)
            {
                // Perceptron equation implemented here (sum of each [weight * input]) 
                output[i] = 0;
                for (int j = 0; j < input.Length; j++)
                {
                    output[i] += input[j] * this.inputWeights[i][j];
                }

                // Add the bias
                output[i] += this.inputWeights[i][this.inputWeights[i].Length - 1] * Params.BIAS_VALUE;

                // Activation function
                output[i] = Util.Sigmoid.sigmoid(output[i]) 
                    * Params.OUTPUT_MULTIPLIER; // When multiple layers are used in this neural net, the output of each layer is multiplied by a number between -1 and 1. This
                                                // makes the output VARIATION 1/10th smaller after every layer. OUTPUT_MULTIPLIER counter's (partly) this effect when set correctly.
            }
        }

        // Getters / Setters

        public double[][] InputWeights
        {
            get
            {
                return this.inputWeights;
            }

            set { }
        }

        public double[] Output
        {
            get
            {
                return this.output;
            }

            set { }
        }
    }
}
