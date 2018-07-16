using System;


namespace Common.MonteCarlo
{


    public static class MonteCarlo_threadLocked
    {
        // data fields : private and static, to let them singleton and synchronized.
        private static readonly Random getrandom = new Random();
        private static readonly object syncLock = new object();

        /// <summary>
        /// a pseudo random integer generator for n | inf smallerOrEqualOf n smallerOf sup.
        /// </summary>
        /// <param name="inf"></param>
        /// <param name="sup"></param>
        /// <returns></returns>
        public static int next_int( int min, int sup )
        {// NB. the extrema : a pseudo random integer generator for n | inf smallerOrEqualOf n smallerOf sup.
            lock (syncLock)
            {// synchronize : take the MutEx
                return getrandom.Next(min, sup);
            }// end : release the MutEx
        }// end : public static int MonteCarlo( int inf, int sup )


        public static void next_byteArray( byte[] theBuffer )
        {// produce un vettore il cui scalare e' un intero nonnegativo
            lock (syncLock)
            {// synchronize : take the MutEx
                getrandom.NextBytes(theBuffer);
            }// end : release the MutEx
        }// end : public static void next_byteArray( byte[] theBuffer ).


        public static double next_probabilityMeasure()
        {// produce un razionale su [0, 1]
            lock (syncLock)
            {// synchronize : take the MutEx
                return getrandom.NextDouble();
            }// end : release the MutEx
        }// end : public double next_probabilityMeasure().


    }// end class : public static int MonteCarlo( int min, int sup )


}// end : nmsp.
