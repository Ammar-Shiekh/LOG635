using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    class CNeuralNet
    {
        private int m_NumInputs;
        private int m_NumOutputs;
        private int m_NumHiddenLayers;
        private int m_NeuronsPerHiddenLyr;

        //List de tous les couches de neurons
        private List<SNeuronLayer> m_vecLayers;

        public CNeuralNet(){}

        public void CreateNet()
        {
            //Crée la couche du réseau de neuronne
            if (this.m_NumHiddenLayers > 0)
            {
                this.m_vecLayers.Add(new SNeuronLayer(this.m_NeuronsPerHiddenLyr, this.m_NumInputs));

                for (int i = 0; i < this.m_NumHiddenLayers - 1; i++)
                {
                    //this.m_vecLayers.Add(new SNeuronLayer(this.m_NeuronsPerHiddenLyr,this.m_NeuronsPerHiddenLyr);
                }

                //Couche de sortie
                this.m_vecLayers.Add(new SNeuronLayer(this.m_NumOutputs, this.m_NeuronsPerHiddenLyr));

            }
            else
            {
                //couche de sortie
                this.m_vecLayers.Add(new SNeuronLayer(this.m_NumOutputs, this.m_NumInputs));
            }
        }


        //Retourne le poids du NeuralNet
        public List<double> GetWeights() 
        {
            List<double> weight = new List<double>();

            //Pour toutes les couches
            for (int i = 0; i < this.m_NumHiddenLayers + 1; ++i)
            {
                //Pour toutes les neurons
                for (int j = 0; j < this.m_vecLayers[i].m_NumNeurons; ++j)
                {
                    //Pour toutes les poids
                    for (int k = 0; k < this.m_vecLayers[i].m_vecNeurons[j].m_NumInputs; ++k)
                    {
                        weight.Add(this.m_vecLayers[i].m_vecNeurons[j].m_vectWeight[k]);

                    }
                }
            }
            return weight;
        }

        //Retourne le nombre total du poids dans le Net
        public int getNumberOfWeights()
        {
            int weights = 0;

            //Pour toutes les couches
            for (int i = 0; i < this.m_NumHiddenLayers + 1; ++i)
            {
                //Pour toutes les neurons
                for (int j = 0; j < this.m_vecLayers[i].m_NumNeurons; ++j)
                {
                    //Pour toutes les poids
                    for (int k = 0; k < this.m_vecLayers[i].m_vecNeurons[j].m_NumInputs; ++k)
                    {
                        weights++;

                    }
                }
            }
            return weights;
        }

        //Remplace les poids
        public void PutWeights(List<double> weights)
        {
            int cWeight = 0;

            //Pour toutes les couches
            for (int i = 0; i < this.m_NumHiddenLayers + 1; ++i)
            {
                //Pour toutes les neurons
                for (int j = 0; j < this.m_vecLayers[i].m_NumNeurons; ++j)
                {
                    //Pour toutes les poids
                    for (int k = 0; k < this.m_vecLayers[i].m_vecNeurons[j].m_NumInputs; ++k)
                    {
                        this.m_vecLayers[i].m_vecNeurons[j].m_vectWeight[k] = weights[cWeight++];

                    }
                }
            }
        }

        //Calcule la sortie à partir des entrées
        public List<double> Update(List<double> inputs)
        {
            //On enregistre la sortie de chaque couche
            List<double> outputs = null;

            int cWeight = 0;

            //On vérifie si on a le même nombre d'entrée
            if(inputs.Count() != this.m_NumInputs)
            {
                //on retourne une sortie vide avec un message d'erreur
                Console.WriteLine("Le nombre de input ne correspond pas");
                return outputs;
            }

            //Pour toutes couches
            for(int i = 0; i < this.m_NumHiddenLayers + 1; i++)
            {
                if(i > 0)
                {
                    inputs = outputs;
                }

                outputs.Clear();
                cWeight = 0;

                //Pour toutes les neurons, on multiplie le poids avec l'entré
                for(int j = 0; j < this.m_vecLayers[i].m_NumNeurons; j++)
                {
                    double netinput = 0;

                    int NumInputs = this.m_vecLayers[i].m_vecNeurons[j].m_NumInputs;

                    //Pour toutes les poids
                    for (int k = 0; k < NumInputs - 1; k++)
                    {
                        //somme des poids x les entrées
                        netinput += this.m_vecLayers[i].m_vecNeurons[k].m_vectWeight[k] * inputs[cWeight++];

                    }

                    //add in the bias
                    //netinput += this.m_vecLayers[i].m_vecNeurons[j].m_vectWeight[NumInputs - 1] * CParams::dBias;

                    //On enregistre la sortie de toutes les couches
                    //outputs.Add(Sigmoid(netinput, CParams::dActivationResponse));

                    cWeight = 0;
                }

            }
            return outputs;
        }

        //Sigmoid response curve
        public double Sigmoid(double netinput, double response)
        {
            return (1 / ( 1+ Math.Exp(-netinput / response)));
        }

    }
}
