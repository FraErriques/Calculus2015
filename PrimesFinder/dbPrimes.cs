using System;
using System.Text;


namespace PrimesFinder
{


    public class dbPrimes : IDisposable
    {
        #region Data

        private static System.Data.SqlClient.SqlConnection stickyConnection = null;
        private Int64 ToBeDivided = 2L;// start from two. One is invariant of the operator.
        private Int64 threshold;
        private Int64 primeOrdinal = 0L;
        private Int64 actualOrdinal = 0L;
        private Int64 actualPrime = 0L;
        private bool canOperate = false;
        // Riemann exponent s in C; s=:(sigma + i*t).
        private double sigma;
        private double t;
        //-----data for BCP mode---------------
        System.IO.FileStream BCP_stream = null;
        public struct OrdinalPrimeCouple
        {
            public Int64 ordinal;
            public Int64 prime;
        }
        private OrdinalPrimeCouple[] BCP_array = null;
        private Int32 BCP_current_scalar = 0;



        public Int64 getActualOrdinal()
        {
            return this.actualOrdinal;
        }

        public Int64 getActualPrime()
        {
            return this.actualPrime;
        }

        public bool getCanOperateStatus()
        {
            return this.canOperate;
        }

        #endregion Data


        #region Ctor


        /// <summary>
        /// TODO
        /// implement dynamic sql to create a table in tempdb, called "#tmpTableNamePrefix_timestamp"
        /// "#" prefix puts the table on tempdb. It's dropped on connection closure.
        /// </summary>
        /// <param name="tmpTableNamePrefix"></param>
        public dbPrimes(string tmpTableNamePrefix)
        { 
        }


        /// <summary>
        /// NB. no error check implemented. If the couple last {ordinal \t prime} known, passed
        ///     is wrong, then all subsequent calculation will be wrong.
        /// Ctor for an operation mode, consisting of:
        ///     - create in RAM an array of {ordinal \t prime}
        ///     - BCP into the "Prime_sequence_noIdentity" table.
        ///     - insert into the table with the identity.
        /// </summary>
        /// <param name="tmp_BCP_stream_fullpath"></param>
        /// <param name="BCP_cardinality">cardinality of the array</param>
        public dbPrimes(
            Int64 lastKnown_Prime,
            Int64 lastKnown_Ordinal,
            string tmp_BCP_stream_fullpath,
            Int32 BCP_cardinality
        )
        {
            this.primeOrdinal = lastKnown_Ordinal + BCP_cardinality;// the ordinal, where the search will stop.
            this.actualPrime = lastKnown_Prime;
            this.actualOrdinal = lastKnown_Ordinal;
            this.ToBeDivided = lastKnown_Prime + 1L;
            this.BCP_array = new OrdinalPrimeCouple[ BCP_cardinality];
            // TODO open file write exclusive.
            this.BCP_stream =
                System.IO.File.Open(
                    tmp_BCP_stream_fullpath,// substitutes a null fullpath.
                    System.IO.FileMode.Create, // substitute old versions, if present.
                    System.IO.FileAccess.Write,
                    System.IO.FileShare.None
                );
            if (null == BCP_stream) { throw new System.Exception("Cannot create the suggested file."); };//exit on wrong stream.
        }// end Ctor


        public dbPrimes()
        {
            LogSinkDb.Wrappers.LogWrappers.SectionOpen("PrimesFinder::dbPrimes::ctor", 0);
            LogSinkDb.Wrappers.LogWrappers.SectionContent("Entering from PrimesFinder::dbPrimes::ctor critical section.", 0);
            lock (typeof(PrimesFinder.dbPrimes))
            {
                PrimesFinder.dbPrimes.stickyConnection =
                    DbLayer.ConnectionManager.connectWithCustomSingleXpath(
                        "ProxyGeneratorConnections/strings",// compulsory xpath
                        "PrimeDataApp"
                    );
                if (null == stickyConnection)
                {
                    LogSinkDb.Wrappers.LogWrappers.SectionContent("connection acquisition failed.", 0);
                    this.canOperate = false;
                    LogSinkFs.Wrappers.LogWrappers.SectionContent(
                        "PrimesFinder::dbPrimes::ctor  connection acquisition failed.", 0);
                }
                else
                {
                    LogSinkDb.Wrappers.LogWrappers.SectionContent("connection acquisition succeeded.", 0);
                    System.Data.DataTable maxCouple_Prime_at_Ordinal =
                        Entity.Proxies.Prime_sequence_LOAD_atMaxOrdinal_SERVICE.Prime_sequence_LOAD_atMaxOrdinal(
                            PrimesFinder.dbPrimes.stickyConnection// NB. same for all, i.e. sticky.
                        );
                    try
                    {
                        this.actualPrime = (Int64)(maxCouple_Prime_at_Ordinal.Rows[0].ItemArray[0]);
                        this.actualOrdinal = this.primeOrdinal = (Int64)(maxCouple_Prime_at_Ordinal.Rows[0].ItemArray[1]);
                        this.ToBeDivided = this.actualPrime + 1L;
                        this.canOperate = true;
                    }
                    catch (System.Exception ex)
                    {
                        LogSinkDb.Wrappers.LogWrappers.SectionContent("No previous data found in the table. Ex= " + ex.Message, 0);
                        this.actualPrime = 2L;
                        this.ToBeDivided = this.actualPrime + 1L;
                        this.primeOrdinal = this.actualOrdinal = 1L;
                        int res =
                            Entity.Proxies.Prime_sequence_INSERT_SERVICE.Prime_sequence_INSERT(
                                this.actualPrime,
                                PrimesFinder.dbPrimes.stickyConnection// NB. same for all, i.e. sticky.
                            );
                        this.canOperate = false;
                    }
                }
            }// end critical section.
            LogSinkDb.Wrappers.LogWrappers.SectionContent("Exited from PrimesFinder::dbPrimes::ctor critical section.", 0);
            LogSinkDb.Wrappers.LogWrappers.SectionClose();
        }//


        /// <summary>
        /// decide calculation threshold and validate it.
        /// </summary>
        /// <param name="threshold"></param>
        public void InitThreshold(
            Int64 threshold
            )
        {
            if (threshold <= 0) return;
            this.threshold = threshold;
            if (this.ToBeDivided >= threshold)
            {
                this.canOperate = false;
            }// else calculation is needed.
        }//


        /// <summary>
        /// Ctor for calculation path.
        /// Ctor for Riemann zeta function calculations.
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="t"></param>
        public dbPrimes(double sigma, double t)
        {
            this.sigma = sigma;
            this.t = t;
        }//



        ~dbPrimes()
        {
            this.connection_closer();
        }


        public void Dispose()
        {
            this.connection_closer();
        }


        private void connection_closer()
        {
            lock (typeof(PrimesFinder.dbPrimes))
            {
                if (null != PrimesFinder.dbPrimes.stickyConnection)
                {
                    if (System.Data.ConnectionState.Open == stickyConnection.State)
                    {
                        stickyConnection.Close();
                        stickyConnection = null;// can garbage-collect it.
                    }//else already closed.
                }//else already nullified.
            }// end critical section.
        }//


        #endregion Ctor


        #region production


        private Int64 read_prime_withoutOrdinal( Int64 requiredOrdinal)
        {
            System.Data.DataTable required_prime_dt =
                Entity.Proxies.Prime_sequence_LOAD_SINGLE_SERVICE.Prime_sequence_LOAD_SINGLE(
                    requiredOrdinal,
                    PrimesFinder.dbPrimes.stickyConnection
                );
            Int64 res = (Int64)(required_prime_dt.Rows[0].ItemArray[0]);
            return res;
        }//



        /// <summary>
        /// extract from N(naturals) a subsequence of the primes, i.e. the primes until a threshold.
        /// 
        /// db record layout:
        /// 
        ///  [ordinal] [bigint] IDENTITY(1,1) NOT NULL,
        ///  [prime] [bigint] NOT NULL,
        ///
        /// </summary>
        public void CoreLoop()
        {
            //----definition of loop-ralated variables and constants.-----
            const double epsilon = double.Epsilon;// system constant ~+4.9e-324
            bool isStillPotentialPrime = true;
            Int64 divider = 2L;// the prime I'm actually using as divider. P[1]=2, i.e. first prime is 2.
            double quotient = 1.0;// due to inner loop rules, must be initialized >divider.
            Int64 innerLoopPrimeIndex = 0L;
            //
            for (// OUTER LOOP: i.e. test each( n in N), until threshold.
                ; // ToBeDivided already assigned, based on "input file", "NO input file".
                ToBeDivided < threshold;
                ToBeDivided++
               )
            {
                // INNER LOOP: try to divide "ToBeDivided" for know primes, searching a Diophantine quotient in the
                //             open set (1,n). So 2 is the smallest possible.
                for (
                        innerLoopPrimeIndex = 0L,// start from the first prime.
                        quotient = divider + 1.0;// due to inner loop rules, must be initialized >divider. 
                    //
                        quotient > divider;// for monotonicity of P, cannot search smallers, going ahead.
                    //NB. no more Diophantine quotient exist in the open set (1,n), for the monotonicity of function 
                    //    quotient=:ToBeDivided/x. It's hyperbolic.
                    //
                    // no increment whatsoever.
                    )
                {
                    divider = read_prime_withoutOrdinal( ++innerLoopPrimeIndex);// each step require next ordinal P[i].
                    //
                    quotient = (double)ToBeDivided / (double)divider;
                    // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(String.Format("{0}={1}/{2}", quotient, ToBeDivided, divider), 0);
                    if (Math.Abs(quotient - (Int64)quotient) > epsilon)
                    {// quotien is not integer. Potential divisor is not a Diophantine divisor; ->still potentially prime; analyze remaining potential divisors.
                        isStillPotentialPrime = true;
                    }
                    else// quotien was integer. Potential divisor is a Diophantine divisor.
                    {// -> not prime.
                        isStillPotentialPrime = false;
                        // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(string.Format("{0} is not prime.\r\n", ToBeDivided), 0);
                        break;// inner loop, i.e. don't put to the test other potential dividers; already foud one, which is enough.
                        // goto test next integer dividend.
                    }
                }// end for each potential divisor.
                // on END OF INNER LOOP, dividend results prime if and only if "isStillPotentialPrime==true".
                //
                if (true == isStillPotentialPrime)
                {
                    // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(String.Format("{0} is prime.\r\n", ToBeDivided), 0);
                    // for speed ignore the integer return value.
                    Entity.Proxies.Prime_sequence_INSERT_SERVICE.Prime_sequence_INSERT(
                        ToBeDivided,
                        stickyConnection
                    );
                    // the start of inner loop seeks to the origin.
                }
                else
                {
                    // just skip it.
                }
            }// end outer loop. Threshold reached.------primes' subsequence built.-----------------------------
            //
            this.connection_closer();
            //-----------
            //ready
        }// end CoreLoop.


        #endregion production


        #region BCP_production



        /// <summary>
        /// extract from N(naturals) a subsequence of the primes, i.e. the primes until a threshold.
        /// 
        /// db record layout:
        /// 
        ///  [ordinal] [bigint]  NOT NULL,   NB. without IDENTITY(1,1)
        ///  [prime] [bigint] NOT NULL,
        ///
        /// </summary>
        public void BCP_CoreLoop()
        {
            //----definition of loop-ralated variables and constants.-----
            const double epsilon = double.Epsilon;// system constant ~+4.9e-324
            bool isStillPotentialPrime = true;
            Int64 divider = 2L;// the prime I'm actually using as divider. P[1]=2, i.e. first prime is 2.
            double quotient = 1.0;// due to inner loop rules, must be initialized >divider.
            Int32 this_BCP_array_Length = this.BCP_array.Length;
            //
            for (// OUTER LOOP: i.e. test each( n in N), until threshold.
                ; // ToBeDivided already assigned.
                this.BCP_current_scalar < this_BCP_array_Length;
                ToBeDivided++
               )
            {
                // INNER LOOP: try to divide "ToBeDivided" for know primes, searching a Diophantine quotient in the
                //             open set (1,n). So 2 is the smallest possible.
                for (
                        divider = 2L,// first of the Primes.
                        quotient = divider + 1.0;// due to inner loop rules, must be initialized >divider. 
                    //
                        quotient > divider;// for monotonicity of P, cannot search smallers, going ahead.
                    //NB. no more Diophantine quotient exist in the open set (1,n), for the monotonicity of function 
                    //    quotient=:ToBeDivided/x. It's hyperbolic.
                    //
                    // no increment whatsoever.
                    )
                {
                    quotient = (double)ToBeDivided / (double)divider;
                    //
                    // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(String.Format("{0}={1}/{2}", quotient, ToBeDivided, divider), 0);
                    if (Math.Abs(quotient - (Int64)quotient) > epsilon)
                    {// quotien is not integer. Potential divisor is not a Diophantine divisor; ->still potentially prime; analyze remaining potential divisors.
                        isStillPotentialPrime = true;
                        ++divider;// fully independent loop; no data sources required, to query for known primes; divides by all naturals under Sqrt( ToBeDivided).
                    }
                    else// quotien was integer. Potential divisor is a Diophantine divisor.
                    {// -> not prime.
                        isStillPotentialPrime = false;
                        // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(string.Format("{0} is not prime.\r\n", ToBeDivided), 0);
                        break;// inner loop, i.e. don't put to the test other potential dividers; already foud one, which is enough.
                        // goto test next integer dividend.
                    }
                }// end inner loop, i.e. for each potential divisor.
                // on END OF INNER LOOP, dividend results prime if and only if "isStillPotentialPrime==true".
                //
                if (true == isStillPotentialPrime)
                {
                    // dbg LogSinkFs.Wrappers.LogWrappers.SectionContent(String.Format("{0} is prime.\r\n", ToBeDivided), 0);
                    // for speed ignore the integer return value.
                    // don't:  this.BCP_array[BCP_current_scalar] = new OrdinalPrimeCouple();// struct are value types; they born jet allocated.
                    this.BCP_array[BCP_current_scalar].ordinal = ++this.actualOrdinal;// NB. increment actualOrdinal on new entry.
                    this.BCP_array[BCP_current_scalar].prime = ToBeDivided;
                    //
                    ++BCP_current_scalar;// NB. incrementing the scalar, after usage.
                    // the start of inner loop seeks to the origin.
                }
                else
                {
                    // just skip it.
                }
            }// end outer loop. Threshold reached.------primes' subsequence built.-----------------------------
            //
            // TODO flush array on stream.
            // dbg
            for (int c = 0; c < this_BCP_array_Length; c++)
            {
                string tmp = this.BCP_array[c].ordinal.ToString() + "\t" + this.BCP_array[c].prime.ToString() + "\r\n";
                byte[] byteArray = new byte[tmp.Length];
                for (int d = 0; d < tmp.Length; d++)
                {
                    byteArray[d] = (byte)tmp[d];
                }
                this.BCP_stream.Write( byteArray, 0, byteArray.Length);
            }// end array flushing.
            this.BCP_stream.Flush();
            this.BCP_stream.Close();
            //-----------
            //ready
        }// end BCP_CoreLoop.


        #endregion BCP_production



    }//


}
