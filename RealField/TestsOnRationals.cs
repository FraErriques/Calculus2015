using System;
 

namespace RealField
{


    public class TestsOnRationals
    {


                /// <summary>
                /// the user enters a string with the following syntax : "-2^3* 3^4* 7^2  *5^3".
                /// and obtains the long_couple array : { { -2, 3}, {3, 4}, {7, 2}, {5, 3} } as data field member.
                /// </summary>
                /// <param name="factorizationString"></param>
                /// <returns></returns>
                public static long[,] fromStringFactoriz_ToTensorFactoriz( string factorizationString )
                {
                    string[] factors = factorizationString.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);// first round : extract pi^ei
                    long[,] res = new long[factors.Length, 2];// columns are fixed 2, since they are base and exponent.
                    //
                    for (int c = 0; c < factors.Length; c++)
                    {
                        string[] BaseExp = factors[c].Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);// second round : separate pi and ei, for each couple pi^ei.
                        res[c, 0] = long.Parse(BaseExp[0]);// convert base; throws on syntax error.
                        res[c, 1] = long.Parse(BaseExp[1]);// convert exponent; throws on syntax error.
                        BaseExp = null;// garbage collect.
                    }// end for.
                    // ready.
                    return res;
                }// end : public static long[,] fromStringFactoriz_ToTensorFactoriz( string factorizationString )


                /// <summary>
                /// receives a long_couple array, i.e. a tensor_rank2 such as : { { -2, 3}, {3, 4}, {7, -2}, {5, 3} }
                /// and computes Product[ base_i^exp_i] and returns the grand total.
                /// </summary>
                /// <param name="theFactorization"></param>
                /// <returns></returns>
                public static double mulUp( long[,] theFactorization )// NB. the rationals are dense in the Reals.
                {// on rationals -> both positive and negative exponents are admitted.
                    double res = +1.0;// we will multiply on it.
                    for (int c = 0; c < theFactorization.Length / 2; c++)
                    {
                        res *= System.Math.Pow(theFactorization[c, 0], theFactorization[c, 1]);// i.e. base^exp.
                    }//
                    // ready.
                    return res;
                }// end : public static double mulUp( long[,] theFactorization )// NB. the rationals are dense in the Reals.





        /*
        public static void Test_RationalReduce_caller()
        {
            int accTest = 0;
            int start_num = -1800;
            int stop_num = -1788;
            int start_den = 8266;
            int stop_den = 8268;
            //
            int accRound = 0;
            for (int num = start_num; num <= stop_num; num++)
            {
                for (int den = start_den; den <= stop_den; den++)
                {
                    if (den != 0 && num != 0)// skip the zero, both on denominator and on numerator.
                    {// skip the zero, both on denominator and on numerator.
                        if ( RealField.TestsOnRationals.Test_RationalReduce_( num, den) )
                        {
                            accTest++;
                        }//else do not increment.
                        accRound++;
                        //Console.WriteLine("Success accumulator = {0}", accTest);
                    }// skip the zero, both on denominator and on numerator.
                    else// skip the zero, both on denominator and on numerator.
                    {// skip the zero, both on denominator and on numerator.
                        continue;// skip the zero, both on denominator and on numerator.
                    }// end : skip the zero, both on denominator and on numerator.
                }// end for
            }// end for
            double success = (double)accTest / (double)accRound;
            Console.WriteLine(" Cardinality of run tests == {0}", accRound);
            Console.WriteLine(" Success percentage = {0}%", success * 100);
        }
        */

        /*
        public static bool Test_RationalReduce_( long num, long den )
        {
            bool res = false;
            double quotientToBe = (double)num / (double)den;
            //
            RealField.Rationals_Q_ R = new RealField.Rationals_Q_(num, den);
            R.RationalReductionOnIntegers();
            long[,] reduced_Num = R.Num_simplified_factorization;
            double mul_Num = RealField.Rationals_Q_.mulUp(reduced_Num);
            long[,] reduced_Den = R.Den_simplified_factorization;
            double mul_Den = RealField.Rationals_Q_.mulUp(reduced_Den);
            double quotient_AsIsfrom_RationalReduce_ = (double)mul_Num / (double)mul_Den;
            //---########################
            if (System.Math.Abs(quotientToBe - quotient_AsIsfrom_RationalReduce_) < double.Epsilon)
            {
                res = true;// test ok !
            }
            else
            {
                res = false;// test failed :-(
            }
            //---########################
            if (true)//( ! res)
            {
                Console.WriteLine("\n\n############\n\nTest result {0}/{1} == {2}", num, den, res);
                Console.WriteLine(R.ToNumDenString());
                Console.WriteLine(R.ToFractionString());
                Console.WriteLine(R.ToSimplifiedFractionString());
                Console.WriteLine("quotientToBe = {0} quotient_AsIsfrom_RationalReduce_ = {1}", quotientToBe, quotient_AsIsfrom_RationalReduce_);
            }// else skip output, on success.
            //
            // ready
            return res;
        }// end public static bool Test_RationalReduce_( long num, long den )
        */

        public static void ConsoleTensor_Ratio_Publisher( string message, RealField.PruningResults thePrunedRatio )
        {
            Console.WriteLine("\r\n   follows the Pruned Ratio : " + message);
            // Console output : it's useful to call it once before the sorting and once kust after.
            for (int c = 0; c < thePrunedRatio.tensor_Num.Length / 2; c++)
            {
                Console.Write(thePrunedRatio.tensor_Num[c, 0] + "^" + thePrunedRatio.tensor_Num[c, 1] + " * ");
            }
            Console.Write("\r\n--------------------------------------------------------\r\n");
            for (int c = 0; c < thePrunedRatio.tensor_Den.Length / 2; c++)
            {
                Console.Write(thePrunedRatio.tensor_Den[c, 0] + "^" + thePrunedRatio.tensor_Den[c, 1] + " * ");
            }
            Console.WriteLine();
        }// end : public void ConsoleTensorPublisher( string message, long[,] SignedArray )


        public static void ConsoleTensor_Ratio_Publisher( string message, RealField.RatioReducingResults theReducedRatio )
        {
            Console.WriteLine("\r\n   follows the Reduced Ratio : " + message);
            // Console output : it's useful to call it once before the sorting and once kust after.
            for (int c = 0; c < theReducedRatio.Num_simplified_factorization.Length / 2; c++)
            {
                Console.Write(theReducedRatio.Num_simplified_factorization[c, 0] + "^" + theReducedRatio.Num_simplified_factorization[c, 1] + " * ");
            }
            Console.Write("\r\n--------------------------------------------------------\r\n");
            for (int c = 0; c < theReducedRatio.Den_simplified_factorization.Length / 2; c++)
            {
                Console.Write(theReducedRatio.Den_simplified_factorization[c, 0] + "^" + theReducedRatio.Den_simplified_factorization[c, 1] + " * ");
            }
            Console.WriteLine();
        }// end : public void ConsoleTensorPublisher( string message, long[,] SignedArray )




        public static string ConsoleTensor_Ratio_Publisher_onStringBuilder( string message, RealField.RatioReducingResults theReducedRatio )
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
//Console.WriteLine("\r\n   follows the Reduced Ratio : " + message);
            sb.Append("\r\n   follows the Reduced Ratio : ");
            sb.Append(message);

            // Console output : it's useful to call it once before the sorting and once kust after.
            for (int c = 0; c < theReducedRatio.Num_simplified_factorization.Length / 2; c++)
            {
//Console.Write(theReducedRatio.Num_simplified_factorization[c, 0] + "^" + theReducedRatio.Num_simplified_factorization[c, 1] + " * ");

                sb.Append(theReducedRatio.Num_simplified_factorization[c, 0].ToString() );
                sb.Append( "^" );
                sb.Append( theReducedRatio.Num_simplified_factorization[c, 1].ToString() );
                sb.Append( " * ");
            }
//Console.Write("\r\n--------------------------------------------------------\r\n");
            sb.Append("\r\n--------------------------------------------------------\r\n");
            for (int c = 0; c < theReducedRatio.Den_simplified_factorization.Length / 2; c++)
            {
//Console.Write(theReducedRatio.Den_simplified_factorization[c, 0] + "^" + theReducedRatio.Den_simplified_factorization[c, 1] + " * ");

                sb.Append( theReducedRatio.Den_simplified_factorization[c, 0].ToString() );
                sb.Append( "^" );
                sb.Append( theReducedRatio.Den_simplified_factorization[c, 1].ToString() );
                sb.Append( " * ");
            }
//Console.WriteLine();
            sb.Append("\r\n");
            // ready.
            return sb.ToString();
        }// end : public void ConsoleTensorPublisher( string message, long[,] SignedArray )


        public static string GenericTensor_Publisher_onStringBuilder( string message, long[,] theTensor )
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //Console.WriteLine("\r\n   follows the Reduced Ratio : " + message);
            sb.Append("\r\n   follows the GenericTensor_Publisher_onStringBuilder : ");
            sb.Append(message);

            // Console output : it's useful to call it once before the sorting and once kust after.
            for (int c = 0; c < theTensor.Length / 2; c++)
            {
                sb.Append(theTensor[c, 0].ToString());
                sb.Append("^");
                sb.Append(theTensor[c, 1].ToString());
                sb.Append(" * ");
            }
            sb.Append("\r\n--------------------------------------------------------\r\n");
            // ready.
            return sb.ToString();
        }// end : GenericTensor_Publisher_onStringBuilder




        public static void ConsoleTensorPublisher( string message, long[,] SignedArray )
        {
            Console.WriteLine("\r\n\r\nfollows the following SignedArray : " + message);
            // Console output : it's useful to call it once before the sorting and once kust after.
            for (int c = 0; c < SignedArray.Length / 2; c++)
            {
                Console.Write(SignedArray[c, 0] + "^" + SignedArray[c, 1] + " * ");
            }
            Console.WriteLine();
        }// end : public void ConsoleTensorPublisher( string message, long[,] SignedArray )





        /// <summary>
        /// /* NB.
        ///  * ----- cruscotto Watch : debug watch cockpit for :  test Prune_and_RationalReduce_test().
        ///  * 
        /// curEpsilon			        1000000000.0	double
        /// int_curMagnitudo		    24	int
        /// curMagnitudo			    24.0	double
        /// totalAfterPrune			    7.0708168248286379E+23	double
        /// totalAfterRationalReduce	7.0708168248286379E+23	double
        /// totalPrePrune			    7.0708168248286366E+23	double
        /// mustLog				        true	bool
        /// */
        /// </summary>
        /// <param name="cardTests">cardinalita' dei test del batch</param>
        /// <param name="fixedEpsilon">l'epsilon prefissato : e' ovvio che ci sia errore su questo, data la variabilita' della magnitudo</param>
        /// <param name="ForceVerbosity">se si vuole il log, anche in assenza di errore sul fixed epsilon; se si passa false il log avviene solo su errore sul fixedEpsilon. L'errore sullo epsilon variabile in funz. della magnitudo non si manifesta mai.</param>
        public static void Prune_and_RationalReduce_test( int cardTests, double fixedEpsilon, bool ForceVerbosity)
        {
            //---- test batch ------------------------------------------------------------
            for (int accTests = 0; accTests < cardTests; accTests++)
            {
                bool mustLog = false;// activated on error or on request.
                //--------------------------- start :  prepare single test inside the batch ---------------------------
                System.Text.StringBuilder resultLogger = new System.Text.StringBuilder();
                resultLogger.Append("\r\n\r\n@@@@@--- start test nr. " );
                resultLogger.Append(accTests.ToString() );
                resultLogger.Append(" ----@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n");
                int factorCardinality = Common.MonteCarlo.MonteCarlo_threadLocked.next_int(1, 20);
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int c = 0; c < factorCardinality; c++)
                {
                    long curBase = PrimesFinder.Primes.readPrimeAtSpecifiedOrdinal(Common.MonteCarlo.MonteCarlo_threadLocked.next_int(1, 10));
                    int curExp = Common.MonteCarlo.MonteCarlo_threadLocked.next_int(-9, 9);
                    sb.Append(curBase.ToString());
                    sb.Append("^");
                    sb.Append(curExp.ToString());
                    sb.Append(" * ");
                }
                sb.Remove(sb.Length - 2, 2);// remove last " * " 
                string stringFactor = sb.ToString();
                sb = null;// gc
                resultLogger.Append("\r\n  the MonteCarlo extracted stringFactor is : ");
                resultLogger.Append(stringFactor);
                //--------------------------- end prepare single test inside the batch ---------------------------
                //
                // tests on pruning
                long[,] SignedArray = RealField.TestsOnRationals.fromStringFactoriz_ToTensorFactoriz(stringFactor);
                double totalPrePrune = RealField.TestsOnRationals.mulUp(SignedArray);
                resultLogger.Append("\r\n  the MonteCarlo extracted stringFactor TOTAL is : ");
                resultLogger.Append( totalPrePrune.ToString() );
                resultLogger.Append("\r\n original input for pruning" );
                resultLogger.Append(GenericTensor_Publisher_onStringBuilder("the Signed Array : ", SignedArray));
                RealField.PruningInstrument thePruner_method = new RealField.PruningInstrument(SignedArray);
                RealField.PruningResults prunRes = thePruner_method.Prune();
                resultLogger.Append("\r\n  the pruned Ratio ");
                resultLogger.Append(GenericTensor_Publisher_onStringBuilder("prunRes.tensor_Num ", prunRes.tensor_Num));
                resultLogger.Append(GenericTensor_Publisher_onStringBuilder("prunRes.tensor_Den ", prunRes.tensor_Den));
                double numPostPrune = RealField.TestsOnRationals.mulUp(prunRes.tensor_Num);
                double denPostPrune = RealField.TestsOnRationals.mulUp(prunRes.tensor_Den);
                double totalAfterPrune = numPostPrune / denPostPrune;
                double AnalyzedObj_Magnitudo = getMagnitudoOrder(totalPrePrune);
                double Error_Magnitudo = getMagnitudoOrder(Math.Abs(totalPrePrune - totalAfterPrune));
                int figureNonDisponibili = (int)AnalyzedObj_Magnitudo - 15;// figure non rappresentabili, i.e. errore
                double curEpsilon = +9.0 * Math.Pow(+10.0, figureNonDisponibili);// la figura sulla quale si manifesta l'errore e' disponibileper tutta la sua magnitudo( i.e. fino al "9").
                // log for error on Prune ----------------
                resultLogger.Append("\r\n error on Prune : Abs(totalPrePrune - totalAfterPrune)== ");
                resultLogger.Append(Math.Abs(totalPrePrune - totalAfterPrune).ToString());
                resultLogger.Append("\r\n AnalyzedObj_Magnitudo==");
                resultLogger.Append(AnalyzedObj_Magnitudo.ToString());
                resultLogger.Append("\r\n Error_Magnitudo==");
                resultLogger.Append(Error_Magnitudo.ToString());
                resultLogger.Append("\r\n\r\n");
                // end : log for error on Prune ----------------
                //
                if (Math.Abs(totalPrePrune - totalAfterPrune) < curEpsilon)//  this is the variable epsilon, related to the magnitudo.
                {
                    //ok
                }
                else
                {
                    mustLog = true;
                }
                if (Math.Abs(totalPrePrune - totalAfterPrune) < fixedEpsilon)//  this is the FIXED epsilon, a param.
                {
                    //ok
                }
                else
                {
                    if (ForceVerbosity)
                    {
                        mustLog = true;
                    }
                }
                // end tests on pruning
                //
                //-------------
                //
                // tests on ratioReducing
                figureNonDisponibili = 0;// re-init to allow a non-confusing vision in the debugger.
                curEpsilon = 0.0;// re-init to allow a non-confusing vision in the debugger.
                RealField.RatioReducingInstrument ratioMethod = new RealField.RatioReducingInstrument(prunRes.tensor_Num, prunRes.tensor_Den);
                RealField.RatioReducingResults ratioSimplified = ratioMethod.RationalReductionOnOmegaData();
                resultLogger.Append(
                RealField.TestsOnRationals.ConsoleTensor_Ratio_Publisher_onStringBuilder("\r\n from RatioReduce "
                , ratioSimplified)
                );
                resultLogger.Append(GenericTensor_Publisher_onStringBuilder("ratioSimplified.Num_simplified_factorization ", ratioSimplified.Num_simplified_factorization));
                resultLogger.Append(GenericTensor_Publisher_onStringBuilder("ratioSimplified.Den_simplified_factorization ", ratioSimplified.Den_simplified_factorization));
                double numPostRationalReduce = RealField.TestsOnRationals.mulUp(ratioSimplified.Num_simplified_factorization);
                double denPostRationalReduce = RealField.TestsOnRationals.mulUp(ratioSimplified.Den_simplified_factorization);
                double totalAfterRationalReduce = numPostRationalReduce / denPostRationalReduce;
                resultLogger.Append("\r\n TOTAL from RatioReduce == ");
                resultLogger.Append(totalAfterRationalReduce.ToString());
                AnalyzedObj_Magnitudo = getMagnitudoOrder(totalAfterRationalReduce);// NB. I'm reusing variables frome the Prune tests.
                Error_Magnitudo = getMagnitudoOrder(Math.Abs(totalPrePrune - totalAfterRationalReduce));
                figureNonDisponibili = (int)AnalyzedObj_Magnitudo - 15;// figure non rappresentabili, i.e. errore
                curEpsilon = +9.0 * Math.Pow(+10.0, figureNonDisponibili);// la figura sulla quale si manifesta l'errore e' disponibileper tutta la sua magnitudo( i.e. fino al "9").
                // log for error on RatioReduce ----------------
                resultLogger.Append("\r\n error on RationalReduce : Abs(totalPrePrune - totalAfterRationalReduce)== ");
                resultLogger.Append(Math.Abs(totalPrePrune - totalAfterRationalReduce).ToString());
                resultLogger.Append("\r\n AnalyzedObj_Magnitudo==");
                resultLogger.Append(AnalyzedObj_Magnitudo.ToString());
                resultLogger.Append("\r\n Error_Magnitudo==");
                resultLogger.Append(Error_Magnitudo.ToString());
                resultLogger.Append("\r\n\r\n");
                // end : log for error on RatioReduce ----------------
                if (Math.Abs(totalPrePrune - totalAfterRationalReduce) < curEpsilon)// variable epsilon==epsilon( magnitude)
                {
                    //ok
                }
                else
                {
                    mustLog = true;
                }
                if (Math.Abs(totalPrePrune - totalAfterRationalReduce) < fixedEpsilon)//  this is the FIXED epsilon, a param.
                {
                    //ok
                }
                else
                {
                    if (ForceVerbosity)
                    {
                        mustLog = true;
                    }
                }
                //
                resultLogger.Append("\r\n #####--- end test nr. ");
                resultLogger.Append(accTests.ToString());
                resultLogger.Append("  ----##########################################\r\n\r\n");
                //----------------------------------------------------------
                if ( mustLog )
                {
                    Console.WriteLine(resultLogger.ToString());
                }// else skip.
                resultLogger = null;// gc
                totalPrePrune = totalAfterPrune = totalAfterRationalReduce = 0.0;// reset
            }// end for #(tests)
        }// end : test Prune_and_RationalReduce_test()
        //
        /* NB.
         * ----- cruscotto Watch : debug watch cockpit for :  test Prune_and_RationalReduce_test().
         * 
        curEpsilon			        1000000000.0	double
		int_curMagnitudo		    24	int
		curMagnitudo			    24.0	double
		totalAfterPrune			    7.0708168248286379E+23	double
		totalAfterRationalReduce	7.0708168248286379E+23	double
		totalPrePrune			    7.0708168248286366E+23	double
		mustLog				        true	bool
        */




        public static string MonteCarloExtract_singleFactorizazion()
        {
            //--------------------------- start :  prepare single test inside the batch ---------------------------
            int factorCardinality = Common.MonteCarlo.MonteCarlo_threadLocked.next_int(1, 20);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int c = 0; c < factorCardinality; c++)
            {
                long curBase = PrimesFinder.Primes.readPrimeAtSpecifiedOrdinal(Common.MonteCarlo.MonteCarlo_threadLocked.next_int(1, 10));
                int curExp = Common.MonteCarlo.MonteCarlo_threadLocked.next_int(-9, 9);
                sb.Append(curBase.ToString());
                sb.Append("^");
                sb.Append(curExp.ToString());
                sb.Append(" * ");
            }
            sb.Remove(sb.Length - 2, 2);// remove last " * " 
            string stringFactor = sb.ToString();
            sb = null;// gc
            // ready.
            return stringFactor;
            //--------------------------- end prepare single test inside the batch ---------------------------
        }// end : public static string MonteCarloExtract_singleFactorizazion().



        /// <summary>
        /// perform a single test only, on a factorization provided by the tester-programmer, as a string.
        /// </summary>
        /// <param name="myFactorization"></param>
        /// <param name="cardTests"></param>
        /// <param name="fixesEpsilon"></param>
        /// <param name="verbosity"></param>
        public static void Prune_and_RationalReduce_test( string stringFactor, double fixedEpsilon, bool ForceVerbosity )
        {
            bool mustLog = false;// activated on error or on request.
            System.Text.StringBuilder resultLogger = new System.Text.StringBuilder();
            resultLogger.Append("\r\n\r\n@@@@@--- start SINGLE test  ");
            resultLogger.Append(" ----@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n");
            resultLogger.Append("\r\n  the provided string tensor is : ");
            resultLogger.Append(stringFactor);// present the isolated factorization.
            //
            // tests on pruning
            long[,] SignedArray = RealField.TestsOnRationals.fromStringFactoriz_ToTensorFactoriz(stringFactor);
            double totalPrePrune = RealField.TestsOnRationals.mulUp(SignedArray);
            resultLogger.Append("\r\n  the MonteCarlo extracted stringFactor TOTAL is : ");
            resultLogger.Append(totalPrePrune.ToString());
            resultLogger.Append("\r\n original input for pruning");
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("the Signed Array : ", SignedArray));
            RealField.PruningInstrument thePruner_method = new RealField.PruningInstrument(SignedArray);
            RealField.PruningResults prunRes = thePruner_method.Prune();
            resultLogger.Append("\r\n  the pruned Ratio ");
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("prunRes.tensor_Num ", prunRes.tensor_Num));
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("prunRes.tensor_Den ", prunRes.tensor_Den));
            double numPostPrune = RealField.TestsOnRationals.mulUp(prunRes.tensor_Num);
            double denPostPrune = RealField.TestsOnRationals.mulUp(prunRes.tensor_Den);
            double totalAfterPrune = numPostPrune / denPostPrune;
            double AnalyzedObj_Magnitudo = getMagnitudoOrder(totalPrePrune);
            double Error_Magnitudo = getMagnitudoOrder(Math.Abs(totalPrePrune - totalAfterPrune));
            int figureNonDisponibili = (int)AnalyzedObj_Magnitudo - 15;// figure non rappresentabili, i.e. errore
            double curEpsilon = +9.0 * Math.Pow(+10.0, figureNonDisponibili);// la figura sulla quale si manifesta l'errore e' disponibileper tutta la sua magnitudo( i.e. fino al "9").
            // log for error on Prune ----------------
            resultLogger.Append("\r\n error on Prune : Abs(totalPrePrune - totalAfterPrune)== ");
            resultLogger.Append(Math.Abs(totalPrePrune - totalAfterPrune).ToString());
            resultLogger.Append("\r\n AnalyzedObj_Magnitudo==");
            resultLogger.Append(AnalyzedObj_Magnitudo.ToString());
            resultLogger.Append("\r\n Error_Magnitudo==");
            resultLogger.Append(Error_Magnitudo.ToString());
            resultLogger.Append("\r\n\r\n");
            // end : log for error on Prune ----------------
            //
            if (Math.Abs(totalPrePrune - totalAfterPrune) < curEpsilon)//  this is the variable epsilon, related to the magnitudo.
            {
                //ok
            }
            else
            {
                mustLog = true;
            }
            if (Math.Abs(totalPrePrune - totalAfterPrune) < fixedEpsilon)//  this is the FIXED epsilon, a param.
            {
                //ok
            }
            else
            {
                if (ForceVerbosity)
                {
                    mustLog = true;
                }
            }
            // end tests on pruning
            //
            //-------------
            //
            // tests on ratioReducing
            figureNonDisponibili = 0;// re-init to allow a non-confusing vision in the debugger.
            curEpsilon = 0.0;// re-init to allow a non-confusing vision in the debugger.
            RealField.RatioReducingInstrument ratioMethod = new RealField.RatioReducingInstrument(prunRes.tensor_Num, prunRes.tensor_Den);
            RealField.RatioReducingResults ratioSimplified = ratioMethod.RationalReductionOnOmegaData();
            resultLogger.Append(
            RealField.TestsOnRationals.ConsoleTensor_Ratio_Publisher_onStringBuilder("\r\n from RatioReduce "
            , ratioSimplified)
            );
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("ratioSimplified.Num_simplified_factorization ", ratioSimplified.Num_simplified_factorization));
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("ratioSimplified.Den_simplified_factorization ", ratioSimplified.Den_simplified_factorization));
            double numPostRationalReduce = RealField.TestsOnRationals.mulUp(ratioSimplified.Num_simplified_factorization);
            double denPostRationalReduce = RealField.TestsOnRationals.mulUp(ratioSimplified.Den_simplified_factorization);
            double totalAfterRationalReduce = numPostRationalReduce / denPostRationalReduce;
            resultLogger.Append("\r\n TOTAL from RatioReduce == ");
            resultLogger.Append(totalAfterRationalReduce.ToString());
            AnalyzedObj_Magnitudo = getMagnitudoOrder(totalAfterRationalReduce);// NB. I'm reusing variables frome the Prune tests.
            Error_Magnitudo = getMagnitudoOrder(Math.Abs(totalPrePrune - totalAfterRationalReduce));
            figureNonDisponibili = (int)AnalyzedObj_Magnitudo - 15;// figure non rappresentabili, i.e. errore
            curEpsilon = +9.0 * Math.Pow(+10.0, figureNonDisponibili);// la figura sulla quale si manifesta l'errore e' disponibileper tutta la sua magnitudo( i.e. fino al "9").
            // log for error on RatioReduce ----------------
            resultLogger.Append("\r\n error on RationalReduce : Abs(totalPrePrune - totalAfterRationalReduce)== ");
            resultLogger.Append(Math.Abs(totalPrePrune - totalAfterRationalReduce).ToString());
            resultLogger.Append("\r\n AnalyzedObj_Magnitudo==");
            resultLogger.Append(AnalyzedObj_Magnitudo.ToString());
            resultLogger.Append("\r\n Error_Magnitudo==");
            resultLogger.Append(Error_Magnitudo.ToString());
            resultLogger.Append("\r\n\r\n");
            // end : log for error on RatioReduce ----------------
            if (Math.Abs(totalPrePrune - totalAfterRationalReduce) < curEpsilon)// variable epsilon==epsilon( magnitude)
            {
                //ok
            }
            else
            {
                mustLog = true;
            }
            if (Math.Abs(totalPrePrune - totalAfterRationalReduce) < fixedEpsilon)//  this is the FIXED epsilon, a param.
            {
                //ok
            }
            else
            {
                if (ForceVerbosity)
                {
                    mustLog = true;
                }
            }
            //
            resultLogger.Append("\r\n #####--- end SINGLE test  ");
            resultLogger.Append("  ----##########################################\r\n\r\n");
            //----------------------------------------------------------
            if (mustLog)
            {
                Console.WriteLine(resultLogger.ToString());
            }// else skip.
            resultLogger = null;// gc
        }// end : Prune_and_RationalReduce_test()



        /// <summary>
        /// perform a single test only, on a factorization provided by the tester-programmer, as a single long[,] tensor.
        /// Prune_and_RationalReduce_test() : receive a single long[,] tensor.
        /// </summary>
        /// <param name="myFactorization"></param>
        /// <param name="cardTests"></param>
        /// <param name="fixesEpsilon"></param>
        /// <param name="verbosity"></param>
        public static void Prune_and_RationalReduce_test( long[,] SignedArray, double fixedEpsilon, bool ForceVerbosity )
        {
            bool mustLog = false;// activated on error or on request.
            System.Text.StringBuilder resultLogger = new System.Text.StringBuilder();
            resultLogger.Append("\r\n\r\n@@@@@--- start SINGLE test  ");
            resultLogger.Append(" ----@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@\r\n");
            resultLogger.Append("\r\n  the provided long[,] tensor is : ");
            resultLogger.Append(SignedArray);// present the isolated factorization. ///// TODO    
            //
            // tests on pruning
            double totalPrePrune = RealField.TestsOnRationals.mulUp(SignedArray);
            resultLogger.Append("\r\n  the MonteCarlo extracted stringFactor TOTAL is : ");
            resultLogger.Append(totalPrePrune.ToString());
            resultLogger.Append("\r\n original input for pruning");
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("the Signed Array : ", SignedArray));
            RealField.PruningInstrument thePruner_method = new RealField.PruningInstrument(SignedArray);
            RealField.PruningResults prunRes = thePruner_method.Prune();
            resultLogger.Append("\r\n  the pruned Ratio ");
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("prunRes.tensor_Num ", prunRes.tensor_Num));
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("prunRes.tensor_Den ", prunRes.tensor_Den));
            double numPostPrune = RealField.TestsOnRationals.mulUp(prunRes.tensor_Num);
            double denPostPrune = RealField.TestsOnRationals.mulUp(prunRes.tensor_Den);
            double totalAfterPrune = numPostPrune / denPostPrune;
            double AnalyzedObj_Magnitudo = getMagnitudoOrder(totalPrePrune);
            double Error_Magnitudo = getMagnitudoOrder(Math.Abs(totalPrePrune - totalAfterPrune));
            int figureNonDisponibili = (int)AnalyzedObj_Magnitudo - 15;// figure non rappresentabili, i.e. errore
            double curEpsilon = +9.0 * Math.Pow(+10.0, figureNonDisponibili);// la figura sulla quale si manifesta l'errore e' disponibileper tutta la sua magnitudo( i.e. fino al "9").
            // log for error on Prune ----------------
            resultLogger.Append("\r\n error on Prune : Abs(totalPrePrune - totalAfterPrune)== ");
            resultLogger.Append(Math.Abs(totalPrePrune - totalAfterPrune).ToString());
            resultLogger.Append("\r\n AnalyzedObj_Magnitudo==");
            resultLogger.Append(AnalyzedObj_Magnitudo.ToString());
            resultLogger.Append("\r\n Error_Magnitudo==");
            resultLogger.Append(Error_Magnitudo.ToString());
            resultLogger.Append("\r\n\r\n");
            // end : log for error on Prune ----------------
            //
            if (Math.Abs(totalPrePrune - totalAfterPrune) < curEpsilon)//  this is the variable epsilon, related to the magnitudo.
            {
                //ok
            }
            else
            {
                mustLog = true;
            }
            if (Math.Abs(totalPrePrune - totalAfterPrune) < fixedEpsilon)//  this is the FIXED epsilon, a param.
            {
                //ok
            }
            else
            {
                if (ForceVerbosity)
                {
                    mustLog = true;
                }
            }
            // end tests on pruning
            //
            //-------------
            //
            // tests on ratioReducing
            figureNonDisponibili = 0;// re-init to allow a non-confusing vision in the debugger.
            curEpsilon = 0.0;// re-init to allow a non-confusing vision in the debugger.
            RealField.RatioReducingInstrument ratioMethod = new RealField.RatioReducingInstrument(prunRes.tensor_Num, prunRes.tensor_Den);
            RealField.RatioReducingResults ratioSimplified = ratioMethod.RationalReductionOnOmegaData();
            resultLogger.Append(
            RealField.TestsOnRationals.ConsoleTensor_Ratio_Publisher_onStringBuilder("\r\n from RatioReduce "
            , ratioSimplified)
            );
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("ratioSimplified.Num_simplified_factorization ", ratioSimplified.Num_simplified_factorization));
            resultLogger.Append(GenericTensor_Publisher_onStringBuilder("ratioSimplified.Den_simplified_factorization ", ratioSimplified.Den_simplified_factorization));
            double numPostRationalReduce = RealField.TestsOnRationals.mulUp(ratioSimplified.Num_simplified_factorization);
            double denPostRationalReduce = RealField.TestsOnRationals.mulUp(ratioSimplified.Den_simplified_factorization);
            double totalAfterRationalReduce = numPostRationalReduce / denPostRationalReduce;
            resultLogger.Append("\r\n TOTAL from RatioReduce == ");
            resultLogger.Append(totalAfterRationalReduce.ToString());
            AnalyzedObj_Magnitudo = getMagnitudoOrder(totalAfterRationalReduce);// NB. I'm reusing variables frome the Prune tests.
            Error_Magnitudo = getMagnitudoOrder(Math.Abs(totalPrePrune - totalAfterRationalReduce));
            figureNonDisponibili = (int)AnalyzedObj_Magnitudo - 15;// figure non rappresentabili, i.e. errore
            curEpsilon = +9.0 * Math.Pow(+10.0, figureNonDisponibili);// la figura sulla quale si manifesta l'errore e' disponibileper tutta la sua magnitudo( i.e. fino al "9").
            // log for error on RatioReduce ----------------
            resultLogger.Append("\r\n error on RationalReduce : Abs(totalPrePrune - totalAfterRationalReduce)== ");
            resultLogger.Append(Math.Abs(totalPrePrune - totalAfterRationalReduce).ToString());
            resultLogger.Append("\r\n AnalyzedObj_Magnitudo==");
            resultLogger.Append(AnalyzedObj_Magnitudo.ToString());
            resultLogger.Append("\r\n Error_Magnitudo==");
            resultLogger.Append(Error_Magnitudo.ToString());
            resultLogger.Append("\r\n\r\n");
            // end : log for error on RatioReduce ----------------
            if (Math.Abs(totalPrePrune - totalAfterRationalReduce) < curEpsilon)// variable epsilon==epsilon( magnitude)
            {
                //ok
            }
            else
            {
                mustLog = true;
            }
            if (Math.Abs(totalPrePrune - totalAfterRationalReduce) < fixedEpsilon)//  this is the FIXED epsilon, a param.
            {
                //ok
            }
            else
            {
                if (ForceVerbosity)
                {
                    mustLog = true;
                }
            }
            //
            resultLogger.Append("\r\n #####--- end SINGLE test  ");
            resultLogger.Append("  ----##########################################\r\n\r\n");
            //----------------------------------------------------------
            if (mustLog)
            {
                Console.WriteLine(resultLogger.ToString());
            }// else skip.
            resultLogger = null;// gc
        }// end : Prune_and_RationalReduce_test() : receive a single long[,] tensor.






        public static int getMagnitudoOrder( double par )
        {
            int MagnitudoOrder = 0;// consider the case of |x|<10 ; the loop must exit without increment, with the MagnitudoOrder of 1.
            double resizedPar = par;
            //
            if ( Math.Abs(par) >= +1.0)
            {
                for (; System.Math.Abs(resizedPar) >= +1.0; MagnitudoOrder++)
                {
                    resizedPar /= 10.0;
                    // debug : Console.WriteLine("MagnitudoOrder = {0}  resizedPar = {1}", MagnitudoOrder, resizedPar);// debug : 
                }
            }
            else if (Math.Abs(par) < double.Epsilon)
            {
                MagnitudoOrder = 0;
            }
            else // if ( Math.Abs(par) < +1.0)
            {
                for (; System.Math.Abs( resizedPar) < +1.0; MagnitudoOrder--)
                {
                    resizedPar *= 10.0;
                    // debug : Console.WriteLine("MagnitudoOrder = {0}  resizedPar = {1}", MagnitudoOrder, resizedPar);// debug : 
                }
            }
            //ready.
            return MagnitudoOrder;
        }// end : public static int getMagnitudoOrder( double par )




    }// end class

}// end nmps
