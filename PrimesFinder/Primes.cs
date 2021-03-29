using System;


namespace PrimesFinder
{

    public class Primes : IDisposable
    {
        # region Data

        private Int64 ToBeDivided = 2L;// start from two. One is invariant of the operator.
        private System.IO.FileStream previousShot = null;
        private Int64 threshold;
        private Int64 primeOrdinal = 0L; // meglio non fare(after test). exTODO switch to System.UInt64 everywhere. It leads from 9*10^18 to 18*10^18
        private Int64 actualLength = 0L;
        private Int64 desiredLength = 0L;
        private Int64 actualOrdinal = 0L;
        private Int64 desiredOrdinal = 0L;
        private Int64 actualPrime = 0L;
        private bool canOperate = false;
        // Riemann exponent s in C; s=:(sigma + i*t).
        private double sigma;
        private double t;

        public Int64 getActualLength()
        {
            return this.actualLength;
        }

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

        # endregion Data


        # region Ctor


        #region secondaryConstructionPath

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="threshold">natural threshold until which the loop will search</param>
        /// <param name="fullpath"></param>
        public Primes(
                Int64 threshold,
                string fullpath // of the stream containing the results of a previous execution.
            )
        {
            if (threshold <= 0) return;
            this.threshold = threshold;
            //
            //---input file parsing----
            if (null == fullpath)
            {
                this.workOnTmpFile();
            }
            else// go parse the file.
            {
                this.workOnSpecifiedFile( fullpath);
                if (this.ToBeDivided >= threshold)
                {
                    this.canOperate = false;
                }// else calculation is needed.
            }//---end input file parsing.----
            // ready.
        }// end ctor


        private void workOnTmpFile()
        {
            string tempPath = System.IO.Path.GetTempPath();
            string desinenza = DateTime.Now.ToString();
            desinenza = desinenza.Replace('/', '_');
            desinenza = desinenza.Replace(' ', '_');
            desinenza = desinenza.Replace('.', '_');
            desinenza = desinenza.Replace(':', '_');
            desinenza = desinenza.Replace(';', '_');
            string tempFile = tempPath + @"\Primes_until_" + desinenza + "_.txt";
            previousShot =
                System.IO.File.Open(
                    tempFile,// substitutes a null fullpath.
                    System.IO.FileMode.OpenOrCreate,
                    System.IO.FileAccess.ReadWrite,// read & append.
                    System.IO.FileShare.None
                );
            if (null == previousShot) return;//exit on wrong input.
            //---init-------
            // start force first record in empty file.
            ToBeDivided = 2L;// start from two. One is invariant of the operator.
            this.primeOrdinal = 0L;// with an empty file the Two too is still to be found.
            byte[] byteArray = ConvertPrimeToByteArrayRepresentation(ToBeDivided, ++this.primeOrdinal);
            previousShot.Write(byteArray, 0, byteArray.Length);
            previousShot.Flush();
            // end force first record in empty file.
            actualLength = previousShot.Length;
            desiredLength = 0L;
            actualPrime = 2L;
            actualOrdinal = primeOrdinal;
            desiredOrdinal = 0L;
            this.canOperate = true;
        }

        #endregion secondaryConstructionPath




        #region primaryConstructionPath



        /// <summary>
        /// default Ctor. Points to the App.config specified file.
        /// </summary>
        public Primes()
        {
            ConfigurationLayer.ConfigurationService config = new ConfigurationLayer.ConfigurationService(
                "FileSystemCash/fullpath");
            string fullpath = config.GetStringValue("dumpfile");
            this.workOnSpecifiedFile(fullpath);
        }


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


        private void workOnSpecifiedFile(
            string fullpath)
        {
            lock (typeof(PrimesFinder.Primes))
            {// ctor on existing file is a critical section.
                try
                {// if file is in use, it will catch
                    previousShot =
                        System.IO.File.Open(
                            fullpath,
                            System.IO.FileMode.OpenOrCreate,
                            System.IO.FileAccess.ReadWrite,// read & append.
                            System.IO.FileShare.None
                        );
                }
                catch (System.Exception ex)
                {
                    LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks("Red semaphore encountered while trying to acces the prime_file. Ex= " + ex.Message, 0);
                    this.canOperate = false;
                    this.previousShot = null;
                }
                if (null == previousShot) return;//exit on wrong input.
                //--evaluation of case "suggested file path, but file is empty"
                if (previousShot.Length < 5)// "\r\n2_1" is the minimal input. It's the first "prime_ordinal".
                {// then init.
                    // start force first record in empty file.
                    ToBeDivided = 2L;// start from two. One is invariant of the operator.
                    this.primeOrdinal = 0L;// with an empty file the Two too is still to be found.
                    byte[] byteArray = ConvertPrimeToByteArrayRepresentation( ToBeDivided, ++this.primeOrdinal);
                    previousShot.Write(byteArray, 0, byteArray.Length);
                    previousShot.Flush();
                    // end force first record in empty file.
                    actualLength = previousShot.Length;
                    desiredLength = 0L;
                    actualPrime = 2L;
                    actualOrdinal = primeOrdinal;
                    desiredOrdinal = 0L;
                    this.canOperate = true;
                }// end "suggested file path, but file is empty".
                else
                {// existing previous shot. 
                    previousShot.Seek(-1, System.IO.SeekOrigin.End);// goto read last published prime; -1 is last char before EOF. EOF==-1.
                    int lastKnownPrime_figureToken = 0;
                    string lastKnownPrimeReversed = "";
                    int acc = -1;
                    for (; ; )
                    {
                        lastKnownPrime_figureToken = previousShot.ReadByte();
                        --acc;
                        if (Math.Abs(acc) > previousShot.Length) break;// reached beginning of file, stepping backwards.
                        previousShot.Seek(acc, System.IO.SeekOrigin.End);// goto read last published prime.
                        if ('\r' != (char)lastKnownPrime_figureToken && '\n' != (char)lastKnownPrime_figureToken)
                        {
                            lastKnownPrimeReversed += (char)lastKnownPrime_figureToken;
                        }
                        else
                        {
                            break;// end number.
                        }
                    }// end acquire last prime string, from the last figure; now revert it.
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(lastKnownPrimeReversed.Length);
                    for (int c = lastKnownPrimeReversed.Length - 1; c >= 0; c--)
                    {
                        sb.Append(lastKnownPrimeReversed[c]);
                    }
                    string lastKnowPrime_ordinal = sb.ToString();// syntax: {"\r\n"prime_ordinal}
                    string[] tokenDecomposition = lastKnowPrime_ordinal.Split('_');
                    //
                    if (2 > tokenDecomposition.Length)
                    {// empty file.
                        // start force first record in empty file.
                        ToBeDivided = 2L;// start from two. One is invariant of the operator.
                        this.primeOrdinal = 0L;// with an empty file the Two too is still to be found.
                        byte[] byteArray = ConvertPrimeToByteArrayRepresentation(ToBeDivided, ++this.primeOrdinal);
                        previousShot.Write(byteArray, 0, byteArray.Length);
                        previousShot.Flush();
                        // end force first record in empty file.
                        actualLength = previousShot.Length;
                        desiredLength = 0L;
                        actualPrime = 2L;
                        actualOrdinal = primeOrdinal;
                        desiredOrdinal = 0L;
                        this.canOperate = true;
                    }// end empty file.
                    else
                    {// file with a valid previous shot.
                        try
                        {
                            string lastKnowPrime = tokenDecomposition[0];
                            this.ToBeDivided = Int64.Parse(lastKnowPrime) + 1L;// next integer after last prime published.
                            this.primeOrdinal = Int64.Parse(tokenDecomposition[1]);// ordinal of last prime found in previous shot.
                            this.actualLength = previousShot.Length;
                            this.desiredLength = 0L;
                            this.actualOrdinal = this.primeOrdinal;
                            this.desiredOrdinal = 0L;
                            this.actualPrime = ToBeDivided - 1L;
                            this.canOperate = true;
                        }
                        catch (System.Exception ex)
                        {
                            LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                                "Wrong content found in prime_file. Ex= " + ex.Message, 0);
                            this.canOperate = false;
                            this.previousShot = null;
                            return;//exit on wrong input.
                        }
                    }// end file with a valid previous shot.
                }// end "existing previous shot".
            }// end critical section
        }//---end input file parsing.----


        #endregion primaryConstructionPath




        /// <summary>
        /// Ctor for calculation path.
        /// Ctor for Riemann zeta function calculations.
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="t"></param>
        public Primes(double sigma, double t)
        {
            this.sigma = sigma;
            this.t = t;
        }//



        ~Primes()
        {
            this.fcloser();
        }

        public void Dispose()
        {
            this.fcloser();
        }


        private void fcloser()
        {
            lock (typeof(PrimesFinder.Primes))
            {
                if (null != previousShot)
                {
                    if (previousShot.CanWrite)
                    {
                        try
                        {
                            previousShot.Flush();
                        }
                        catch (System.Exception ex)
                        {// trap all. Tested that such Abort related exceptions do not harm the database.
                            LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                                "Primes::fcloser  Exception while flushing the prime_file. Exception= " + ex.Message,
                                0
                            );
                        }
                    }// else nothing to be flushed. NB. can throw, due to the state in which the calc-thread abort occurs.
                    previousShot.Close();
                    previousShot = null;
                }
            }// end critical section.
        }


        # endregion Ctor


        # region consultation

        /// <summary>
        /// read both prime and its ordinal.
        /// the format is prime_ordinal in the file.
        /// </summary>
        public void readPreviousRecordfromAnywhere()
        {
            int actualCharacter = 0;
            string primeAccumulator = "";
            string ordinalAccumulator = "";
            //
            for (; ;  )
            {
                actualCharacter = this.previousShot.ReadByte();
                if (-1 == actualCharacter) break;// EOF
                if ('\n' == (char)actualCharacter) break;// last element in the sequence "\r\n".
                this.previousShot.Seek(-2, System.IO.SeekOrigin.Current);// 2 back for each read ahead.
            }
            //
            for (int acc = 0; ; acc++)
            {
                actualCharacter = this.previousShot.ReadByte();
                //
                if (-1 == actualCharacter)// i.e. EOF
                {
                    if (0 == acc)
                    {
                        this.previousShot.Seek(-3, System.IO.SeekOrigin.Current);//"\r\nx_y" i.e. 5 bytes, in the smaller case.
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }// else continue.
                if ('\r' == actualCharacter)
                {
                    this.previousShot.ReadByte();// skip '\n' on Windows.
                    if (0 == acc)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }// else continue.
                if ('\n' == actualCharacter)
                {
                    break;
                }// else continue.
                if ('_' == actualCharacter)
                {
                    for (; ; )
                    {
                        actualCharacter = this.previousShot.ReadByte();
                        if (-1 == actualCharacter) break;// EOF.
                        if ('\r' == actualCharacter)
                        {
                            actualCharacter = this.previousShot.ReadByte();// burn the '\n'
                            break;
                        }
                        else// no EOF, no '\r' no '\n'.
                        {
                            ordinalAccumulator += (char)actualCharacter;
                        }
                    }
                    break;
                }// else continue.
                //
                // append until '_'
                primeAccumulator += (char)actualCharacter;
            }
            //
            try
            {
                this.actualPrime = Int64.Parse(primeAccumulator);
                this.actualOrdinal = Int64.Parse(ordinalAccumulator);
            }
            catch (System.Exception ex)
            {
                LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                    "wrong input file. Give an empty file, or a file containing sequences like newLinePrime_Ordinal. Exception was: " + ex.Message,
                    0
                );
            }
            // ready
        }//


        private bool TrySeek_inCollection(Int64 where)
        {
            bool result = false;
            if (
                where <= this.previousShot.Length
                && 0 <= where)
            {
                this.previousShot.Seek(where, System.IO.SeekOrigin.Begin);
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }


        /// <summary>
        /// questo metodo ha una tecnica di bisezione lineare.
        /// Come ordinale di ingresso, nel caso non si disponga dell'ordinale esatto, sono utili le stime logaritmiche:
        /// Ord ==circa:  Soglia / Log[Soglia]
        /// Soglia ==circa:  Ord * Log[Ord] (vedasi considerazioni di Derbyshire)
        /// 
        /// </summary>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public bool SeekingEngine(Int64 ordinal)
        {
            bool result = true;// init to true, to give ok when fpointer is already positioned, and delta results=1.0 without entering the loop.
            this.desiredOrdinal = ordinal;
            double delta = (double)this.desiredOrdinal / (double)this.actualOrdinal;// to boot the loop.
            //
            for (; Math.Abs(delta - 1.0)>double.Epsilon; )
            {
                this.desiredLength = (Int64)(delta * this.actualLength);// at first loop, actualLength is the whole file size.
                result = this.TrySeek_inCollection( this.desiredLength );
                if (! result) break;// else continue.
                this.readPreviousRecordfromAnywhere();
                delta = (double)this.desiredOrdinal / (double)this.actualOrdinal;// to boot the loop.
                this.actualLength = this.desiredLength;
            }
            // ready
            return result;
        }



        
        
        /// <summary>
        /// read the Prime at a specified ordinal.
        /// </summary>
        /// <param name="specifiedOrdinal">the specified Ordinal </param>
        /// <returns>the Prime, which has the position "specifiedOrdinal" in the sequence of Primes.</returns>
        public static long readPrimeAtSpecifiedOrdinal( Int64 specifiedOrdinal )
        {
            long res = -1L;// init to invalid.
            PrimesFinder.Primes thePrimes = new PrimesFinder.Primes();
            //
            if (thePrimes.getCanOperateStatus())
            {
                if (
                    0L < specifiedOrdinal
                    && thePrimes.getActualOrdinal() >= specifiedOrdinal
                    )
                {
                    bool canSeek = thePrimes.SeekingEngine(specifiedOrdinal);
                    if (!canSeek)
                    {
                        res = -1L;// set res to invalid.
                    }
                    else
                    {
                        res = thePrimes.getActualPrime();// that's the answer.
                    }
                }
                else
                {
                    res = -1L;// set res to invalid. Unreachable Prime.
                }
            }// else can do nothing
            else
            {
                res = -1L;// set res to invalid.boardMessage = "\r\n dump file busy: unable to read.";
            }
            // anyway
            thePrimes.Dispose();
            // ready
            return res;
        }//




        # endregion consultation


        #region production


        /// <summary>
        /// TODO bugged: truncates randomly rounfing the suggested length.
        /// tool to truncate after an estabilished record, on emergency due to wrong tails.
        /// </summary>
        /// <param name="newLength"></param>
        private void deleteAfter(Int64 requiredLastOrdinal)
        {// NB.  no \r\n at EOF.
            this.SeekingEngine( requiredLastOrdinal);
            this.readPreviousRecordfromAnywhere();
            Int64 newLength = this.previousShot.Position;
            int actualChar = -1;
            for (
                ;//
                ;
                this.previousShot.Seek(-2L, System.IO.SeekOrigin.Current), newLength -= 1L
              )
            {
                actualChar = previousShot.ReadByte();
                if (13 == actualChar) break;
            }
            this.previousShot.SetLength( newLength  );
            this.previousShot.Flush();
            this.actualLength = previousShot.Length;
        }//


        /// <summary>
        /// disregard the ordinal. Prime only is needed in collection-enriching.
        /// NB. one "\r\n" at beginning of file.
        ///     neither "\r" nor "\n" at EOF.
        ///     
        /// ex ReadOneMultiFigureInteger
        /// 
        /// </summary>
        /// <param name="this.previousShot"></param>
        /// <returns></returns>
        private Int64 read_prime_withoutOrdinal()
        {
            int actualCharacter = 0;
            string wholeSequence = "";
            int acc = 0;
            for (; ; acc++)
            {
                actualCharacter = this.previousShot.ReadByte();
                //
                if (-1 == actualCharacter)// i.e. EOF
                {
                    if (0 == acc)
                    {
                        wholeSequence = "0";
                        break;
                    }
                    else
                    {
                        break;
                    }
                }// else continue.
                if ('\r' == actualCharacter)
                {
                    this.previousShot.ReadByte();// skip '\n' on Windows.
                    if (0 == acc)
                    {
                        continue;
                    }
                    else
                    {
                        break;
                    }
                }// else continue.
                if ('\n' == actualCharacter)
                {
                    break;
                }// else continue.
                if ('_' == actualCharacter)
                {
                    for (; ; )
                    {
                        actualCharacter = this.previousShot.ReadByte();// disregard the ordinal. Prime only is needed in collection-enriching.
                        if (-1 == actualCharacter) break;// EOF.
                        if('\r'==actualCharacter)
                        {
                            actualCharacter = this.previousShot.ReadByte();// burn the '\n'
                            break;
                        }
                    }
                    break;
                }// else continue.
                //
                // append until '_'
                wholeSequence += (char)actualCharacter;
            }
            //
            Int64 result = default(Int64);
            try
            {
                result = Int64.Parse(wholeSequence);
            }
            catch (System.Exception ex)
            {
                LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                    "wrong input file. Give an empty file, or a file containing sequences like newLinePrime_Ordinal. Exception was: " + ex.Message,
                    0
                );
            }
            // ready
            return result;
        }//



        /// <summary>
        /// NB. one "\r\n" at beginning or of file.
        ///     neither "\r" nor "\n" at EOF.
        ///     
        ///     syntax for generic element is "\r\n"prime_ordinal.
        /// </summary>
        /// <param name="ToBeDivided"></param>
        /// <returns></returns>
        private byte[] ConvertPrimeToByteArrayRepresentation(
            Int64 ToBeDivided,
            Int64 ordinalInPrimes
        )
        {
            string str_ToBeDivided = ToBeDivided.ToString();
            string str_ordinalInPrimes = ordinalInPrimes.ToString();
            byte[] byteArray = new byte[str_ToBeDivided.Length + 2 + 1 + str_ordinalInPrimes.Length];//+2 to add '\r' '\n', +1 for '_' separator.
            byteArray[0] = (byte)'\r';
            byteArray[1] = (byte)'\n';
            int c = 2;
            for (; c < str_ToBeDivided.Length + 2; c++)
            {
                byteArray[c] = (byte)str_ToBeDivided[c - 2];
            }
            byteArray[c] = (byte)'_';
            for ( c = str_ToBeDivided.Length + 2+1; c < str_ToBeDivided.Length + 2 + 1 + str_ordinalInPrimes.Length; c++)
            {
                byteArray[c] = (byte)str_ordinalInPrimes[c - (str_ToBeDivided.Length + 2 + 1)];
            }
            // ready
            return byteArray;
        }//




        /// <summary>
        /// extract from N(naturals) a subsequence of the primes, i.e. the primes until a threshold.
        ///
        /// NB. one "\r\n" at beginning of file.
        ///     neither "\r" nor "\n" at EOF.
        /// </summary>
        public void CoreLoop()
        {
            if (null == this.previousShot)
                LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                    "Call constructor to initialize, before starting the loop.",
                    0
                );
            //----definition of loop-ralated variables and constants.-----
            const double epsilon = double.Epsilon;// system constant ~+4.9e-324
            bool isStillPotentialPrime = true;
            Int64 divider = 2L;// the prime I'm actually using as divider. P[1]=2, i.e. first prime is 2.
            double quotient = 1.0;// due to inner loop rules, must be initialized >divider. 
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
                        previousShot.Seek(0L, System.IO.SeekOrigin.Begin),// start from the first prime.
                        quotient = divider + 1.0;// due to inner loop rules, must be initialized >divider. 
                        //
                        quotient > divider;// for monotonicity of P, cannot search smallers, going ahead.
                        //NB. no more Diophantine quotient exist in the open set (1,n), for the monotonicity of function 
                        //    quotient=:ToBeDivided/x. It's hyperbolic.
                        //
                        // no increment whatsoever.
                    )
                {
                    divider = read_prime_withoutOrdinal();
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
                    previousShot.Seek(0, System.IO.SeekOrigin.End);
                    byte[] byteArray = ConvertPrimeToByteArrayRepresentation( ToBeDivided, ++this.primeOrdinal );
                    previousShot.Write(byteArray, 0, byteArray.Length);
                    // the start of inner loop seeks to the origin.
                }
                else
                {
                    // just skip it.
                }
            }// end outer loop. Threshold reached.------primes' subsequence built.-----------------------------
            //
            this.fcloser();
            //-----------
            //ready
        }// end CoreLoop.


        #endregion production


        # region calculation


        public struct Euler_Riemann_data
        {
            public double[] Euler_product;
            public double[] Dirichlet_sum;
            public double[] gap;
        }//


        /// <summary>
        ///    notazione di Riemann.
        ///    s in C
        ///    s =: (sigma + i*t)
        ///    in an istance variable.
        /// 
        /// </summary>
        /// <param name="threshold"> in N; p in P, p_not_greater_of_threshold </param>
        /// <returns></returns>
        public Euler_Riemann_data Euler_Riemann_equation(
            Int64 threshold
        )
        {
            Euler_Riemann_data result;// struct, i.e. value type.
            //
            result.Euler_product = Euler_product( threshold);
            result.Dirichlet_sum =  Dirichlet_sum( threshold);
            //
            // convention: Euler - Dirichlet, in signed difference.
            result.gap = new double[2];
            result.gap[0] = result.Euler_product[0] - result.Dirichlet_sum[0];
            result.gap[1] = result.Euler_product[1] - result.Dirichlet_sum[1];
            //
            // ready.
            return result;
        }


        /// <summary>
        /// Product 1 / (1-(1/p)^s)
        /// 
        /// where p in Primes
        /// s in Complex plane
        /// </summary>
        public double[] Euler_product(Int64 threshold)
        {
            double[] result = new double[2];
            result[0] = 1.0;// init to product invariant.
            result[1] = 0.0;// init to product invariant.
            // primes' database needed; Euler product goes on Prime sequence.
            ConfigurationLayer.ConfigurationService config = new ConfigurationLayer.ConfigurationService(
                "FileSystemCash/fullpath");
            string fullpath = config.GetStringValue("dumpfile");
            this.workOnSpecifiedFile(fullpath);
            this.previousShot.Seek( 0L, System.IO.SeekOrigin.Begin);// start from first prime.
            //
            for (Int64 p = default(Int64); p <= threshold; )
            {
                p = this.read_prime_withoutOrdinal();// get next prime.
                if (p > threshold) break;//NB. from a prime to the next, threshold can be jumped over.
                double[] Euler_currentFactor = Euler_factor( p);//with current factor do: result *= current_fact.
                double swap_result0 = result[0];
                result[0] = result[0] * Euler_currentFactor[0] - result[1] * Euler_currentFactor[1];
                result[1] = swap_result0 * Euler_currentFactor[1] + result[1] * Euler_currentFactor[0];
            }
            // release primes' database.
            this.Dispose();
            //
            return result;
        }


        /// <summary>
        /// 1 / (1-(1/p)^s)
        /// </summary>
        private double[] Euler_factor(Int64 current_prime)
        {//
            double[] result = new double[2];
            //
            double exp_less_sigma_ln_p = Math.Exp(-sigma * Math.Log(current_prime));
            double less_t_ln_p = -t * Math.Log(current_prime);
            double denom = exp_less_sigma_ln_p * (exp_less_sigma_ln_p - 2 * Math.Cos(less_t_ln_p)) + 1.0;
            result[0] = (1.0 - exp_less_sigma_ln_p * Math.Cos(less_t_ln_p)) / denom; // real part.
            result[1] = (exp_less_sigma_ln_p * Math.Sin(less_t_ln_p)) / denom; // immaginary part.
            //
            return result;
        }


        /// <summary>
        /// Sum 1 / n^s
        /// 
        /// n in Naturals
        /// s in Complex plane
        /// 
        /// n^-s = exp( -s * ln(n) ).
        /// being s =: sigma + i*t  -> n^-s = exp( -sigma*ln(n) -i*t*ln(n) )
        /// exp(-sigma*ln(n)) * exp(-i*t*ln(n)) = exp(-sigma*ln(n)) * ( cos(-t ln(n)) +i*sin(-t ln(n)))
        /// exp(-sigma*ln(n)) * ( cos(t ln(n)) -i*sin(t ln(n)))
        /// {exp(-sigma*ln(n)) *  cos(t ln(n)), -exp(-sigma*ln(n)) *  sin(t ln(n))}
        /// </summary>
        public double[] Dirichlet_sum(Int64 threshold)
        {
            double[] result = new double[2];
            result[0] = result[1] = 0.0;// init for sum.
            //
            for (Int64 n = 1L; n <= threshold; n++)
            {
                double[] Dirichlet_current_element = this.Dirichlet_element( n);
                // dbg
                //Console.WriteLine(Dirichlet_current_element[0].ToString() + "  " + Dirichlet_current_element[1].ToString());
                // end dbg
                result[0] += Dirichlet_current_element[0];// real part.
                result[1] += Dirichlet_current_element[1];// immaginary part.
            }
            //
            return result;
        }


        /// <summary>
        ///  1 / n^s
        /// 
        /// n in Naturals
        /// s in Complex plane
        /// 
        /// n^-s = exp( -s * ln(n) ).
        /// being s =: sigma + i*t  -> n^-s = exp( -sigma*ln(n) -i*t*ln(n) )
        /// exp(-sigma*ln(n)) * exp(-i*t*ln(n)) = exp(-sigma*ln(n)) * ( cos(-t ln(n)) +i*sin(-t ln(n)))
        /// exp(-sigma*ln(n)) * ( cos(t ln(n)) -i*sin(t ln(n)))
        /// {exp(-sigma*ln(n)) *  cos(t ln(n)), -exp(-sigma*ln(n)) *  sin(t ln(n))}
        /// </summary>
        private double[] Dirichlet_element(Int64 current_natural)
        {
            double[] result = new double[2];
            //
            double exp_less_sigma_ln_n = Math.Exp(-sigma * Math.Log(current_natural));
            double less_t_ln_n = -t * Math.Log(current_natural);
            result[0] = exp_less_sigma_ln_n * Math.Cos(less_t_ln_n);// real part.
            result[1] = exp_less_sigma_ln_n * Math.Sin(less_t_ln_n);// immaginary part.
            //
            return result;
        }



        /// <summary>
        /// funzione a gradino; utilizza il file di dump integrale.
        /// </summary>
        /// <param name="threshold"></param>
        /// <param name="theInfinPrimes"></param>
        /// <returns></returns>
        public long GetCumulatedOrdinalNotOverThreshold( long threshold, out long theInfinPrimes )
        {
            long CumulatedOrdinalNotOverThreshold = 0L;
            theInfinPrimes = -1L;// means no prime below threshold.
            // tale valore integrale va ricercato nell'ordinale corrispondente al piu' grande dei primi non maggiori di arg.
            // Ovvero presso il sup dei primi minoranti arg. Non vi sono altri primi fra quello individuato ed arg.
            // Pertanto PiGreco() vale :
            // l'ordinale del piu' grande dei primi non maggiori di arg, meno 1/2, se arg è primo.
            // l'ordinale del piu' grande dei primi non maggiori di arg,           se arg NON è primo.
            //
            if (threshold < +2.0)
            { return CumulatedOrdinalNotOverThreshold; }// zero, by now.
            else // else threshold >= +2.0)
            { // x/Log(x)==circa all'ordinale che cerco, ovvero ordinale( Primo_vicino_x)
                double hypotizedOrdinal = threshold / Math.Log(threshold);
                Int64 hypotizedOrdinal_long = (Int64)hypotizedOrdinal;
                bool canSeek = this.SeekingEngine(hypotizedOrdinal_long);
                if (!canSeek)
                {//  "\r\n required ordinal is outside the actual sequence.";
                    throw new System.Exception("Required ordinal is outside the actual sequence.");
                }// else continue.
                Int64 lastPrimeFound = this.getActualPrime();
                long lastOrdinal = this.getActualOrdinal();
                // cerco il piu' grande dei primi minori o uguali( x)
                if (lastPrimeFound >= threshold)
                {
                    for (; lastPrimeFound > threshold; )
                    {
                        this.SeekingEngine(--lastOrdinal);// go back
                        //this.readPreviousRecordfromAnywhere();// update this'fields.
                        lastPrimeFound = this.actualPrime;
                        if (this.actualPrime < threshold)
                        {
                            this.SeekingEngine(++this.desiredOrdinal);
                            break;// from for
                        }// end exit adjustment. else continue.
                    }// end for (; lastPrimeFound >= threshold; )
                }// end if (lastPrimeFound >= threshold)
                else if (lastPrimeFound < threshold)
                {
                    for (; lastPrimeFound <= threshold; )
                    {
                        this.SeekingEngine(++lastOrdinal);// go forth
                        //this.readPreviousRecordfromAnywhere();// update this'fields.
                        lastPrimeFound = this.actualPrime;
                        if (this.actualPrime > threshold)
                        {
                            this.SeekingEngine(--this.desiredOrdinal);
                            break;// from for
                        }// end exit adjustment. else continue.
                    }// end for (; lastPrimeFound <= threshold;  )
                }// end else if (lastPrimeFound <= threshold)
                else // == TODO ??
                { }
                theInfinPrimes = this.actualPrime;// the Inf.
                CumulatedOrdinalNotOverThreshold = this.actualOrdinal;// cumulated Prime cardinality, on the greatestPrime<=arg.
                //
            }// end // else threshold >= +2.0)
            //
            // ready.
            return CumulatedOrdinalNotOverThreshold;
        }// end  long GetCumulatedOrdinalNotOverThreshold( long threshold )



        /// <summary>
        /// funzione a gradino; utilizza il file di dump integrale.
        /// salta di mezza unita' ad ogni primo. Altra mezza unita' di salto a (primo + epsilon).
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public double Pi_Greco( double arg )
        {
            double PrimeCardinality_untilArg = 0.0;
            double delta = arg - (Int64)arg;
            Int64 arg_intPart = (Int64)arg;
            bool is_arg_natural = (delta < double.Epsilon);
            long theInfinPrimes;
            //
            long CumulatedOrdinalNotOverThreshold = this.GetCumulatedOrdinalNotOverThreshold(arg_intPart, out theInfinPrimes);
            //
            if (Math.Abs(arg - theInfinPrimes) < double.Epsilon)
            {
                PrimeCardinality_untilArg = (double)CumulatedOrdinalNotOverThreshold - 0.5;
            }
            else // arg is not a Prime
            {
                PrimeCardinality_untilArg = (double)CumulatedOrdinalNotOverThreshold;
            }
            //
            //
            return PrimeCardinality_untilArg;
        }// end public double Pi_Greco(double arg)



        /// <summary>
        /// funzione a gradino; utilizza il file di dump integrale.
        /// salta di un'unita' ad ogni primo.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public double Pi_Greco_nonInterpolata( double arg )
        {
            double PrimeCardinality_untilArg = 0.0;
            double delta = arg - (Int64)arg;
            Int64 arg_intPart = (Int64)arg;
            bool is_arg_natural = (delta < double.Epsilon);
            long theInfinPrimes;
            //
            long CumulatedOrdinalNotOverThreshold = this.GetCumulatedOrdinalNotOverThreshold(arg_intPart, out theInfinPrimes);
            // don't; if (Math.Abs(arg - theInfinPrimes) < double.Epsilon); 
            PrimeCardinality_untilArg = (double)CumulatedOrdinalNotOverThreshold;
            //
            return PrimeCardinality_untilArg;
        }// end public double Pi_Greco_nonInterpolata( double arg )




        /// <summary>
        /// funzione a gradino; utilizza il file di dump integrale.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public double J( double arg )
        {// J=:Sum[(1/k)*Pi_Greco( arg^(1/k) ) , {k,1,stop}// stop is the Sup(k in N) where arg^(1/k)>=+2.
            double res = 0.0;
            System.Collections.ArrayList theNthRoots = new System.Collections.ArrayList();
            int c = 1;
            double NthRoot = 0.0;
            for (; ; )
            {
                NthRoot = Math.Pow(arg, 1.0 / c);
                if (NthRoot < +2.0) { break; }
                else 
                {
                    theNthRoots.Add(NthRoot);
                    c++;
                }
            }// end for
            //
            for (int d = 0; d < c - 1; d++)// the ArrayList starts from zero, but the nthRoot indexes start from 1
            {// the index has to start from +1 but the ArrayList starts from zero.
                double tempNthRootPival = this.Pi_Greco( (double)(theNthRoots[d]) );
                res += (1.0 / ((double)d+1.0)) * tempNthRootPival ;
            }
            // ready
            return res;
        }// end public double J( double arg )


        /// <summary>
        /// funzione a gradino; utilizza il file di dump integrale.
        /// esprime J(x) in funzione della Pi_Greco_nonInterpolata()
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public double J_nonInterpolata( double arg )
        {// J=:Sum[(1/k)*Pi_Greco_nonInterpolata( arg^(1/k) ) , {k,1,stop}// stop is the Sup(k in N) where arg^(1/k)>=+2.
            double res = 0.0;
            System.Collections.ArrayList theNthRoots = new System.Collections.ArrayList();
            int c = 1;
            double NthRoot = 0.0;
            for (; ; )
            {
                NthRoot = Math.Pow(arg, 1.0 / c);
                if (NthRoot < +2.0) { break; }
                else
                {
                    theNthRoots.Add(NthRoot);
                    c++;
                }
            }// end for
            //
            for (int d = 0; d < c - 1; d++)// the ArrayList starts from zero, but the nthRoot indexes start from 1
            {// the index has to start from +1 but the ArrayList starts from zero.
                double tempNthRootPival = this.Pi_Greco_nonInterpolata((double)(theNthRoots[d]));
                res += (1.0 / ((double)d + 1.0)) * tempNthRootPival;
            }
            // ready
            return res;
        }// end public double J( double arg )


        public double Pi_calculated_onJ( double x_thresholdInR )
        {
            double primeCardinalityUntilThresholdInR = 0.0;// init
            //-------
            // NB. this is not yet a result, since Pi is still counted in the first place. So J is fed with a counted Pi(x). But then, by means of Moebius inversion,
            //     the Pi gets calculated on the J -which although was still tabulated on the counted Pi(x)-
            //     The crucial step by Riemann, will be to obtain J=J(Z(s)) by a Mellin transform of J(x), which gives 1/s*Log(Z(s))
            //     and then, having J=J(Z(s)) there will be a fully calculated version of Pi(x)=Pi(J(Z(s)) where no more tabulated versions 
            //     of neither of Pi nor of J appear. The whole package is computed by means of Z-Analysis.
            //
            System.Collections.ArrayList theNthRoots = new System.Collections.ArrayList();
            int c = 1;
            double NthRoot = 0.0;
            for (; ; )
            {
                NthRoot = Math.Pow( x_thresholdInR, 1.0 / c);
                if (NthRoot < +2.0) { break; }
                else
                {
                    theNthRoots.Add(NthRoot);
                    c++;
                }
            }// end for
            double sumRes = 0.0;// init
            for (int n = 1; n < theNthRoots.Count; n++)
            {
                sumRes += (1.0 / n) * PrimesFinder.ElementFullCalculation.Moebius_Mi(n) * this.J(Math.Pow(x_thresholdInR, 1.0 / n));
            }
            //throw new System.Exception("TODO : implement Pi=Pi(J), the Pi calculated on J=J(Pi).");
            //
            primeCardinalityUntilThresholdInR = sumRes;
            return primeCardinalityUntilThresholdInR;
        }// Pi_calculated_onJ


        # endregion calculation


    }// end class Primes


}// end nmsp
