using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork.Brain
{
    class NeuralNet
    {

        private int nbNeuronsPerLayer;
        private int nbHiddenLayers;

        private NeuronLayer inputLayer;
        private NeuronLayer[] hiddenLayers;

        private float outputError;

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

                int[] vals = new int[8];
                foreach (double _output in this.inputLayer.Output)
                {
                    vals[(int) Math.Round(_output * 7, 0)]++;
                }

                int prediction = 0;

                for (int j = 0; j < 8; j++)
                {
                    if (vals[j] > vals[prediction]) prediction = j;
                }

                Console.WriteLine();

            }
        }
    }
}
