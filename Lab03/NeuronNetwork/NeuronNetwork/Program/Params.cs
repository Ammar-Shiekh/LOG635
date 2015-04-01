
namespace NeuronNetwork.Program
{
    class Params
    {
        public const int NB_INPUT_COLUMNS = 17;

        public const int NB_PARALLEL_BRAINS = 4; // Max 11

        public const double ERROR_TRESHOLD = 0.4;

        public const int MAX_BAD_MUTATIONS = 200; // After that amount of bad mutations, the brain restarts

        public const double LEARNING_PERCENTAGE = 0.7;

    }
}
