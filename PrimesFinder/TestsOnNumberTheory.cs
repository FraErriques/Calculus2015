using System;
using System.Collections.Generic;
using System.Text;


namespace PrimesFinder
{

    public class TestsOnNumberTheory
    {


        public static void PiGreco_and_J()
        {
            Random myGenerator = new Random();
            double epsilon = myGenerator.NextDouble();
            //for (int c = 0; c < 100; c++)
            //{
            //    Console.WriteLine("\n\t Epsilon == {0} ", myGenerator.NextDouble());
            //}
            //
            double previousPiG_Level = 0.0;
            double PiG_Level = 0.0;
            PrimesFinder.Primes P = new PrimesFinder.Primes();// default Ctor is the one for reading the log.
            double Pi_64 = P.Pi_Greco(64);
            double J_64 = P.J(64); // NB. J(x)>=Pi(x)
            //
            for (long thresholdInNaturals = 0; thresholdInNaturals < 100; thresholdInNaturals++)
            {
                double pollutedInteger = thresholdInNaturals + myGenerator.NextDouble();
                //Console.Write("\t GetCumulatedOrdinalNotOverThreshold({0}) == {1}  the Inf={2}"
                //    , thresholdInNaturals
                //    , P.GetCumulatedOrdinalNotOverThreshold(thresholdInNaturals, out theInf)
                //    , theInf
                //    );
                PiG_Level = P.Pi_Greco(thresholdInNaturals);
                if (Math.Abs(PiG_Level - previousPiG_Level) > double.Epsilon)
                {
                    Console.WriteLine();
                }// else continue on the same line;
                previousPiG_Level = PiG_Level;// update the PiGreco level.
                Console.Write("____\t PiGreco({0}) == {1} "
                    , thresholdInNaturals
                    , P.Pi_Greco(thresholdInNaturals)
                    );
                PiG_Level = P.Pi_Greco(pollutedInteger);
                if (Math.Abs(PiG_Level - previousPiG_Level) > double.Epsilon)
                {
                    Console.WriteLine();
                }// else continue on the same line;
                previousPiG_Level = PiG_Level;// update the PiGreco level.
                //
                Console.Write("____\t PiGreco({0}) == {1}"
                    , pollutedInteger
                    , P.Pi_Greco(pollutedInteger)
                    );
                //
                Console.Write("____\t J({0}) == {1}"
                    , pollutedInteger
                    , P.J(pollutedInteger) // NB. J(x)>=Pi(x)
                    );
            }
        }// end static void PiGreco_and_J( )


        public static void GetCumulatedOrdinalNotOverThreshold_test()
        {
            PrimesFinder.Primes primes = new PrimesFinder.Primes();// default Ctor is the one for reading the log.
            long theInfinPrimes = -1L;
            //
            long Pi_99_ = primes.GetCumulatedOrdinalNotOverThreshold(99, out theInfinPrimes );
            long Pi_100_ = primes.GetCumulatedOrdinalNotOverThreshold(100L, out theInfinPrimes);
            long Pi_97_ = primes.GetCumulatedOrdinalNotOverThreshold(97L, out theInfinPrimes);
            //
            long[] testL = new long[100];
            for (int c = 0; c < 100; c++)
            {
                testL[c] = primes.GetCumulatedOrdinalNotOverThreshold((long)(c + 100), out theInfinPrimes);
            }
        }// end public static void GetCumulatedOrdinalNotOverThreshold_test()


        public static void EulerProduct_test()
        {
            PrimesFinder.Primes primes = new PrimesFinder.Primes();// default Ctor is the one for reading the log.
            long threshold = 999L;
            //
            double[] Euler_prod = primes.Euler_product( threshold);
            double[] Dirich_sum = primes.Dirichlet_sum( threshold);
            double[] delta = new double[2];
            delta[0] = Euler_prod[0] - Dirich_sum[0];
            delta[1] = Euler_prod[1] - Dirich_sum[1];
            System.Console.WriteLine("\n\t soglia={0} , Errore={1} +i* {2} ", threshold, delta[0], delta[1]);
        }// end public static EulerProduct_test()


        public static void PrintFactorization( long theIsolatedNatural )
        {
            long[,] myOmegaData = PrimesFinder.ElementFullCalculation.OmegaData(theIsolatedNatural);
            double[] theProd = new double[myOmegaData.Length / 2];
            double ToTProd = 1.0;
            System.Console.WriteLine("\n\n");
            System.Console.WriteLine("\t|-----------------------------------------------------");
            for (int c = 0; c < myOmegaData.Length / 2; c++)
            {
                theProd[c] = Math.Pow((double)myOmegaData[c, 0], (double)myOmegaData[c, 1]);
                System.Console.WriteLine("\t|p^k\t==\t{0}^{1}\t==\t{2}", myOmegaData[c, 0], myOmegaData[c, 1], theProd[c]);
                ToTProd *= theProd[c];
                System.Console.WriteLine("\t|-----------------------------------------------------");
            }
            System.Console.WriteLine("\t|\ttotal facor product\t==\t{0}", ToTProd);
            System.Console.WriteLine("\t|-----------------------------------------------------");
        }// end public static void PrintFactorization( long theIsolatedNatural )



        public static void Tests_on_Omegas_Mi_Lambda()
        {
            int omega_small = PrimesFinder.ElementFullCalculation.Omega_small(1L);
            long omega_big = PrimesFinder.ElementFullCalculation.Omega_big(1L);
            int Lambda = PrimesFinder.ElementFullCalculation.Lambda_Liouville(1L);

            PrimesFinder.TestsOnNumberTheory.PrintFactorization(1L);
            PrimesFinder.TestsOnNumberTheory.PrintFactorization(3L * 3L * 3L * 3L * 3L);
            PrimesFinder.TestsOnNumberTheory.PrintFactorization(3L);
            PrimesFinder.TestsOnNumberTheory.PrintFactorization(5L);
            PrimesFinder.TestsOnNumberTheory.PrintFactorization(19L);
            PrimesFinder.TestsOnNumberTheory.PrintFactorization(2L);
            //int omega_small = PrimesFinder.ElementFullCalculation.Omega_small(302870L);
            //long omega_big = PrimesFinder.ElementFullCalculation.Omega_big(302870L);
            //int Lambda = PrimesFinder.ElementFullCalculation.Lambda_Liouville(302870L);
            //int DiracDelta_onOmegas = PrimesFinder.ElementFullCalculation.DiracDelta_Omegas(302870L);
            //DiracDelta_onOmegas = PrimesFinder.ElementFullCalculation.DiracDelta_Omegas(302870L);

            System.Console.WriteLine("\n\n\n");
            for (int c = 1; c < 100; c++)
            {
                System.Console.WriteLine("Omega_small({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Omega_small(c));
            }
            System.Console.WriteLine("\n\n\n");
            for (int c = 1; c < 100; c++)
            {
                System.Console.WriteLine("Omega_big({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Omega_big(c));
            }
            System.Console.WriteLine("\n\n\n");
            for (int c = 1; c < 100; c++)
            {
                System.Console.WriteLine("Mi_Moebius({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Moebius_Mi(c));
            }
            System.Console.WriteLine("\n\n\n");
            for (int c = 1; c < 100; c++)
            {
                System.Console.WriteLine("Mertens_M({0}) == {1}", c, PrimesFinder.ElementFullCalculation.Mertens_M(c));
            }
        }// end public static void Tests_on_Omegas_Mi_Lambda()


    }// class TestsOnNumberTheory

}// end namespace PrimesFinder


#region cantina

            //long[,] myOmegaData = PrimesFinder.ElementFullCalculation.OmegaData( 2L*2L*2L * 3L*3L*3L*3L*3L*3L*3L* 5L*5L*5L*5L*5L*5L*5L*5L*5L  );
            //double[] theProd = new double[myOmegaData.Length / 2];
            //double ToTProd = 1.0;
            //System.Console.WriteLine("\n\n");
            //System.Console.WriteLine("\t|-----------------------------------------------------");
            //for (int c = 0; c < myOmegaData.Length / 2; c++)
            //{
            //    theProd[c] = Math.Pow( (double)myOmegaData[c, 0], (double)myOmegaData[c, 1] );
            //    System.Console.WriteLine("\t|p^k\t==\t{0}^{1}\t==\t{2}", myOmegaData[c, 0], myOmegaData[c, 1], theProd[c]);
            //    ToTProd *= theProd[c];
            //    System.Console.WriteLine("\t|-----------------------------------------------------");
            //}
            //System.Console.WriteLine("\t|\ttotal facor product\t==\t{0}", ToTProd);
            //System.Console.WriteLine("\t|-----------------------------------------------------");

#endregion cantina
