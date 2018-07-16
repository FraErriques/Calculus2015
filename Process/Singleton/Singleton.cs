using System;
using Process.Singleton;



namespace SingletonPrototype
{

	/// <summary>
	/// memory model: oneServer, multiClient.
	/// </summary>
	public class Singleton
	{
		private static HandledServer handle = null;
		private static int reference_counter = 0;

		private Singleton()
		{// no instantiation allowed
		}// end Ctor

		~Singleton()
		{
            do_destruction();
		}// end Dtor




        public static HandledServer instance()
		{
            lock (typeof(SingletonPrototype.Singleton ))
			{
				try
				{
                    if (null == handle)
					{
                        handle = new HandledServer();
					}// else:  handle gia' istanziata
					// in entrambi i casi iscrivo il nuovo client
					reference_counter++;
				}
				catch( System.Exception ex)
				{
					string exs = ex.Message;// Debug
					handle = null;// without incrementing reference counter
				}
			}// end critical section
			return handle;
		}// unique public access point




		/// <summary>
		/// an utility, to make destruction;
		/// inutile rendere il Singleton IDisposable, perche' l'interfaccia non supporta l'attributo
		/// static per il metodo Dispose. Il Singleton non puo' essere istanziato, quindi e' inutile
		/// una Dispose non statica. Faccio quella custom statica.
		/// </summary>
		public static void do_destruction()
		{
			lock( typeof( Singleton))
			{// NB. do_destruction cannot fail, since it's unconditional. It shuts down everything, no matter if still active client exist.
                reference_counter = 0;
                if (null != handle)
                {
                    handle.Dispose();
                }
                handle = null;
			}// end critical section
		}// end Dispose


        public static void Try_do_destruction()
        {
            lock (typeof(Singleton))
            {
                --reference_counter;// one client less
                if (0 >= reference_counter)
                {
                    reference_counter = 0;
                    handle.Dispose();
                    handle = null;
                }// else TryDestruction failed, due to active clients still alive.
            }// end critical section
        }// end Dispose


        /// <summary>
        /// unsubscribe, but don't want to try destruction. Just refresh client counter.
        /// </summary>
        /// <returns></returns>
        public static int Unsubscribe_sigle_client()
        {
            lock (typeof(Singleton))
            {
                --reference_counter;// one client less
                if (0 > reference_counter)
                {
                    reference_counter = 0;
                }
                return reference_counter;
            }// end critical section
        }//


	}// end Singleton class


}// end nmsp
