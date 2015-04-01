using System;
using System.Linq;
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
        private NeuronLayer outputLayer;

        private double outputErrorAbs;
        private double outputErrorRel;
        private int outputErrorSumAbs;
        private int outputErrorSumRel;

        /**
         * Build's the neural net with one input layer and nbHiddenLayers hidden layers
         **/
        public NeuralNet(int nbNeuronsPerLayer, int nbHiddenLayers)
        {
            this.nbNeuronsPerLayer = nbNeuronsPerLayer;
            this.nbHiddenLayers = nbHiddenLayers;

            // Build input layer
            this.inputLayer = NeuronLayer.buildNewLayer(nbNeuronsPerLayer, Program.Params.NB_INPUT_COLUMNS);

            // Build hidden layers
            this.hiddenLayers = new NeuronLayer[nbHiddenLayers];

            for (int i = 0; i < nbHiddenLayers; i++)
            {
                this.hiddenLayers[i] = NeuronLayer.buildNewLayer(nbNeuronsPerLayer, nbNeuronsPerLayer);
            }

            this.outputLayer = NeuronLayer.buildNewLayer(1, nbNeuronsPerLayer);
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

            clone.inputLayer = NeuronLayer.copyLayer(original.nbNeuronsPerLayer, Program.Params.NB_INPUT_COLUMNS, original.inputLayer);

            clone.hiddenLayers = new NeuronLayer[original.nbHiddenLayers];

            for (int i = 0; i < original.nbHiddenLayers; i++)
            {
                clone.hiddenLayers[i] = NeuronLayer.copyLayer(original.nbNeuronsPerLayer, original.nbNeuronsPerLayer, original.hiddenLayers[i]);
            }

            clone.outputLayer = NeuronLayer.copyLayer(1, original.nbNeuronsPerLayer, original.outputLayer);

            return clone;
        }

        /**
         * Function through which the neural net is going to calculate it's error.
         **/
        public void computeError(List<Model.Player> players)
        {
            this.OutputErrorAbs = 0;
            this.outputErrorSumAbs = 0; 
            this.OutputErrorRel = 0;
            this.outputErrorSumRel = 0;

            foreach (Model.Player _player in players)
            {
                int prediction = predictRank(_player);

                this.outputErrorSumAbs += _player.rank == prediction ? 0 : 1;
                this.outputErrorSumRel += Math.Abs(_player.rank - prediction);
            }

            this.OutputErrorAbs = (double)this.outputErrorSumAbs / (players.Count);
            this.OutputErrorRel = (double)this.outputErrorSumRel / (players.Count * 8);

        }

        public int predictRank(Model.Player player)
        {
            inputLayer.process(player.myValues);

            this.hiddenLayers[0].process(inputLayer.Output);

            for (int j = 1; j < this.nbHiddenLayers; j++)
            {
                this.hiddenLayers[j].process(this.hiddenLayers[j - 1].Output);
            }

            this.outputLayer.process(this.hiddenLayers[this.nbHiddenLayers - 1].Output);

            return (int) Math.Round(this.outputLayer.Output[0] * (Model.Player.NB_RANKS - 1) / Params.OUTPUT_MULTIPLIER) + 1;
        }

        public double OutputErrorRel
        {
            get
            {
                return this.outputErrorRel;
            }
            set
            {
                this.outputErrorRel = value;
            }
        }

        public double OutputErrorAbs
        {
            get
            {
                return this.outputErrorAbs;
            }
            set
            {
                this.outputErrorAbs = value;
            }
        }
    }
}
