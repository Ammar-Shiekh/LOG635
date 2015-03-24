using System;

namespace NeuronNetwork.Brain
{
    class NeuronLayer
    {
        private double[] output;

        private double[][] inputWeights;
        private int nbNeurons;

        // Pass through static factory methods
        private NeuronLayer(int nbNeurons, double[][] inputWeights) 
        {
            this.nbNeurons = nbNeurons;
            this.inputWeights = inputWeights;
            this.output = new double[nbNeurons];
        }

        // Factory method
        public static NeuronLayer buildNewLayer(int nbNeurons, int nbInput)
        {
            double[][] weights = new double[nbNeurons][];
            Random weightRandomizer = new Random();

            for (int i = 0; i < nbNeurons; i++)
            {
                weights[i] = new double[nbInput + 1]; // +1 for the bias
                for (int j = 0; j < nbInput + 1; j++)
                {
                    weights[i][j] = Params.getRandomWeight(weightRandomizer);
                }
            }

            return new NeuronLayer(nbNeurons, weights);
        }

        // Factory method
        public static NeuronLayer copyLayer(int nbNeurons, int nbInputs, NeuronLayer original)
        {
            Random geneticRandomizer = new Random();

            double[][] weights = new double[nbNeurons][];
            Random weightRandomizer = new Random();

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

        // Calculates the output of each neurons
        public void process(double[] input)
        {

            for (int i = 0; i < nbNeurons; i++)
            {
                output[i] = 0;
                for (int j = 0; j < input.Length; j++)
                {
                    output[i] += input[j] * this.inputWeights[i][j];
                }

                output[i] += this.inputWeights[i][this.inputWeights[i].Length - 1] * Params.BIAS_VALUE;
                output[i] = Util.Sigmoid.sigmoid(output[i]) * Params.OUTPUT_MULTIPLIER;
            }
        }

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
