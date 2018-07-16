using System;
using System.Text;


namespace Process
{


    public static class DataProduction
    {
        private static System.Threading.Thread calculationThread = null;// calc worker thread.
        private static PrimesFinder.Primes primesCalculationInstance = null;// instance devoted to calculation only.


        /// <summary>
        /// NB.  checkWorkerThreadStatus must return true, before You can call this method.
        /// </summary>
        /// <param name="theOrdinal"></param>
        /// <returns></returns>
        public static string startCalculationThread( Int64 threshold )
        {
            // all in critical section.
            lock (typeof( DataProduction))// critical section for the worker thread.
            {
                string boardMessage = "";// in append.
                DataProduction.primesCalculationInstance = new PrimesFinder.Primes();
                DataProduction.primesCalculationInstance.InitThreshold( threshold );//--set working threshold.
                //
                if( DataProduction.primesCalculationInstance.getCanOperateStatus() )
                {
                    if (null != DataProduction.calculationThread)
                    {// this branch occurs only after a previous calculation reached its natural threshold: thread notNull and notAlive.
                        boardMessage +=
                            DataProduction.resetCompletedCalculation();// thread not null, but calculation is over. Reset.
                    }// else build from scratch.
                    DataProduction.calculationThread = new System.Threading.Thread(
                        new System.Threading.ThreadStart(DataProduction.primesCalculationInstance.CoreLoop)
                    );
                    calculationThread.Name = "filesystem_calculationThread";
                    calculationThread.Priority = System.Threading.ThreadPriority.Lowest;
                    calculationThread.Start();
                    boardMessage += "\r\n-----------------calculation started.----------------";
                }
                else
                {
                    boardMessage += "\r\n build failed, due to invalid threshold.";
                    DataProduction.primesCalculationInstance.Dispose();
                    DataProduction.primesCalculationInstance = null;// build failed, due to invalid threshold.
                    DataProduction.calculationThread = null;
                }
                // ready
                return boardMessage;
            }// end critical section for the worker thread.
        }//



        public static string voluntarilyStopCalculation()
        {
            lock (typeof(DataProduction))// critical section for the worker thread.
            {
                string boardMessage;
                if (null != DataProduction.calculationThread)
                {
                    Process.DataProduction.resetWorkerThread();
                    boardMessage = "\r\n calculation thread stopped.";
                }
                else
                {
                    boardMessage = "\r\n calculation thread was already still.";
                }
                //
                if (null != DataProduction.primesCalculationInstance)
                {
                    // NB. crucial unlock. Necessary on voluntarily stop.
                    DataProduction.primesCalculationInstance.Dispose();// this unlocks the data-file.
                    DataProduction.primesCalculationInstance = null;
                    // end NB. crucial unlock. Necessary on voluntarily stop.
                    boardMessage += "\r\n calculation instance has been disposed.";
                }
                else
                {
                    boardMessage += "\r\n calculation instance was already disposed.";
                }
                //
                // ready.
                return boardMessage;
            }// end critical section for the worker thread.
        }//



        public static string resetCompletedCalculation()
        {
            lock (typeof( DataProduction))// critical section for the worker thread.
            {
                string boardMessage;
                if (null != DataProduction.calculationThread)
                {
                    Process.DataProduction.resetWorkerThread();
                    boardMessage = "\r\n ensured that previous calculation is stopped.";
                }
                else
                {
                    boardMessage = "\r\n previous calculation was already still.";
                }
                // NB. don't do this: the instance has just been renewed.  ---DataProduction.primesCalculationInstance.Dispose();// this unlocks the data-file.
                //
                // ready.
                return boardMessage;
            }// end critical section for the worker thread.
        }//


        public static void resetWorkerThread()
        {
            // all in critical section.
            lock (typeof(DataProduction))// critical section for the worker thread.
            {
                try
                {
                    DataProduction.calculationThread.Interrupt();// better than .Join(TimeSpan.FromMilliseconds(1800.0));// avoid immediate stop; try exit safely from loops.
                    DataProduction.calculationThread.Abort();
                    DataProduction.calculationThread = null;
                }
                catch (System.Exception ex)
                {// trap all. Tested that such Abort related exceptions do not harm the database.
                    string s = ex.Message;//dbg
                }
            }// end critical section for the worker thread.
        }//


        public static bool checkWorkerThreadStatus( out string message)
        {
            // all in critical section.
            lock (typeof(DataProduction))// critical section for the worker thread.
            {
                bool result = false;
                if (
                    null != Process.DataProduction.calculationThread
                    && null != Process.DataProduction.primesCalculationInstance
                    )
                {
                    if (false == Process.DataProduction.calculationThread.IsAlive)
                    {// previous calculation reached the threshold, without manual stop.
                        message = "\r\n previous calculation reached the threshold, without manual stop. Ensuring garbage collection.";
                        message += voluntarilyStopCalculation();
                        result = true;
                    }
                    else
                    {// alive, i.e. calculating.
                        message = "\r\n the calculation thread is already working.";
                        result = false;
                    }
                }
                else if (
                    (
                        null != Process.DataProduction.calculationThread
                        && null == Process.DataProduction.primesCalculationInstance
                    )
                    ||
                    (
                        null == Process.DataProduction.calculationThread
                        && null != Process.DataProduction.primesCalculationInstance
                    )
                )
                {
                    message = "\r\n SERIOUS ALARM: the system is in an instable state. STOP the application immediately.";
                    result = false;
                }
                else// both null
                {
                    message = "\r\n a new calculation can start.";
                    result = true;
                }
                // ready
                return result;
            }// end critical section for the worker thread.
        }//


    }//


}
