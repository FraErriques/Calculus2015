using System;
using System.Text;
using System.Threading;


namespace Process
{


    public static class db_DataProduction
    {
        public static System.Threading.Thread db_calculationThread = null;// calc worker thread.
        public static PrimesFinder.dbPrimes db_primesCalculationInstance = null;// instance devoted to calculation only.


        /// <summary>
        /// NB.  checkWorkerThreadStatus must return true, before You can call this method.
        /// </summary>
        /// <param name="theOrdinal"></param>
        /// <returns></returns>
        public static string db_startCalculationThread(Int64 threshold)
        {
            // all in critical section.
            lock (typeof(db_DataProduction))// critical section for the worker thread.
            {
                string boardMessage = "";// in append.
                db_DataProduction.db_primesCalculationInstance = new PrimesFinder.dbPrimes();
                db_DataProduction.db_primesCalculationInstance.InitThreshold(threshold);//--set working threshold.
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
                    //// NB. crucial unlock. Necessary on voluntarily stop.
                    //db_DataProduction.db_primesCalculationInstance.Dispose();// this unlocks the data-file.
                    //db_DataProduction.db_primesCalculationInstance = null;
                    //// end NB. crucial unlock. Necessary on voluntarily stop.
                    //boardMessage += "\r\n calculation instance has been disposed.";
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
                    if( Thread.CurrentThread.ThreadState == ThreadState.AbortRequested)
                    {
                        //TODO Process.db_DataProduction.db_calculationThread.ThreadState.
                        //int i = 2;//do something
                        //if ((currentThread.ThreadState & ThreadState.AbortRequested) == 0)
                    }
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

    }// class

    }// nmsp



/*  --- cantina ---threading----------------------
 *  
 *  public enum ThreadState
{
Running = 0,
StopRequested = 1,
SuspendRequested = 2,
Background = 4,
Unstarted = 8,
Stopped = 0x10,
WaitSleepJoin = 0x20,
Suspended = 0x40,
AbortRequested = 0x80,
Aborted = 0x100
}
 *  
 *  
 *  
 *  
 *  
using System;
using System.Security.Permissions;
using System.Threading;

class ThreadInterrupt
{
    static void Main()
    {
        StayAwake stayAwake = new StayAwake();
        Thread newThread =
            new Thread(new ThreadStart(stayAwake.ThreadMethod));
        newThread.Start();

        // The following line causes an exception to be thrown 
        // in ThreadMethod if newThread is currently blocked
        // or becomes blocked in the future.
        newThread.Interrupt();
        Console.WriteLine("Main thread calls Interrupt on newThread.");

        // Tell newThread to go to sleep.
        stayAwake.SleepSwitch = true;

        // Wait for newThread to end.
        newThread.Join();
    }
}

class StayAwake
{
    bool sleepSwitch = false;

    public bool SleepSwitch
    {
        set { sleepSwitch = value; }
    }

    public StayAwake() { }

    public void ThreadMethod()
    {
        Console.WriteLine("newThread is executing ThreadMethod.");
        while (!sleepSwitch)
        {
            // Use SpinWait instead of Sleep to demonstrate the 
            // effect of calling Interrupt on a running thread.
            Thread.SpinWait(10000000);
        }
        try
        {
            Console.WriteLine("newThread going to sleep.");

            // When newThread goes to sleep, it is immediately 
            // woken up by a ThreadInterruptedException.
            Thread.Sleep(Timeout.Infinite);
        }
        catch (ThreadInterruptedException e)
        {
            Console.WriteLine("newThread cannot go to sleep - " +
                "interrupted by main thread.");
        }
    }
}
*/

