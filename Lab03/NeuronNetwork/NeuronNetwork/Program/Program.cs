using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuronNetwork.Program
{
    class Program
    {
        static void Main(string[] args)
        {

            List<Model.Player> players = new List<Model.Player>();

            // Read file and fill players list
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
                                            // double.Parse(splitLine[0]), Skip player id
                                            // double.Parse(splitLine[1]), Skip rank column for obvious reasons
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

            // Randomize players
            Random randomizer = new Random();
            players.Sort(Comparer<Model.Player>.Create((p1, p2) =>  randomizer.Next(-1, 2)));
            
            List<Model.Player> listForLearning = new List<Model.Player>();
            List<Model.Player> listForTesting = new List<Model.Player>();

            // Normalize and split players
            for (int i = 0; i < players.Count; i++)
			{
                players[i].normalize(players.Count);

                if (i < players.Count * Params.LEARNING_PERCENTAGE)
                {
                    listForLearning.Add(players[i]);
                }
                else
                {
                    listForTesting.Add(players[i]);
                }
			}

            // Params
            int nbDecimals = 1;
            int cursorPadding = nbDecimals + 6;

            System.Threading.Thread[] threads = new System.Threading.Thread[Params.NB_PARALLEL_BRAINS];
            System.Threading.Semaphore consoleAccess = new System.Threading.Semaphore(1, 1);

            Brain.NeuralNet bestNet = null;

            for (int i = 0; i < Params.NB_PARALLEL_BRAINS; i++)
            {
                int cursorPosition = 0;

                for (int j = 0; j < i; j++)
                {
                    cursorPosition += cursorPadding;
                }

                // Display thread id
                consoleAccess.WaitOne();
                Console.SetCursorPosition(cursorPosition, 1);
                Console.Write("T : " + i + "  ");
                consoleAccess.Release();

                System.Threading.Thread t = new System.Threading.Thread((Object tId) =>
                {
                    System.Threading.Thread.Sleep((int) tId); // Without this sleep, randoms don't really work.

                    // Initialize previous net
                    Brain.NeuralNet previousNet = new Brain.NeuralNet(Brain.Params.NB_NEURON_PER_LAYER, Brain.Params.NB_HIDDEN_LAYERS);
                    previousNet.process(listForLearning);

                    // Initialize best net
                    if (tId.Equals(0)) bestNet = previousNet;

                    Brain.NeuralNet newNet;
                    int nbMutations = 0;
                    int nbBadMutations = 0;

                    // Display "Alive"
                    consoleAccess.WaitOne();
                    Console.SetCursorPosition(cursorPosition, 4);
                    Console.Write("Alive");
                    consoleAccess.Release();

                    while (true)
                    {
                        // Create genetic clone
                        newNet = Brain.NeuralNet.getGeneticClone(previousNet);
                        nbMutations++;
                        nbBadMutations++;

                        // Display nb mutations
                        consoleAccess.WaitOne();
                        Console.SetCursorPosition(cursorPosition, 3);
                        Console.Write(nbMutations);
                        consoleAccess.Release();
                        

                        // Process players to get output
                        newNet.process(listForLearning);

                        // Keep clone?
                        if (previousNet.OutputError > newNet.OutputError)
                        {
                            previousNet = newNet;

                            // Reset bad mutations counter
                            nbBadMutations = 0;

                            consoleAccess.WaitOne();
                            // Display new error %
                            Console.SetCursorPosition(cursorPosition, 2);
                            Console.Write(previousNet.OutputError.ToString("P" + nbDecimals));

                            // Set best net if this one is better
                            if (previousNet.OutputError < bestNet.OutputError)
                            {
                                bestNet = previousNet;

                                Console.SetCursorPosition(0, 10);
                                Console.WriteLine("Best net error % : " + bestNet.OutputError.ToString("P" + nbDecimals));
                            }                        
                            consoleAccess.Release(); // Using console semaphore for best net access because I'm lazy
                        }

                        // Kill thread if brain is evolving too slowly
                        if (nbBadMutations >= Params.MAX_BAD_MUTATIONS)
                        {
                            consoleAccess.WaitOne();
                            Console.SetCursorPosition(cursorPosition, 4);
                            Console.Write("     ");
                            Console.SetCursorPosition(cursorPosition, 5);
                            Console.Write("Maxed");
                            consoleAccess.Release();
                            break;
                        }
                    }
                });

                t.Start(i);
                threads[i] = t;
            }

            // Wait for all the threads to finish
            for (int i = 0; i < Params.NB_PARALLEL_BRAINS; i++)
            {
                threads[i].Join();
            }

            int[,] dispersion = new int[8,8];

            Console.SetCursorPosition(0, 12);
            Console.WriteLine("Testing with " + listForTesting.Count + " players (press Enter):");
            foreach (Model.Player _player  in listForTesting)
            {
                //Console.ReadLine();
                //Console.WriteLine(_player.rank + " -> " + bestNet.processPlayer(_player));
                dispersion[_player.rank - 1, bestNet.processPlayer(_player) - 1]++;
            }

            for (int i = 0; i < 8; i++)
            {
                Console.Write(i + 1);
                Console.Write(" -> [");
                for (int j = 0; j < 8; j++)
                {
                    Console.Write(dispersion[i, j] + ", ");
                }
                Console.WriteLine("]");
            }

            Console.ReadLine();
        }

    } // Program
} // Namespace
