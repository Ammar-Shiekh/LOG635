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

        private double[] outputsAvg = new double[Model.Player.NB_RANKS];
        private double outputError;

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

        public void learn(List<Model.Player> players)
        {

            List<double>[] outputs = new List<double>[Model.Player.NB_RANKS];

            for (int i = 0; i < 8; i++)
            {
                outputs[i] = new List<double>();
            }

            foreach (Model.Player _player in players)
            {
                inputLayer.process(_player.myValues);

                this.hiddenLayers[0].process(inputLayer.Output);

                for (int j = 1; j < this.nbHiddenLayers; j++)
                {
                    this.hiddenLayers[j].process(this.hiddenLayers[j - 1].Output);
                }

                for (int i = 0; i < nbNeuronsPerLayer; i++)
                {
                    outputs[_player.rank - 1].Add(this.hiddenLayers[this.nbHiddenLayers - 1].Output[i]);
                }
            }

            this.outputsAvg = new double[Model.Player.NB_RANKS];

            for (int i = 0; i < 8; i++)
            {
                this.outputsAvg[i] = outputs[i].Average();
            }
        }

        public void calcultateError(List<Model.Player> players)
        {
            foreach (Model.Player _player in players)
            {
                inputLayer.process(_player.myValues);

                this.hiddenLayers[0].process(inputLayer.Output);

                for (int j = 1; j < this.nbHiddenLayers; j++)
                {
                    this.hiddenLayers[j].process(this.hiddenLayers[j - 1].Output);
                }

                int prediction = getPrediction2();

                this.outputError += _player.rank == prediction ? 0 : 1;
            }

            this.outputError /= players.Count;

        }

        private int getPrediction2()
        {
            int[] vals = new int[Model.Player.NB_RANKS];

            foreach (double _output in this.hiddenLayers[nbHiddenLayers - 1].Output) // For each neuron in last hidden layer
            {
                double gap;
                double closestGap = 1;
                int rankOfClosestGap = 0;
                for (int i = 0; i < Model.Player.NB_RANKS; i++)
                {
                    gap = Math.Abs(outputsAvg[i] - _output);
                    if (gap < closestGap)
                    {
                        rankOfClosestGap = i;
                        closestGap = gap;
                    }
                }

                vals[rankOfClosestGap]++;
            }

            int prediction = 0;
            for (int j = 0; j < Model.Player.NB_RANKS; j++)
            {
                if (vals[j] > vals[prediction]) prediction = j;
            }

            return prediction + 1;
        }

        /**
         * Function through which the neural net is going to calculate it's error.
         **/
        public void computeError(List<Model.Player> players)
        {
            this.outputError = 0;

            foreach (Model.Player _player in players)
            {
                int prediction = predictRank(_player);
                
                this.outputError += _player.rank == prediction ? 0 : 1;
            }

            this.outputError /= players.Count;
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
