using System;
using System.Collections.Generic;


namespace Common.Template_Singleton
{


    /// <summary>
    ///  Template_Singleton
    ///  
    ///  NB. il "vincolo-IDisposable", che obbliga il tipo generico "T", ad avere metodo pubblico "Dispose"
    ///  il costrutto e': 
    ///     public class className<genericType> where genericType : IDisposable
    ///     
    ///  NB. il "vincolo-new", che obbliga il tipo generico "T", ad avere costruttore
    ///  il costrutto e': 
    ///     public class className<genericType> where genericType : new()
    ///  il vincolo "new" deve essere l'ultimo dell'elenco dei vincoli.
    ///  
    ///  NB.
    ///     static classes cannot contain destructors; so it's better to do not let it static,
    ///     to take advantage of GarbageCollector (gc), which executes the destructor..
    ///     
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class TSingleton<T> where T : IDisposable, new()
    {
        private static T handle = default(T);
        private static int reference_counter = 0;


        /// <summary>
        ///  NB.
        ///     static classes cannot contain destructors; so it's better to do not let it static,
        ///     to take advantage of GarbageCollector (gc), which executes the destructor..
        /// </summary>
        ~TSingleton()
        {
            TSingleton<T>.unsubscribe_all_();
        }// end Dtor


        /// <summary>
        ///  NB.
        ///     static classes cannot contain destructors; so it's better to do not let it static,
        ///     to take advantage of GarbageCollector (gc), which executes the destructor..
        /// </summary>
        private TSingleton()
        {
        }// end private Ctor


        public static T instance()
        {
            lock (typeof(TSingleton<T>))
            {
                try
                {
                    if (null == TSingleton<T>.handle)
                    {
                        TSingleton<T>.handle = new T();
                    }// else:  logDbConnection gia' istanziata
                    // in entrambi i casi iscrivo il nuovo client
                    TSingleton<T>.reference_counter++;
                }
                catch (System.Exception ex)
                {
                    string exs = ex.Message;// Debug
                    handle = default(T);// without incrementing reference counter
                }
            }// end critical section
            return TSingleton<T>.handle;
        }// unique public access point


        /// <summary>
        /// an utility, to notify a client less, to the server.
        /// inutile rendere il Singleton IDisposable, perche' l'interfaccia non supporta l'attributo
        /// static per il metodo Dispose. Il Singleton non puo' essere istanziato, quindi e' inutile
        /// una Dispose non statica. Faccio quella custom statica.
        /// </summary>
        public static void unsubscribe_one_()
        {
            lock (typeof(TSingleton<T>))
            {
                --TSingleton<T>.reference_counter;// one client less
                if (0 == TSingleton<T>.reference_counter)
                {
                    // kill
                    if (null != TSingleton<T>.handle)
                    {
                        TSingleton<T>.handle.Dispose();
                        TSingleton<T>.handle = default(T);
                    }
                }
            }// end critical section
        }// end unsubscribe_one_


        /// <summary>
        /// an utility, to make destruction, completely.
        /// inutile rendere il Singleton IDisposable, perche' l'interfaccia non supporta l'attributo
        /// static per il metodo Dispose. Il Singleton non puo' essere istanziato, quindi e' inutile
        /// una Dispose non statica. Faccio quella custom statica.
        /// </summary>
        public static void unsubscribe_all_()
        {
            lock (typeof(TSingleton<T>))
            {
                TSingleton<T>.reference_counter = 0;// unsubscribe_all_
                // kill
                if (null != TSingleton<T>.handle)
                {
                    TSingleton<T>.handle.Dispose();
                    TSingleton<T>.handle = default(T);
                }
            }// end critical section
        }// end unsubscribe_all_


    }// end  class TSingleton<T>


}// end nmsp
