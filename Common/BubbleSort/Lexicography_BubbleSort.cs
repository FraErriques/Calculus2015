using System;
//using System.Collections.Generic;
//using System.Text;



namespace Common.BubbleSort
{


    /// <summary>
    /// // NB. template concretization on the string(i.e. without boxing struct) is not allowed.
    /// </summary>
    public class LexicographyBrick
    {// NB. template concretization on the string(i.e. without boxing struct) is not allowed.
        public string field;// NB. template concretization on the string(i.e. without boxing struct) is not allowed.
    }//// NB. template concretization on the string(i.e. without boxing struct) is not allowed.




    public class Lexicography_BubbleSort : BubbleSortGeneric<LexicographyBrick>
    {

        public Lexicography_BubbleSort( int arrayCardinality )
        {// NB. call father's Ctor().
            base.Allocator(arrayCardinality);
        }// end : Ctor()


        /// <summary>
        /// Lexicographycally compare two string fields.
        /// </summary>
        /// <param name="AnalyzedObject_par"></param>
        /// <param name="ComparisonTerm_par"></param>
        /// <returns></returns>
        private int lexicography( string AnalyzedObject, string ComparisonTerm )
        {
            int res = 0;///  0 means the evaluated term is equivalent to the comparison term
            //
            int comparisonLength = Math.Min(AnalyzedObject.Length, ComparisonTerm.Length);
            for (int c = 0; c < comparisonLength; c++)
            {
                if ((int)(AnalyzedObject[c]) < (int)(ComparisonTerm[c]))
                {
                    res = -1;// is before.
                    break;
                }
                else if ((int)(AnalyzedObject[c]) > (int)(ComparisonTerm[c]))
                {
                    res = +1;// +1 means the evaluated term is after the comparison term
                    break;
                }
                else if ((int)(AnalyzedObject[c]) == (int)(ComparisonTerm[c]))
                {
                    continue;///  0 means the evaluated term is equivalent to the comparison term
                }// no other cases here.
            }// end : for.
            // ready.
            return res;
        }


        /// <summary>
        /// specific
        /// </summary>
        /// <param name="AnalyzedObject_par"></param>
        /// <param name="ComparisonTerm_par"></param>
        /// <returns></returns>
        public override int isBefore( LexicographyBrick AnalyzedObject, LexicographyBrick ComparisonTerm )// parameters are here concretized on the specific type. In the father template they are generic <T>.
        {// parameters are here concretized on the specific type. In the father template they are generic <T>.
            int res = -2;// init to invalid.
            //
            if (this.lexicography(AnalyzedObject.field, ComparisonTerm.field) == -1)
            {
                res = -1;// is before.
            }
            else if (this.lexicography(AnalyzedObject.field, ComparisonTerm.field) == +1)
            {
                res = +1;/// +1 means the evaluated term is after the comparison term
            }
            else if (this.lexicography(AnalyzedObject.field, ComparisonTerm.field) == 0)
            {
                res = 0;///  0 means the evaluated term is equivalent to the comparison term
            }// no other cases here.
            // ready.
            return res;
        }// end : public override int isBefore( object A, object B ).



        public void show()
        {
            for (int c = 0; c < this.Cardinality_reader(); c++)
            {
                Console.WriteLine(this.Element_reader(c).field);
            }
        }// end : show().


    }// end : class.


}// end : namespace Common.BubbleSort
