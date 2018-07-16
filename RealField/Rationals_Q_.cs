using System;



namespace RealField
{

    public class PruningResults
    {
        public long[,] tensor_Num;
        public long[,] tensor_Den;
    }


    public class PruningInstrument
    {
        // data fields
        public long[,] input_SignedArray;// valorizzato da costruttore.
        public PruningResults output_pruningResults = null;// valorizzato dal metodo Prune() come output e return_val.

        public PruningInstrument( long[,] the_SignedArray )
        {
            this.input_SignedArray = the_SignedArray;
        }// end : Ctor()


        /// <summary>
        /// Buble-sort based rational-Q-Prune.
        /// trasforma due prodotti di interi con segno elevati ad esponente intero con segno, in due vettori di interi positivi, rispettivamente con
        /// esponente positivo( i.e. al numeratore) ed esponente negativo( i.e. al denominatore).
        /// Il segno della frazione è rappresentato al numeratore.
        /// Vengono raccolti i fattori comuni nell'ambito del numeratore ed altrettanto nell'ambito del denominatore.
        /// Non viene eseguita la semplificazione fra fattori del numeratore e fattori del denominatore. Per questo chiamare RationalReduction().
        /// I fattori, sia di numeratore che di denominatore, vengono ordinati per modulo crescente.
        /// Il segno di ciascun fattore contribuisce al segno della frazione, in base alla parita' del suo esponente.
        /// Il segno dell'esponenete determina la destinazione di un fattore a numeratore o a denominatore.
        /// </summary>
        /// <param name="SignedArray">input from Ctor()</param>
        /// <param name="Num">output in data field</param>
        /// <param name="Den">output in data field</param>
        public RealField.PruningResults Prune()
        {
            //---prepare to BubbleSort, in order to position the factors in growing modulus order(N.B. repeated factors are grouped)---
            long signFlag = -33333L;// init to invalid : let the BubbleSorter produce the SignFlag.
            Common.BubbleSort.BubbleSort_Specific_forFactorCouple bubbleSorter = new Common.BubbleSort.BubbleSort_Specific_forFactorCouple(this.input_SignedArray, out signFlag);
            bubbleSorter.BubbleSort_movingEngine_();// do the position changes : // done sorting.
            this.input_SignedArray = bubbleSorter.returnSignedArray();// swap the originary data field with the sorted data.
            //--#################################################### end Bubble-Sort#####################################
            //---start compiling numerator and denominator, based on the BubbleSorted this.input_SignedArray.
            long prevNumBase = +1L;
            long curNumBase = +1L;
            int numeratorFactorCardinality = +1;
            long prevDenBase = +1L;
            long curDenBase = +1L;
            int denominatorFactorCardinality = +1;
            for (int c = 0; c < this.input_SignedArray.Length / 2; c++)// start ################## num & denom cardinality evaluation. ###############
            {// start ################################ num & denom cardinality evaluation. ####################
                if (this.input_SignedArray[c, 1] > 0)// evaluate exponent's sign.
                {
                    curNumBase = this.input_SignedArray[c, 0];
                    if (System.Math.Abs(curNumBase) > +1L && System.Math.Abs(curNumBase) != System.Math.Abs(prevNumBase))
                    {
                        numeratorFactorCardinality++;
                    }// else : the product invariant doesn't count.
                    prevNumBase = curNumBase;// update.
                }
                else if (this.input_SignedArray[c, 1] < 0)// evaluate exponent's sign.
                {
                    curDenBase = this.input_SignedArray[c, 0];
                    if (System.Math.Abs(curDenBase) > +1L && System.Math.Abs(curDenBase) != System.Math.Abs(prevDenBase))
                    {
                        denominatorFactorCardinality++;
                    }// else : the product invariant doesn't count.
                    prevDenBase = curDenBase;// update.
                }
                else //  (this.input_SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
                {
                    // do nothing.
                }
            }// end : ################################ num & denom cardinality evaluation. ####################
            //
            //---ready to prepare the return variable.
            this.output_pruningResults = new PruningResults();
            this.output_pruningResults.tensor_Num = new long[numeratorFactorCardinality, 2];//  raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
            this.output_pruningResults.tensor_Den = new long[denominatorFactorCardinality, 2];// raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
            prevNumBase = +1L;
            curNumBase = +1L;
            prevDenBase = +1L;
            curDenBase = +1L;
            int curGroupedElement_Num = 0;
            int curGroupedElement_Den = 0;
            //
            this.output_pruningResults.tensor_Num[curGroupedElement_Num, 0] = signFlag;// NB.---#### SignFlag ####---only here---
            this.output_pruningResults.tensor_Num[curGroupedElement_Num, 1] = +1L;
            curGroupedElement_Num++; // invariant inserted.
            //
            this.output_pruningResults.tensor_Den[curGroupedElement_Den, 0] = +1L;
            this.output_pruningResults.tensor_Den[curGroupedElement_Den, 1] = +1L;
            curGroupedElement_Den++; // invariant inserted.
            //
            for (int c = 0; c < this.input_SignedArray.Length / 2; c++)//--########## num & denom compilation ##########################################
            {//--########## num & denom compilation ##########################################
                if (this.input_SignedArray[c, 1] > 0)// evaluate exponent's sign: positive exponent means that factor belongs to numerator.
                {
                    curNumBase = System.Math.Abs(this.input_SignedArray[c, 0]);
                    if (System.Math.Abs(curNumBase) > +1L && curNumBase != prevNumBase)
                    {
                        this.output_pruningResults.tensor_Num[curGroupedElement_Num, 0] = curNumBase;
                        this.output_pruningResults.tensor_Num[curGroupedElement_Num, 1] = this.input_SignedArray[c, 1];
                        curGroupedElement_Num++; // new base found.
                    }// else : there is already such a bese -> Sum exponents.
                    else
                    {
                        if (curGroupedElement_Num - 1 > 0 && this.input_SignedArray[c, 0] != +1)
                        {
                            this.output_pruningResults.tensor_Num[curGroupedElement_Num - 1, 1] += System.Math.Abs(this.input_SignedArray[c, 1]);// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
                        }// else don't chenge exponent to the signFlag.
                    }
                    prevNumBase = curNumBase;// update.
                }
                else if (this.input_SignedArray[c, 1] < 0)// evaluate exponent's sign: negative exponent means that factor belongs to denominator.
                {//##############// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
                    curDenBase = System.Math.Abs(this.input_SignedArray[c, 0]);
                    if (System.Math.Abs(curDenBase) > +1L && curDenBase != prevDenBase)
                    {
                        this.output_pruningResults.tensor_Den[curGroupedElement_Den, 0] = curDenBase;
                        this.output_pruningResults.tensor_Den[curGroupedElement_Den, 1] = (-1L) * this.input_SignedArray[c, 1];// correct negative exponents in positive, since they belong now to the denominator.
                        curGroupedElement_Den++; // new base found.
                    }// else : there is already such a bese -> Sum exponents.
                    else
                    {
                        if (curGroupedElement_Den - 1 > 0 && this.input_SignedArray[c, 0] != +1)
                        {
                            this.output_pruningResults.tensor_Den[curGroupedElement_Den - 1, 1] += System.Math.Abs(this.input_SignedArray[c, 1]);// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
                        }// else don't chenge exponent to the signFlag.
                    }
                    prevDenBase = curDenBase;// update.
                }// end : ###########// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
                else //  (this.input_SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
                {//  (this.input_SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
                    // do nothing.
                }// end :  (this.input_SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
            }// end : ####### num & denom compilation ##########################################
            // ready.
            return this.output_pruningResults;// it's also a class data field.
        }// end public  void Prune() acts on class data fields.


    }// end : public class PruningInstrument



    public class RatioReducingResults
    {
        public long[,] signedExponentResult;
        public long[,] Num_simplified_factorization;
        public long[,] Den_simplified_factorization;
    }


    public class RatioReducingInstrument
    {
        long[,] input_Num;// valorizzato da Ctor()
        long[,] input_Den;// valorizzato da Ctor()
        public RatioReducingResults output_ratioReducingResults = null;// valorizzato dal metodo RationalReduction() come output e return_val.

        public RatioReducingInstrument( long[,] the_input_Num, long[,] the_input_Den )
        {
            this.input_Num = the_input_Num;
            this.input_Den = the_input_Den;
        }// end : Ctor() : RatioReducingInstrument( long[,] input_Num, long[,] input_Den )

                

        /// <summary>
        /// This method concatenates two factorizaions, in one that includes the factors of both.
        /// It means to obtain the product of the two factors.
        /// As it's written, it is suitable both for integers and rationals.
        /// An uphill filter could stop the rational factors, for calls from a context which allows integers only.
        /// This core is then suitable for both.
        /// </summary>
        /// <param name="firstOne">first factor</param>
        /// <param name="secondOne">second factor</param>
        /// <returns>a factorizaion tensor, that includes the factors of both the input parameters</returns>
        public long[,] factorizationConcatenator( long[,] firstOne, long[,] secondOne )
        {// the addend tensors are [ownRows, 2] since they both are factorization vectors, i.e. vectors of the couple [base, exp].
            // so the sum tensor has cardinality [rows_firstOne + rows_secondOne, 2].
            long[,] res = new long[firstOne.Length / 2 + secondOne.Length / 2, 2];// rows are the sum of the rows; columns are two in the result too.
            int c = 0;// declared outside the loops, since it has to serve both.
            for (/*declared outside the loops, since it has to serve both.*/; c < firstOne.Length / 2; c++)
            {
                //// for debug purposes : Console.WriteLine(c);
                res[c, 0] = firstOne[c, 0];// copy the base
                res[c, 1] = firstOne[c, 1];// copy the exponent
            }// end : copy the firstOne.
            for ( /*NB. don't re-init! counter must proceed from where it just got.*/; c < firstOne.Length / 2 + secondOne.Length / 2; c++)
            {
                // for debug purposes : Console.WriteLine("{0}   {1}  ", c,  -(firstOne.Length / 2 - c));
                res[c, 0] = secondOne[-(firstOne.Length / 2 - c), 0];// copy the base. --###--NB. carefull with the row index !
                res[c, 1] = secondOne[-(firstOne.Length / 2 - c), 1];// copy the exponent. --###--NB. carefull with the row index !
            }// end : copy the secondOne.
            // ready.
            return res;
        }// end : public long[,] factorizationConcatenator( long[,] firstOne, long[,] secondOne ).



        /// <summary>
        /// it is devoted to adapt a denominator factorization, to let it compliant to be concatenated with a signedArray.
        /// Eg. if I have  -1/(3*7) this method should receive (3^1*7^1) ad return (3^-1*7^-1) so that the signedArray should
        /// become (-1^1*3^-1*7^-1).
        /// </summary>
        /// <param name="theDenominator">the input factorization. The method will return it elevated to exponent==-1.</param>
        /// <returns></returns>
        public long[,] denominatorInverter( long[,] theDenominator )
        {
            long[,] theInvertedFactor = new long[theDenominator.Length, 2];// columns are always 2 (i.e. base, exp).
            for (int c = 0; c < theDenominator.Length / 2; c++)
            {
                theInvertedFactor[c, 0] = theDenominator[c, 0];// same base
                theInvertedFactor[c, 1] = (-1L) * theDenominator[c, 1];// exponent with its sign changed, since it has to go at the denominator.
            }// end : for.
            // ready.
            return theInvertedFactor;
        }// end : public long[,] denominatorInverter( long[,] theDenominator ).



        
        /// <summary>
        /// works on data fields, filled by Ctor().
        /// returns a data field, qith propertary record layout.
        /// scopo del metodo e' produrre una frazione ai minimi termini(i.e. con numeratore e donominatore primi fra loro) partendo da due 
        /// prodotti di interi con segno elevati ad esponente intero con segno, ovvero un quoziente di razionali.
        /// TODO : studiare mediante test, la combinazione col metodo Prune.
        /// </summary>
        /// <returns>RatioReducingResults which contains the minimized fraction.</returns>
        public RatioReducingResults RationalReductionOnOmegaData()//( long[,] Num, long[,] Den )
        {
            long[,] aWholeSignedArray = this.factorizationConcatenator(
                this.input_Num
                , this.denominatorInverter(this.input_Den));//  (a/b)/(c/d)==((a*d)/(b*c)).
            RealField.PruningInstrument thePruner_whole = new PruningInstrument(aWholeSignedArray);
            PruningResults theFirstPruningResults = thePruner_whole.Prune();
            long[,] NumeratorOmegaData = theFirstPruningResults.tensor_Num;
            long[,] DenominatorOmegaData = theFirstPruningResults.tensor_Den;
            //---ready to prepare the return variable.
            this.output_ratioReducingResults = new RatioReducingResults();
            this.output_ratioReducingResults.signedExponentResult = new long[NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2, 2];
            // init to product invariant.
            for (int curRow = 0; curRow < this.output_ratioReducingResults.signedExponentResult.Length / 2; curRow++)
            {
                for (int curCol = 0; curCol < 2; curCol++)
                {
                    this.output_ratioReducingResults.signedExponentResult[curRow, curCol] = +1L;// init to product invariant.
                }// init to product invariant.
            }// end : init to product invariant.
            //
            int accResultInsertion = 0;
            for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
            {//---################################################## pre-init with numerator's factors.---###################################
                this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base i.e. prime-factor
                this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 1] = NumeratorOmegaData[element_num, 1];// exponent i.e. multiplicity
                accResultInsertion++;// one factor^multiplicity inserted.
            }// end for : pre-init with numerator's factors.
            //-end: --################################################## pre-init with numerator's factors.---###################################
            //######### DON'T   re-init  accResultInsertion : it's an accumulator. We need its present value #######################
            // NB.  se il numeratore ha il solo fattore "1" il denominatore neanche viene inserito : TODO copiarlo da DenominatorOmegaData.
            //
            if (+2L == NumeratorOmegaData.Length)// means if numerator has only the "1" then just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
            {// just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
                for (int element_den = 0; element_den < DenominatorOmegaData.Length / 2; element_den++)// just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
                {// if numerator has only the "1" then just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
                    this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];// base
                    this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 1] = (-1L) * DenominatorOmegaData[element_den, 1];// exp : mul_(-1) because exponents are at denominator.
                    accResultInsertion++;// one factor^multiplicity inserted.
                }// end : for : just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
            }// end : if (+2L == NumeratorOmegaData.Length)// means if numerator has only the "1".
            else if (+2L <= NumeratorOmegaData.Length)// means if numerator has more factors than only the "1"
            {// else if (+2L <= NumeratorOmegaData.Length)// means if numerator has more factors than only the "1"
                for (int element_num = 1; element_num < NumeratorOmegaData.Length / 2; element_num++)// NB. start from first base>1. Don't treat the invariants.
                {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
                    for (int element_den = 1; element_den < DenominatorOmegaData.Length / 2; element_den++)// NB. start from first base>1. Don't treat the invariants.
                    {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
                        if (System.Math.Abs(NumeratorOmegaData[element_num, 0]) == System.Math.Abs(DenominatorOmegaData[element_den, 0]))
                        {// in case a denominator's factor matches with a numerator's factor : simplify the exponents (i.e. the multiplicity).
                            bool isAlreadyTreatedFacor = false;// check if such factor is already present in the treatd ones' list.
                            int readOnes = 0;// a counter to step in the already treated factors.
                            for (; readOnes < accResultInsertion; readOnes++)
                            {// is a factor has already been treated :
                                if (System.Math.Abs(DenominatorOmegaData[element_den, 0]) == System.Math.Abs(this.output_ratioReducingResults.signedExponentResult[readOnes, 0]))
                                {// then stop searching : it will be overwritten with the updated exponent.
                                    isAlreadyTreatedFacor = true;
                                    break;// the current factor has been already treated.
                                }// else continue searching.
                            }// end search in already treated factors.
                            if (!isAlreadyTreatedFacor)// if the current factor was never treated before insert it as a brand new one.
                            {// if the current factor was never treated before insert it as a brand new one.
                                long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
                                this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base
                                this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 1] = esponente_final;// exponent
                                accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
                            }// else don't repeat an already treated factor.
                            else// i.e. if the current factor is already present between the treated ones
                            {// sostituzione del fattore inserito senza match, con quello matchato( i.e. update its exponent).
                                long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
                                this.output_ratioReducingResults.signedExponentResult[readOnes, 0] = NumeratorOmegaData[element_num, 0];// base
                                this.output_ratioReducingResults.signedExponentResult[readOnes, 1] = esponente_final;// exponent
                                // DON'T go ahead : the place was already existing. accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
                            }// end else :  sostituzione del fattore inserito senza match, con quello matchato.( i.e. update its exponent).
                        }// end if(base_num==base_den) else continue searching
                        else // no match in current factor : it's an un-simplifiable factor.
                        { // no match in current factor : it's an un-simplifiable factor.
                            bool isAlreadyTreatedFacor = false;
                            for (int readOnes = 0; readOnes < accResultInsertion; readOnes++)
                            {
                                if (System.Math.Abs(DenominatorOmegaData[element_den, 0]) == System.Math.Abs(this.output_ratioReducingResults.signedExponentResult[readOnes, 0]))
                                {
                                    isAlreadyTreatedFacor = true;
                                    break;// the current factor has been already treated.
                                }// else continue searching.
                            }// end search in already treated factors.
                            if (!isAlreadyTreatedFacor)// it's a new factor.
                            {
                                long esponente_final = (-1) * DenominatorOmegaData[element_den, 1];// mul_(-1) because exponents are at denominator.
                                this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];
                                this.output_ratioReducingResults.signedExponentResult[accResultInsertion, 1] = esponente_final;
                                accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
                            }// else don't repeat an already treated factor.
                        }// end else // no match in current factor
                    }// end for element_den
                }// end for element_num
            }// end : if( +2L<=NumeratorOmegaData.Length)// means if numerator has more factors than only the "1"
            //the simplified matrix, which collects together factors with positive and with negative exponents  is now ready.(i.e. this.output_ratioReducingResults.signedExponentResult).
            // on this.output_ratioReducingResults.signedExponentResult I am going to instantiate a final Pruner().
            RealField.PruningInstrument thePrunedFinal_Instrument = new PruningInstrument(this.output_ratioReducingResults.signedExponentResult);
            RealField.PruningResults thePrunedFinalRatio = thePrunedFinal_Instrument.Prune();
            this.output_ratioReducingResults.Num_simplified_factorization = thePrunedFinalRatio.tensor_Num;
            this.output_ratioReducingResults.Den_simplified_factorization = thePrunedFinalRatio.tensor_Den;
            // ready.
            return this.output_ratioReducingResults;// it's also a data field; for clarity it is returned.
        }// end RationalReductionOnOmegaData


    }// end : public class RatioReducingInstrument






}// nmsp



//#################################################################################################################################
namespace sviluppo
{



}// end namespace sviluppo
//#################################################################################################################################


#region cantina




//    public class Rationals_Q_
//    {
//        public long Numerator;
//        public long Denominator;
//        bool isNumDen_ready = false;
//        //
//        public long[,] Num_factorization;
//        public long[,] Den_factorization;
//        bool is_factorization_ready = false;
//        //
//        public long[,] Num_simplified_factorization;
//        public long[,] Den_simplified_factorization;
//        bool is_simplified_factorization_ready = false;
//        //
//        public long[,] SignedArray;// TODO : stabilire come usarlo
//        public long[,] Num;// TODO : stabilire come usarlo
//        public long[,] Den;// TODO : stabilire come usarlo


//        /// <summary>
//        /// Ctor :
//        /// </summary>
//        /// <param name="theNumerator"></param>
//        /// <param name="theDenominator"></param>
//        public Rationals_Q_( long theNumerator, long theDenominator )
//        {
//            this.Numerator = theNumerator;
//            this.Denominator = theDenominator;
//            isNumDen_ready = true;
//        }// Ctor : public Rationals_Q_( long theNumerator, long theDenominator )


//        /// <summary>
//        /// the user enters a string with the following syntax : "-2^3* 3^4* 7^2  *5^3".
//        /// and obtains the long_couple array : { { -2, 3}, {3, 4}, {7, 2}, {5, 3} } as data field member.
//        /// </summary>
//        /// <param name="factorizationString"></param>
//        /// <returns></returns>
//        public Rationals_Q_( string factorizationString )
//        {

//            string[] factors = factorizationString.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);// first round : extract pi^ei
//            long[,] res = new long[factors.Length, 2];// columns are fixed 2, since they are base and exponent.
//            //
//            for (int c = 0; c < factors.Length; c++)
//            {
//                string[] BaseExp = factors[c].Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);// second round : separate pi and ei, for each couple pi^ei.
//                res[c, 0] = long.Parse(BaseExp[0]);// convert base; throws on syntax error.
//                res[c, 1] = long.Parse(BaseExp[1]);// convert exponent; throws on syntax error.
//                BaseExp = null;// garbage collect.
//            }// end for.
//            // ready.
//            this.SignedArray = res;
//        }// end public long[,] factorizationParser( string factorizationString )


//        /// <summary>
//        /// receives a long_couple array, such as : { { -2, 3}, {3, 4}, {7, 2}, {5, 3} }
//        /// and computes Product[ base_i^exp_i] and returns the grand total.
//        /// </summary>
//        /// <param name="theFactorization"></param>
//        /// <returns></returns>
//        public static double mulUp( long[,] theFactorization )// NB. the rationals are dense in the Reals.
//        {// on integers -> only positive exponents are admitted.
//            double res = +1.0;// we will multiply on it.
//            for (int c = 0; c < theFactorization.Length / 2; c++)
//            {
//                //if (theFactorization[c, 1] < 0L)
//                //{
//                //    throw new System.Exception("Only integer factorizations in this method : no negative exponents allowed.");
//                //}// end throw.
//                res *= System.Math.Pow(theFactorization[c, 0], theFactorization[c, 1]);// i.e. base^exp.
//            }//
//            // ready.
//            return res;
//        }// end public static long addUp( long[,] theFactorization )



//    public void RationalReductionOnIntegers()// non può ricevere razionali, ma solo interi 
//    {
//        long[,] omegaData_this_Num_factorization = PrimesFinder.ElementFullCalculation.OmegaData( System.Math.Abs( (long)this.Numerator));
//        long[,] check_Den_NumFactorization = null;
//        Prune(omegaData_this_Num_factorization, out this.Num_factorization, out check_Den_NumFactorization);
//        if (2 < check_Den_NumFactorization.Length)
//        {
//            throw new System.Exception("Only integer factorizaion can be passed here( i.e. only positive exponents).");
//        }// else ok.
//        if (this.Numerator < 0)
//        {
//            this.Num_factorization[0, 0] *= -1L;
//        }// else ok.
//        //
//        // start denominator treatment.
//        if (0 == this.Denominator)
//        {
//            throw new System.Exception("Division by zero.");
//        }// else ok.
//        long[,] omegaData_this_Den_factorization = PrimesFinder.ElementFullCalculation.OmegaData(System.Math.Abs( (long)this.Denominator));
//        long[,] check_Den_DenFactorization = null;
//        Prune(omegaData_this_Den_factorization, out this.Den_factorization, out check_Den_DenFactorization);
//        if (2 < check_Den_DenFactorization.Length)
//        {
//            throw new System.Exception("Only integer factorizaion can be passed here( i.e. only positive exponents).");
//        }// else ok.
//        if (this.Denominator< 0)
//        {
//            this.Num_factorization[0, 0] *= -1L;//---####---NB. sign corrections always at Numerator, for notation convention.
//        }// else ok.
//        this.is_factorization_ready = true;
//        //
//        this.RationalReductionOnOmegaData(this.Num_factorization, this.Den_factorization);
//        this.is_simplified_factorization_ready = true;
//    }// public bool RationalReductionOnIntegers()


//}// end class : Rationals_Q_




//    public class Rationals_Q_
//    {
//        public long Numerator;
//        public long Denominator;
//        bool isNumDen_ready = false;
//        //
//        public long[,] Num_factorization;
//        public long[,] Den_factorization;
//        bool is_factorization_ready = false;
//        //
//        public long[,] Num_simplified_factorization;
//        public long[,] Den_simplified_factorization;
//        bool is_simplified_factorization_ready = false;
//        //
//        public long[,] SignedArray;// TODO : stabilire come usarlo
//        public long[,] Num;// TODO : stabilire come usarlo
//        public long[,] Den;// TODO : stabilire come usarlo


//        ///// <summary>
//        ///// Ctor :
//        ///// </summary>
//        ///// <param name="theNumerator"></param>
//        ///// <param name="theDenominator"></param>
//        //public Rationals_Q_( long theNumerator, long theDenominator )
//        //{
//        //    this.Numerator = theNumerator;
//        //    this.Denominator = theDenominator;
//        //    isNumDen_ready = true;
//        //}// Ctor : public Rationals_Q_( long theNumerator, long theDenominator )


//        /// <summary>
//        /// the user enters a string with the following syntax : "-2^3* 3^4* 7^2  *5^3".
//        /// and obtains the long_couple array : { { -2, 3}, {3, 4}, {7, 2}, {5, 3} } as data field member.
//        /// </summary>
//        /// <param name="factorizationString"></param>
//        /// <returns></returns>
//        public Rationals_Q_( string factorizationString )
//        {

//            string[] factors = factorizationString.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);// first round : extract pi^ei
//            long[,] res = new long[factors.Length, 2];// columns are fixed 2, since they are base and exponent.
//            //
//            for (int c = 0; c < factors.Length; c++)
//            {
//                string[] BaseExp = factors[c].Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);// second round : separate pi and ei, for each couple pi^ei.
//                res[c, 0] = long.Parse(BaseExp[0]);// convert base; throws on syntax error.
//                res[c, 1] = long.Parse(BaseExp[1]);// convert exponent; throws on syntax error.
//                BaseExp = null;// garbage collect.
//            }// end for.
//            // ready.
//            this.SignedArray = res;
//        }// end public long[,] factorizationParser( string factorizationString )



////        /// <summary>
////        /// Buble-sort based rational-Q-Prune.
////        /// trasforma due prodotti di interi con segno elevati ad esponente intero con segno, in due vettori di interi positivi, rispettivamente con
////        /// esponente positivo( i.e. al numeratore) ed esponente negativo( i.e. al denominatore).
////        /// Il segno della frazione è rappresentato al numeratore.
////        /// Vengono raccolti i fattori comuni nell'ambito del numeratore ed altrettanto nell'ambito del denominatore.
////        /// Non viene eseguita la semplificazione fra fattori del numeratore e fattori del denominatore. Per questo chiamare RationalReduction().
////        /// I fattori, sia di numeratore che di denominatore, vengono ordinati per modulo crescente.
////        /// Il segno di ciascun fattore contribuisce al segno della frazione, in base alla parita' del suo esponente.
////        /// Il segno dell'esponenete determina la destinazione di un fattore a numeratore o a denominatore.
////        /// </summary>
////        /// <param name="SignedArray"></param>
////        /// <param name="Num"></param>
////        /// <param name="Den"></param>
////        public void Prune()// ( out long[,] Num, out long[,] Den ) // long[,] SignedArray,
////        {
//////long[] tmp = new long[2];
//////bool swap = true;
//////int for_loop_accumulator = 0;
////            int signFlag = +1;
////            for (int c = 0; c < SignedArray.Length / 2; c++)////############ Sign-flag management & data-struct translation ----#########
////            {
////                if (SignedArray[c, 0] < 0 // se la base e' negativa
////                    && (System.Math.Abs(SignedArray[c, 1] / 2.0 - SignedArray[c, 1] / 2) > double.Epsilon) // e l'esponente e' dispari
////                    )
////                {
////                    signFlag *= -1;// a negative factor in the product.
////                }// else skip sign change, on positive factors.
////            }////####################################### Sign-flag management & data-struct translation ----###########################
////            //---TODO---
////            Common.BubbleSort.BubbleSort_Specific_forFactorCouple bubbleSorter = new Common.BubbleSort.BubbleSort_Specific_forFactorCouple(this.SignedArray);
////            bubbleSorter.BubbleSort_movingEngine_();// do the position changes.
////            //long[,] res = bubbleSorter.returnSignedArray();// TODO compare with this.SignedArray.
////            this.SignedArray = bubbleSorter.returnSignedArray();// TODO compare with this.SignedArray.
////            //
////            //while (swap && for_loop_accumulator < SignedArray.Length / 2 - 1)//####_Bubble-Sort_#### n-1 for_loops are sufficient.
////            //{//####_Bubble-Sort_#### n-1 for_loops are sufficient.
////            //    swap = false;// reset at each new "for". An entire "for" loop without "swap" is sufficient to desume pruning end.
////            //    //
////            //    for (int c = 0; c < SignedArray.Length / 2 - 1; c++)
////            //    {
////            //        if (System.Math.Abs(SignedArray[c, 0]) < System.Math.Abs(SignedArray[c + 1, 0]))
////            //        {
////            //            // NO swap in this case, since the existing order is the required one.
////            //        }
////            //        else if (System.Math.Abs(SignedArray[c, 0]) > System.Math.Abs(SignedArray[c + 1, 0]))
////            //        {
////            //            tmp[0] = System.Math.Abs(SignedArray[c + 1, 0]);//---temporary is necessary to do not loose the overwritten data.
////            //            tmp[1] = SignedArray[c + 1, 1];
////            //            //
////            //            SignedArray[c + 1, 0] = System.Math.Abs(SignedArray[c, 0]);
////            //            SignedArray[c + 1, 1] = SignedArray[c, 1];
////            //            //
////            //            SignedArray[c, 0] = tmp[0];
////            //            SignedArray[c, 1] = tmp[1];
////            //            //
////            //            swap = true;// this is the real swap. If it takes place at least once in a "for", the pruning is still active.
////            //        }
////            //        else if (System.Math.Abs(SignedArray[c, 0]) == System.Math.Abs(SignedArray[c + 1, 0]))
////            //        {
////            //            SignedArray[c, 0] = System.Math.Abs(SignedArray[c, 0]);
////            //            SignedArray[c, 1] = SignedArray[c, 1];
////            //            //
////            //            SignedArray[c + 1, 0] = System.Math.Abs(SignedArray[c + 1, 0]);
////            //            SignedArray[c + 1, 1] = SignedArray[c + 1, 1];
////            //            //
////            //            // non si falsifica se è stato almeno una volta vero nel "for", quindi non mettere l'istruzione: swap = false
////            //            // resta a false Se_solo non stato vero neanche una volta in tutto il "for".
////            //        }
////            //    }// for
////            //    //
////            //    for_loop_accumulator++;// count the cardinality of the pruning loops : each "for" is a pruning turn.
////            //}// end : while swap &&... //####_Bubble-Sort_#### n-1 for_loops are sufficient.
////            //--#################################################### end Bubble-Sort#####################################
////            //---start compiling numerator and denominator, based on the BubbleSorted SignedArray.
////            long prevNumBase = +1L;
////            long curNumBase = +1L;
////            int numeratorFactorCardinality = +1;
////            long prevDenBase = +1L;
////            long curDenBase = +1L;
////            int denominatorFactorCardinality = +1;
////            for (int c = 0; c < SignedArray.Length / 2; c++)// start ################## num & denom cardinality evaluation. ###############
////            {// start ################################ num & denom cardinality evaluation. ####################
////                if (SignedArray[c, 1] > 0)// evaluate exponent's sign.
////                {
////                    curNumBase = SignedArray[c, 0];
////                    if (System.Math.Abs(curNumBase) > +1L && System.Math.Abs(curNumBase) != System.Math.Abs(prevNumBase) )
////                    {
////                        numeratorFactorCardinality++;
////                    }// else : the product invariant doesn't count.
////                    prevNumBase = curNumBase;// update.
////                }
////                else if (SignedArray[c, 1] < 0)// evaluate exponent's sign.
////                {
////                    curDenBase = SignedArray[c, 0];
////                    if (System.Math.Abs(curDenBase) > +1L && System.Math.Abs(curDenBase) != System.Math.Abs(prevDenBase) )
////                    {
////                        denominatorFactorCardinality++;
////                    }// else : the product invariant doesn't count.
////                    prevDenBase = curDenBase;// update.
////                }
////                else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
////                {
////                    // do nothing.
////                }
////            }// end ################################ num & denom cardinality evaluation. ####################
////            Num = new long[numeratorFactorCardinality, 2];//  raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
////            Den = new long[denominatorFactorCardinality, 2];// raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
////            prevNumBase = +1L;
////            curNumBase = +1L;
////            prevDenBase = +1L;
////            curDenBase = +1L;
////            int curGroupedElement_Num = 0;
////            int curGroupedElement_Den = 0;
////            //
////            Num[curGroupedElement_Num, 0] = signFlag;
////            Num[curGroupedElement_Num, 1] = +1L;
////            curGroupedElement_Num++; // invariant inserted.
////            //
////            Den[curGroupedElement_Den, 0] = +1L;
////            Den[curGroupedElement_Den, 1] = +1L;
////            curGroupedElement_Den++; // invariant inserted.
////            //
////            for (int c = 0; c < SignedArray.Length / 2; c++)//--########## num & denom compilation ##########################################
////            {//--########## num & denom compilation ##########################################
////                if (SignedArray[c, 1] > 0)// evaluate exponent's sign: positive exponent means that factor belongs to numerator.
////                {
////                    curNumBase = System.Math.Abs(SignedArray[c, 0]);
////                    if (System.Math.Abs(curNumBase) > +1L && curNumBase != prevNumBase)
////                    {
////                        Num[curGroupedElement_Num, 0] = curNumBase;
////                        Num[curGroupedElement_Num, 1] = SignedArray[c, 1];
////                        curGroupedElement_Num++; // new base found.
////                    }// else : there is already such a bese -> Sum exponents.
////                    else
////                    {
////                        if (curGroupedElement_Num - 1 > 0 && SignedArray[c, 0] != +1)
////                        {
////                            Num[curGroupedElement_Num - 1, 1] += System.Math.Abs( SignedArray[c, 1]);// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
////                        }// else don't chenge exponent to the signFlag.
////                    }
////                    prevNumBase = curNumBase;// update.
////                }
////                else if (SignedArray[c, 1] < 0)// evaluate exponent's sign: negative exponent means that factor belongs to denominator.
////                {//##############// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
////                    curDenBase = System.Math.Abs(SignedArray[c, 0]);
////                    if (System.Math.Abs(curDenBase) > +1L && curDenBase != prevDenBase)
////                    {
////                        Den[curGroupedElement_Den, 0] = curDenBase;
////                        Den[curGroupedElement_Den, 1] = (-1L) * SignedArray[c, 1];// correct negative exponents in positive, since they belong now to the denominator.
////                        curGroupedElement_Den++; // new base found.
////                    }// else : there is already such a bese -> Sum exponents.
////                    else
////                    {
////                        if (curGroupedElement_Den - 1 > 0 && SignedArray[c, 0] != +1)
////                        {
////                            Den[curGroupedElement_Den - 1, 1] += System.Math.Abs( SignedArray[c, 1]);// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
////                        }// else don't chenge exponent to the signFlag.
////                    }
////                    prevDenBase = curDenBase;// update.
////                }// end : ###########// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
////                else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
////                {//  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
////                    // do nothing.
////                }// end :  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
////            }// end : ####### num & denom compilation ##########################################
////            // ready.
////        }// end public  void Prune() acts on class data fields : long[,] SignedArray, out long[,] Num, out long[,] Den )






//    }// end class.


//public class Rationals_Q_
//{
//    public double Numerator;
//    public double Denominator;
//    bool isNumDen_ready = false;
//    //
//    public long[,] Num_factorization;
//    public long[,] Den_factorization;
//    bool is_factorization_ready = false;
//    //
//    public long[,] Num_simplified_factorization;
//    public long[,] Den_simplified_factorization;
//    bool is_simplified_factorization_ready = false;
//    //
//    public long[,] SignedArray;// TODO : stabilire come usarlo
//    public PruningResults pruningResults = null;// TODO : stabilire come usarlo


//    /// <summary>
//    /// Ctor :
//    /// </summary>
//    /// <param name="theNumerator"></param>
//    /// <param name="theDenominator"></param>
//    public Rationals_Q_( double theNumerator, double theDenominator )
//    {
//        this.Numerator = theNumerator;
//        this.Denominator = theDenominator;
//        isNumDen_ready = true;
//    }// Ctor : public Rationals_Q_( long theNumerator, long theDenominator )



//    ///// <summary>
//    ///// Buble-sort based rational-Q-Prune.
//    ///// trasforma due prodotti di interi con segno elevati ad esponente intero con segno, in due vettori di interi positivi, rispettivamente con
//    ///// esponente positivo( i.e. al numeratore) ed esponente negativo( i.e. al denominatore).
//    ///// Il segno della frazione è rappresentato al numeratore.
//    ///// Vengono raccolti i fattori comuni nell'ambito del numeratore ed altrettanto nell'ambito del denominatore.
//    ///// Non viene eseguita la semplificazione fra fattori del numeratore e fattori del denominatore. Per questo chiamare RationalReduction().
//    ///// I fattori, sia di numeratore che di denominatore, vengono ordinati per modulo crescente.
//    ///// Il segno di ciascun fattore contribuisce al segno della frazione, in base alla parita' del suo esponente.
//    ///// Il segno dell'esponenete determina la destinazione di un fattore a numeratore o a denominatore.
//    ///// </summary>
//    ///// <param name="SignedArray"></param>
//    ///// <param name="Num"></param>
//    ///// <param name="Den"></param>
//    //public RealField.PruningResults Prune_candidate()// ( out long[,] Num, out long[,] Den ) // long[,] SignedArray,
//    //{
//    //    ////####################################### Sign-flag management & data-struct translation ----###########################
//    //    int signFlag = +1;
//    //    for (int c = 0; c < SignedArray.Length / 2; c++)////############ Sign-flag management & data-struct translation ----#########
//    //    {
//    //        if (SignedArray[c, 0] < 0 // se la base e' negativa
//    //            && (System.Math.Abs(SignedArray[c, 1] / 2.0 - SignedArray[c, 1] / 2) > double.Epsilon) // e l'esponente e' dispari
//    //            )
//    //        {
//    //            signFlag *= -1;// a negative factor in the product.
//    //        }// else skip sign change, on positive factors.
//    //    }// end :###################################### Sign-flag management & data-struct translation ----###########################
//    //    //---prepare to BubbleSort, in order to position the factors in growing modulus order(N.B. repeated factors are grouped)---
//    //    Common.BubbleSort.BubbleSort_Specific_forFactorCouple bubbleSorter = new Common.BubbleSort.BubbleSort_Specific_forFactorCouple(this.SignedArray);
//    //    bubbleSorter.BubbleSort_movingEngine_();// do the position changes : // done sorting.
//    //    this.SignedArray = bubbleSorter.returnSignedArray();// swap the originary data field with the sorted data.
//    //    //--#################################################### end Bubble-Sort#####################################
//    //    //---start compiling numerator and denominator, based on the BubbleSorted SignedArray.
//    //    long prevNumBase = +1L;
//    //    long curNumBase = +1L;
//    //    int numeratorFactorCardinality = +1;
//    //    long prevDenBase = +1L;
//    //    long curDenBase = +1L;
//    //    int denominatorFactorCardinality = +1;
//    //    for (int c = 0; c < SignedArray.Length / 2; c++)// start ################## num & denom cardinality evaluation. ###############
//    //    {// start ################################ num & denom cardinality evaluation. ####################
//    //        if (SignedArray[c, 1] > 0)// evaluate exponent's sign.
//    //        {
//    //            curNumBase = SignedArray[c, 0];
//    //            if (System.Math.Abs(curNumBase) > +1L && System.Math.Abs(curNumBase) != System.Math.Abs(prevNumBase))
//    //            {
//    //                numeratorFactorCardinality++;
//    //            }// else : the product invariant doesn't count.
//    //            prevNumBase = curNumBase;// update.
//    //        }
//    //        else if (SignedArray[c, 1] < 0)// evaluate exponent's sign.
//    //        {
//    //            curDenBase = SignedArray[c, 0];
//    //            if (System.Math.Abs(curDenBase) > +1L && System.Math.Abs(curDenBase) != System.Math.Abs(prevDenBase))
//    //            {
//    //                denominatorFactorCardinality++;
//    //            }// else : the product invariant doesn't count.
//    //            prevDenBase = curDenBase;// update.
//    //        }
//    //        else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//    //        {
//    //            // do nothing.
//    //        }
//    //    }// end : ################################ num & denom cardinality evaluation. ####################
//    //    //
//    //    //---ready to prepare the return variable.
//    //    this.pruningResults = new PruningResults();
//    //    pruningResults.tensor_Num = new long[numeratorFactorCardinality, 2];//  raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
//    //    pruningResults.tensor_Den = new long[denominatorFactorCardinality, 2];// raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
//    //    prevNumBase = +1L;
//    //    curNumBase = +1L;
//    //    prevDenBase = +1L;
//    //    curDenBase = +1L;
//    //    int curGroupedElement_Num = 0;
//    //    int curGroupedElement_Den = 0;
//    //    //
//    //    pruningResults.tensor_Num[curGroupedElement_Num, 0] = signFlag;// NB.---#### SignFlag ####---only here---
//    //    pruningResults.tensor_Num[curGroupedElement_Num, 1] = +1L;
//    //    curGroupedElement_Num++; // invariant inserted.
//    //    //
//    //    pruningResults.tensor_Den[curGroupedElement_Den, 0] = +1L;
//    //    pruningResults.tensor_Den[curGroupedElement_Den, 1] = +1L;
//    //    curGroupedElement_Den++; // invariant inserted.
//    //    //
//    //    for (int c = 0; c < SignedArray.Length / 2; c++)//--########## num & denom compilation ##########################################
//    //    {//--########## num & denom compilation ##########################################
//    //        if (SignedArray[c, 1] > 0)// evaluate exponent's sign: positive exponent means that factor belongs to numerator.
//    //        {
//    //            curNumBase = System.Math.Abs(SignedArray[c, 0]);
//    //            if (System.Math.Abs(curNumBase) > +1L && curNumBase != prevNumBase)
//    //            {
//    //                pruningResults.tensor_Num[curGroupedElement_Num, 0] = curNumBase;
//    //                pruningResults.tensor_Num[curGroupedElement_Num, 1] = SignedArray[c, 1];
//    //                curGroupedElement_Num++; // new base found.
//    //            }// else : there is already such a bese -> Sum exponents.
//    //            else
//    //            {
//    //                if (curGroupedElement_Num - 1 > 0 && SignedArray[c, 0] != +1)
//    //                {
//    //                    pruningResults.tensor_Num[curGroupedElement_Num - 1, 1] += System.Math.Abs(SignedArray[c, 1]);// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
//    //                }// else don't chenge exponent to the signFlag.
//    //            }
//    //            prevNumBase = curNumBase;// update.
//    //        }
//    //        else if (SignedArray[c, 1] < 0)// evaluate exponent's sign: negative exponent means that factor belongs to denominator.
//    //        {//##############// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
//    //            curDenBase = System.Math.Abs(SignedArray[c, 0]);
//    //            if (System.Math.Abs(curDenBase) > +1L && curDenBase != prevDenBase)
//    //            {
//    //                pruningResults.tensor_Den[curGroupedElement_Den, 0] = curDenBase;
//    //                pruningResults.tensor_Den[curGroupedElement_Den, 1] = (-1L) * SignedArray[c, 1];// correct negative exponents in positive, since they belong now to the denominator.
//    //                curGroupedElement_Den++; // new base found.
//    //            }// else : there is already such a bese -> Sum exponents.
//    //            else
//    //            {
//    //                if (curGroupedElement_Den - 1 > 0 && SignedArray[c, 0] != +1)
//    //                {
//    //                    pruningResults.tensor_Den[curGroupedElement_Den - 1, 1] += System.Math.Abs(SignedArray[c, 1]);// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
//    //                }// else don't chenge exponent to the signFlag.
//    //            }
//    //            prevDenBase = curDenBase;// update.
//    //        }// end : ###########// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
//    //        else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//    //        {//  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//    //            // do nothing.
//    //        }// end :  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//    //    }// end : ####### num & denom compilation ##########################################
//    //    // ready.
//    //    return pruningResults;// it's also a class data field.
//    //}// end public  void Prune_candidate() acts on class data fields : long[,] SignedArray, out long[,] pruningResults.tensor_Num, out long[,] pruningResults.tensor_Den )







    //public void RationalReductionOnIntegers()// non può ricevere razionali, ma solo interi 
    //{
    //    long[,] omegaData_this_Num_factorization = PrimesFinder.ElementFullCalculation.OmegaData( System.Math.Abs( (long)this.Numerator));
    //    long[,] check_Den_NumFactorization = null;
    //    Prune(omegaData_this_Num_factorization, out this.Num_factorization, out check_Den_NumFactorization);
    //    if (2 < check_Den_NumFactorization.Length)
    //    {
    //        throw new System.Exception("Only integer factorizaion can be passed here( i.e. only positive exponents).");
    //    }// else ok.
    //    if (this.Numerator < 0)
    //    {
    //        this.Num_factorization[0, 0] *= -1L;
    //    }// else ok.
    //    //
    //    // start denominator treatment.
    //    if (0 == this.Denominator)
    //    {
    //        throw new System.Exception("Division by zero.");
    //    }// else ok.
    //    long[,] omegaData_this_Den_factorization = PrimesFinder.ElementFullCalculation.OmegaData(System.Math.Abs( (long)this.Denominator));
    //    long[,] check_Den_DenFactorization = null;
    //    Prune(omegaData_this_Den_factorization, out this.Den_factorization, out check_Den_DenFactorization);
    //    if (2 < check_Den_DenFactorization.Length)
    //    {
    //        throw new System.Exception("Only integer factorizaion can be passed here( i.e. only positive exponents).");
    //    }// else ok.
    //    if (this.Denominator< 0)
    //    {
    //        this.Num_factorization[0, 0] *= -1L;//---####---NB. sign corrections always at Numerator, for notation convention.
    //    }// else ok.
    //    this.is_factorization_ready = true;
    //    //
    //    this.RationalReductionOnOmegaData(this.Num_factorization, this.Den_factorization);
    //    this.is_simplified_factorization_ready = true;
    //}// public bool RationalReductionOnIntegers()


//    public void RationalReductionOnRationals()// p/q , p and q in Q. p=m/n , q=i/j -> p/q==m/n*j/i ==(m*j)/(n*i).
//    {
//        //RealField.PruningResults pruneResults = this.Prune_candidate();

//        ////
//        //long[,] omegaData_this_Num_factorization = PrimesFinder.ElementFullCalculation.OmegaData(System.Math.Abs((long)this.Numerator));
//        //long[,] check_Den_NumFactorization = null;
//        //Prune(omegaData_this_Num_factorization, out this.Num_factorization, out check_Den_NumFactorization);
//        //if (2 < check_Den_NumFactorization.Length)
//        //{
//        //    throw new System.Exception("Only integer factorizaion can be passed here( i.e. only positive exponents).");
//        //}// else ok.
//        //if (this.Numerator < 0)
//        //{
//        //    this.Num_factorization[0, 0] *= -1L;
//        //}// else ok.
//        ////
//        //// start denominator treatment.
//        //if (0 == this.Denominator)
//        //{
//        //    throw new System.Exception("Division by zero.");
//        //}// else ok.
//        //long[,] omegaData_this_Den_factorization = PrimesFinder.ElementFullCalculation.OmegaData(System.Math.Abs((long)this.Denominator));
//        //long[,] check_Den_DenFactorization = null;
//        //Prune(omegaData_this_Den_factorization, out this.Den_factorization, out check_Den_DenFactorization);
//        //if (2 < check_Den_DenFactorization.Length)
//        //{
//        //    throw new System.Exception("Only integer factorizaion can be passed here( i.e. only positive exponents).");
//        //}// else ok.
//        //if (this.Denominator < 0)
//        //{
//        //    this.Num_factorization[0, 0] *= -1L;//---####---NB. sign corrections always at Numerator, for notation convention.
//        //}// else ok.
//        //this.is_factorization_ready = true;
//        ////
//        //this.RationalReductionOnOmegaData(this.Num_factorization, this.Den_factorization);
//        //this.is_simplified_factorization_ready = true;
//    }// public bool RationalReductionOnRationals()


//    private void check_factorizations_are_Integer_( long[,] Num, long[,] Den )
//    {
//        // check that input factorizations are both of integer and not rational(i.e. all positive exponents at each factor of both).
//        // it's necessary a preliminar Prune, on both the numerator and the denominator, since the following reduction algorithm
//        // between numerator and denominator is not suitable to recognize signs nor at the bases neither at the exponents.
//        long[,] NumeratorOmegaData, DenominatorOmegaData;
//        long[,] Numerator_residue, Denominator_residue;
//        Prune(Num, out NumeratorOmegaData, out Numerator_residue);
//        if (2 != Numerator_residue.Length || +1 != Numerator_residue[0, 0])
//        {
//            throw new System.Exception("Invalid SignedVecotr passed to the Prune ! NB devono essere passate fattorizzazioni di interi in questa sede e NON di razionali.");
//        }// else can continue.
//        Prune(Den, out DenominatorOmegaData, out Denominator_residue);
//        if (2 != Denominator_residue.Length || +1 != Denominator_residue[0, 0])
//        {
//            throw new System.Exception("Invalid SignedVecotr passed to the Prune ! NB devono essere passate fattorizzazioni di interi in questa sede e NON di razionali.");
//        }// else can continue.
//        //
//    }// end private void check_factorizations_are_Integer_( long[,] Num, long[,] Den )



//    /// <summary>
//    /// This method concatenates two factorizaions, in one that includes the factors of both.
//    /// It means to obtain the product of the two factors.
//    /// As it's written, it is suitable both for integers and rationals.
//    /// An uphill filter could stop the rational factors, for calls from a context which allows integers only.
//    /// This core is then suitable for both.
//    /// </summary>
//    /// <param name="firstOne">first factor</param>
//    /// <param name="secondOne">second factor</param>
//    /// <returns>a factorizaion tensor, that includes the factors of both the input parameters</returns>
//    public long[,] factorizationConcatenator( long[,] firstOne, long[,] secondOne )
//    {// the addend tensors are [ownRows, 2] since they both are factorization vectors, i.e. vectors of the couple [base, exp].
//        // so the sum tensor has cardinality [rows_firstOne + rows_secondOne, 2].
//        long[,] res = new long[firstOne.Length / 2 + secondOne.Length / 2, 2];// rows are the sum of the rows; columns are two in the result too.
//        int c = 0;// declared outside the loops, since it has to serve both.
//        for (/*declared outside the loops, since it has to serve both.*/; c < firstOne.Length / 2; c++)
//        {
//            //// for debug purposes : Console.WriteLine(c);
//            res[c, 0] = firstOne[c, 0];// copy the base
//            res[c, 1] = firstOne[c, 1];// copy the exponent
//        }// end : copy the firstOne.
//        for ( /*NB. don't re-init! counter must proceed from where it just got.*/; c < firstOne.Length / 2 + secondOne.Length / 2; c++)
//        {
//            // for debug purposes : Console.WriteLine("{0}   {1}  ", c,  -(firstOne.Length / 2 - c));
//            res[c, 0] = secondOne[-(firstOne.Length / 2 - c), 0];// copy the base. --###--NB. carefull with the row index !
//            res[c, 1] = secondOne[-(firstOne.Length / 2 - c), 1];// copy the exponent. --###--NB. carefull with the row index !
//        }// end : copy the secondOne.
//        // ready.
//        return res;
//    }// end : public long[,] factorizationConcatenator( long[,] firstOne, long[,] secondOne ).




//    /// <summary>
//    /// scopo del metodo e' produrre una frazione ai minimi termini(i.e. con numeratore e donominatore primi fra loro) partendo da due 
//    /// prodotti di interi con segno elevati ad esponente intero con segno.
//    /// Il metodo semplifica i fattori comuni fra numeratore e denominatore, ma non elimina le potenze dell'uno (i.e. +1^n) ne' 
//    /// riordina i fattori per modulo.
//    /// TODO : studiare mediante test, la combinazione col metodo Prune.
//    /// TODO : eliminare la staticita' dei due metodi e rendere di istanza.
//    /// TODO NB devono essere passate fattorizzazioni di interi in questa sede e NON di razionali.
//    /// </summary>
//    /// <param name="NumeratorOmegaData"></param>
//    /// <param name="DenominatorOmegaData"></param>
//    /// <returns></returns>
//    public void RationalReductionOnOmegaData( long[,] Num, long[,] Den )
//    {
//        // check that input factorizations are both of integer and not rational(i.e. all positive exponents at each factor of both).
//        // it's necessary a preliminar Prune, on both the numerator and the denominator, since the following reduction algorithm
//        // between numerator and denominator is not suitable to recognize signs nor at the bases neither at the exponents.
//        long[,] NumeratorOmegaData, DenominatorOmegaData;
//        long[,] Numerator_residue, Denominator_residue;
//        Prune(Num, out NumeratorOmegaData, out Numerator_residue);
//        if (2 != Numerator_residue.Length || +1 != Numerator_residue[0, 0])
//        {
//            throw new System.Exception("Invalid SignedVecotr passed to the Prune ! NB devono essere passate fattorizzazioni di interi in questa sede e NON di razionali.");
//        }// else can continue.
//        Prune(Den, out DenominatorOmegaData, out Denominator_residue);
//        if(2!=Denominator_residue.Length || +1!=Denominator_residue[0,0])
//        {
//            throw new System.Exception("Invalid SignedVecotr passed to the Prune ! NB devono essere passate fattorizzazioni di interi in questa sede e NON di razionali.");
//        }// else can continue.
//        //
//        long[,] signedExponentResult = new long[NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2, 2];
//        for (int curRow = 0; curRow < signedExponentResult.Length / 2; curRow++)
//        {
//            for (int curCol = 0; curCol < 2; curCol++)
//            {
//                signedExponentResult[curRow, curCol] = +1L;// init to product invariant.
//            }// init to product invariant.
//        }// init to product invariant.
//        //
//        int accResultInsertion = 0;
//        for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
//        {// pre-init with numerator's factors.
//            signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base i.e. factor
//            signedExponentResult[accResultInsertion, 1] = NumeratorOmegaData[element_num, 1];// exponent i.e. multiplicity
//            accResultInsertion++;// one factor^multiplicity inserted.
//        }// end for : pre-init with numerator's factors.
//        // DON'T   re-init  accResultInsertion.
//        // NB. TODO se il numeratore ha il solo fattore "1" il denominatore neanche viene inserito : TODO copiarlo da DenominatorOmegaData.
//        //
//        if (+2L == NumeratorOmegaData.Length)// means if numerator has only the "1"
//        {
//            for (int element_den = 0; element_den < DenominatorOmegaData.Length / 2; element_den++)// just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
//            {
//                signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];// base
//                signedExponentResult[accResultInsertion, 1] = (-1L) * DenominatorOmegaData[element_den, 1];// exp : mul_(-1) because exponents are at denominator.
//                accResultInsertion++;// one factor^multiplicity inserted.
//            }// end for (int element_den = 0; element_den < DenominatorOmegaData.Length / 2; element_den++)// just copy the denominator. No way the algorithm would copy it, since it works for each numerator's factor > 1.
//        }// end : if (+2L == NumeratorOmegaData.Length)// means if numerator has only the "1"
//        else if (+2L <= NumeratorOmegaData.Length)// means if numerator has more factors than only the "1"
//        {
//            for (int element_num = 1; element_num < NumeratorOmegaData.Length / 2; element_num++)// NB. start from first base>1. Don't treat the invariants.
//            {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
//                for (int element_den = 1; element_den < DenominatorOmegaData.Length / 2; element_den++)// NB. start from first base>1. Don't treat the invariants.
//                {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
//                    if (System.Math.Abs(NumeratorOmegaData[element_num, 0]) == System.Math.Abs(DenominatorOmegaData[element_den, 0]))
//                    {// in case a denominator's factor matches with a numerator's factor : simplify the exponents (i.e. the multiplicity).
//                        bool isAlreadyTreatedFacor = false;// check if such factor is already present in the treatd ones' list.
//                        int readOnes = 0;// a counter to step in the already treated factors.
//                        for (; readOnes < accResultInsertion; readOnes++)
//                        {// is a factor has already been treated :
//                            if (System.Math.Abs(DenominatorOmegaData[element_den, 0]) == System.Math.Abs(signedExponentResult[readOnes, 0]))
//                            {// then stop searching : it will be overwritten with the updated exponent.
//                                isAlreadyTreatedFacor = true;
//                                break;// the current factor has been already treated.
//                            }// else continue searching.
//                        }// end search in already treated factors.
//                        if (!isAlreadyTreatedFacor)// if the current factor was never treated before insert it as a brand new one.
//                        {// if the current factor was never treated before insert it as a brand new one.
//                            long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
//                            signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base
//                            signedExponentResult[accResultInsertion, 1] = esponente_final;// exponent
//                            accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
//                        }// else don't repeat an already treated factor.
//                        else// i.e. if the current factor is already present between the treated ones
//                        {// sostituzione del fattore inserito senza match, con quello matchato( i.e. update its exponent).
//                            long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
//                            signedExponentResult[readOnes, 0] = NumeratorOmegaData[element_num, 0];// base
//                            signedExponentResult[readOnes, 1] = esponente_final;// exponent
//                            // DON'T go ahead : the place was already existing. accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
//                        }// end else :  sostituzione del fattore inserito senza match, con quello matchato.( i.e. update its exponent).
//                    }// end if(base_num==base_den) else continue searching
//                    else // no match in current factor
//                    {
//                        bool isAlreadyTreatedFacor = false;
//                        for (int readOnes = 0; readOnes < accResultInsertion; readOnes++)
//                        {
//                            if (System.Math.Abs(DenominatorOmegaData[element_den, 0]) == System.Math.Abs(signedExponentResult[readOnes, 0]))
//                            {
//                                isAlreadyTreatedFacor = true;
//                                break;// the current facto has been already treated.
//                            }// else continue searching.
//                        }// end search in already treated factors.
//                        if (!isAlreadyTreatedFacor)
//                        {
//                            long esponente_final = (-1) * DenominatorOmegaData[element_den, 1];// mul_(-1) because exponents are at denominator.
//                            signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];
//                            signedExponentResult[accResultInsertion, 1] = esponente_final;
//                            accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
//                        }// else don't repeat an already treated factor.
//                    }// end else // no match in current factor
//                }// end for element_den
//            }// end for element_num
//        }// end : if( +2L<=NumeratorOmegaData.Length)// means if numerator has more factors than only the "1"
//        //the simplified matrix, which collects together factors with positive and with negative exponents(i.e. signedExponentResult), is now ready.
//        //######
//        //long[,] pruned_num, pruned_den;
//        //pruned_num = pruned_den = null;
//        ////RealField.Rationals_Q_.Prune(signedExponentResult, out pruned_num, out pruned_den);
//        ////_simplified_
//        RealField.Rationals_Q_.Prune(signedExponentResult, out this.Num_simplified_factorization, out this.Den_simplified_factorization);
//        // ready.
//    }// end RationalReductionOnOmegaData




//    /// <summary>
//    /// Buble-sort based rational-Q-Prune.
//    /// trasforma due prodotti di interi con segno elevati ad esponente intero con segno, in due vettori di interi positivi, rispettivamente con
//    /// esponente positivo( i.e. al numeratore) ed esponente negativo( i.e. al denominatore).
//    /// Il segno della frazione è rappresentato al numeratore.
//    /// Vengono raccolti i fattori comuni nell'ambito del numeratore ed altrettanto nell'ambito del denominatore.
//    /// Non viene eseguita la semplificazione fra fattori del numeratore e fattori del denominatore. Per questo chiamare RationalReduction().
//    /// I fattori, sia di numeratore che di denominatore, vengono ordinati per modulo crescente.
//    /// Il segno di ciascun fattore contribuisce al segno della frazione, in base alla parita' del suo esponente.
//    /// Il segno dell'esponenete determina la destinazione di un fattore a numeratore o a denominatore.
//    /// </summary>
//    /// <param name="SignedArray"></param>
//    /// <param name="Num"></param>
//    /// <param name="Den"></param>
//    public static void Prune( long[,] SignedArray, out long[,] Num, out long[,] Den )
//    {
//        long[] tmp = new long[2];
//        bool swap = true;
//        int for_loop_accumulator = 0;
//        int signFlag = +1;
//        for (int c = 0; c < SignedArray.Length / 2; c++)////################################################# Sign-flag management.
//        {
//            if (SignedArray[c, 0] < 0 // se la base e' negativa
//                && (System.Math.Abs(SignedArray[c, 1] / 2.0 - SignedArray[c, 1] / 2) > double.Epsilon) // e l'esponente e' dispari
//                )
//            {
//                signFlag *= -1;// a negative factor in the product.
//            }// else skip sign change, on positive factors.
//        }//######################################################################### Sign-flag management.
//        //
//        while (swap && for_loop_accumulator < SignedArray.Length / 2 - 1)//####_Bubble-Sort_#### n-1 for_loops are sufficient.
//        {//####_Bubble-Sort_#### n-1 for_loops are sufficient.
//            swap = false;// reset at each new "for". An entire "for" loop without "swap" is sufficient to desume pruning end.
//            //
//            for (int c = 0; c < SignedArray.Length / 2 - 1; c++)
//            {
//                if (System.Math.Abs(SignedArray[c, 0]) < System.Math.Abs(SignedArray[c + 1, 0]))
//                {
//                    // NO swap in this case, since the existing order is the required one.
//                }
//                else if (System.Math.Abs(SignedArray[c, 0]) > System.Math.Abs(SignedArray[c + 1, 0]))
//                {
//                    tmp[0] = System.Math.Abs(SignedArray[c + 1, 0]);//---temporary is necessary to do not loose the overwritten data.
//                    tmp[1] = SignedArray[c + 1, 1];
//                    //
//                    SignedArray[c + 1, 0] = System.Math.Abs(SignedArray[c, 0]);
//                    SignedArray[c + 1, 1] = SignedArray[c, 1];
//                    //
//                    SignedArray[c, 0] = tmp[0];
//                    SignedArray[c, 1] = tmp[1];
//                    //
//                    swap = true;// this is the real swap. If it takes place at least once in a "for", the pruning is still active.
//                }
//                else if (System.Math.Abs(SignedArray[c, 0]) == System.Math.Abs(SignedArray[c + 1, 0]))
//                {
//                    SignedArray[c, 0] = System.Math.Abs(SignedArray[c, 0]);
//                    SignedArray[c, 1] = SignedArray[c, 1];
//                    //
//                    SignedArray[c + 1, 0] = System.Math.Abs(SignedArray[c + 1, 0]);
//                    SignedArray[c + 1, 1] = SignedArray[c + 1, 1];
//                    //
//                    // non si falsifica se è stato almeno una volta vero nel "for", quindi non mettere l'istruzione: swap = false
//                    // resta a false Se_solo non stato vero neanche una volta in tutto il "for".
//                }
//            }// for
//            //
//            for_loop_accumulator++;// count the cardinality of the pruning loops : each "for" is a pruning turn.
//        }// end : while swap &&... //####_Bubble-Sort_#### n-1 for_loops are sufficient.
//        //--#################################################### end Bubble-Sort#####################################
//        //---start compiling numerator and denominator, based on the BubbleSorted SignedArray.
//        long prevNumBase = +1L;
//        long curNumBase = +1L;
//        int numeratorFactorCardinality = +1;
//        long prevDenBase = +1L;
//        long curDenBase = +1L;
//        int denominatorFactorCardinality = +1;
//        for (int c = 0; c < SignedArray.Length / 2; c++)// start ################## num & denom cardinality evaluation. ###############
//        {// start ################################ num & denom cardinality evaluation. ####################
//            if (SignedArray[c, 1] > 0)// evaluate exponent's sign.
//            {
//                curNumBase = SignedArray[c, 0];
//                if (System.Math.Abs(curNumBase) > +1L && curNumBase != prevNumBase)
//                {
//                    numeratorFactorCardinality++;
//                }// else : the product invariant doesn't count.
//                prevNumBase = curNumBase;// update.
//            }
//            else if (SignedArray[c, 1] < 0)// evaluate exponent's sign.
//            {
//                curDenBase = SignedArray[c, 0];
//                if (System.Math.Abs(curDenBase) > +1L && curDenBase != prevDenBase)
//                {
//                    denominatorFactorCardinality++;
//                }// else : the product invariant doesn't count.
//                prevDenBase = curDenBase;// update.
//            }
//            else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//            {
//                // do nothing.
//            }
//        }// end ################################ num & denom cardinality evaluation. ####################
//        Num = new long[numeratorFactorCardinality, 2];//  raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
//        Den = new long[denominatorFactorCardinality, 2];// raggruppare i fattori ripetuti; già aggiunta una posiz. per l'uno.
//        prevNumBase = +1L;
//        curNumBase = +1L;
//        prevDenBase = +1L;
//        curDenBase = +1L;
//        int curGroupedElement_Num = 0;
//        int curGroupedElement_Den = 0;
//        //
//        Num[curGroupedElement_Num, 0] = signFlag;
//        Num[curGroupedElement_Num, 1] = +1L;
//        curGroupedElement_Num++; // invariant inserted.
//        //
//        Den[curGroupedElement_Den, 0] = +1L;
//        Den[curGroupedElement_Den, 1] = +1L;
//        curGroupedElement_Den++; // invariant inserted.
//        //
//        for (int c = 0; c < SignedArray.Length / 2; c++)//--########## num & denom compilation ##########################################
//        {//--########## num & denom compilation ##########################################
//            if (SignedArray[c, 1] > 0)// evaluate exponent's sign: positive exponent means that factor belongs to numerator.
//            {
//                curNumBase = System.Math.Abs( SignedArray[c, 0]);
//                if( System.Math.Abs( curNumBase) > +1L && curNumBase != prevNumBase)
//                {
//                    Num[curGroupedElement_Num, 0] = curNumBase;
//                    Num[curGroupedElement_Num, 1] = SignedArray[c, 1];
//                    curGroupedElement_Num++; // new base found.
//                }// else : there is already such a bese -> Sum exponents.
//                else
//                {
//                    if (curGroupedElement_Num - 1 > 0 && SignedArray[c, 0] != +1)
//                    {
//                        Num[curGroupedElement_Num - 1, 1] += SignedArray[c, 1];// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
//                    }// else don't chenge exponent to the signFlag.
//                }
//                prevNumBase = curNumBase;// update.
//            }
//            else if (SignedArray[c, 1] < 0)// evaluate exponent's sign: negative exponent means that factor belongs to denominator.
//            {//##############// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
//                curDenBase = System.Math.Abs( SignedArray[c, 0]);
//                if (System.Math.Abs( curDenBase) > +1L && curDenBase != prevDenBase)
//                {
//                    Den[curGroupedElement_Den, 0] = curDenBase;
//                    Den[curGroupedElement_Den, 1] = (-1L) * SignedArray[c, 1];// correct negative exponents in positive, since they belong now to the denominator.
//                    curGroupedElement_Den++; // new base found.
//                }// else : there is already such a bese -> Sum exponents.
//                else
//                {
//                    if (curGroupedElement_Den - 1 > 0 && SignedArray[c, 0] != +1)
//                    {
//                        Den[curGroupedElement_Den - 1, 1] += SignedArray[c, 1];// NB. : +=  -> Sum exponents on prev(i.e. -1 not a next position, the same prev) base.
//                    }// else don't chenge exponent to the signFlag.
//                }
//                prevDenBase = curDenBase;// update.
//            }// end : ###########// evaluate exponent's sign: negative exponent means that factor belongs to denominator.########
//            else //  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//            {//  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//                // do nothing.
//            }// end :  (SignedArray[c, 1] == 0) a productInvariant(i.e. +1) gets no place in numerator neither in denominator.
//        }// end : ####### num & denom compilation ##########################################
//        // ready.
//    }// end public static void Prune( long[,] SignedArray, out long[,] Num, out long[,] Den )


//    public string ToNumDenString()
//    {
//        // build the response.
//        System.Text.StringBuilder message = new System.Text.StringBuilder();
//        if (this.isNumDen_ready)
//        {
//            message.Append("\r\n");
//            //##########
//            message.Append(this.Numerator);
//            message.Append("\r\n---------------\r\n");
//            message.Append(this.Denominator);
//            message.Append("\r\n\r\n");
//        }
//        else return "not yet ready.";
//        // ready.
//        return message.ToString();
//    }// public string ToNumDenString()




//    public string ToFractionString()
//    {
//        // build the response.
//        System.Text.StringBuilder message = new System.Text.StringBuilder();
//        if (this.is_factorization_ready )
//        {
//            message.Append("\r\n");
//            //##########
//            if (this.Num_factorization.Length == 2)// means only base and exp are 1^1
//            {
//                message.Append(this.Num_factorization[0, 0].ToString());// numerator "+1 or -1".
//                // give no exponent to base +1. Only write the base.
//            }// end : if (this.Num_factorization.Length == 2)// means only base and exp are 1^1.
//            else// bases >+1 are present, so skip the "1" factor.//##########
//            {// bases >+1 are present, so skip the "1" factor.//##########
//                if (this.Num_factorization[0, 0] < 0)// the SignFlag for the whole fraction says minus.
//                {// the SignFlag for the whole fraction says minus.
//                    message.Append(" -");// the SignFlag for the whole fraction says minus.
//                }// end :  this.Num_factorization[0, 0] < 0) -> the SignFlag for the whole fraction says minus.
//                // else signFlag says the deafault : (+).
//                message.Append(" ");// the SignFlag for the whole fraction says minus.
//                for (int c = 1; c < this.Num_factorization.Length / 2; c++)// skip (+-)(1^n)
//                {
//                    message.Append(this.Num_factorization[c, 0].ToString());
//                    if (this.Num_factorization[c, 1] > +1L)
//                    {
//                        message.Append("^" + this.Num_factorization[c, 1].ToString());
//                    }// else skip expontnt==+1, as default.
//                    if (c < this.Num_factorization.Length / 2 - 1)
//                    {
//                        message.Append(" * ");
//                    }// else : skip last
//                }// end for num
//            }// end :  bases >+1 are present, so skip the "1" factor.
//            message.Append("\r\n");
//            if (this.Den_factorization.Length / 2 > +1)
//            {
//                for (int c = 0; c < (this.Num_factorization.Length + this.Den_factorization.Length) * 2; c++)
//                {
//                    message.Append("-");
//                }// end for fraction symbol
//            }// else skip denominator==+1 as default.
//            message.Append("\r\n");
//            //##########
//            if (this.Den_factorization[0, 0] < 0)
//            {
//                message.Append(" -");//########------- denominator's sign --(prevention for future bugs. Sign should be managed by the num).
//            }// else skip positive sign, as default one.
//            message.Append(" ");// the SignFlag for the whole fraction says minus.
//            //##########
//            for (int c = 1; c < this.Den_factorization.Length / 2; c++)// skip (+-)(1^n)
//            {
//                message.Append(this.Den_factorization[c, 0].ToString());
//                if (this.Den_factorization[c, 1] > +1L)
//                {
//                    message.Append("^" + this.Den_factorization[c, 1].ToString());
//                }// else skip expontnt==+1, as default.
//                if (c < this.Den_factorization.Length / 2 - 1)
//                {
//                    message.Append(" * ");
//                }// else : skip last
//            }// end for den
//            message.Append("\r\n");
//        }
//        else return "not yet ready.";
//        // ready.
//        return message.ToString();
//    }// public string ToFractionString()



//    public string ToSimplifiedFractionString()
//    {
//        // build the response.
//        System.Text.StringBuilder message = new System.Text.StringBuilder();
//        if (this.is_simplified_factorization_ready)
//        {
//            message.Append("\r\n");
//            //##########
//            if (this.Num_simplified_factorization.Length == 2)// means only base and exp are 1^1
//            {
//                message.Append(this.Num_simplified_factorization[0, 0].ToString());// numerator "+1 or -1".
//                // give no exponent to base +1. Only write the base.
//            }// end : if (this.Num_simplified_factorization.Length == 2)// means only base and exp are 1^1.
//            else// bases >+1 are present, so skip the "1" factor.//##########
//            {// bases >+1 are present, so skip the "1" factor.//##########
//                if (this.Num_simplified_factorization[0, 0] < 0)// the SignFlag for the whole fraction says minus.
//                {// the SignFlag for the whole fraction says minus.
//                    message.Append(" -");// the SignFlag for the whole fraction says minus.
//                }// end :  this.Num_simplified_factorization[0, 0] < 0) -> the SignFlag for the whole fraction says minus.
//                // else signFlag says the deafault : (+).
//                message.Append(" ");// the SignFlag for the whole fraction says minus.
//                for (int c = 1; c < this.Num_simplified_factorization.Length / 2; c++)// skip (+-)(1^n)
//                {
//                    message.Append(this.Num_simplified_factorization[c, 0].ToString());
//                    if (this.Num_simplified_factorization[c, 1] > +1L)
//                    {
//                        message.Append("^" + this.Num_simplified_factorization[c, 1].ToString());
//                    }// else skip expontnt==+1, as default.
//                    if (c < this.Num_simplified_factorization.Length / 2 - 1)
//                    {
//                        message.Append(" * ");
//                    }// else : skip last
//                }// end for num
//            }// end :  bases >+1 are present, so skip the "1" factor.
//            message.Append("\r\n");
//            if (this.Den_simplified_factorization.Length / 2 > +1)
//            {
//                for (int c = 0; c < (this.Num_simplified_factorization.Length + this.Den_simplified_factorization.Length) * 2; c++)
//                {
//                    message.Append("-");
//                }// end for fraction symbol
//            }// else skip denominator==+1 as default.
//            message.Append("\r\n");
//            //##########
//            if (this.Den_simplified_factorization[0, 0] < 0)
//            {
//                message.Append(" -");//########------- denominator's sign --(prevention for future bugs. Sign should be managed by the num).
//            }// else skip positive sign, as default one.
//            message.Append(" ");// the SignFlag for the whole fraction says minus.
//            //##########
//            for (int c = 1; c < this.Den_simplified_factorization.Length / 2; c++)// skip (+-)(1^n)
//            {
//                message.Append(this.Den_simplified_factorization[c, 0].ToString());
//                if (this.Den_simplified_factorization[c, 1] > +1L)
//                {
//                    message.Append("^" + this.Den_simplified_factorization[c, 1].ToString());
//                }// else skip expontnt==+1, as default.
//                if (c < this.Den_simplified_factorization.Length / 2 - 1)
//                {
//                    message.Append(" * ");
//                }// else : skip last
//            }// end for den
//            message.Append("\r\n");
//        }
//        else return "not yet ready.";
//        // ready.
//        return message.ToString();
//    }// public string ToFractionString()



//    /// <summary>
//    /// the user enters a string with the following syntax : "-2^3* 3^4* 7^2  *5^3".
//    /// and obtains the long_couple array : { { -2, 3}, {3, 4}, {7, 2}, {5, 3} }.
//    /// </summary>
//    /// <param name="factorizationString"></param>
//    /// <returns></returns>
//    public static long[,] factorizationParser( string factorizationString )
//    {

//        string[] factors = factorizationString.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries);// first round : extract pi^ei
//        long[,] res = new long[factors.Length, 2];// columns are fixed 2, since they are base and exponent.
//        //
//        for (int c = 0; c < factors.Length; c++)
//        {
//            string[] BaseExp = factors[c].Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);// second round : separate pi and ei, for each couple pi^ei.
//            res[c, 0] = long.Parse(BaseExp[0]);// convert base; throws on syntax error.
//            res[c, 1] = long.Parse(BaseExp[1]);// convert exponent; throws on syntax error.
//            BaseExp = null;// garbage collect.
//        }// end for.
//        // ready.
//        return res;
//    }// end public long[,] factorizationParser( string factorizationString )



//    /// <summary>
//    /// receives a long_couple array, such as : { { -2, 3}, {3, 4}, {7, 2}, {5, 3} }
//    /// and computes Product[ base_i^exp_i] and returns the grand total.
//    /// </summary>
//    /// <param name="theFactorization"></param>
//    /// <returns></returns>
//    public static double mulUp( long[,] theFactorization )// NB. the rationals are dense in the Reals.
//    {// on integers -> only positive exponents are admitted.
//        double res = +1.0;// we will multiply on it.
//        for (int c = 0; c < theFactorization.Length / 2; c++)
//        {
//            //if (theFactorization[c, 1] < 0L)
//            //{
//            //    throw new System.Exception("Only integer factorizations in this method : no negative exponents allowed.");
//            //}// end throw.
//            res *= System.Math.Pow(theFactorization[c, 0], theFactorization[c, 1]);// i.e. base^exp.
//        }//
//        // ready.
//        return res;
//    }// end public static long addUp( long[,] theFactorization )


//}//class


//public class BubbleSortGeneric<T> where T : new()
//{
//    private T[] theAnalyzedArray;

//    public BubbleSortGeneric()
//    {// default Ctor().
//    }

//    public void Allocator(int theArray_cardinality)
//    {
//        this.theAnalyzedArray = new T[theArray_cardinality];
//    }

//    public void Add( T curElement , int curPosition)
//    {
//        this.theAnalyzedArray[curPosition] = curElement;
//    }

//    virtual public int isBefore( object A, object B )
//    {
//        bool wantWarning = true;
//        if (wantWarning)
//        {
//            throw new System.Exception("Implement in a son class");
//        }
//        return -1;
//    }

//    public T Element_reader( int position)
//    {
//        return theAnalyzedArray[position];
//    }

//    public int Cardinality_reader()
//    {
//        return theAnalyzedArray.Length;
//    }


//    /// <summary>
//    /// Physically swap the array elements : void return.
//    /// </summary>
//    public void BubbleSort_movingEngine_()
//    {
//        T tmp = new T();
//        bool swap = true;
//        int for_loop_accumulator = 0;
//        //---#########################################---start Bubble-Sort---##################################
//        while (swap && for_loop_accumulator < theAnalyzedArray.Length - 1)//####_Bubble-Sort_#### n-1 for_loops are sufficient.
//        {//####_Bubble-Sort_#### n-1 for_loops are sufficient.
//            swap = false;// reset at each new "for". An entire "for" loop without "swap" is sufficient to desume pruning end.
//            //
//            for (int c = 0; c < theAnalyzedArray.Length - 1; c++)
//            {
//                if (-1 == isBefore(theAnalyzedArray[c], theAnalyzedArray[c + 1]))//if (System.Math.Abs(SignedArray[c, 0]) < System.Math.Abs(SignedArray[c + 1, 0]))
//                {
//                    // NO swap in this case, since the existing order is the required one.
//                }
//                else if (+1 == isBefore(theAnalyzedArray[c], theAnalyzedArray[c + 1]))//else if (System.Math.Abs(SignedArray[c, 0]) > System.Math.Abs(SignedArray[c + 1, 0]))
//                {
//                    tmp = theAnalyzedArray[c + 1];//---temporary is necessary to do not loose the overwritten data.
//                    //
//                    theAnalyzedArray[c + 1] = theAnalyzedArray[c];
//                    //
//                    theAnalyzedArray[c] = tmp;
//                    //
//                    swap = true;// this is the real swap. If it takes place at least once in a "for", the pruning is still active.
//                }
//                else if (0 == isBefore(theAnalyzedArray[c], theAnalyzedArray[c + 1]))//else if (System.Math.Abs(SignedArray[c, 0]) == System.Math.Abs(SignedArray[c + 1, 0]))
//                {
//                    //theAnalyzedArray[c] = theAnalyzedArray[c];// NO swap in this case, since the existing order is the required one.
//                    //theAnalyzedArray[c + 1] = theAnalyzedArray[c + 1];// NO swap in this case, since the existing order is the required one.
//                    //
//                    // non si falsifica se è stato almeno una volta vero nel "for", quindi non mettere l'istruzione: swap = false
//                    // resta a false Se_solo non stato vero neanche una volta in tutto il "for".
//                }
//            }// for
//            //
//            for_loop_accumulator++;// count the cardinality of the pruning loops : each "for" is a pruning turn.
//        }// end : while swap &&... //####_Bubble-Sort_#### n-1 for_loops are sufficient.
//        //--#################################################### end Bubble-Sort#####################################
//    }// end BubbleSort swapping engine.

//}// end : public class BubbleSortGeneric<T>




//public class FactorCouple
//{
//    private long theBase;
//    private long theExp;
//    //
//    public FactorCouple( )
//    {
//    }// end : Ctor : default.

//    public FactorCouple( long theBase_par, long theExp_par )
//    {
//        this.theBase = theBase_par;
//        this.theExp = theExp_par;
//    }// end : Ctor : public FactorCouple( long theBase_par, long theExp_par )

//    public long reader_Base()
//    {
//        return this.theBase;
//    }// end : public long reader_Base().

//    public long reader_Exp()
//    {
//        return this.theExp;
//    }// end : public long reader_Exp().

//}// end : public class FactorCouple.





//public  class BubbleSort_Specific_forFactorCouple : BubbleSortGeneric<FactorCouple>
//{
//    public BubbleSort_Specific_forFactorCouple( int arrayCardinality )
//    {
//        base.Allocator(arrayCardinality);
//    }

//    public override int isBefore( object A, object B )
//    {
//        sviluppo.FactorCouple AnalyzedObject = (sviluppo.FactorCouple)A;
//        sviluppo.FactorCouple ComparisonTerm = (sviluppo.FactorCouple)B;
//        int res = -2;// init to invalid.
//        //
//        if (System.Math.Abs(AnalyzedObject.reader_Base()) < System.Math.Abs(ComparisonTerm.reader_Base()))//if (System.Math.Abs(SignedArray[c, 0]) < System.Math.Abs(SignedArray[c + 1, 0]))
//        {
//            res = -1;// is before.
//        }
//        else if (System.Math.Abs(AnalyzedObject.reader_Base()) > System.Math.Abs(ComparisonTerm.reader_Base()))
//        {
//            res = +1;/// +1 means the evaluated term is after the comparison term
//        }
//        else if (System.Math.Abs(AnalyzedObject.reader_Base()) == System.Math.Abs(ComparisonTerm.reader_Base()))
//        {
//            res = 0;///  0 means the evaluated term is equivalent to the comparison term
//        }// no other cases here.
//        // ready.
//        return res;
//    }// end : public override int isBefore( object A, object B ).


//    public void show()
//    {
//        for (int c = 0; c < this.Cardinality_reader(); c++)
//        {
//            Console.Write(this.Element_reader(c).reader_Base().ToString() + "^" + this.Element_reader(c).reader_Exp().ToString() + " * ");
//        }
//    }// end : show().


//}// end class Specific_BubbleSort


//public class LexicographyBrick
//{
//    public string field;
//}


//public class Lexicography_BubbleSort : BubbleSortGeneric<LexicographyBrick>
//{
//    public Lexicography_BubbleSort( int arrayCardinality )
//    {
//        base.Allocator(arrayCardinality);
//    }


//    private int lexicography( LexicographyBrick AnalyzedObject_par, LexicographyBrick ComparisonTerm_par )
//    {
//        string AnalyzedObject = (string)(AnalyzedObject_par.field);
//        string ComparisonTerm = (string)(ComparisonTerm_par.field);
//        int res = 0;///  0 means the evaluated term is equivalent to the comparison term
//        //
//        int comparisonLength = Math.Min(AnalyzedObject.Length, ComparisonTerm.Length);
//        for (int c = 0; c < comparisonLength; c++)
//        {
//            if( (int)(AnalyzedObject[c]) < (int)(ComparisonTerm[c]) )
//            {
//                res = -1;// is before.
//                break;
//            }
//            else if ((int)(AnalyzedObject[c]) > (int)(ComparisonTerm[c]))
//            {
//                res = +1;// +1 means the evaluated term is after the comparison term
//                break;
//            }
//            else if ((int)(AnalyzedObject[c]) == (int)(ComparisonTerm[c]))
//            {
//                continue;///  0 means the evaluated term is equivalent to the comparison term
//            }// no other cases here.
//        }// end : for.
//        // ready.
//        return res;
//    }


//    public override int isBefore( object AnalyzedObject_par, object ComparisonTerm_par )
//    {
//        sviluppo.LexicographyBrick AnalyzedObject = (sviluppo.LexicographyBrick)(AnalyzedObject_par);
//        sviluppo.LexicographyBrick ComparisonTerm = (sviluppo.LexicographyBrick)(ComparisonTerm_par);
//        int res = -2;// init to invalid.
//        //
//        if (this.lexicography(AnalyzedObject, ComparisonTerm)==-1)
//        {
//            res = -1;// is before.
//        }
//        else if (this.lexicography(AnalyzedObject, ComparisonTerm) == +1)
//        {
//            res = +1;/// +1 means the evaluated term is after the comparison term
//        }
//        else if (this.lexicography(AnalyzedObject, ComparisonTerm) == 0)
//        {
//            res = 0;///  0 means the evaluated term is equivalent to the comparison term
//        }// no other cases here.
//        // ready.
//        return res;
//    }// end : public override int isBefore( object A, object B ).


//    public void show()
//    {
//        for (int c = 0; c < this.Cardinality_reader(); c++)
//        {
//            Console.WriteLine(this.Element_reader(c).field);
//        }
//    }// end : show().


//}



//public class Integers_Z_
//{
//    // private data field
//    private long zinZ;


//    // Ctor()
//    public Integers_Z_( long theInt )
//    {
//        this.zinZ = theInt;
//    }// end :  Ctor() public Integers_Z_( long theInt )


//    public long Zreader()
//    {// public reader , for private data field.
//        return this.zinZ;
//    }

//    public static Integers_Z_ operator +( Integers_Z_ a, Integers_Z_ b )
//    {
//        return new Integers_Z_(a.Zreader() + b.Zreader());
//    }// end : operator public static Integers_Z_ operator +( Integers_Z_ a, Integers_Z_ b )


//}// end : public class Integers_Z_


//public class Rationals_Q_
//{
//    // private data field
//    //private long numerator_inZ;
//    //private long denominator_inZ;
//    //private Integers_Z_ numerator_class_inZ;
//    //private Integers_Z_ denominator_class_inZ;
//    //

//}// end : public class Rationals_Q_


///// <summary>
///// scopo del metodo e' produrre una frazione ai minimi termini(i.e. con numeratore e donominatore primi fra loro) partendo da due 
///// prodotti di interi con segno elevati ad esponente intero con segno.
///// Il metodo semplifica i fattori comuni fra numeratore e denominatore, ma non elimina le potenze dell'uno (i.e. +1^n) ne' 
///// riordina i fattori per modulo.
///// TODO : studiare mediante test, la combinazione col metodo Prune.
///// TODO : eliminare la staticita' dei due metodi e rendere di istanza.
///// </summary>
///// <param name="NumeratorOmegaData"></param>
///// <param name="DenominatorOmegaData"></param>
///// <returns></returns>
//public static long[, ,] RationalReductionOnOmegaData_old( long[,] NumeratorOmegaData, long[,] DenominatorOmegaData )
//{
//    int matrici = 2;
//    int righe = NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2;
//    int colonne = 2;
//    long[, ,] reducedFractionOmegaData = new long[matrici, righe, colonne];
//    // pre-init the response-tensor with product invariant +1.
//    for (int mat = 0; mat < matrici; mat++)
//    {
//        for (int row = 0; row < righe; row++)
//        {
//            for (int col = 0; col < colonne; col++)
//            {
//                reducedFractionOmegaData[mat, row, col] = +1;
//            }// ed for col
//        }//end for row
//    }//end for mat
//    //
//    long[,] signedExponentResult = new long[NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2, 2];
//    int accResultInsertion = 0;
//    for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
//    {// pre-init with numerator's factors.
//        signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base i.e. factor
//        signedExponentResult[accResultInsertion, 1] = NumeratorOmegaData[element_num, 1];// exponent i.e. multiplicity
//        accResultInsertion++;// one factor^multiplicity inserted.
//    }// end for : pre-init with numerator's factors.
//    // DON'T   re-init  accResultInsertion.
//    for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
//    {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
//        for (int element_den = 0; element_den < DenominatorOmegaData.Length / 2; element_den++)
//        {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
//            if (NumeratorOmegaData[element_num, 0] == DenominatorOmegaData[element_den, 0])
//            {// in case a denominator's factor matches with a numerator's factor : simplify the exponents (i.e. the multiplicity).
//                bool isAlreadyTreatedFacor = false;// check if such factor is already present in the treatd ones' list.
//                int readOnes = 0;// a counter to step in the already treated factors.
//                for (; readOnes < accResultInsertion; readOnes++)
//                {// is a factor has already been treated :
//                    if (DenominatorOmegaData[element_den, 0] == signedExponentResult[readOnes, 0])
//                    {// then stop searching : it will be overwritten with the updated exponent.
//                        isAlreadyTreatedFacor = true;
//                        break;// the current factor has been already treated.
//                    }// else continue searching.
//                }// end search in already treated factors.
//                if (!isAlreadyTreatedFacor)// if the current factor was never treated before insert it as a brand new one.
//                {// if the current factor was never treated before insert it as a brand new one.
//                    long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
//                    signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base
//                    signedExponentResult[accResultInsertion, 1] = esponente_final;// exponent
//                    accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
//                }// else don't repeat an already treated factor.
//                else// i.e. if the current factor is already present between the treated ones
//                {// sostituzione del fattore inserito senza match, con quello matchato( i.e. update its exponent).
//                    long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
//                    signedExponentResult[readOnes, 0] = NumeratorOmegaData[element_num, 0];// base
//                    signedExponentResult[readOnes, 1] = esponente_final;// exponent
//                    // DON'T go ahead : the place was already existing. accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
//                }// end else :  sostituzione del fattore inserito senza match, con quello matchato.( i.e. update its exponent).
//            }// end if(base_num==base_den) else continue searching
//            else // no match in current factor
//            {
//                bool isAlreadyTreatedFacor = false;
//                for (int readOnes = 0; readOnes < accResultInsertion; readOnes++)
//                {
//                    if (DenominatorOmegaData[element_den, 0] == signedExponentResult[readOnes, 0])
//                    {
//                        isAlreadyTreatedFacor = true;
//                        break;// the current facto has been already treated.
//                    }// else continue searching.
//                }// end search in already treated factors.
//                if (!isAlreadyTreatedFacor)
//                {
//                    long esponente_final = (-1) * DenominatorOmegaData[element_den, 1];// mul_(-1) because are exponents at denominator.
//                    signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];
//                    signedExponentResult[accResultInsertion, 1] = esponente_final;
//                    accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
//                }// else don't repeat an already treated factor.
//            }// end else // no match in current factor
//        }// end for element_den
//    }// end for element_num
//    //the simplified matrix, which collects together factors with positive and with negative exponents, is now ready.
//    // this method returns a tensor of two matrices mx2 long[2, m,2]; i must fill it up, with the data of the simplified matrix.
//    // the following loop is devoted to that:
//    int accNumInsertion = 0;
//    int accDenInsertion = 0;
//    for (int row = 0; row < righe; row++)
//    {// in the tensor, matrix[0,n,m] is the one with positive exponents (i.e. the numerator).
//        if (signedExponentResult[row, 1] > 0)// num== matrice[0,n,m]
//        {// num== matrice[0,n,m]
//            reducedFractionOmegaData[0, accNumInsertion, 0] = signedExponentResult[row, 0];// base
//            reducedFractionOmegaData[0, accNumInsertion, 1] = signedExponentResult[row, 1];// exp
//            accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
//        }// end insertion in numerator.
//        else if (signedExponentResult[row, 1] < 0)// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
//        {// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
//            reducedFractionOmegaData[1, accDenInsertion, 0] = signedExponentResult[row, 0];// base
//            reducedFractionOmegaData[1, accDenInsertion, 1] = signedExponentResult[row, 1];// exp
//            accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
//        }// end insertion in denominator.
//        else // exp==multiplicity==0 -> write +1.
//        {// exp==multiplicity==0 -> write +1.
//            reducedFractionOmegaData[0, accNumInsertion, 0] = +1;// base
//            reducedFractionOmegaData[0, accNumInsertion, 1] = +1;// exp
//            accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
//            // exp==multiplicity==0 -> write +1.
//            reducedFractionOmegaData[1, accDenInsertion, 0] = +1;// base
//            reducedFractionOmegaData[1, accDenInsertion, 1] = +1;// exp
//            accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
//        }// end // exp==multiplicity==0 -> don't write anywhere.
//    }// end for : insertion in final tensor, from simplified matrix.
//    //ready.
//    return reducedFractionOmegaData;// return the tensor.
//}// end RationalReductionOnOmegaData




        ///// <summary>
        ///// scopo del metodo e' produrre una frazione ai minimi termini(i.e. con numeratore e donominatore primi fra loro) partendo da due 
        ///// prodotti di interi con segno elevati ad esponente intero con segno.
        ///// Il metodo semplifica i fattori comuni fra numeratore e denominatore, ma non elimina le potenze dell'uno (i.e. +1^n) ne' 
        ///// riordina i fattori per modulo.
        ///// TODO : studiare mediante test, la combinazione col metodo Prune.
        ///// TODO : eliminare la staticita' dei due metodi e rendere di istanza.
        ///// </summary>
        ///// <param name="NumeratorOmegaData"></param>
        ///// <param name="DenominatorOmegaData"></param>
        ///// <returns></returns>
        //public static long[, ,] RationalReductionOnOmegaData( long[,] NumeratorOmegaData, long[,] DenominatorOmegaData )
        //{
        //    int matrici = 2;
        //    int righe = NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2;
        //    int colonne = 2;
        //    long[, ,] reducedFractionOmegaData = new long[matrici, righe, colonne];
        //    // pre-init the response-tensor with product invariant +1.
        //    for (int mat = 0; mat < matrici; mat++)
        //    {
        //        for (int row = 0; row < righe; row++)
        //        {
        //            for (int col = 0; col < colonne; col++)
        //            {
        //                reducedFractionOmegaData[mat, row, col] = +1;
        //            }// ed for col
        //        }//end for row
        //    }//end for mat
        //    //
        //    long[,] signedExponentResult = new long[NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2, 2];
        //    int accResultInsertion = 0;
        //    for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
        //    {// pre-init with numerator's factors.
        //        signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base i.e. factor
        //        signedExponentResult[accResultInsertion, 1] = NumeratorOmegaData[element_num, 1];// exponent i.e. multiplicity
        //        accResultInsertion++;// one factor^multiplicity inserted.
        //    }// end for : pre-init with numerator's factors.
        //    // DON'T   re-init  accResultInsertion.
        //    for (int element_num = 0; element_num < NumeratorOmegaData.Length / 2; element_num++)
        //    {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
        //        for (int element_den = 0; element_den < DenominatorOmegaData.Length / 2; element_den++)
        //        {// with these nested loops compare each denominator's factor with a numerator's factor. Numerator's factors are already inserted.
        //            if (NumeratorOmegaData[element_num, 0] == DenominatorOmegaData[element_den, 0])
        //            {// in case a denominator's factor matches with a numerator's factor : simplify the exponents (i.e. the multiplicity).
        //                bool isAlreadyTreatedFacor = false;// check if such factor is already present in the treatd ones' list.
        //                int readOnes = 0;// a counter to step in the already treated factors.
        //                for (; readOnes < accResultInsertion; readOnes++)
        //                {// is a factor has already been treated :
        //                    if (DenominatorOmegaData[element_den, 0] == signedExponentResult[readOnes, 0])
        //                    {// then stop searching : it will be overwritten with the updated exponent.
        //                        isAlreadyTreatedFacor = true;
        //                        break;// the current factor has been already treated.
        //                    }// else continue searching.
        //                }// end search in already treated factors.
        //                if (!isAlreadyTreatedFacor)// if the current factor was never treated before insert it as a brand new one.
        //                {// if the current factor was never treated before insert it as a brand new one.
        //                    long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
        //                    signedExponentResult[accResultInsertion, 0] = NumeratorOmegaData[element_num, 0];// base
        //                    signedExponentResult[accResultInsertion, 1] = esponente_final;// exponent
        //                    accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
        //                }// else don't repeat an already treated factor.
        //                else// i.e. if the current factor is already present between the treated ones
        //                {// sostituzione del fattore inserito senza match, con quello matchato( i.e. update its exponent).
        //                    long esponente_final = NumeratorOmegaData[element_num, 1] - DenominatorOmegaData[element_den, 1];// calculate the right moltiplicity.
        //                    signedExponentResult[readOnes, 0] = NumeratorOmegaData[element_num, 0];// base
        //                    signedExponentResult[readOnes, 1] = esponente_final;// exponent
        //                    // DON'T go ahead : the place was already existing. accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
        //                }// end else :  sostituzione del fattore inserito senza match, con quello matchato.( i.e. update its exponent).
        //            }// end if(base_num==base_den) else continue searching
        //            else // no match in current factor
        //            {
        //                bool isAlreadyTreatedFacor = false;
        //                for (int readOnes = 0; readOnes < accResultInsertion; readOnes++)
        //                {
        //                    if (DenominatorOmegaData[element_den, 0] == signedExponentResult[readOnes, 0])
        //                    {
        //                        isAlreadyTreatedFacor = true;
        //                        break;// the current facto has been already treated.
        //                    }// else continue searching.
        //                }// end search in already treated factors.
        //                if (!isAlreadyTreatedFacor)
        //                {
        //                    long esponente_final = (-1) * DenominatorOmegaData[element_den, 1];// mul_(-1) because are exponents at denominator.
        //                    signedExponentResult[accResultInsertion, 0] = DenominatorOmegaData[element_den, 0];
        //                    signedExponentResult[accResultInsertion, 1] = esponente_final;
        //                    accResultInsertion++;//one factor ahead, after having inserted both base and exponent.
        //                }// else don't repeat an already treated factor.
        //            }// end else // no match in current factor
        //        }// end for element_den
        //    }// end for element_num
        //    //the simplified matrix, which collects together factors with positive and with negative exponents, is now ready.
        //    // this method returns a tensor of two matrices mx2 long[2, m,2]; i must fill it up, with the data of the simplified matrix.
        //    // the following loop is devoted to that:
        //    int accNumInsertion = 0;
        //    int accDenInsertion = 0;
        //    for (int row = 0; row < righe; row++)
        //    {// in the tensor, matrix[0,n,m] is the one with positive exponents (i.e. the numerator).
        //        if (signedExponentResult[row, 1] > 0)// num== matrice[0,n,m]
        //        {// num== matrice[0,n,m]
        //            reducedFractionOmegaData[0, accNumInsertion, 0] = signedExponentResult[row, 0];// base
        //            reducedFractionOmegaData[0, accNumInsertion, 1] = signedExponentResult[row, 1];// exp
        //            accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
        //        }// end insertion in numerator.
        //        else if (signedExponentResult[row, 1] < 0)// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
        //        {// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
        //            reducedFractionOmegaData[1, accDenInsertion, 0] = signedExponentResult[row, 0];// base
        //            reducedFractionOmegaData[1, accDenInsertion, 1] = signedExponentResult[row, 1];// exp
        //            accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
        //        }// end insertion in denominator.
        //        else // exp==multiplicity==0 -> write +1.
        //        {// exp==multiplicity==0 -> write +1.
        //            reducedFractionOmegaData[0, accNumInsertion, 0] = +1;// base
        //            reducedFractionOmegaData[0, accNumInsertion, 1] = +1;// exp
        //            accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
        //            // exp==multiplicity==0 -> write +1.
        //            reducedFractionOmegaData[1, accDenInsertion, 0] = +1;// base
        //            reducedFractionOmegaData[1, accDenInsertion, 1] = +1;// exp
        //            accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
        //        }// end // exp==multiplicity==0 -> don't write anywhere.
        //    }// end for : insertion in final tensor, from simplified matrix.
        //    //ready.
        //    return reducedFractionOmegaData;// return the tensor.
        //}// end RationalReductionOnOmegaData



// old tensor output
//if (2 != pruned_den.Length || +1 != pruned_den[0, 0])
//{
//    throw new System.Exception("Invalid SignedVecotr passed to the Prune ! ");
//}// else can continue.
////
//int matrici = 2;
//int righe = pruned_num.Length / 2;// TODO check correction : NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2;
//int colonne = 2;
//long[, ,] reducedFractionOmegaData = new long[matrici, righe, colonne];
//// pre-init the response-tensor with product invariant +1.
//for (int mat = 0; mat < matrici; mat++)
//{
//    for (int row = 0; row < righe; row++)
//    {
//        for (int col = 0; col < colonne; col++)
//        {
//            reducedFractionOmegaData[mat, row, col] = +1;
//        }// ed for col
//    }//end for row
//}//end for mat
////
//// this method returns a tensor of two matrices mx2 long[2, m,2]; i must fill it up, with the data of the simplified matrix.
//// the following loop is devoted to that:
//int accNumInsertion = 0;
//int accDenInsertion = 0;
//for (int row = 0; row < righe; row++)
//{// in the tensor, matrix[0,n,m] is the one with positive exponents (i.e. the numerator).
//    if (pruned_num[row, 1] > 0)// num== matrice[0,n,m]
//    {// num== matrice[0,n,m]
//        reducedFractionOmegaData[0, accNumInsertion, 0] = pruned_num[row, 0];// base
//        reducedFractionOmegaData[0, accNumInsertion, 1] = pruned_num[row, 1];// exp
//        accNumInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
//    }// end insertion in numerator.
//    else if (pruned_num[row, 1] < 0)// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
//    {// den== matrice[1,n,m]// in the tensor, matrix[1,n,m] is the one with negative exponents (i.e. the denominator).
//        reducedFractionOmegaData[1, accDenInsertion, 0] = pruned_num[row, 0];// base
//        reducedFractionOmegaData[1, accDenInsertion, 1] = pruned_num[row, 1];// exp
//        accDenInsertion++;// NB. separate counters for numerator's insertions and denominator's ones.
//    }// end insertion in denominator.
//    else // exp==multiplicity==0 ->  don't write anywhere.
//    {// exp==multiplicity==0 ->  don't write anywhere.
//    }// end // exp==multiplicity==0 -> don't write anywhere.
//}// end for : insertion in final tensor, from simplified matrix.
////ready.

//int matrici = 2;
//int righe = NumeratorOmegaData.Length / 2 + DenominatorOmegaData.Length / 2;
//int colonne = 2;
//long[, ,] reducedFractionOmegaData = new long[matrici, righe, colonne];
//// pre-init the response-tensor with product invariant +1.
//for (int mat = 0; mat < matrici; mat++)
//{
//    for (int row = 0; row < righe; row++)
//    {
//        for (int col = 0; col < colonne; col++)
//        {
//            reducedFractionOmegaData[mat, row, col] = +1;
//        }// ed for col
//    }//end for row
//}//end for mat
//


#endregion cantina
