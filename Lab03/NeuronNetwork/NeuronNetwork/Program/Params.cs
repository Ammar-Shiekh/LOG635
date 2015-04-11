
namespace NeuronNetwork.Program
{
    class Params
    {
        public const int NB_INPUT_COLUMNS = 17; // Nb of columns of the csv file used to create a player object.

        public const int NB_PARALLEL_BRAINS = 11; // Max 11 because of console display

        public const bool OPTIMIZE_RELATIVE_ERROR = true; // Whether to optimize relative error or absolute error

        public const int MAX_BAD_MUTATIONS = 500; // After that amount of bad mutations, the brain restarts from scratch

    }
}
