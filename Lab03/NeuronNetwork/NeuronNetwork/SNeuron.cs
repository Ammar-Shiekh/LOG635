using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    class SNeuron
    {

        //Nombre d'entrée dans le neuron
        public int m_NumInputs;

        //Le poids pour chaque entrée
        public List<double> m_vectWeight;

        //Contructeur d'un neuron
        public SNeuron(int nbrInput)
        {

            for (int i = 0; i < nbrInput + 1; i++)
            {
                Random random = new Random();
                //On donne un poids aléatoire entre -1 et 1
                m_vectWeight.Add(random.NextDouble() * 2 - 1);
            }
        }
    }
}
