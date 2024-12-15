using System;
using System.Runtime.CompilerServices;
using System.Text;


namespace Process
{


    public static class db_DataProduction
    {
        private static System.Threading.Thread db_calculationThread = null;// calc worker thread.
        private static PrimesFinder.dbPrimes db_primesCalculationInstance = null;// instance devoted to calculation only.


        /// <summary>
        /// NB.  checkWorkerThreadStatus must return true, before You can call this method.
        /// </summary>
        /// <param name="theOrdinal"></param>
        /// <returns></returns>
        public static string db_startCalculationThread( Int64 threshold , System.Data.SqlClient.SqlConnection stickyConnection )
        {
            // all in critical section.
            lock (typeof(db_DataProduction))// critical section for the worker thread.
            {
                string boardMessage = "";// in append.
                db_DataProduction.db_primesCalculationInstance = new PrimesFinder.dbPrimes( stickyConnection );
                db_DataProduction.db_primesCalculationInstance.InitThreshold( threshold);//--set working threshold.
                //
                if (db_DataProduction.db_primesCalculationInstance.getCanOperateStatus())
                {
                    if (null != db_DataProduction.db_calculationThread)
                    {// this branch occurs only after a previous calculation reached its natural threshold: thread notNull and notAlive.
                        boardMessage +=
                            db_DataProduction.db_resetCompletedCalculation();// thread not null, but calculation is over. Reset.
                    }// else build from scratch.
                    db_DataProduction.db_calculationThread = new System.Threading.Thread(
                        new System.Threading.ThreadStart(db_DataProduction.db_primesCalculationInstance.CoreLoop)
                    );
                    db_calculationThread.Name = "db_calculationThread";
                    db_calculationThread.Priority = System.Threading.ThreadPriority.Lowest;
                    db_calculationThread.Start();
                    boardMessage += "\r\n-----------------calculation started.----------------";
                }
                else
                {
                    boardMessage += "\r\n build failed, due to invalid threshold.";
                    db_DataProduction.db_primesCalculationInstance.Dispose();
                    db_DataProduction.db_primesCalculationInstance = null;// build failed, due to invalid threshold.
                    db_DataProduction.db_calculationThread = null;
                }
                // ready
                return boardMessage;
            }// end critical section for the worker thread.
        }//



        public static string db_voluntarilyStopCalculation()
        {
            lock (typeof(db_DataProduction))// critical section for the worker thread.
            {
                string boardMessage;
                if (null != db_DataProduction.db_calculationThread)
                {
                    Process.db_DataProduction.db_resetWorkerThread();
                    boardMessage = "\r\n calculation thread stopped.";
                }
                else
                {
                    boardMessage = "\r\n calculation thread was already still.";
                }
                //
                if (null != db_DataProduction.db_primesCalculationInstance)
                {
                    // NB. crucial unlock. Necessary on voluntarily stop.
                    db_DataProduction.db_primesCalculationInstance.Dispose();// this unlocks the data-file.
                    db_DataProduction.db_primesCalculationInstance = null;
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



        public static string db_resetCompletedCalculation()
        {
            lock (typeof(db_DataProduction))// critical section for the worker thread.
            {
                string boardMessage;
                if (null != db_DataProduction.db_calculationThread)
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


        public static void db_resetWorkerThread()
        {
            // all in critical section.
            lock (typeof(db_DataProduction))// critical section for the worker thread.
            {
                try
                {
                    db_DataProduction.db_calculationThread.Interrupt();// better than .Join(TimeSpan.FromMilliseconds(1800.0));// avoid immediate stop; try exit safely from loops.
                    db_DataProduction.db_calculationThread.Abort();
                    db_DataProduction.db_calculationThread = null;
                }
                catch (System.Exception ex)
                {// trap all. Tested that such Abort related exceptions do not harm the database.
                    string s = ex.Message;//dbg
                }
            }// end critical section for the worker thread.
        }//


        public static bool db_checkWorkerThreadStatus(out string message)
        {
            // all in critical section.
            lock (typeof(db_DataProduction))// critical section for the worker thread.
            {
                bool result = false;
                if (
                    null != Process.db_DataProduction.db_calculationThread
                    && null != Process.db_DataProduction.db_primesCalculationInstance
                    )
                {
                    if (false == Process.db_DataProduction.db_calculationThread.IsAlive)
                    {// previous calculation reached the threshold, without manual stop.
                        message = "\r\n previous calculation reached the threshold, without manual stop. Ensuring garbage collection.";
                        message += db_voluntarilyStopCalculation();
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
                        null != Process.db_DataProduction.db_calculationThread
                        && null == Process.db_DataProduction.db_primesCalculationInstance
                    )
                    ||
                    (
                        null == Process.db_DataProduction.db_calculationThread
                        && null != Process.db_DataProduction.db_primesCalculationInstance
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
