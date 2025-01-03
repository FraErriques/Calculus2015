using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace winPrimes
{


    /// <summary>
    /// Instantiates an application uniquely. Verifies that no other AppDomain exists in the OS,
    /// devoted to the same application.
    /// </summary>
    public class Launcher : IWin32Window
    {
        private static System.Threading.Mutex ProcMutex = null;


        /// <summary>
        /// This example shows how a Mutex is used to synchronize access
        /// to a protected resource. Unlike Monitor, Mutex can be used with
        /// WaitHandle.WaitAll and WaitAny, and can be passed across
        /// AppDomain boundaries.
        /// </summary>
        /// <param name="accepted"></param>
        /// <returns></returns>
        public bool TestMutex()
        {// Create a new Mutex.
            bool accepted = false;
            Launcher.ProcMutex = new System.Threading.Mutex(
                false,// is initially owned: the creating thread does not own the Mutex.
                "C1B575FC-7D32-4cca-86B3-6FB93E88232F_19700409",// a named mutex is global to all processes in the OS-kernel.
                out accepted// response from Mutex Ctor():true==entered, false==Mutex-busy->rejected.
            );
            // ready
            return accepted;
        }//


        /// <summary>
        /// Ctor:
        ///     tests the Mutex
        ///     creates a OS-wide unique instance of this application
        ///     on re-entry, for main_form closure, class Launcher and the launched application, die.
        /// </summary>
        public Launcher()
        {
            //---test a global-mutex( i.e. a named one, which is OS-wide scoped).
            if (this.TestMutex())
            {
                // Wait until it is safe to enter.
                Launcher.ProcMutex.WaitOne();
                //-------main action: show the main form-----------------------------------
                    System.Windows.Forms.Application.Run( new frmWinPrimes() );
                //-------on re-entry, for main_form closure: ------------------------------
                // Release the Mutex.
                Launcher.ProcMutex.ReleaseMutex();
            }// end branch "green semaphore".
            else
            {
                System.Windows.Forms.MessageBox.Show(
                    this,// owner of the modal dialog.
                    " This application can run one instance per host.\r\n Please refer to the active instance.",
                    "Error: trying to run more than one instance.",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }// end branch "red semaphore".
        }// end Ctor



        /// <summary>
        /// NB. implement interface IWin32Window. Necessary for the MessageBox call.
        /// </summary>
        IntPtr IWin32Window.Handle
        {
            get
            {
                return new IntPtr();
            }
        }


    }// end class Launcher







    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //
            winPrimes.Launcher launcher = new Launcher();
        }


    }// end class Program


}// end nmsp
