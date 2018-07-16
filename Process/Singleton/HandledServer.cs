using System;


namespace Process.Singleton
{


    public class HandledServer : IDisposable
    {
        /// <summary>
        /// Ctor.
        /// </summary>
        public HandledServer()
        {
            // build.
        }

        ~HandledServer()
        {
            do_garbage_collect();
        }


        public void Dispose()
        {
            do_garbage_collect();
        }


        private void do_garbage_collect()
        {
            lock (typeof(HandledServer))
            {
                // destroy.
            }// end critical section.
        }


    }//


}
