using System;



namespace Common.BubbleSort
{


    /// <summary>
    /// this is the generic integer factor, i.e. base^exp
    /// </summary>
    public class FactorCouple
    {
        private long theBase;
        private long theExp;
        //
        public FactorCouple()
        {
        }// end : Ctor : default.

        public FactorCouple( long theBase_par, long theExp_par )
        {
            this.theBase = theBase_par;
            this.theExp = theExp_par;
        }// end : Ctor : public FactorCouple( long theBase_par, long theExp_par )

        public long reader_Base()
        {
            return this.theBase;
        }// end : public long reader_Base().

        public long reader_Exp()
        {
            return this.theExp;
        }// end : public long reader_Exp().

    }// end : public class FactorCouple.





    public class BubbleSort_Specific_forFactorCouple : BubbleSortGeneric<FactorCouple>
    {


        //public BubbleSort_Specific_forFactorCouple( int arrayCardinality )
        //{// NB. call to the father's allocator().
        //    base.Allocator(arrayCardinality);// NB. call to the father's allocator().
        //}// end : Ctor().

        public BubbleSort_Specific_forFactorCouple( long[,] SignedArray, out long signFlag )
        {
            this.Allocator(SignedArray.Length / 2);
            signFlag = +1;
            for (int c = 0; c < SignedArray.Length / 2; c++)////############ Sign-flag management & data-struct translation ----#########
            {
                if (SignedArray[c, 0] < 0 // se la base e' negativa
                    && (System.Math.Abs(SignedArray[c, 1] / 2.0 - SignedArray[c, 1] / 2) > double.Epsilon) // e l'esponente e' dispari
                    )
                {
                    signFlag *= -1;// a negative factor in the product.
                }// else skip sign change, on positive factors.
            }////####################################### Sign-flag management & data-struct translation ----###########################
            for (int c = 0; c < SignedArray.Length / 2; c++)
            {
                Common.BubbleSort.FactorCouple curFactor = null;// re-init at each round.
                //
                //if(0==c && -1==signFlag)// we're on first factor ad the resulting sign of all factors is (-1).
                //{// we're on first factor ad the resulting sign of all factors is (-1).
                //    curFactor = new Common.BubbleSort.FactorCouple(
                //        (-1L) * System.Math.Abs(SignedArray[c, 0]) // NB. take the modulus, since the sign has already been treated in the SignFlag variable.
                //        , SignedArray[c, 1]// at the exponent instead take the signed value, since rationals have factors both at the numerator and at the denominator.
                //        );
                //}// end : we're on first factor ad the resulting sign of all factors is (-1).
                //else// we're NOT any more on first factor. The resulting sign of all factors has already been assigned.
                //{// we're NOT any more on first factor. The resulting sign of all factors has already been assigned.
                    curFactor = new Common.BubbleSort.FactorCouple(
                        System.Math.Abs(SignedArray[c, 0]) // NB. take the modulus, since the sign has already been treated in the SignFlag variable.
                        , SignedArray[c, 1]// at the exponent instead take the signed value, since rationals have factors both at the numerator and at the denominator.
                        );
                //}// end :  we're NOT any more on first factor. The resulting sign of all factors has already been assigned.
                this.Add( curFactor, c);
            }// end : for.
            //
        }// end : Ctor().


        /// <summary>
        /// this comparer is specific to the actual structure layout.
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        public override int isBefore( FactorCouple AnalyzedObject, FactorCouple ComparisonTerm )// parameters are here concretized on the specific type. In the father template they are generic <T>.
        {// parameters are here concretized on the specific type. In the father template they are generic <T>.
            int res = -2;// init to invalid.
            //
            if (System.Math.Abs(AnalyzedObject.reader_Base()) < System.Math.Abs(ComparisonTerm.reader_Base()))
            {
                res = -1;// is before.
            }
            else if (System.Math.Abs(AnalyzedObject.reader_Base()) > System.Math.Abs(ComparisonTerm.reader_Base()))
            {
                res = +1;/// +1 means the evaluated term is after the comparison term
            }
            else if (System.Math.Abs(AnalyzedObject.reader_Base()) == System.Math.Abs(ComparisonTerm.reader_Base()))
            {
                res = 0;///  0 means the evaluated term is equivalent to the comparison term
            }// no other cases here.
            // ready.
            return res;
        }// end : public override int isBefore( object A, object B ).


        public void show()
        {// Console output : it's useful to call it once before the sorting and once kust after.
            for (int c = 0; c < this.Cardinality_reader(); c++)
            {
                Console.Write(this.Element_reader(c).reader_Base().ToString() + "^" + this.Element_reader(c).reader_Exp().ToString() + " * ");
            }
        }// end : show().


        public long[,] returnSignedArray()
        {// return to the format long[,]
            long[,] res = new long[this.Cardinality_reader(), 2];// rows==hm_structs ; columns==2.
            for (int c = 0; c < this.Cardinality_reader(); c++)
            {
                res[c, 0] = this.Element_reader(c).reader_Base();
                res[c, 1] = this.Element_reader(c).reader_Exp();
            }
            // ready.
            return res;
        }// end : returnSignedArray().


    }// end class Specific_BubbleSort


}// end nmsp.
