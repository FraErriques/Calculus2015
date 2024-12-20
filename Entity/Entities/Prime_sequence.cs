﻿using System;
using System.Collections.Generic;
using System.Text;



namespace Entity.Entities
{


    public class Prime_sequence
    {
        // original db record layout.
        private Int64 ordinal;    // [ordinal] [bigint] IDENTITY(1,1) NOT NULL,
        private Int64 prime;      // [prime] [bigint] NOT NULL,
        //
        private Int64 min, max;
        // generic P[i] element.
        public struct SelectedCouple
        {
            public Int64 Prime;
            public Int64 atOrdinal;
        }//


        public Int64 getCurrentOrdinal()
        {
            return this.ordinal;
        }


        public Int64 getCurrentPrime()
        {
            return this.prime;
        }




        public Prime_sequence( )
        {
        }//



        public Prime_sequence(
            Int64 requiredordinal
          )
        {
            this.ordinal = requiredordinal;
        }//




        public Prime_sequence(
            Int64 rangeMin,
            Int64 rangeMax
          )
        {
            this.min = rangeMin;
            this.max = rangeMax;
        }//




        public Int64 GetActualOrdinal()
        {
            System.Data.DataTable lastCouple =
                Entity.Proxies.volatile_Prime_sequence_LOAD_atMaxOrdinal_SERVICE.volatile_Prime_sequence_LOAD_atMaxOrdinal();
			//
            try
            {
	            if(null == lastCouple) 
	            {
	                LogSinkFs.Wrappers.LogWrappers.SectionContent("failed to retrieve the resultset. It is NULL.",0);
	                throw new System.Exception("failed to retrieve the resultset.It is NULL.");
	            }            	
                this.prime = (Int64)(lastCouple.Rows[0]["prime"]);
                this.ordinal = (Int64)(lastCouple.Rows[0]["ordinal"]);
            }
            catch (System.Exception ex)
            {
                LogSinkFs.Wrappers.LogWrappers.SectionContent("failed to connect to db, or to retrieve the resultset. Ex=" + ex.Message,0);
            }
            //
            return this.ordinal;
        }//


        public void dbSeekingEngine( Int64 specifiedOrdinal)
        {
            System.Data.DataTable specifiedCouple =
            Entity.Proxies.volatile_Prime_sequence_LOAD_SINGLE_SERVICE.volatile_Prime_sequence_LOAD_SINGLE(
                specifiedOrdinal );
			//
            try
            {
	            if (null == specifiedCouple)
	            {
	                LogSinkFs.Wrappers.LogWrappers.SectionContent("failed to retrieve the resultset. It is NULL.", 0);
	                throw new System.Exception("failed to retrieve the resultset.It is NULL.");
	            }            	
                this.prime = (Int64)(specifiedCouple.Rows[0]["prime"]);
                this.ordinal = (Int64)(specifiedCouple.Rows[0]["ordinal"]);
            }
            catch (System.Exception ex)
            {
                LogSinkFs.Wrappers.LogWrappers.SectionContent("failed to connect to db, or to retrieve the resultset. Ex=" + ex.Message, 0);
            }
        }// end dbSeekingEngine



        /// <summary>
        /// NB. to use this method, instantiate by the ctor
        /// 
        ///  public Prime_sequence(
        ///    Int64 rangeMin,
        ///    Int64 rangeMax       )
        /// 
        /// </summary>
        /// <returns></returns>
        public SelectedCouple[] LoadRange(
          )
        {
            SelectedCouple[] theRange = null;
            System.Data.DataTable theSequence =
                Entity.Proxies.volatile_Prime_sequence_LOAD_MULTI_SERVICE.volatile_Prime_sequence_LOAD_MULTI(
                    min, max);
            if (null == theSequence)
            {
                theRange = new SelectedCouple[0];
                return theRange;
            }
            else
            {
                int theSequence_Rows_Count = theSequence.Rows.Count;
                theRange = new SelectedCouple[ theSequence_Rows_Count];
                for (int c = 0; c < theSequence_Rows_Count; c++)
                {
                    theRange[c].Prime = (Int64)(theSequence.Rows[c]["prime"]);
                    theRange[c].atOrdinal = (Int64)(theSequence.Rows[c]["ordinal"]);
                }
            }
            // ready
            return theRange;
        }//


    }//


}
