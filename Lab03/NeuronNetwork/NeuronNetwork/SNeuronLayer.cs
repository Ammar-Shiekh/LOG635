using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    class SNeuronLayer
    {
        //le nombre de neuron dans cette couche
        public int m_NumNeurons;

        //les neurons
        public List<SNeuron> m_vecNeurons;

        public SNeuronLayer(int nbrNeurons, int nbrInputsPerNeuron)
        {
            for (int i = 0; i < nbrNeurons; i++)
            {
                this.m_vecNeurons.Add(new SNeuron(nbrInputsPerNeuron));
            }
        }

    }
}
