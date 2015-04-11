using System;
using System.Collections.Generic;


namespace NeuronNetwork.Program
{
    class Program
    {
        // Main method. All the program is executed here
        static void Main(string[] args)
        {
            // List of input object used for the learning part of the program
            List<Model.Player> listForLearning = getPlayersFromFile("learn.csv", true);

            // Normalize data
            foreach (Model.Player _player in listForLearning)
            {
                _player.normalize(listForLearning.Count);
            }


            // Variables for display purposes (#refactor to constants)
            int nbDecimals = 1;
            int cursorPadding = nbDecimals + 6;

            // Nb of parallel neural network working asynchronously
            System.Threading.Thread[] threads = new System.Threading.Thread[Params.NB_PARALLEL_BRAINS];

            // Semaphore for access to console.
            System.Threading.Semaphore consoleAccess = new System.Threading.Semaphore(1, 1);

            // Best neural net yet
            Brain.NeuralNet bestNet = null;

            // Thread Id of the thread that came up with the best net
            int threadWorkingOnBestNet = 0;

            bool stopThreads = false; // Whether to stop computing in threads that aren't the main thread
            bool focusOnBest = false; // Whether to stop restarting from scratch and focus computing of all threads on the best net

            // Create each threads
            for (int i = 0; i < Params.NB_PARALLEL_BRAINS; i++)
            {
                int cursorPosition = 0; // Cursor position of the console

                for (int j = 0; j < i; j++)
                {
                    cursorPosition += cursorPadding;
                }

                // Display thread id
                consoleAccess.WaitOne();
                Console.SetCursorPosition(cursorPosition, 1);
                Console.Write("T : " + i + "  ");
                consoleAccess.Release();

                // Create thread (From here on, execution is parallel)
                System.Threading.Thread t = new System.Threading.Thread((Object tId) =>
                {
                    int myThreadId = (int)tId;

                    // Initialize my best net
                    Brain.NeuralNet myBestNet = new Brain.NeuralNet(Brain.Params.NB_NEURON_PER_LAYER, Brain.Params.NB_HIDDEN_LAYERS);

                    myBestNet.computeError(listForLearning); // Compute my best net's error

                    // Initialize (absolute) best net
                    if (myThreadId == 0) bestNet = myBestNet;

                    Brain.NeuralNet newNet; // New net that will or won't replace my best net

                    int nbMutations = 0; // Nb mutations between my best net and new net
                    int nbBadMutations = 0; // Nb mutations that didn't improve the new net's performance

                    while (!stopThreads)
                    {
                        // Create genetic clone of my best net
                        newNet = Brain.NeuralNet.getGeneticClone(myBestNet);
                        nbMutations++;
                        nbBadMutations++;

                        // Display nb mutations
                        consoleAccess.WaitOne();
                        Console.SetCursorPosition(cursorPosition, 3);
                        Console.Write(nbMutations.ToString("D5"));
                        consoleAccess.Release();
                        

                        // Process players to get error
                        newNet.computeError(listForLearning);

                        // If clone is better than my best net, my best net becomes the clone
                        if ((Params.OPTIMIZE_RELATIVE_ERROR && myBestNet.OutputErrorRel > newNet.OutputErrorRel) || // If optimizing relative error
                            (!Params.OPTIMIZE_RELATIVE_ERROR && myBestNet.OutputErrorAbs > newNet.OutputErrorAbs))  // If optimizing absolute error
                        {
                            myBestNet = newNet;

                            // Reset bad mutations counter
                            nbBadMutations = 0;

                            consoleAccess.WaitOne();
                            // Display new error %
                            Console.SetCursorPosition(cursorPosition, 2);
                            Console.Write(myBestNet.OutputErrorAbs.ToString("P" + nbDecimals));

                            // Set (absolute) best net if my best net is better
                            if (myBestNet.OutputErrorAbs < bestNet.OutputErrorAbs)
                            {
                                if (bestNet.OutputErrorAbs > 100) // Dunno why this is there
                                    Console.WriteLine();

                                bestNet = myBestNet;
                                threadWorkingOnBestNet = myThreadId;

                                Console.SetCursorPosition(0, 7);
                                Console.Write("Best neural net (" + threadWorkingOnBestNet + ") with error of " + bestNet.OutputErrorAbs.ToString("P" + nbDecimals) + " | " + bestNet.OutputErrorRel.ToString("P" + nbDecimals));
                            }                        
                            consoleAccess.Release(); // Using console semaphore for best net access because I'm lazy
                        }

                        // Kill thread if brain is evolving too slowly
                        if (nbBadMutations >= Params.MAX_BAD_MUTATIONS &&
                            threadWorkingOnBestNet != myThreadId) // But don't do it if my best net is the (absolute) best net
                        {

                            if (focusOnBest) myBestNet = bestNet; // Restart from best net if focusing computation
                            else myBestNet = new Brain.NeuralNet(Brain.Params.NB_NEURON_PER_LAYER, Brain.Params.NB_HIDDEN_LAYERS); // Restart from scratch otherwise

                            myBestNet.computeError(listForLearning); // Compute error

                            // Reset mutation counters
                            nbMutations = 0;
                            nbBadMutations = 0;

                            // Display new best net error
                            consoleAccess.WaitOne();
                            Console.SetCursorPosition(cursorPosition, 2);
                            Console.Write(myBestNet.OutputErrorAbs.ToString("P" + nbDecimals));
                            consoleAccess.Release();
                        }
                    }
                });

                t.Start(i); // Start thread
                System.Threading.Thread.Sleep(i * 2); // For better random number generation
                threads[i] = t;
            }
            
            // Time counting thread
            (new System.Threading.Thread(() =>{

                DateTime start = DateTime.Now;

                while (!stopThreads)
                {
                    // Display elapsed time
                    consoleAccess.WaitOne();
                    Console.SetCursorPosition(0, 5);
                    Console.Write("Elapsed time : " + DateTime.Now.Subtract(start).TotalSeconds.ToString("N0") + " s");
                    consoleAccess.Release();

                    System.Threading.Thread.Sleep(1000);
                }

            })).Start();

            // Display instructions
            consoleAccess.WaitOne();
            Console.SetCursorPosition(0, 9);
            Console.Write("Press Enter to focus computation on best net");
            consoleAccess.Release();

            Console.ReadKey(true); // Press enter to focus
            focusOnBest = true;

            // Display new instructions
            consoleAccess.WaitOne();
            Console.SetCursorPosition(0, 9);
            Console.Write("Press Enter to use best net to parse new data");
            consoleAccess.Release();

            Console.ReadKey(true); // Press enter to stop
            stopThreads = true;

            // Wait for all the threads to finish
            for (int i = 0; i < Params.NB_PARALLEL_BRAINS; i++)
            {
                threads[i].Join();
            }

            // Load testing file in memory
            List<Model.Player> listForTesting = getPlayersFromFile("test.csv", false);

            // Normalize data
            foreach (Model.Player _player in listForTesting)
            {
                _player.normalize(listForLearning.Count + listForTesting.Count);
            }


            Console.SetCursorPosition(0, 11);

            // Test neural network
            foreach (Model.Player _player  in listForTesting)
            {
                Console.WriteLine(" - " + bestNet.predictRank(_player));
            }

            Console.ReadLine();
        } // End main

        /// <summary> Creates player objects from a csv file with player stats at each line </summary>
        /// <param name="fileName">The relative path of the csv file</param>
        /// <param name="extractRank">Whether to extract the rank of each player from the csv file and set the Rank attribute with the value. Set to TRUE when using the players for learning, FALSE for testing.</param>
        /// <returns>A list of player objects</returns>
        private static List<Model.Player> getPlayersFromFile(String fileName, bool extractRank)
        {
            List<Model.Player> players = new List<Model.Player>(); // The list to be returned

            String line; // Buffer for a line of text from the file

            String[] splitLine; // Buffer for the exploded line of text

            char[] comma = { ',' };
            char[] doubleQuotes = { '"' };

            System.IO.StreamReader file = new System.IO.StreamReader(fileName);

            file.ReadLine(); // Skip first line because it's the columns name

            while ((line = file.ReadLine()) != null)
            {
                splitLine = line.Split(comma);

                players.Add(new Model.Player(new double[] {
                                            // double.Parse(splitLine[0]), // skip player id (we don't believe it has any corrolation to the players rank
                                            // double.Parse(splitLine[1]) // skip league index
                                            // splitLine[2].Equals("\"?\"") ? -1 : double.Parse(splitLine[2]), // age
                                            splitLine[3].Equals("\"?\"") ? -1 : double.Parse(splitLine[3]), // hours per week
                                            splitLine[4].Equals("\"?\"") ? -1 : double.Parse(splitLine[4]), // total hours
                                            double.Parse(splitLine[5]), // APM
                                            double.Parse(splitLine[6].Trim(doubleQuotes), System.Globalization.NumberStyles.Float), // SelectByHotkeys
                                            double.Parse(splitLine[7].Trim(doubleQuotes), System.Globalization.NumberStyles.Float), // AssignToHotkeys
                                            double.Parse(splitLine[8]), // UniqueHotkeys
                                            double.Parse(splitLine[9].Trim(doubleQuotes), System.Globalization.NumberStyles.Float), // MinimapAttacks
                                            double.Parse(splitLine[10].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[11]),
                                            double.Parse(splitLine[12]),
                                            double.Parse(splitLine[13]),
                                            double.Parse(splitLine[14]),
                                            double.Parse(splitLine[15]), // Total map explored
                                            double.Parse(splitLine[16].Trim(doubleQuotes), System.Globalization.NumberStyles.Float), // Total murder make
                                            double.Parse(splitLine[17]),
                                            double.Parse(splitLine[18].Trim(doubleQuotes), System.Globalization.NumberStyles.Float),
                                            double.Parse(splitLine[19].Trim(doubleQuotes), System.Globalization.NumberStyles.Float)

                }, extractRank ? int.Parse(splitLine[1]) : 0)); // Set the rank parameter of the player object constructor to 0 if extract rank is false.
            }

            file.Close();

            return players;
        }

    } // Program
} // Namespace
