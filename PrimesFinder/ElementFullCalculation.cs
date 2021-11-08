using System;
using System.Collections.Generic;
using System.Text;


namespace PrimesFinder
{


    public static class ElementFullCalculation
    {

 
        /// <summary>
        /// This method is homologous of Primes::CoreLoop(), but it's static since there is no need of any
        /// instance variables. So no Process is needed to instantiate; interface can call here directly.
        /// This is due to the fact that it does not interact with the integral file dump.txt. It does only punctual 
        /// evaluation of an isolated natural number. Integral functions such as Pi(n) and J(n) need to use the integral file
        /// and so an instance of the class is necessary.
        /// </summary>
        /// <param name="ToBeDivided">input</param>
        /// <param name="LastCandidateDivider">output</param>
        /// <param name="DiophantineQuotient">output</param>
        /// <returns></returns>
        public static bool ElementFullEvaluation(
            Int64 ToBeDivided, 
            out Int64 LastCandidateDivider,
            out Int64 DiophantineQuotient
            )
        {
            //----definition of loop-ralated variables and constants.-----
            const double epsilon = double.Epsilon;// system constant ~+4.9e-324
            bool isStillPotentialPrime = true;
            double Sqrt_ToBeDivided = Math.Sqrt(ToBeDivided);
            double quotient = 0.0;
            Int64 divider;
            DiophantineQuotient = -1L;// init to "non diophantine quotient".
            //
            // Searching a Diophantine quotient in the open (1,n). So 2 is the smallest possible.
            for (
                    divider = 2L;// the prime I'm actually using as divider.// start from the first prime.
                //
                // NB.for the monotonicity of sequence Primes, which is extracted from N( monotonic), successive
                // elements can be only greater. If( divider>Sqrt_ToBeDivided) -> a candidate smaller of divider
                // would be needed, which cannot be found stepping into a monotonic increasing sequence.
                    divider <= Sqrt_ToBeDivided;
                //
                    ++divider
                )
            {
                quotient = (double)ToBeDivided / (double)divider;
                // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(String.Format("{0}={1}/{2}", quotient, ToBeDivided, divider), 0);
                if (Math.Abs(quotient - (Int64)quotient) > epsilon)
                {// quotien is not integer. Potential divisor is not a Diophantine divisor; ->still potentially prime; analyze remaining potential divisors.
                    DiophantineQuotient = -1L;// non diophantine quotient.
                    isStillPotentialPrime = true;
                }
                else// quotien was integer. Potential divisor is a Diophantine divisor.
                {// -> not prime.
                    isStillPotentialPrime = false;
                    DiophantineQuotient = (Int64)quotient;
                    // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(string.Format("{0} is not prime.\r\n", ToBeDivided), 0);
                    break;// inner loop, i.e. don't put to the test other potential dividers; already foud one, which is enough.
                }
            }// end LOOP for each potential divisor.
            //
            // on end of loop, dividend results prime if and only if "isStillPotentialPrime==true".
            // publish it. NB for primes the right value is before last increment, which has never been used, cause it caused the threshold crossing.
            // for non primes, the published value is the right one.
            LastCandidateDivider = divider;
            //
            // ready.
            if (true == isStillPotentialPrime)
            {
                // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(String.Format("{0} is prime.\r\n", ToBeDivided), 0);
                LastCandidateDivider--;//NB for primes the right value is before last increment.
                return true;
            }
            else
            {
                return false;
            }
        }// end method.




        /// <summary>
        /// this class works like a structure; it contains the basic brick of a prime factorization.
        /// It's the couple {prime_factor, multiplicity}.
        /// An array of such structures represents every natural number, as in the Fundamental Arithmetic Theorem.
        /// </summary>
        private class omegaCouple
        {/// An array of such structures represents every natural number, as in the Fundamental Arithmetic Theorem.
            private long primeFactor;// the cardinality of such factors is omega_small.
            private long factorMultiplicity;// the sum of these multiplicities is omega_big.
            //

            /// <summary>
            /// first factor insertion. When this constructor is used, the multiplicity is always one, since it's the first
            /// role for that factor.
            /// </summary>
            /// <param name="theFactor"></param>
            /// <param name="Multiplicity"></param>
            public omegaCouple( long theFactor, long Multiplicity)
            {
                this.primeFactor = theFactor;
                this.factorMultiplicity = Multiplicity;
            }

            /// <summary>
            /// add multiplicity to an existing factor, when it raises during the factorization process.
            /// </summary>
            /// <param name="theMultiplicity"></param>
            public void setMultiplicity( long theMultiplicity)
            {
                this.factorMultiplicity = theMultiplicity;
            }

            public long get_primeFactor()
            {
                return this.primeFactor;
            }// end public long get_PrimeFactor()

            public long get_factorMultiplicity()
            {
                return this.factorMultiplicity;
            }// end public long get_factorMultiplicity()

        }// end  class omegaCouple



        /// <summary>
        /// No instance underneath. This core loop is static.
        /// Synchronous action. No thread forked.
        /// Omega_small and Omega_capital are two fundamental arithmetical indicators.
        /// Omega_small is the number of primes that factor the argument.
        /// Omega_capital is the sum of multiplicities of the primes that factor the argument.
        /// </summary>
        /// <param name="theIsolatedNatural"></param>
        public static long[,] OmegaData( Int64 theIsolatedNatural )
        {
            if (+1L > theIsolatedNatural)
            {
                throw new System.Exception("Omega functions are defined for n in N, n>=+1.");
            }// else can continue.
            //
            long[,] omegaData = null;// this will be imported from an ArrayList, since we don't know yet how many factors we have.
            Int64 LastCandidateDivider = default(Int64);
            Int64 DiophantineQuotient = 2L;// init to >1.
            bool res = true;
            Int64 loop_theIsolatedNatural = theIsolatedNatural;// init.
            System.Collections.ArrayList factors = new System.Collections.ArrayList();
            //
            while (
                 1 < DiophantineQuotient
                )
            {
                res =
                    PrimesFinder.ElementFullCalculation.ElementFullEvaluation(
                        loop_theIsolatedNatural,
                        out LastCandidateDivider,
                        out DiophantineQuotient
                    );
                if (1 < DiophantineQuotient)
                {
                    factors.Add(LastCandidateDivider);
                }
                else
                {//NB. when the isolatedNatural leads to a quotient==-1, it means it must be divided for itsself and give quotient==1.
                    factors.Add(loop_theIsolatedNatural);
                }
                // after usage, refresh loop engine.
                loop_theIsolatedNatural = DiophantineQuotient;// refresh loop engine.
            }
            // build the response.
            //
            long currentPrime = (long)(factors[0]);// I need it already initialized.
            long factorAccumulator = 1;// I need it already initialized.
            long currentMultiplicity = 0;// I need it already initialized.
            System.Collections.ArrayList factorLager = new System.Collections.ArrayList();
            factorLager.Add(new omegaCouple(currentPrime, +1));// init 
            //
            for (int c = 0; c < factors.Count; c++)
            {
                if (currentPrime == (long)(factors[c]))
                {
                    // currentPrime stays the same.
                    // factorAccumulator stays the same.
                    currentMultiplicity++;
                    ((omegaCouple)(factorLager[(int)(factorAccumulator-1)])).setMultiplicity(currentMultiplicity);
                }
                else
                {
                    currentPrime = (long)(factors[c]);// update the current prime.
                    factorAccumulator++;// a new prime facor.
                    currentMultiplicity = 1;
                    factorLager.Add(new omegaCouple( currentPrime, currentMultiplicity));
                }
            }// end for (int c = 0; c < factors.Count; c++)
            //
            omegaData = new long[factorAccumulator, 2];
            for (int c = 0; c < factorAccumulator; c++)
            {
                omegaData[c, 0] = ((omegaCouple)(factorLager[c])).get_primeFactor();
                omegaData[c, 1] = ((omegaCouple)(factorLager[c])).get_factorMultiplicity();
            }// end for (int c = 0; c < factors.Count; c++)
            //
            // ready.
            return omegaData;
        }// end public static int[,] OmegaData( Int64 theIsolatedNatural )


        public static int Omega_small( Int64 theIsolatedNatural )
        {
            if (+1L > theIsolatedNatural)
            {
                throw new System.Exception("Omega functions are defined for n in N, n>=+1.");
            }// else can continue.
            //
            int omega_small = -1;// init; I will reset it.
            //
            long[,] omegaData = PrimesFinder.ElementFullCalculation.OmegaData(theIsolatedNatural);
            omega_small = omegaData.Length / 2;
            if ((omega_small == +1) && (omegaData[0, 0] == +1))
            {
                omega_small = 0;// since (+1) is no prime number.
            }// else continue.
            // ready
            return omega_small;
        }// end  Omega_small( Int64 theIsolatedNatural )


        public static long Omega_big( Int64 theIsolatedNatural )
        {
            if (+1L > theIsolatedNatural)
            {
                throw new System.Exception("Omega functions are defined for n in N, n>=+1.");
            }// else can continue.
            //
            long omega_big = 0;// init; I will sum on it.
            //
            long[,] omegaData = PrimesFinder.ElementFullCalculation.OmegaData(theIsolatedNatural);
            int omega_small = omegaData.Length / 2;
            if ((omega_small == +1) && (omegaData[0, 0] == +1))
            {
                omega_small = 0;// since (+1) is no prime number.
            }// else continue.
            for (int c = 0; c < omega_small; c++)
            {
                omega_big += omegaData[c, 1];// in column the second there are the multiplicities;
            }// omega_big is their sum.
            // ready
            return omega_big;
        }// end  Omega_big( Int64 theIsolatedNatural )


        public static int Lambda_Liouville( Int64 theIsolatedNatural )
        {
            double Lambda = -1;
            long omega_big = PrimesFinder.ElementFullCalculation.Omega_big( theIsolatedNatural);
            Lambda = Math.Pow(-1.0, (double)omega_big);
            // ready
            return (int)Lambda;
        }// end int Lambda_Liouville( Int64 theIsolatedNatural )


        public static int DiracDelta_Omegas( Int64 theIsolatedNatural )
        {
            int DiracDelta_onOmegas = -1;
            // separate calls to omega_small and omega_big would cause two calls to omegaData() which is expensive.
            long[,] omegaData = PrimesFinder.ElementFullCalculation.OmegaData(theIsolatedNatural);// NB. call it only once.
            int omega_small = omegaData.Length / 2;
            long omega_big = 0L;
            for (int c = 0; c < omega_small; c++)
            {
                omega_big += omegaData[c, 1];// in column the second there are the multiplicities;
            }// omega_big is their sum.
            if (omega_small == (int)omega_big)
            {
                DiracDelta_onOmegas = +1;
            }
            else
            {
                DiracDelta_onOmegas = 0;
            }
            // ready
            return DiracDelta_onOmegas;
        }// end int DiracDelta_Omegas( Int64 theIsolatedNatural )


        public static int Moebius_Mi( Int64 theIsolatedNatural )
        {
            if (+1L > theIsolatedNatural)
            {
                throw new System.Exception("Omega functions are defined for n in N, n>=+1.");
            }// else can continue.
            //
            int DiracDelta_onOmegas = ElementFullCalculation.DiracDelta_Omegas(theIsolatedNatural);
            int Lambda = ElementFullCalculation.Lambda_Liouville(theIsolatedNatural);
            // ready
            return DiracDelta_onOmegas * Lambda;
        }// end  int Moebius_Mi( Int64 theIsolatedNatural )


        /// <summary>
        /// It's an integral function (in the sense of a cumulative function).
        /// NB. have to start from (+1) not from zero !!!!
        /// </summary>
        /// <param name="theIsolatedNatural"></param>
        /// <returns></returns>
        public static long Mertens_M( Int64 theIsolatedNatural )
        {// It's an integral function (in the sense of a cumulative function).
            long Mertens_M = 0L;// sum on it; it's the reduct of a series.
            for (int c = 1; c <= theIsolatedNatural; c++)// NB. have to start from (+1) not from zero !!!!
            {// NB. have to start from (+1) not from zero !!!!
                Mertens_M += ElementFullCalculation.Moebius_Mi(c);
            }// end for : // NB. have to start from (+1) not from zero !!!!
            // ready
            return Mertens_M;
        }// end  long Mertens_M( Int64 theIsolatedNatural )


        /// <summary>
        /// this method is devoted to reduce a fraction to mutually primes numerator and denominator.
        /// Each of them comes as an OmegaData array, i.e. with prime factors and respective multiplicities.
        /// For each OmegaData entry, numerator and denominator are compared to find common factors and in the case the respective
        /// multiplicities are subtracted, as in the power properties.
        /// returns a tensor of two matrices long[n,2], i.e. long[n,2,2]
        /// </summary>
        /// <param name="NumeratorOmegaData"></param>
        /// <param name="DenominatorOmegaData"></param>
        /// <returns></returns>
        public static string[, ,] test_RationalReductionOnOmegaData( string[,] NumeratorOmegaData, string[,] DenominatorOmegaData )
        {
            int righe = Math.Max(NumeratorOmegaData.Length / 2, DenominatorOmegaData.Length / 2);
            int matrici = 2;
            int colonne = 2;
            string[, ,] reducedFractionOmegaData = new string[matrici, righe, colonne];
            // TODO
            for (int mat = 0; mat < matrici; mat++)
            {
                for (int row = 0; row < righe; row++)
                {
                    for (int col = 0; col < colonne; col++)
                    {
                        reducedFractionOmegaData[mat, row, col] = "matrice_" + mat.ToString() + "_riga_" + row.ToString() + "_colonna_" + col.ToString();
                    }// ed for col
                }//end for row
            }//end for mat
            //
            /*
             * avere un accumulatore di inserimento per la matrice num ed un altro per quella den.
             * inizializzare le due matrici a tutti "1", cosicchè i posti inutilizzati risultino invarianti del prodotto.
            for( each_element of num)
             * {
             * for( each_element of den)
             * {
             *      if(base_num==base_den)
             *      {
             *          esponente_final = esponente_num - esponente_den;
             *          if( esponente_final > 0)
             *          {
             *              inserire in matrice numeratore
             *          }
             *          else if( esponente_final < 0)
             *          {
             *              inserire in matrice denominatore
             *          }
             *          // else // i.e.  esponente_final == 0 -> non inserire alcundove.
             *          break;// NB. once found the omologous basis, stop searching for it.
             *      }// end if(base_num==base_den) else continue searching
             */
            //ready.
            return reducedFractionOmegaData;
        }// end RationalReductionOnOmegaData




        /// <summary>
        /// TODO : documentare abbondantemente !
        /// </summary>
        /// <param name="NumeratorOmegaData"></param>
        /// <param name="DenominatorOmegaData"></param>
        /// <returns></returns>
        public static long[, ,] RationalReductionOnOmegaData( long[,] NumeratorOmegaData, long[,] DenominatorOmegaData )
        {
            int matrici = 2;
            int righe = NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2;
            int colonne = 2;
            long[, ,] reducedFractionOmegaData = new long[matrici, righe, colonne];
            // pre-init the response-tensor with product invariant +1.
            for (int mat = 0; mat < matrici; mat++)
            {
                for (int row = 0; row < righe; row++)
                {
                    for (int col = 0; col < colonne; col++)
                    {
                        reducedFractionOmegaData[mat, row, col] = +1;
                    }// ed for col
                }//end for row
            }//end for mat
            //
            long[,] signedExponentResult = new long[NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2, 2];
            int accResultInsertion = 0;
            for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
            {// pre-init with numerator's factors.
                signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base i.e. factor
                signedExponentResult[accResultInsertion, 1] = NumeratorOmegaData[element_num, 1];// exponent i.e. multiplicity
                accResultInsertion++;// one factor^multiplicity inserted.
            }// end for : pre-init with numerator's factors.
            // DON'T   re-init  accResultInsertion.
            for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
            {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
                for (int element_den = 0; element_den < DenominatorOmegaData.Length / 2; element_den++)
                {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
                    if (NumeratorOmegaData[element_num, 0] == DenominatorOmegaData[element_den, 0])
                    {// in case a denominator's factor matches with a numerator's factor : simplify the exponents (i.e. the multiplicity).
                        bool isAlreadyTreatedFacor = false;// check if such factor is already present in the treatd ones' list.
                        int readOnes = 0;// a counter to step in the already treated factors.
                        for ( ; readOnes < accResultInsertion; readOnes++)
                        {// is a factor has already been treated :
                            if (DenominatorOmegaData[element_den, 0] == signedExponentResult[readOnes, 0])
                            {// then stop searching : it will be overwritten with the updated exponent.
                                isAlreadyTreatedFacor = true;
                                break;// the current factor has been already treated.
                            }// else continue searching.
                        }// end search in already treated factors.
                        if (!isAlreadyTreatedFacor)// if the current factor was never treated before insert it as a brand new one.
                        {// if the current factor was never treated before insert it as a brand new one.
                            long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
                            signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base
                            signedExponentResult[accResultInsertion, 1] = esponente_final;// exponent
                            accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
                        }// else don't repeat an already treated factor.
                        else// i.e. if the current factor is already present between the treated ones
                        {// sostituzione del fattore inserito senza match, con quello matchato( i.e. update its exponent).
                            long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
                            signedExponentResult[readOnes, 0] = NumeratorOmegaData[element_num, 0];// base
                            signedExponentResult[readOnes, 1] = esponente_final;// exponent
                            // DON'T go ahead : the place was already existing. accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
                        }// end else :  sostituzione del fattore inserito senza match, con quello matchato.( i.e. update its exponent).
                    }// end if(base_num==base_den) else continue searching
                    else // no match in current factor
                    {
                        bool isAlreadyTreatedFacor = false;
                        for (int readOnes = 0; readOnes < accResultInsertion; readOnes++)
                        {
                            if (DenominatorOmegaData[element_den, 0] == signedExponentResult[readOnes, 0])
                            {
                                isAlreadyTreatedFacor = true;
                                break;// the current facto has been already treated.
                            }// else continue searching.
                        }// end search in already treated factors.
                        if (!isAlreadyTreatedFacor)
                        {
                            long esponente_final = (-1) * DenominatorOmegaData[element_den, 1];// mul_(-1) because are exponents at denominator.
                            signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];
                            signedExponentResult[accResultInsertion, 1] = esponente_final;
                            accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
                        }// else don't repeat an already treated factor.
                    }// end else // no match in current factor
                }// end for element_den
            }// end for element_num
            //the simplified matrix, which collects together factors with positive and with negative exponents, is now ready.
            // this method returns a tensor of two matrices mx2 long[2, m,2]; i must fill it up, with the data of the simplified matrix.
            // the following loop is devoted to that:
            int accNumInsertion = 0;
            int accDenInsertion = 0;
            for (int row = 0; row < righe; row++)
            {// in the tensor, matrix[0,n,m] is the one with positive exponents (i.e. the numerator).
                if (signedExponentResult[row, 1] > 0)// num== matrice[0,n,m]
                {// num== matrice[0,n,m]
                    reducedFractionOmegaData[0, accNumInsertion, 0] = signedExponentResult[row, 0];// base
                    reducedFractionOmegaData[0, accNumInsertion, 1] = signedExponentResult[row, 1];// exp
                    accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
                }// end insertion in numerator.
                else if (signedExponentResult[row, 1] < 0)// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
                {// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
                    reducedFractionOmegaData[1, accDenInsertion, 0] = signedExponentResult[row, 0];// base
                    reducedFractionOmegaData[1, accDenInsertion, 1] = signedExponentResult[row, 1];// exp
                    accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
                }// end insertion in denominator.
                else // exp==multiplicity==0 -> write +1.
                {// exp==multiplicity==0 -> write +1.
                    reducedFractionOmegaData[0, accNumInsertion, 0] = +1;// base
                    reducedFractionOmegaData[0, accNumInsertion, 1] = +1;// exp
                    accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
                    // exp==multiplicity==0 -> write +1.
                    reducedFractionOmegaData[1, accDenInsertion, 0] = +1;// base
                    reducedFractionOmegaData[1, accDenInsertion, 1] = +1;// exp
                    accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
                }// end // exp==multiplicity==0 -> don't write anywhere.
            }// end for : insertion in final tensor, from simplified matrix.
            //ready.
            return reducedFractionOmegaData;// return the tensor.
        }// end RationalReductionOnOmegaData


    }// end class.


}// end nmsp.
