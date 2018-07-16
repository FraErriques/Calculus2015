using System;
using System.Collections.Generic;
using System.Text;


namespace Process
{


    public static class Calculation
    {
        #region Data

        public delegate void AsynchronousMessageWriterPtr(string theMessage);
        public delegate void Asynchronous_calculation_Starter(object theInput);
        //
        public struct CommonCalculationInput
        {
            // Euler Riemann data
            public double sigma;
            public double t;
            public Int64 naturalThreshold;
            //
            // LogIntegral data
            public Int64 integrationDomain_sup;
            //
            // common data
            public int thread_index;
        }
        //
        /// <summary>
        /// TODO: when new calculations get implemented, their names need to be published here, in order to invoke them.
        /// </summary>
        public enum CalculationActions
        {
            // invalid
            Invalid = 0,
            //
            EulerProduct = 1,
            //
            DirichletSum = 2,
            //
            RiemannZetaFunction = 3,
            //
            EulerRiemannEquation = 4,
            //
            LogIntegralFrom2ToX = 5,
            // it's the Integrate::Integrate_percentile() method, called to solve in x the integral equation: Integrate[dt/Log(t), {t,+2,x}]==measure.
            GetThresholdGivenCardinality = 6,
            // NB. IsolatedNaturalEvaluation is let synchronous since it requires a such small amount of calculation resources that
            //     is not worth starting an asynchronous thread.
        }//
        private static System.Threading.Thread[] CalculationAsynchronousThreads = null;
        private static int CalculationAsynchronousThreads_cardinality = 0;
        private static int CalculationAsynchronousThreads_available = 0;

        #endregion Data


        /// <summary>
        /// static Ctor; system invoked only.
        /// </summary>
        static Calculation()
        {
            ConfigurationLayer.ConfigurationService config = new ConfigurationLayer.ConfigurationService(
                "CalculationAsynchronousThreads/parallel");
            string str_card = config.GetStringValue("cardinality");
            try
            {
                Calculation.CalculationAsynchronousThreads_available =
                    Calculation.CalculationAsynchronousThreads_cardinality = int.Parse(str_card);
                Calculation.CalculationAsynchronousThreads = new System.Threading.Thread[CalculationAsynchronousThreads_cardinality];
            }
            catch (System.Exception ex)
            {
                LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                    "Exception in static ctor Process::Calculation. Review the app.config <CalculationAsynchronousThreads/parallel>. Ex=" + ex.Message,
                    0
                );
            }
        }//


        /// <summary>
        /// indexed on the primes -> lock required on the stream.
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="t"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static string EulerRiemannEquation(double sigma, double t, Int64 threshold)
        {
            PrimesFinder.Primes primes = new PrimesFinder.Primes( sigma, t);
            PrimesFinder.Primes.Euler_Riemann_data equationResults = // struct->stack-allocated.
                primes.Euler_Riemann_equation( threshold);
            primes.Dispose();
            string res =
                String.Format("\r\n required Euler Riemann equation with s=:sigma +i* t = {0} +i* {1}  and threshold in Naturals {2}", sigma, t, threshold);
            res += "\r\n Euler product result = " + equationResults.Euler_product[0].ToString() + " +i* " + equationResults.Euler_product[1].ToString();
            res += "\r\n Riemann sum = " + equationResults.Dirichlet_sum[0].ToString() + " +i* " + equationResults.Dirichlet_sum[1].ToString();
            res += "\r\n gap in between = " + equationResults.gap[0].ToString() + " +i* " + equationResults.gap[1].ToString() + "\r\n";
            return res;
        }//


        /// <summary>
        /// no data required -> no lock required.
        /// no data because goes on Naturals, not on Primes only.
        /// </summary>
        /// <param name="sigma"></param>
        /// <param name="t"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static double[] DirichletSum( double sigma, double t, Int64 threshold)
        {
            PrimesFinder.Primes RiemannZeta = new PrimesFinder.Primes(sigma, t);
            double[] res = RiemannZeta.Dirichlet_sum(threshold);
            RiemannZeta.Dispose();
            // ready
            return res;
        }//



        /// <summary>
        /// indexed on the primes -> lock required on the stream.
        /// </summary>
        /// <param name="boardMessage"></param>
        /// <param name="sigma"></param>
        /// <param name="t"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        private static double[] EulerProduct(
            double sigma, double t,
            Int64 threshold )
        {
            PrimesFinder.Primes thePrimes = new PrimesFinder.Primes( sigma, t);
            // NB. do not check "..if (thePrimes.getCanOperateStatus())". It's only for data production.
            double[] res = thePrimes.Euler_product( threshold);
            // ready
            thePrimes.Dispose();
            return res;
        }//




        /// <summary>
        /// No instance underneath. This core loop is static.
        /// Synchronous action. No thread forked.
        /// </summary>
        /// <param name="theIsolatedNatural"></param>
        public static string IsolatedNaturalEvaluation( Int64 theIsolatedNatural )
        {
            Int64 LastCandidateDivider;
            Int64 DiophantineQuotient;
            bool res =
                PrimesFinder.ElementFullCalculation.ElementFullEvaluation(
                    theIsolatedNatural,
                    out LastCandidateDivider,
                    out DiophantineQuotient
                );
            string message =
                String.Format(
                    "\r\n Isolated  Natural {0} primality = {1}. Last candidate divider was {2}",
                    theIsolatedNatural,
                    res.ToString(),
                    LastCandidateDivider
                );
            //
            return message;
        }//



        /// <summary>
        /// No instance underneath. This core loop is static.
        /// Synchronous action. No thread forked.
        /// </summary>
        /// <param name="theIsolatedNatural"></param>
        public static string FactorResearcher(Int64 theIsolatedNatural)
        {
            Int64 LastCandidateDivider = default(Int64);
            Int64 DiophantineQuotient = 2L;// init to >1.
            bool res = true;
            Int64 loop_theIsolatedNatural = theIsolatedNatural;// init.
            System.Collections.ArrayList factors = new System.Collections.ArrayList();
            //
            while (
                 1<DiophantineQuotient
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
                    factors.Add( LastCandidateDivider);
                }
                else
                {//NB. when the isolatedNatural leads to a quotient==-1, it means it must be divided for itsself and give quotient==1.
                    factors.Add( loop_theIsolatedNatural);
                }
                // after usage, refresh loop engine.
                loop_theIsolatedNatural = DiophantineQuotient;// refresh loop engine.
            }
            // build the response.
            System.Text.StringBuilder message = new StringBuilder();
            message.Append(
                "\r\n\r\n  Natural " + theIsolatedNatural.ToString() + " is composed of {\r\n"
            );
            for (int c = 0; c < factors.Count; c++)
            {
                message.Append(
                    Process.Consultation.fillWithDots(
                        factors[c].ToString()
                    )
                );
                if (c < factors.Count - 1)
                {
                    message.Append(",\r\n");
                }// else: do not add a comma after the last element.
            }
            message.Append("\r\n}\r\n\r\n");
            // ready.
            return message.ToString();
        }// public static string FactorResearcher(Int64 theIsolatedNatural)




        /*
        /// <summary>
        /// Evaluates a fraction of two integers and reduces it to mutually primes numerator and denominator.
        /// No instance underneath. This core loop is static.
        /// Synchronous action. No thread forked.
        /// </summary>
        /// <param name="theIsolatedNatural"></param>
        public static string FractionReducer( Int64 theNumerator, Int64 theDenominator )
        {
            // build the response.
            System.Text.StringBuilder message = new StringBuilder();
            message.Append(
                "\r\n\r\n Evaluates a fraction of two integers and reduces it to mutually primes numerator and denominator. "+
                "\r\nNumerator = "+theNumerator+"   Denominator = "+theDenominator + "\r\n\r\n" );
            RealField.Rationals_Q_ Q = new RealField.Rationals_Q_(theNumerator, theDenominator);
            Q.RationalReductionOnIntegers();
            message.Append( Q.ToSimplifiedFractionString() );
            message.Append("\r\n\r\n\r\n");
            // ready.
            return message.ToString();
        }// end public static string FactorResearcher( Int64 theIsolatedNatural )
        */




        /// <summary>
        /// No instance underneath. This core loop is static.
        /// Synchronous action. No thread forked.
        /// </summary>
        /// <param name="theIsolatedNatural"></param>
        /// <returns></returns>
        public static string FactorOmegaInformation( Int64 theIsolatedNatural )
        {
            long[,] myOmegaData = PrimesFinder.ElementFullCalculation.OmegaData(theIsolatedNatural);
            double[] theProd = new double[myOmegaData.Length / 2];
            double ToTProd = 1.0;
            // build the response.
            System.Text.StringBuilder message = new StringBuilder();
            message.Append(
                "\r\n\r\n  Natural " + theIsolatedNatural.ToString() + " is composed of {\r\n"
            );
            message.Append("\r\n\r\n");
            message.Append("   |-----------------------------------------------------------------------------------\r\n");
            for (int c = 0; c < myOmegaData.Length / 2; c++)
            {
                theProd[c] = Math.Pow((double)myOmegaData[c, 0], (double)myOmegaData[c, 1]);
                //message.Append("\t|p^k\t==\t{0}^{1}\t==\t{2}", myOmegaData[c, 0], myOmegaData[c, 1], theProd[c]);
                message.Append("   |   p^k   ==   " + myOmegaData[c, 0] + "^" + myOmegaData[c, 1] + "   ==   " + theProd[c] + "\r\n");
                ToTProd *= theProd[c];
                message.Append("   |-----------------------------------------------------------------------------------\r\n");
            }
            message.Append("   |   total facor product   ==   " + ToTProd + "\r\n");
            message.Append("   |-----------------------------------------------------------------------------------\r\n\r\n");
            // ready.
            //
            return message.ToString();
        }// end public static string FactorOmegaInformation( Int64 theIsolatedNatural )


        /// <summary>
        /// private, to not be accessed outside its asynchronous launcher.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double LogIntegralFrom2toX(Int64 x)
        {
            double lowerBound = 2.0;
            double upperBound = (double)x;
            double domainMeasure = Math.Abs( x - 2.0);
            double requiredPieceWidth = 0.03;// resulted optimal in a test.
            // requiredPieceWidth=domainMeasure/pieceCardinality ->
            Int64 pieceCardinality = (Int64)Math.Ceiling( domainMeasure / requiredPieceWidth);// no less than the required cardinality.
            return
            PrimesFinder.Integrate.Integrate_equi_log(
                lowerBound, // compulsory lower bound, from Prime Number Theorem.
                upperBound,
                pieceCardinality
            );
        }//


        /// <summary>
        /// private, to not be accessed outside its asynchronous launcher.
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static double PercentileLogIntegralFrom2toX(Int64 requiredMeasure)
        {
            double lowerBound = 2.0;
            // step size is not parametric, by now.
            return
            PrimesFinder.Integrate.Integrate_percentile_equilog(
                lowerBound, // compulsory lower bound, from Prime Number Theorem.
                (double)requiredMeasure,
                0.03 // delta resulted optimal in a test, for 1/ln(t).
            );
        }//



        /// <summary>
        /// executed asynchronously by a child thread, forked by "CalcThreadManager".
        /// No lock required. The unsubscribing action of a dead thread is an all asynchronous job. Locks here would compromise
        /// the mesh between background jobs, realized by the Operating System.
        /// The subscribing action of a borning thread, instead, needs a lock to ensure that the acquisition of the
        /// cardinality of available threads in the pool and the decrease of them, due to a thread start, are an atomic action.
        /// An interruption in such sequence could result in a false information of more threads available in the pool than
        /// the actual availability.
        /// </summary>
        /// <param name="par"></param>
        private static void Asynchronous_EulerProduct_Starter(object obj_in_par)
        {
            Process.Calculation.CommonCalculationInput in_par = (Process.Calculation.CommonCalculationInput)obj_in_par;
            // the following call is synchronous with respect to the forker thread, which is asynchronous with respect to the interface one.
            double[] EulerProduct_res = EulerProduct(
                in_par.sigma,
                in_par.t,
                in_par.naturalThreshold
            );// this result will be waited for.
            // NB. cannot acces interface from a thread other than the interface thread.   in_par.fptr( String.Format(" LogIntegralFrom2toX( {0}) = {1}", in_par.integrationDomain_sup, res) );
            // NB. Join() stops the caller, waiting for the callee; so the action becomes synchronous. That's undesired here. Calculation.CalculationAsynchronousThreads[in_par.thread_index].Join();
            //
            LogSinkFs.Wrappers.LogWrappers.SectionContent(//  results on the fs-log are accessible via menu.
                String.Format(
                    "\r\n required Euler product with s=:sigma +i* t = {0} +i* {1}  and threshold in Naturals {2}" +
                    "\r\n Euler product result = {3}  +i*  {4}",
                    in_par.sigma,
                    in_par.t,
                    in_par.naturalThreshold,
                    EulerProduct_res[0].ToString(),
                    EulerProduct_res[1].ToString()
                    ),
                0
            );// NB. cannot let null the asynchronous thread, cause we're within it. Th counter only will tell the main thread about the status of each asynchrnous one, in the pool.
            ++CalculationAsynchronousThreads_available;// give back the availability of this thread, since it's just dead.
        }//




        /// <summary>
        /// executed asynchronously by a child thread, forked by "CalcThreadManager".
        /// No lock required. The unsubscribing action of a dead thread is an all asynchronous job. Locks here would compromise
        /// the mesh between background jobs, realized by the Operating System.
        /// The subscribing action of a borning thread, instead, needs a lock to ensure that the acquisition of the
        /// cardinality of available threads in the pool and the decrease of them, due to a thread start, are an atomic action.
        /// An interruption in such sequence could result in a false information of more threads available in the pool than
        /// the actual availability.
        /// </summary>
        /// <param name="par"></param>
        private static void Asynchronous_DirichletSum_Starter(object obj_in_par)
        {
            Process.Calculation.CommonCalculationInput in_par = (Process.Calculation.CommonCalculationInput)obj_in_par;
            // the following call is synchronous with respect to the forker thread, which is asynchronous with respect to the interface one.
            double[] RiemannZetaSum_res = DirichletSum(
                in_par.sigma,
                in_par.t,
                in_par.naturalThreshold
            );// this result will be waited for.
            // NB. cannot acces interface from a thread other than the interface thread.   in_par.fptr( String.Format(" LogIntegralFrom2toX( {0}) = {1}", in_par.integrationDomain_sup, res) );
            // NB. Join() stops the caller, waiting for the callee; so the action becomes synchronous. That's undesired here. Calculation.CalculationAsynchronousThreads[in_par.thread_index].Join();
            //
            LogSinkFs.Wrappers.LogWrappers.SectionContent(//  results on the fs-log are accessible via menu.
                String.Format(
                    "\r\n required Riemann Zeta sum with s=:sigma +i* t = {0} +i* {1}  and threshold in Naturals {2}" +
                    "\r\n Riemann Zeta sum result = {3}  +i*  {4}",
                    in_par.sigma,
                    in_par.t,
                    in_par.naturalThreshold,
                    RiemannZetaSum_res[0].ToString(),
                    RiemannZetaSum_res[1].ToString()
                    ),
                0
            );// NB. cannot let null the asynchronous thread, cause we're within it. The counter only will tell the main thread about the status of each asynchrnous one, in the pool.
            ++CalculationAsynchronousThreads_available;// give back the availability of this thread, since it's just dead.
        }//




        /// <summary>
        /// executed asynchronously by a child thread, forked by "CalcThreadManager".
        /// No lock required. The unsubscribing action of a dead thread is an all asynchronous job. Locks here would compromise
        /// the mesh between background jobs, realized by the Operating System.
        /// The subscribing action of a borning thread, instead, needs a lock to ensure that the acquisition of the
        /// cardinality of available threads in the pool and the decrease of them, due to a thread start, are an atomic action.
        /// An interruption in such sequence could result in a false information of more threads available in the pool than
        /// the actual availability.
        /// </summary>
        /// <param name="par"></param>
        private static void Asynchronous_EulerRiemannEquation_Starter(object obj_in_par)
        {
            Process.Calculation.CommonCalculationInput in_par = (Process.Calculation.CommonCalculationInput)obj_in_par;
            // the following call is synchronous with respect to the forker thread, which is asynchronous with respect to the interface one.
            string EulerRiemannEquation_res = EulerRiemannEquation(
                in_par.sigma,
                in_par.t,
                in_par.naturalThreshold
            );// this result will be waited for.
            // NB. cannot acces interface from a thread other than the interface thread.   in_par.fptr( String.Format(" LogIntegralFrom2toX( {0}) = {1}", in_par.integrationDomain_sup, res) );
            // NB. Join() stops the caller, waiting for the callee; so the action becomes synchronous. That's undesired here. Calculation.CalculationAsynchronousThreads[in_par.thread_index].Join();
            //
            LogSinkFs.Wrappers.LogWrappers.SectionContent(//  results on the fs-log are accessible via menu.
                EulerRiemannEquation_res,
                0
            );// NB. cannot let null the asynchronous thread, cause we're within it. Th counter only will tell the main thread about the status of each asynchrnous one, in the pool.
            ++CalculationAsynchronousThreads_available;// give back the availability of this thread, since it's just dead.
        }//


        /// <summary>
        /// executed asynchronously by a child thread, forked by "CalcThreadManager".
        /// No lock required. The unsubscribing action of a dead thread is an all asynchronous job. Locks here would compromise
        /// the mesh between background jobs, realized by the Operating System.
        /// The subscribing action of a borning thread, instead, needs a lock to ensure that the acquisition of the
        /// cardinality of available threads in the pool and the decrease of them, due to a thread start, are an atomic action.
        /// An interruption in such sequence could result in a false information of more threads available in the pool than
        /// the actual availability.
        /// </summary>
        /// <param name="par"></param>
        private static void Asynchronous_LogIntegral_Starter( object obj_in_par)
        {
            Process.Calculation.CommonCalculationInput in_par = (Process.Calculation.CommonCalculationInput)obj_in_par;
            // the following call is synchronous with respect to the forker thread, which is asynchronous with respect to the interface one.
            double measure = LogIntegralFrom2toX( in_par.integrationDomain_sup);// this result will be waited for.
            // NB. cannot acces interface from a thread other than the interface thread.   in_par.fptr( String.Format(" LogIntegralFrom2toX( {0}) = {1}", in_par.integrationDomain_sup, res) );
            // NB. Join() stops the caller, waiting for the callee; so the action becomes synchronous. That's undesired here. Calculation.CalculationAsynchronousThreads[in_par.thread_index].Join();
            //
            LogSinkFs.Wrappers.LogWrappers.SectionContent(//  results on the fs-log are accessible via menu.
                String.Format(" LogIntegralFrom2toX( {0}) = {1}", in_par.integrationDomain_sup, measure ),
                0
            );// NB. cannot let null the asynchronous thread, cause we're within it. Th counter only will tell the main thread about the status of each asynchrnous one, in the pool.
            ++CalculationAsynchronousThreads_available;// give back the availability of this thread, since it's just dead.
        }//



        /// <summary>
        /// executed asynchronously by a child thread, forked by "CalcThreadManager".
        /// No lock required. The unsubscribing action of a dead thread is an all asynchronous job. Locks here would compromise
        /// the mesh between background jobs, realized by the Operating System.
        /// The subscribing action of a borning thread, instead, needs a lock to ensure that the acquisition of the
        /// cardinality of available threads in the pool and the decrease of them, due to a thread start, are an atomic action.
        /// An interruption in such sequence could result in a false information of more threads available in the pool than
        /// the actual availability.
        /// </summary>
        /// <param name="par"></param>
        private static void Asynchronous_PercentileLogIntegral_Starter(object obj_in_par)
        {
            Process.Calculation.CommonCalculationInput in_par = (Process.Calculation.CommonCalculationInput)obj_in_par;
            // the following call is synchronous with respect to the forker thread, which is asynchronous with respect to the interface one.
            double integration_sup = PercentileLogIntegralFrom2toX( in_par.naturalThreshold);// this result will be waited for.
            // NB. cannot acces interface from a thread other than the interface thread.   in_par.fptr( String.Format(" LogIntegralFrom2toX( {0}) = {1}", in_par.integrationDomain_sup, res) );
            // NB. Join() stops the caller, waiting for the callee; so the action becomes synchronous. That's undesired here. Calculation.CalculationAsynchronousThreads[in_par.thread_index].Join();
            //
            LogSinkFs.Wrappers.LogWrappers.SectionContent(//  results on the fs-log are accessible via menu.
                String.Format(" PercentileLogIntegralFrom2toX( {0}) = {1}", in_par.naturalThreshold, integration_sup ),
                0
            );// NB. cannot let null the asynchronous thread, cause we're within it. Th counter only will tell the main thread about the status of each asynchrnous one, in the pool.
            ++CalculationAsynchronousThreads_available;// give back the availability of this thread, since it's just dead.
        }//


        /// <summary>
        /// The subscribing action of a borning thread needs a lock to ensure that the acquisition of the
        /// cardinality of available threads in the pool and the decrease of them, due to a thread start, are an atomic action.
        /// An interruption in such sequence could result in a false information of more threads available in the pool than
        /// the actual availability.
        /// </summary>
        /// <param name="integrationDomain_sup"></param>
        /// <param name="fptr"></param>
        public static void CalcThreadManager(
            CommonCalculationInput in_par,
            CalculationActions requiredAction,
            AsynchronousMessageWriterPtr fptr
          )
        {
            lock (typeof(Process.Calculation))
            {
                if (Calculation.CalculationAsynchronousThreads_available > 0)
                {
                    // Assign the first available pool-thread to the borning task. By now it will be null, if never used by now, or notnull but IsAlive==false, if used and ended.
                    // In case a pool-thread is still running, the index will show it as unavailable.
                    // The index reset is asynchronous, since occurs at thread termination.
                    // The index booking is synchronous, since is at thread start.
                    in_par.thread_index = Calculation.CalculationAsynchronousThreads_cardinality - Calculation.CalculationAsynchronousThreads_available;
                    //
                    Asynchronous_calculation_Starter calculationStarter = null;
                    switch (requiredAction)
                    {
                        case Process.Calculation.CalculationActions.EulerProduct:
                            {
                                calculationStarter = new Asynchronous_calculation_Starter(
                                    Process.Calculation.Asynchronous_EulerProduct_Starter
                                );
                                break;
                            }
                        case Process.Calculation.CalculationActions.DirichletSum:
                            {
                                calculationStarter = new Asynchronous_calculation_Starter(
                                    Process.Calculation.Asynchronous_DirichletSum_Starter
                                );
                                break;
                            }
                        // TODO case CalculationActions.RiemannZetaFunction
                        case Process.Calculation.CalculationActions.EulerRiemannEquation:
                            {
                                calculationStarter = new Asynchronous_calculation_Starter(
                                    Process.Calculation.Asynchronous_EulerRiemannEquation_Starter
                                );
                                break;
                            }
                        case Process.Calculation.CalculationActions.LogIntegralFrom2ToX:
                            {
                                calculationStarter = new Asynchronous_calculation_Starter( 
                                    Process.Calculation.Asynchronous_LogIntegral_Starter
                                );
                                break;
                            }
                        case Process.Calculation.CalculationActions.GetThresholdGivenCardinality:
                            {
                                calculationStarter = new Asynchronous_calculation_Starter(
                                    Process.Calculation.Asynchronous_PercentileLogIntegral_Starter
                                );
                                break;
                            }
                        //
                        default:
                        case Process.Calculation.CalculationActions.Invalid:
                            {
                                return;// no valid action possible. Aborting.
                            }
                    }// end switch
                    //
                    //-------fork------------------------------------
                    Calculation.CalculationAsynchronousThreads[in_par.thread_index] =
                        new System.Threading.Thread(
                            new System.Threading.ParameterizedThreadStart(
                                calculationStarter
                            )
                        );
                    Calculation.CalculationAsynchronousThreads[in_par.thread_index].Name = "_calculationThread_" + in_par.thread_index.ToString();
                    Calculation.CalculationAsynchronousThreads[in_par.thread_index].Priority = System.Threading.ThreadPriority.Lowest;
                    Calculation.CalculationAsynchronousThreads[in_par.thread_index].Start(
                        in_par);
                    //NB. this thread-booking must be done synchronously, by the forker thread;
                    --CalculationAsynchronousThreads_available;
                    // NB. Join() stops the caller, waiting for the callee; so the action becomes synchronous. That's undesired here. Calculation.CalculationAsynchronousThreads[in_par.thread_index].Join()
                    //NB. this thread-restitution must be done asynchronously, by the child thread; i.e. ++CalculationAsynchronousThreads_available.
                }
                else
                {
                    fptr("All calculation threads are busy now. Retry later, please.");
                }
            }// end critical section.
        }//


        /// <summary>
        /// Immediately abort all of the calculation thread pool.
        /// </summary>
        /// <returns></returns>
        public static string StopAllCalculationThreads()
        {
            lock (typeof(Process.Calculation))
            {
                for (int c = 0; c < Process.Calculation.CalculationAsynchronousThreads_cardinality; c++)
                {
                    if (null != Process.Calculation.CalculationAsynchronousThreads[c])
                    {
                        try
                        {
                            Process.Calculation.CalculationAsynchronousThreads[c].Interrupt();
                            Process.Calculation.CalculationAsynchronousThreads[c].Abort();
                            Process.Calculation.CalculationAsynchronousThreads[c] = null;
                            Process.Calculation.CalculationAsynchronousThreads_available++;// born againg, after dead. Newly available.
                        }
                        catch (System.Exception ex)
                        {
                            LoggingToolsContainerNamespace.LoggingToolsContainer.LogAllSinks(
                                "Exception while aborting calculation asynchronous thread number=" + c.ToString() + ". Ex=" + ex.Message,
                                0
                            );
                        }
                    }// end if thread is not null; else it was never created until now.
                }// end for each thread to be aborted.
                return "Aborted all of the calculation thread pool.";
            }// end critical section
        }//


        /// <summary>
        /// plan:
        ///     - ask for current log name.
        ///     - open it read only.
        ///     - dump its contents on the interface, via the string retval.
        /// </summary>
        /// <returns></returns>
        public static string CheckLog()
        {
            string res = null;
            //string current_log_name = LogSinkFs.Library.Singleton.instance().GetCurrentLogName();
            string current_log_name = Common.Template_Singleton.TSingleton<LogSinkFs.Library.SinkFs>.instance().GetCurrentLogName();
            ////
            System.IO.StreamReader reader = new System.IO.StreamReader(
                current_log_name,
                System.Text.Encoding.Default
            );
            if (null == reader)
                return res;// null by now.
            // else get whole content.
            for (string tmp = ""; null != tmp; tmp = reader.ReadLine() )
            {
                res += tmp + "\r\n";// avoid overflow risks, related to "ReadToEnd()" usage.
            }
            reader.Close();
            // ready
            return res;
        }//


    }//


}
