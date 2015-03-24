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

        /**
         * Build's the neural net with one input layer and nbHiddenLayers hidden layers
         **/
        public NeuralNet(int nbNeuronsPerLayer, int nbHiddenLayers)
        {
            this.nbNeuronsPerLayer = nbNeuronsPerLayer;
            this.nbHiddenLayers = nbHiddenLayers;

            // Build input layer
            this.inputLayer = NeuronLayer.buildNewLayer(nbNeuronsPerLayer, Program.NB_COLS);

            // Build hidden layers
            this.hiddenLayers = new NeuronLayer[nbHiddenLayers];

            for (int i = 0; i < nbHiddenLayers; i++)
            {
                this.hiddenLayers[i] = NeuronLayer.buildNewLayer(nbNeuronsPerLayer, nbNeuronsPerLayer);
            }
        }

        private NeuralNet() { } // private constructor for cloning

        /**
         *  Copy's the neural net and returns a new one with genetic mutations
         **/
        public static NeuralNet getGeneticClone(NeuralNet original)
        {
            NeuralNet clone = new NeuralNet();

            clone.nbNeuronsPerLayer = original.nbNeuronsPerLayer;
            clone.nbHiddenLayers = original.nbHiddenLayers;

            clone.inputLayer = NeuronLayer.copyLayer(original.nbNeuronsPerLayer, Program.NB_COLS, original.inputLayer);


            clone.hiddenLayers = new NeuronLayer[original.nbHiddenLayers];

            for (int i = 0; i < original.nbHiddenLayers; i++)
            {
                clone.hiddenLayers[i] = NeuronLayer.copyLayer(original.nbNeuronsPerLayer, original.nbNeuronsPerLayer, original.hiddenLayers[i]);
            }

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

                int prediction = getPrediction();

                this.outputError += _player.rank == prediction ? 0 : 1;
            }

            this.outputError /= players.Count;
        }

        /**
         * Calculates the prediction with the output of the last layer by doing the statistical mode of the layer's neurons outputs.
         **/
        private int getPrediction()
        {
            int[] vals = new int[Model.Player.NB_RANKS];
            foreach (double _output in this.hiddenLayers[nbHiddenLayers - 1].Output)
            {
                vals[(int)Math.Round(_output * (Model.Player.NB_RANKS - 1) / Params.OUTPUT_MULTIPLIER, 0)]++;
            }

            int prediction = 0;
            for (int j = 0; j < Model.Player.NB_RANKS; j++)
            {
                if (vals[j] > vals[prediction]) prediction = j;
            }

            return prediction + 1;
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
