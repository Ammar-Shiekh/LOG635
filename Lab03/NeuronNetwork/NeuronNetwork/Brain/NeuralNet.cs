using System;
using System.Collections.Generic;
using System.Threading;

namespace NeuronNetwork.Brain
{
    class NeuralNet
    {
        private int nbNeuronsPerLayer;
        private int nbHiddenLayers;

        private NeuronLayer inputLayer;
        private NeuronLayer[] hiddenLayers;

        private double outputError;

        public NeuralNet(int nbNeuronsPerLayer, int nbHiddenLayers)
        {
            this.nbNeuronsPerLayer = nbNeuronsPerLayer;
            this.nbHiddenLayers = nbHiddenLayers;

            // Input stuff
            double[][] inputWeights = new double[nbNeuronsPerLayer][];
            Random inputWeightRandomizer = new Random();

            for (int i = 0; i < nbNeuronsPerLayer; i++)
            {
                inputWeights[i] = new double[Program.NB_COLS + 1]; // +1 for the bias
                for (int j = 0; j < Program.NB_COLS + 1; j++)
                {
                    inputWeights[i][j] = inputWeightRandomizer.NextDouble() * inputWeightRandomizer.Next(-1, 2);
                }
            }

            this.inputLayer = new NeuronLayer(nbNeuronsPerLayer, inputWeights);

            // Hidden layers stuff
            this.hiddenLayers = new NeuronLayer[nbHiddenLayers];

            DateTime d = DateTime.Now;
            Thread[] threads = new Thread[nbHiddenLayers];

            for (int i = 0; i < nbHiddenLayers; i++)
            {
                Random hiddenWeightsRandomizer = new Random();
                double[][] hiddenWeights = new double[nbNeuronsPerLayer][];
                for (int j = 0; j < nbNeuronsPerLayer; j++)
                {
                    hiddenWeights[j] = new double[nbNeuronsPerLayer + 1]; // +1 for the bias
                    for (int k = 0; k < nbNeuronsPerLayer + 1; k++)
                    {
                        hiddenWeights[j][k] = hiddenWeightsRandomizer.NextDouble() * hiddenWeightsRandomizer.Next(-1, 2);
                    }
                }

                this.hiddenLayers[i] = new NeuronLayer(nbNeuronsPerLayer, hiddenWeights);
            }
        }

        private NeuralNet() { } // private constructor for cloning

        public static NeuralNet getGeneticClone(NeuralNet original, int geneticFrequency)
        {
            NeuralNet clone = new NeuralNet();

            clone.nbNeuronsPerLayer = original.nbNeuronsPerLayer;
            clone.nbHiddenLayers = original.nbHiddenLayers;

            Random geneticRandomizer = new Random();

            // Input layer stuff
            double[][] inputWeights = new double[original.nbNeuronsPerLayer][];
            Random inputWeightRandomizer = new Random();

            int nbMutations = 0;
            int nbNoMutation = 0;

            for (int i = 0; i < original.nbNeuronsPerLayer; i++)
            {
                inputWeights[i] = new double[Program.NB_COLS + 1]; // +1 for the bias
                
                for (int j = 0; j < Program.NB_COLS + 1; j++)
                {
                    if (geneticRandomizer.Next(0, geneticFrequency) % geneticFrequency == 0) // Mutation
                    {
                        inputWeights[i][j] = inputWeightRandomizer.NextDouble() * inputWeightRandomizer.Next(-1, 2);
                    }
                    else // no mutation
                    {
                        inputWeights[i][j] = original.inputLayer.InputWeights[i][j];
                    }
                }
            }

            clone.inputLayer = new NeuronLayer(original.nbNeuronsPerLayer, inputWeights);

            // Hidden layers stuff
            clone.hiddenLayers = new NeuronLayer[original.nbHiddenLayers];

            for (int i = 0; i < original.nbHiddenLayers; i++)
            {
                Random hiddenWeightsRandomizer = new Random();
                double[][] hiddenWeights = new double[original.nbNeuronsPerLayer][];
                for (int j = 0; j < original.nbNeuronsPerLayer; j++)
                {
                    hiddenWeights[j] = new double[original.nbNeuronsPerLayer + 1]; // +1 for the bias

                    for (int k = 0; k < original.nbNeuronsPerLayer + 1; k++)
                    {

                        if (geneticRandomizer.Next(0, geneticFrequency) % geneticFrequency == 0) // Mutation
                        {
                            hiddenWeights[j][k] = hiddenWeightsRandomizer.NextDouble() * hiddenWeightsRandomizer.Next(-1, 2);
                            nbMutations++;
                        }
                        else // no mutation
                        {
                            hiddenWeights[j][k] = original.hiddenLayers[i].InputWeights[j][k];
                            nbNoMutation++;
                        }
                    }
                }

                clone.hiddenLayers[i] = new NeuronLayer(original.nbNeuronsPerLayer, hiddenWeights);
            }

            Console.WriteLine("Weights | Modified : " + nbMutations + " | Copied : " + nbNoMutation); 

            return clone;
        }

        /**
         * Function through which the neural net is going to calculate it's error.
         **/
        public void process(List<Model.Player> players)
        {
            foreach (Model.Player _player in players)
            {
                inputLayer.process(_player.myValues);

                this.hiddenLayers[0].process(inputLayer.Output);

                for (int j = 1; j < this.nbHiddenLayers; j++)
                {
                    this.hiddenLayers[j].process(this.hiddenLayers[j - 1].Output);
                }

                this.outputError += getErrorMode(_player.rank);
            }

            this.outputError /= players.Count;
        }

        private double getErrorMode(int rank)
        {
            int[] vals = new int[8];
            foreach (double _output in this.hiddenLayers[nbHiddenLayers - 1].Output)
            {
                vals[(int)Math.Round(_output * 7, 0)]++;
            }

            int prediction = 0;

            for (int j = 0; j < 8; j++)
            {
                if (vals[j] > vals[prediction]) prediction = j;
            }

            prediction += 1;

            return rank == prediction? 0 : 1;
        }

        public double OutputError
        {
            get
            {
                return this.outputError;
            }
            set
            {
                this.outputError = value;
            }
        }
    }
}
