

//namespace LogSinkFs.Library
//{

//    public class Singleton
//    {
//        private static SinkFs handle = null;
//        private static int		referenceCounter = 0;

//        public static SinkFs instance() 
//        {
//            lock( typeof(Singleton))
//            {
//                try
//                {
//                    if( null==handle) 
//                    {
//                        handle = new SinkFs();
//                    }
//                    // else:  logStream gia' istanziata
//                    // in entrambi i casi iscrivo il nuovo client
//                    ++Singleton.referenceCounter;
//                }
//                catch
//                {
//                    handle = null;// without incrementing reference counter
//                }
//            }// end critical section
//            return handle;
//        }// instance method


//        ~Singleton( ) 
//        {
//            Singleton.do_destruction();
//        }// end make_destruction

//        // hidden methods
//        private Singleton()
//        {}

//        /// <summary>
//        /// inutile rendere il Singleton IDisposable, perche' l'interfaccia non supporta l'attributo
//        /// static per il metodo Dispose. Il Singleton non puo' essere istanziato, quindi e' inutile
//        /// una Dispose non statica. Faccio quella custom statica.
//        /// </summary>
//        public static void __custom_Dispose()
//        {
//            Singleton.do_destruction();
//        }


//        // an internal utility, to make destruction
//        private static void do_destruction()
//        {
//            lock( typeof( Singleton))
//            {
//                --Singleton.referenceCounter;// one client less
//                // if no more clients...
//                if( 0==Singleton.referenceCounter)
//                {
//                    handle.Dispose();// destroy the Singleton content
//                    handle = null;
//                }
//            }// end critical section
//        }// end private void do_destruction()



//    }// end class Singleton
//}// end namespace
