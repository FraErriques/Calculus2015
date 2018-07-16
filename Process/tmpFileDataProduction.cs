using System;


namespace Process
{


    /// <summary>
    /// no locks on worker thread, since fileName is unique. It depends on creation time,
    /// so there can be no concurrency on the same file.
    /// The nullity of the "fullpath" parameter stimulates the generation of a temp file.
    /// NB. pay attention to do not pass notNull params that conflict with an existing file on which another thread is  working.
    /// </summary>
    public static class tmpFileDataProduction
    {

        public static void GenerateUntilThresholdOnTmpFile(Int64 threshold)
        {
            PrimesFinder.Primes p = new PrimesFinder.Primes(
                threshold,
                null// the nullity of this parameter stimulates the generation of a temp file. NB. pay attention to do not pass notNull params that conflict with an existing file on which another thread is  working.
            );
            p.CoreLoop();
        }//


        /// <summary>
        /// The nullity of the "fullpath" parameter stimulates the generation of a temp file.
        /// NB. pay attention to do not pass notNull params that conflict with an existing file on which another thread is  working.
        /// </summary>
        /// <param name="threshold"></param>
        public static void GenerateUntilThresholdOnSpecifiedFile(Int64 threshold, string fullpath)
        {
            PrimesFinder.Primes p = new PrimesFinder.Primes(
                threshold,
                fullpath// the nullity of this parameter stimulates the generation of a temp file. NB. pay attention to do not pass notNull params that conflict with an existing file on which another thread is  working.
            );
            p.CoreLoop();
        }//


    }//


}
