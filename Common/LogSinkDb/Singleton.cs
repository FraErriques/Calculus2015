//using System;

//namespace LogSinkDb.Library
//{
//    /// <summary>
//    /// This Singleton is the access point of each application to it's own Log.
//    /// The SinkDb( such as the FileSystem sink) has an application wide instance.
//    /// </summary>
//    public class Singleton
//    {
//        private static LogSinkDb.Library.SinkDb handle = null;
//        private static int reference_counter = 0;

//        private Singleton()
//        {// no instantiation allowed
//        }// end Ctor

//        ~Singleton()
//        {
//            LogSinkDb.Library.Singleton.do_destruction();
//        }// end Dtor


//        public static LogSinkDb.Library.SinkDb instance()
//        {
//            lock( typeof( LogSinkDb.Library.Singleton))
//            {
//                try
//                {
//                    if( null==LogSinkDb.Library.Singleton.handle)
//                    {
//                        LogSinkDb.Library.Singleton.handle = new SinkDb( );
//                    }// else:  logDbConnection gia' istanziata
//                    // in entrambi i casi iscrivo il nuovo client
//                    LogSinkDb.Library.Singleton.reference_counter++;
//                }
//                catch( System.Exception ex)
//                {
//                    string exs = ex.Message;// Debug
//                    handle = null;// without incrementing reference counter
//                }
//            }// end critical section
//            return LogSinkDb.Library.Singleton.handle;
//        }// unique public access point




//        /// <summary>
//        /// an utility, to make destruction;
//        /// inutile rendere il Singleton IDisposable, perche' l'interfaccia non supporta l'attributo
//        /// static per il metodo Dispose. Il Singleton non puo' essere istanziato, quindi e' inutile
//        /// una Dispose non statica. Faccio quella custom statica.
//        /// </summary>
//        public static void do_destruction()
//        {
//            lock( typeof( LogSinkDb.Library.Singleton))
//            {
//                --LogSinkDb.Library.Singleton.reference_counter;// one client less
//                if( 0==LogSinkDb.Library.Singleton.reference_counter)
//                {
//                    LogSinkDb.Library.Singleton.handle.Dispose();
//                    LogSinkDb.Library.Singleton.handle = null;
//                }
//            }// end critical section
//        }// end Dispose


//    }// end Singleton class

//}// end nmsp
