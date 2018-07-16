using System;

namespace Common.CalendarLib
{

	/// <summary>
	/// CalendarStore e' il filtro che regola l'accesso ai singoli calendari nazionali.
	/// E' buona pratica non accedere direttamente da applicazione a CalendarStore
	/// e tanto meno a SingleNationMap. L'interfaccia da utilizzare e' 
	/// LocalizedSingleDate. Tale classe offre tutte le funzionalita' relative al 
	/// solo tempo, ovvero quelle della classe padre GenericSingleDate, piu'
	/// quelle relative ad un calendario nazionale, che le viene associato in 
	/// costruzione. E' dunque il costruttore(i) di LocalizedSingleDate che si
	/// prende in carico il rapporto con CalendarStore. Tutte le operazioni di
	/// interrogazione e costruzione sono incapsulate nella classe LocalizedSingleDate.
	/// Quindi i vari servizi, il principale dei quali e' "IsHoliday()" vanno richiesti
	/// a LocalizedSingleDate, specificando il nome della nazione di riferimento
	/// in costruzione.
	/// </summary>
	[Serializable]
	public class CalendarStore : System.IDisposable
	{
		# region data

		private System.Collections.Hashtable data = null;
		private System.Collections.ArrayList builtNations = null;
		private bool first_build_occurred = false;
		// la classe non e' istanziabile per creazione di oggetti; espone solo questa handle statica.
		private static CalendarStore handle = null;
		private static int brokerClientsCounter = 0;// Broker clients counter

		# endregion data

		# region Building services

		/// <summary>
		/// CalendarStore is the memory container where all national calendars are stored.
		/// It's a Singleton : uniqueness and thread-safety are guaranteed.
		/// They are crucial since all financial instrumants concurrently access this data
		/// structure in write mode. Uniqueness( i.e. Singleton-filter) and thread-locks
		/// have been fully tested.
		/// </summary>
		private CalendarStore( )// Broker access point
		{
			lock( typeof( CalendarStore))
			{// in questa sezione critica e' cruciale che vengano accodate le valutazioni della variabile first_build_occurred
				// e le Add nelle collections
				if( ! this.first_build_occurred )
				{
					this.first_build();
				}
			}// end critical section
		}// end Singleton access point




		public static CalendarStore instance()
		{
			lock( typeof( CalendarStore))
			{
				try
				{
					if( null ==  handle)
					{
						handle = new CalendarStore();// fa solo first_build()
					}// le Nazioni vanno costruite esplicitamente
					++brokerClientsCounter;
				}
				catch( System.Exception ex)
				{
					string msg = ex.Message;// debug
					handle = null;
				}
				finally
				{
				}
				return handle;
			}// end lock
		}//


		~CalendarStore( ) 
		{
			this.Dispose();
		}// end make_destruction

		# endregion Building services

		#region IDisposable Members

		public void Dispose()
		{
			lock( typeof( CalendarStore))
			{
				--CalendarStore.brokerClientsCounter;// one client less
				// if no more clients...
				if( 0==CalendarStore.brokerClientsCounter)
				{	// empty the two data structures
					this.data.Clear();
					this.builtNations.Clear();
					// destroy the Singleton content : unlink it from its unique instance.
					// The garbage collector will ake it away.
					handle = null;
				}
			}// end critical section
		}// end dispose

		public void Dispose_single( object unsubscriber )
		{
			string nationName = ((LocalizedSingleDate)unsubscriber).CurrentNationFullName();
			lock( typeof( CalendarStore))
			{
				SingleNationMap requiredCalendar = (SingleNationMap)this.data[ nationName];
				// anyway unsubscribe
				requiredCalendar.unsubscribe( unsubscriber);
				// then if this unsubscriber was the last one -> remove the map it was subscribing
				if( 0 == requiredCalendar.howManySubscribers() )
				{// the map is deleted if no subscribers left.
					this.builtNations.Remove( nationName );
					this.data.Remove( nationName);
				}// else do not remove: other subscribers still use this map.
			}// end lock
		}// end Dispose_single

		#endregion IDisposable Members

		# region private

		private void first_build()
		{
			lock( typeof( CalendarStore) )
			{// in questa sezione e' cruciale l'accodamento delle new alle collection e la valorizzazione del flag first_build_occurred
				// prepare the data-store
				this.data = new System.Collections.Hashtable();
				// prepare the readiness-flags
				this.builtNations = new System.Collections.ArrayList();
				// set 
				this.first_build_occurred = true;
			}// end critical section
		}// end first build


		private bool isNationBuilt( string calendarFullName, out string handledCalendar )
		{
			lock( typeof( CalendarStore))
			{
				// get candidate calendar name & boundaries
				string[] candidateTokens = calendarFullName.Split(' ');
				string candidateName = candidateTokens[0];
				int candidate_minJd, candidate_max_jd;
				candidate_minJd  = int.Parse( candidateTokens[2]);
				candidate_max_jd = int.Parse( candidateTokens[7]);
				// loop on existing calendars
				for( int c=0; c< this.builtNations.Count; c++)
				{
					// get existing calendar name & boundaries( for each existing cal)
					string[] currentTokens = ((string)builtNations[c]).Split(' ');
					string currentName = currentTokens[0];
					int current_minJd, current_max_jd;
					current_minJd  = int.Parse( currentTokens[2]);
					current_max_jd = int.Parse( currentTokens[7]);
					// The current calendar is considered suitable if( and only if)
					// the nation is the same, the minimum day is not successive
					// to the candidate minimum and the maximum day is not
					// preceeding the candidate maximum.
					if( candidateName		==	currentName		&& // short-circuit
						candidate_minJd		>=	current_minJd	&&
						candidate_max_jd	<=	current_max_jd		 )
					{
						handledCalendar = (string)builtNations[c];// see note below.(*)
						return true;// an existing calendar is suitable. Take a handle.
					}
				}
				handledCalendar = calendarFullName;
				// (*)
				// If the required map is not compatible with any of the available maps,
				// then a new one will be built with exactly the same
				// specifics proposed by the client. So the out par equals the proposed name.
				// Otherwise, if the proposed map fits into one of the existing ones, then its
				// handle not necessarily has the name proposed by the requirer. It happens only
				// if the required calendar fits into an existing map which has exactly the same
				// specifics. In a more general situation, the handle for an existing calendar 
				// will be the name of the existing map, that is with the same nation name, but
				// with larger or equal time span.
				return false;// must build your own brand new calendar.
			}// end critical section
		}// end isNationBuilt

		# endregion private

		# region public


		/// <summary>
		/// the only access-point of CalendarStore's data.
		/// Gives the flag of a single date, in a specified nation.
		/// </summary>
		/// <param name="nationName">the desired nation name</param>
		/// <param name="dayPosition">the desired date's JulianDay</param>
		/// <returns>0 on working-day, 1 on holiday, 2 on wrong nation-name</returns>
		public byte GetDayFlag( string nationName, int dayPosition )
		{
			byte dayFlag = 2;// init to error
			try
			{
				dayFlag =
					// NB. la validita' di "dayPosition" come range di indice viene
					// verificata dal chiamato( i.e. SingleNationMap::indexer).
					( (SingleNationMap) this.data[ nationName])[ dayPosition];
			}
			catch( System.Exception ex)
			{
				string msg = ex.Message;// debug
				dayFlag = 2;// error
			}
			return dayFlag;
		}// end GetDayFlag


		/// <summary>
		/// gets the number of national-calendars at present.
		/// </summary>
		/// <returns>the number of national-calendars at present</returns>
		public int GetStoreCardinality( )
		{// gets the number of national-calendars at present.
			return this.data.Count;
		}// end getStoreCardinality




		/// <summary>
		/// // scalar building : one national map.
		/// </summary>
		/// <param name="singleNation">nation name</param>
		/// <param name="lowerDay"></param>
		/// <param name="lowerMonth"></param>
		/// <param name="lowerYear"></param>
		/// <param name="upperDay"></param>
		/// <param name="upperMonth"></param>
		/// <param name="upperYear"></param>
		/// <returns></returns>
		public bool InitSingle(
					string singleNation,
					out string complete_nationName,
					object subscriber, // reference to the subscriber
					int lowerDay,int lowerMonth,int lowerYear,
					int upperDay,int upperMonth,int upperYear  )
		{
			lock( typeof( CalendarStore))
			{// in questa sezione critica e' cruciale che vengano accodate le valutazioni della variabile first_build_occurred
				// e le Add nelle collections
				if( ! this.first_build_occurred )
				{
					this.first_build();
				}
				// Instantiate the SingleNation firts, to specify the TimeDomain through its father.
				// Only if needed( i.e. missing in the store) call SingleNationMap::Init to build the map.
				SingleNationMap domainChecker =
					new SingleNationMap(
						singleNation,
						lowerDay, lowerMonth, lowerYear,
						upperDay, upperMonth, upperYear
					);
				// try-store a single national-calendar
				string calHandle;
				if( ! this.isNationBuilt( domainChecker.CalendarFullName(), out calHandle ) )
				{
					try // another map needed
					{
						// on failure the SingleNationMap::Init throws
						domainChecker.Init( singleNation, subscriber);// NB. only "XXCAL"
						// store, if data has been built. Otherwise catch.
						// Left member contains:
						// full-name eg. "ITCAL 01/01/1940 29629 Monday 1 -> 31/12/2200 124957 Wednesday 365"
						this.data.Add(
							calHandle,
							domainChecker		//data
						);
						// store the full-name, both in the HashTable and in the Arraylist
						this.builtNations.Add( (string)calHandle );// first reference
						complete_nationName = calHandle;// out par.
					}
					catch( System.Exception ex)
					{// NB. SingleNationMap::Init throws in the following cases :
						//	-	missing db entry
						//	-	missing section or entry, when quering from Config file
						string msg = ex.Message;// Debug
						// give LocalizedSingleDate its complete name
						complete_nationName = null;
						return false;// esco senza iscrivere la nazione fra quelle costruite.
					}
				}// else e' gia' pronta: devo prendere la handle
				else
				{// else e' gia' pronta: devo prendere la handle
					// ricevo da isSingleNationBuilt il par di out per la handle
					complete_nationName = calHandle;// out par.
					SingleNationMap currentMap = (SingleNationMap)this.data[ calHandle];
					currentMap.subscribe( subscriber);
				}
				// ready
				return true;// nuova nazione iscritta
			}// end critical section
		}// end init_single




		# endregion public

	}// end class CalendarStore
}// end nmsp
