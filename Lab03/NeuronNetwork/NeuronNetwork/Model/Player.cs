using System;

namespace NeuronNetwork.Model
{
    class Player
    {
        public const int NB_RANKS = 8;

        public int rank;

        public static double[] MIN_VALUES = null;
        public static double[] MAX_VALUES = null;
        public static double[] SUM_VALUES = null;

        public double[] myValues;

        public Player(double[] myValues, int rank)
        {
            this.rank = rank;

            this.myValues = myValues;

            if (MIN_VALUES == null && MAX_VALUES == null && SUM_VALUES == null)
            {
                MIN_VALUES = new double[myValues.Length];
                myValues.CopyTo(MIN_VALUES, 0);

                MAX_VALUES = new double[myValues.Length];
                myValues.CopyTo(MAX_VALUES, 0);

                SUM_VALUES = new double[myValues.Length];
                myValues.CopyTo(SUM_VALUES, 0);
            }
            else
            {
                for (int i = 0; i < myValues.Length; i++)
                {
                    if (1 <= i && i <= 3 && myValues[i] == -1) continue;

                    MIN_VALUES[i] = Math.Min(MIN_VALUES[i], myValues[i]);
                    MAX_VALUES[i] = Math.Max(MAX_VALUES[i], myValues[i]);
                    SUM_VALUES[i] += myValues[i];
                }
            }
        }

        /**
         * Change normalization strategy here
         **/
        public void normalize(int nbPlayers)
        {
            for (int i = 0; i < myValues.Length; i++)
            {
                if (1 <= i && i <= 3 && myValues[i] == -1)
                {
                    myValues[i] = SUM_VALUES[i] / nbPlayers;    // If unknown value, set to average
                }

                myValues[i] = (myValues[i] - MIN_VALUES[i]) / (MAX_VALUES[i] - MIN_VALUES[i]);
            }
        }
    }
}
