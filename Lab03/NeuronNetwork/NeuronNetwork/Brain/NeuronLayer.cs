
namespace NeuronNetwork.Brain
{
    class NeuronLayer
    {

        private double[] input;
        private double[] output;

        private double[][] inputWeights;
        private int nbNeurons;

        public NeuronLayer(int nbNeurons, double[][] inputWeights) 
        {
            this.nbNeurons = nbNeurons;
            this.inputWeights = inputWeights;
            this.output = new double[nbNeurons];
        }

        public void process(double[] input)
        {

            for (int i = 0; i < nbNeurons; i++)
            {
                output[i] = 0;
                for (int j = 0; j < input.Length; j++)
                {
                    output[i] += input[j] * this.inputWeights[i][j];
                }

                output[i] += this.inputWeights[i][this.inputWeights[i].Length -1] *  nbNeurons;
                output[i] = Util.Sigmoid.sigmoid(output[i]);
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
