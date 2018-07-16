using System;
//using System.Collections.Generic;
//using System.Text;


namespace Common.BubbleSort
{

    /// <summary>
    /// this class is both a template on the record layout of the struct, which constitutes the scalar of the one tensor
    /// and has a virtual method, to let the son class decide how the structs have to be compared.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BubbleSortGeneric<T> where T : new()
    {
        private T[] theAnalyzedArray;// the Array(i.e. the one-tensor) to be sorted.

        public BubbleSortGeneric()
        {// default Ctor().
        }


        public void Allocator( int theArray_cardinality )
        {// prepare the Array(i.e. the one-tensor) to be sorted.
            this.theAnalyzedArray = new T[theArray_cardinality];
        }// end : prepare the Array(i.e. the one-tensor) to be sorted.


        public void Add( T curElement, int curPosition )
        {// build the data field, for each of its own scalars.
            this.theAnalyzedArray[curPosition] = curElement;
        }// end : // build the data field, for each of its own scalars.


        /// <summary>
        /// TODO : every son of this class must implement its own comparer.
        /// The use of "object" data type is necessary, to let the user fill the vector with any kind of struct( class).
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <returns></returns>
        virtual public int isBefore( T A, T B )//---######---NB. virtual method for the sons to implement ! ---#######
        {//---######---NB. virtual method for the sons to implement ! ---#######
            bool wantWarning = true;
            if (wantWarning)
            {//---######---NB. virtual method for the sons to implement ! ---#######
                throw new System.Exception("Implement in a son class");
            }//---######---NB. virtual method for the sons to implement ! ---#######
            return -1;
        }// end virtual().


        public T Element_reader( int position )
        {
            return theAnalyzedArray[position];
        }

        public int Cardinality_reader()
        {
            return theAnalyzedArray.Length;
        }


        /// <summary>
        /// Physically swap the array elements : void return.
        /// </summary>
        public void BubbleSort_movingEngine_()
        {
            T tmp = new T();
            bool swap = true;
            int for_loop_accumulator = 0;
            //---#########################################---start Bubble-Sort---##################################
            while (swap && for_loop_accumulator < theAnalyzedArray.Length - 1)//####_Bubble-Sort_#### n-1 for_loops are sufficient.
            {//####_Bubble-Sort_#### n-1 for_loops are sufficient.
                swap = false;// reset at each new "for". An entire "for" loop without "swap" is sufficient to desume pruning end.
                //
                for (int c = 0; c < theAnalyzedArray.Length - 1; c++)
                {
                    if (-1 == isBefore(theAnalyzedArray[c], theAnalyzedArray[c + 1]))
                    {
                        // NO swap in this case, since the existing order is the required one.
                    }
                    else if (+1 == isBefore(theAnalyzedArray[c], theAnalyzedArray[c + 1]))
                    {
                        tmp = theAnalyzedArray[c + 1];//---temporary is necessary to do not loose the overwritten data.
                        //
                        theAnalyzedArray[c + 1] = theAnalyzedArray[c];
                        //
                        theAnalyzedArray[c] = tmp;
                        //
                        swap = true;// this is the real swap. If it takes place at least once in a "for", the pruning is still active.
                    }
                    else if (0 == isBefore(theAnalyzedArray[c], theAnalyzedArray[c + 1]))
                    {
                        // NO swap in this case, since the existing order is the required one.
                        // non si falsifica se è stato almeno una volta vero nel "for", quindi non mettere l'istruzione: swap = false
                        // resta a false Se_solo non stato vero neanche una volta in tutto il "for".
                    }
                }// for
                //
                for_loop_accumulator++;// count the cardinality of the pruning loops : each "for" is a pruning turn.
            }// end : while swap &&... //####_Bubble-Sort_#### n-1 for_loops are sufficient.
            //--#################################################### end Bubble-Sort#####################################
        }// end BubbleSort swapping engine.


    }// end : public class BubbleSortGeneric<T>


}// end nmsp
