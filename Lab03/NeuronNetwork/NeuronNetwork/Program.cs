using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork
{
    class Program
    {

        public const int NB_COLS = 19;

        static void Main(string[] args)
        {

            List<Model.Player> players = new List<Model.Player>();

            String line;
            String[] splitLine;
            char[] comma = {','};
            char[] doubleQuotes = {'"'};
            System.IO.StreamReader file = new System.IO.StreamReader("Donnees_sources.csv");
            file.ReadLine(); // Skip first line
            while ((line = file.ReadLine()) != null)
            {
                splitLine = line.Split(comma);
                players.Add(new Model.Player(new double[] {
                                            double.Parse(splitLine[0]),
                                            // double.Parse(splitLine[1]),
                                            splitLine[2].Equals("\"?\"") ? -1 : double.Parse(splitLine[2]),
                                            splitLine[3].Equals("\"?\"") ? -1 : double.Parse(splitLine[3]),
                                            splitLine[4].Equals("\"?\"") ? -1 : double.Parse(splitLine[4]),
                                            double.Parse(splitLine[5]),
                                            double.Parse(splitLine[6].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[7].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[8]),
                                            double.Parse(splitLine[9].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[10].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[11]),
                                            double.Parse(splitLine[12]),
                                            double.Parse(splitLine[13]),
                                            double.Parse(splitLine[14]),
                                            double.Parse(splitLine[15]),
                                            double.Parse(splitLine[16].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[17]),
                                            double.Parse(splitLine[18].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[19].Trim(doubleQuotes), System.Globalization.NumberStyles.Float)}, int.Parse(splitLine[1])));
            }

            file.Close();

            // Randomize list
            Random randomizer = new Random();
            players.Sort(Comparer<Model.Player>.Create((p1, p2) =>  randomizer.Next(-1, 2)));

            // do

            int nbNeurons = 1000;
            int nbHiddenLayers = 10;
            Brain.NeuralNet previousNet = new Brain.NeuralNet(nbNeurons, nbHiddenLayers);
            Brain.NeuralNet newNet;

            foreach (Model.Player _player in players)
            {
                _player.normalize(players.Count);
            }


            // Split list here

            previousNet.process(players);

            int nbMutations = 0;
            int nbGoodMutations = 0;
            while (previousNet.OutputError > 0.5)
            {
                Console.WriteLine(previousNet.OutputError);

                newNet = Brain.NeuralNet.getGeneticClone(previousNet, 3);
                nbMutations++;

                newNet.process(players);

                if (previousNet.OutputError > newNet.OutputError)
                {
                    previousNet = newNet;
                    nbGoodMutations++;
                }
            }

            Console.WriteLine();

                // create neuralNet

                
                // For each sample line

                    // Set inputs of input layer

                    // Execute input layer

                    // Set input of first hidden layer

                    // For each hidden layers

                        // Execute hidden layer
                
                        // Set input of next layer with output of layer

                    // End for

                    // With last hidden layer, calculate rank with outputs

                    // Calculate error and sum it up

                // End for

                // Calculate average error with sum

                // If error is smaller than previous settings
                    
                    // apply genetic to this

                // else
                    
                    // Apply genetic to previous
                
                // end if

            // while average error is higher than Z

            Console.ReadLine();
        }

    } // Program
} // Namespace
