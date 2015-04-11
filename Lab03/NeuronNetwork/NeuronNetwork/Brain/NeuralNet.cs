using System;
using System.Collections.Generic;

namespace NeuronNetwork.Brain
{
    /// <summary>
    /// Neural net formed of an input neuron layer, hidden neuron layers and an output neuron layer.
    /// </summary>
    class NeuralNet
    {
        private int nbNeuronsPerLayer;
        private int nbHiddenLayers;

        private NeuronLayer inputLayer;
        private NeuronLayer[] hiddenLayers;
        private NeuronLayer outputLayer;

        // average output error
        private double outputErrorAbs; // calculated +1 per wrong output and +0 per right output
        private double outputErrorRel; // calculated with distance with the right answer (e.g. if input = 3 and output = 5 then error = 2)

        // Buffer to store sum of errors used for averages
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

            // Build output layer
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
            // Reset error attributes
            this.OutputErrorAbs = 0;
            this.outputErrorSumAbs = 0; 
            this.OutputErrorRel = 0;
            this.outputErrorSumRel = 0;

            // Predict rank for each players and add up errors together
            foreach (Model.Player _player in players)
            {
                int prediction = predictRank(_player);

                this.outputErrorSumAbs += _player.rank == prediction ? 0 : 1;

                this.outputErrorSumRel += Math.Abs(_player.rank - prediction);
            }

            // Compute error averages
            this.OutputErrorAbs = (double)this.outputErrorSumAbs / (players.Count);
            this.OutputErrorRel = (double)this.outputErrorSumRel / (players.Count * 8);
        }

        // Returns a player's rank by processing the player's other attribute through the neural net
        public int predictRank(Model.Player player)
        {
            // Process player values through input layer
            inputLayer.process(player.myValues);

            // Then feed input layer's output through first hidden layer
            this.hiddenLayers[0].process(inputLayer.Output);

            // Then feed each hidden layers output into the next ones input
            for (int j = 1; j < this.nbHiddenLayers; j++)
            {
                this.hiddenLayers[j].process(this.hiddenLayers[j - 1].Output);
            }

            // Then last hidden layer's output becomes output layer's input
            this.outputLayer.process(this.hiddenLayers[this.nbHiddenLayers - 1].Output);

            // Return "denormalized" prediction
            return (int) Math.Round(this.outputLayer.Output[0] * (Model.Player.NB_RANKS - 1) / Params.OUTPUT_MULTIPLIER) + 1;
        }

        // Getters / Setters ...

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
