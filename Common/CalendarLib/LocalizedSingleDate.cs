using System;

namespace Common.CalendarLib
{

	
	/// <summary>
	/// LocalizedSingleDate rappresenta una singola data
	/// in relazione a una nazione. Ovvero permette operazioni di traslazione
	/// di quantita' di giorni lavorativi, in relazione ad un calendario nazionale.
	/// </summary>
	[Serializable]
	public class LocalizedSingleDate : GenericSingleDate
	{

		# region data
		/// La SingleNationMap di riferimento non e' istanziata come data member,
		/// bensi' ottenuta dal "filtro" CalendarStore, il quale garantisce che di
		/// ciascuna nazione ci sia un'unica copia. Che la nazione richiesta sia da
		/// costruire, o solo da consultare, e' valutato e messo in atto da CalendarStore.
		/// E' vietato dunque accedere ad istanze di SingleNationMap, se non attraverso
		/// il filtro CalendarStore.
		//
		/// calendar name : name of the nation, which "this" refers to.
		private string calendar = null;
		/// calendar full name =: name of the nation + range.
		private string calendarFullName = null;
		/// minimum julianDay mapped
		private int minJd = -1;// init to invalid
		/// maximum julianDay mapped
		private int maxJd = -1;// init to invalid
		/// minimum Year mapped
		private int minYear = -1;// init to invalid
		/// maximum Year mapped
		private int maxYear = -1;// init to invalid

		# endregion data

		#region Ctors



		//		/// <summary>
		//		/// default constructor OSCURATO
		//		/// </summary>


		/// <summary>
		/// build from a .Net DateTime
		/// </summary>
		/// <param name="dt"></param>
		public LocalizedSingleDate( 
			System.DateTime dotNet_DateTime,
			string my_cal,
			int lowerYear,
			int upperYear  ) : base( dotNet_DateTime)
		{
			if( ! this.isInsideDomain(
					new GenericSingleDate(dotNet_DateTime),
					new GenericSingleDate(01, 01,lowerYear),
					new GenericSingleDate(31, 12,upperYear) ) )
			{
				throw new System.Exception("The required day is outside its calendar.");
			}
			// else can continue building
			this.common_builder(
				my_cal,
				01, 01, lowerYear,
				31, 12, upperYear  );
		}


		/// <summary>
		/// construct from a given date( the father class) and a given calendar
		/// </summary>
		/// <param name="dt">reference date</param>
		/// <param name="my_cal">reference calendar</param>
		public LocalizedSingleDate(
			GenericSingleDate dt,
			string my_cal,
			int lowerYear,
			int upperYear  ) : base( dt)
		{
			if( ! this.isInsideDomain(
				dt,
				new GenericSingleDate(01, 01, lowerYear),
				new GenericSingleDate(31, 12, upperYear) ) )
			{
				throw new System.Exception("The required day is outside its calendar.");
			}
			// else can continue building
			this.common_builder(
				my_cal,
				01, 01, lowerYear,
				31, 12, upperYear  );
		}// end Ctor( data, calendario)



		/// <summary>
		///  construct from a given julian day and a given calendar
		/// </summary>
		/// <param name="my_jday">reference julian day</param>
		/// <param name="my_cal">reference calendar</param>
		public LocalizedSingleDate(
			int my_jday,
			string my_cal,
			int lowerYear,
			int upperYear  ) : base( my_jday)
		{
			if( ! this.isInsideDomain(
				new GenericSingleDate(my_jday),
				new GenericSingleDate(01, 01, lowerYear),
				new GenericSingleDate(31, 12, upperYear) ) )
			{
				throw new System.Exception("The required day is outside its calendar.");
			}
			// else can continue building
			this.common_builder(
				my_cal,
				01, 01, lowerYear,
				31, 12, upperYear  );
		}// end Ctor( julian_day, calendar)



		/// <summary>
		///  construct from a given day/month/year specification and a given calendar
		/// </summary>
		/// <param name="my_day">reference_day</param>
		/// <param name="my_month">reference month</param>
		/// <param name="my_year">reference year</param>
		/// <param name="my_cal">reference calendar</param>
		public LocalizedSingleDate(
			int	my_day,
			int	my_month,
			int	my_year,
			string	my_cal,
			int lowerYear,
			int upperYear  ) : base( my_day, my_month, my_year )
		{
			if( ! this.isInsideDomain(
				new GenericSingleDate(my_day,my_month, my_year),
				new GenericSingleDate(01, 01, lowerYear),
				new GenericSingleDate(31, 12, upperYear) ) )
			{
				throw new System.Exception("The required day is outside its calendar.");
			}
			// else can continue building
			this.common_builder(
				my_cal,
				01, 01, lowerYear,
				31, 12, upperYear  );
		}// end Ctor( day, month, year, calendar )



		/// <summary>
		/// Copy Constructor. In the "copy-construction" the TimeDomain remains
		/// the same.
		/// </summary>
		/// <param name="cdt">the original, to be copied</param>
		public LocalizedSingleDate( LocalizedSingleDate that) : base( that.Jday())
		{// NB. non passare dal common_builder che inizializza ai default.
			this.calendar			= that.calendar;
			this.calendarFullName	= that.calendarFullName;
			this.minJd				= that.minJd;
			this.maxJd				= that.maxJd;
			this.minYear			= that.minYear;
			this.maxYear			= that.maxYear;
		}// end Copy-Constructor





		//		/// assigment operator
		//		LocalizedSingleDate& operator=(const LocalizedSingleDate& cdt);
		//		NB. assigment operator non e' overloadable
		//		basta usare quello predefinito e le reference puntano
		//		alla stessa memoria. I member poi sono value-type. Quindi
		//		l'assegnamento e' sufficiente.

		#endregion Ctors

		#region public


		public string CurrentNationName( )
		{
			return this.calendar;
		}// currentNationName

		public string CurrentNationFullName( )
		{
			return this.calendarFullName;
		}// calendarFullName


		/// <summary>
		/// The question that this method can answer is:
		///  the current instance( this, i.e. dd/mm/yyyy in "XXCAL") is a holiday ?
		/// </summary>
		/// <returns>a boolean; true on holiday.</returns>
		public bool IsHoliday( )// crucial   method !
		{
			byte theFlag = // NB. returns 2 on invalid calendar, 0 on wokd, 1 on holiday.
				CalendarStore.instance().GetDayFlag(
					this.calendarFullName,
					(int)(base.jd - this.minJd)
				);
			//
			if( theFlag == 2 )// ERROR : invalid calendar!
			{
				throw new System.Exception(" ERROR : invalid calendar!");
			}
			else if( theFlag == 1 )// holiday
			{
				return true;
			}
			else if( theFlag == 0 )// working day
			{
				return false;
			}
			else// wrong flag value : Debug required !
			{
				throw new System.Exception("ERROR : wrong flag value : Debug required !");
			}
		}// end IsHoliday


		public void DropSingle( )
		{//  WARN  prevederne l'uso con cautela. Ricostruire un calendario costa
			CalendarStore.instance().Dispose_single( this );
		}// end drop


		#endregion public

		#region private

		/// <summary>
		/// checks that the required LocalizedSingleDate belongs the
		/// local calendar span.
		/// It's called by all the constructors.
		/// </summary>
		/// <param name="candidate"></param>
		/// <param name="min"></param>
		/// <param name="max"></param>
		/// <returns></returns>
		private bool isInsideDomain(
			GenericSingleDate candidate,
			GenericSingleDate min,
			GenericSingleDate max	)
		{
			if( candidate >= min &&
				candidate <= max		)
				return true;
			return false;
		}// end isInsideDomain


		/// <summary>
		/// All the provided constructors call this unified building-channel.
		/// The various constructors represent different building stategies, but all
		/// of them require to call "common_builder", which invokes the
		/// CalendarStrore::init_single() method, to add a single nation to the
		/// calendar collection.
		/// </summary>
		/// <param name="nationName"></param>
		private void common_builder( 
			string nationName,
			int lowerDay,int lowerMonth,int lowerYear,
			int upperDay,int upperMonth,int upperYear  )
		{
			string complete_nationName = null;
			if( lowerYear == upperYear )
			{// time intervals must be greater than a year.
				++upperYear;
			}
			bool requiredNationIsBuilt =
				CalendarStore.instance().InitSingle(
					nationName,
					out complete_nationName,
					this,// reference to the subscriber
					lowerDay, lowerMonth, lowerYear,
					upperDay, upperMonth, upperYear  );
			if( ! requiredNationIsBuilt)
			{
				throw new System.Exception("Invalid nation required.");
			}// else go on
			this.calendar = nationName;
			this.calendarFullName = complete_nationName;
			// full name parser
			string[] nameTokens = complete_nationName.Split(' ');
			this.minJd = int.Parse( nameTokens[2]);
			this.maxJd = int.Parse( nameTokens[7]);
			this.minYear =  int.Parse( nameTokens[1].Substring(6) );
			this.maxYear =  int.Parse( nameTokens[6].Substring(6) );
		}// end common_builder


		/// <summary>
		/// prevederne l'uso con cautela. Ricostruire tutti i calendari costa molto.
		/// </summary>
		private void drop( )
		{//  WARN  prevederne l'uso con cautela. Ricostruire tutti i calendari costa molto
			CalendarStore.instance().Dispose();
		}// end drop



		/// <summary>
		/// Acts like a copy constructor, called to copy this. But it's virtual and the
		/// ovverrides can return the child types, implicitly casted to the father type.
		/// The reason for this method is that no override is allowed on constructors,
		/// while this method, being virtual, allows to build the son's type,
		/// returning it as the father type. The cast is possible ONLY because
		/// behind that father instance there is a son instance. If the instance
		/// were father-constructor produced, the cast would fail.
		/// </summary>
		/// <returns>a GenericSingleDate, copy-constructed on this( the caller),
		/// which is of the son-type LocalizedSingleDate.
		/// </returns>
		protected override GenericSingleDate clone()
		{
			return new LocalizedSingleDate(
				this,
				this.calendar,
				this.minYear,
				this.maxYear   );
		}

		#endregion private

		#region time_shift



		/// <summary>
		/// individua il giorno lavorativo a distanza "lag" da quello relativo alla
		/// istanza "this".
		/// NB. workd( 0) restituisce il successivo giorno lavorativo rispetto a this,
		/// sia che this sia festivo che feriale. Quindi non cadere nell'illusione
		/// che this.workd( lag==0) sia uguale a this ! E' comunque next-business-day.
		/// Ne deriva (l'assurdo) che workd(0)==workd(1)...sono le follie della Finanza!
		/// </summary>
		/// <param name="lag"></param>
		/// <returns></returns>
		public LocalizedSingleDate Workd( int lag)
		{
			LocalizedSingleDate result = new LocalizedSingleDate( this);
			//
			int i;
			if( lag == 0)// nessuno shift
			{// prendo direttamente il successivo giorno non-festivo
				result = result.nextBusDay();
			}
			else if( lag < 0)// shift-temporali all'indietro
			{
				for(i=0; i<-lag; i++)
				{// all'indietro, cerco il primo giorno feriale utile
					result = result.prevBusDay();
				}
			}// end // shift-temporali all'indietro
			else// shift-temporali in avanti
			{
				for(i=0; i<lag; i++)
				{// in avanti, cerco il primo giorno feriale utile
					result = result.nextBusDay();
				}
			}// end// shift-temporali in avanti
			//ready
			return result;
		}// end workd


		/// <summary>
		/// next business day
		/// </summary>
		private LocalizedSingleDate nextBusDay( )
		{// calculates the next business day, with respect to today.
			LocalizedSingleDate result = new LocalizedSingleDate(
				this.jd + 1,
				this.calendar,
				this.minYear,
				this.maxYear   );

			while( result.IsHoliday() )
			{
				result.jd++;
				result.setCoordinates( result.jd );
			}
			return result;
		}// end next_bus_day( i.e. next business day)


		/// <summary>
		/// modified previous business day.
		/// </summary>
		private LocalizedSingleDate prevBusDay( )
		{
			LocalizedSingleDate result = new LocalizedSingleDate(
				this.jd - 1,
				this.calendar,
				this.minYear,
				this.maxYear   );

			while( result.IsHoliday() )
			{
				result.jd--;
				result.setCoordinates( result.jd );
			}
			return result;
		}// prev_bus_day





		/// <summary>
		/// Modified next business day.
		/// </summary>
		private LocalizedSingleDate modNextBusDay( )
		{
			LocalizedSingleDate result = new LocalizedSingleDate(
				this.jd + 1,
				this.calendar,
				this.minYear,
				this.maxYear   );

			if( result.d==1)
			{
				result = result.prevBusDay( );
			}
			else// se il giorno e' diverso da uno
			{
				while( result.IsHoliday() )// fintantoche' e' festa
				{
					result.jd++;
					result.setCoordinates( result.jd);
					if( result.d == 1)
					{
						result.jd--;
						result.setCoordinates( result.jd );
						result = result.prevBusDay();
						break;
					}// end if
				}// end while
			}// end else
			return result;
		}// end mod_next_bus_day



		/// <summary>
		/// Modified previous business day.
		/// </summary>
		private LocalizedSingleDate modPrevBusDay( )
		{// convenzione "modified previous business day"
			LocalizedSingleDate result = new LocalizedSingleDate(
				this.jd - 1,
				this.calendar,
				this.minYear,
				this.maxYear   );

			int max_day = GenericSingleDate.LastMonthDay( result.m, result.y, true);
			if( result.d == max_day )
			{
				result = result.nextBusDay();
			}
			else
			{
				while( result.IsHoliday() )
				{
					result.jd--;
					result.setCoordinates( result.jd );
					if( result.d == max_day)
					{
						result.jd++;
						result.setCoordinates( result.jd );
						result = result.nextBusDay();
						break;
					}// end if( d==max_day)
				}// end while( IsHoliday() )
			}// end else
			return result;
		}// end mod_prev_bus_day



		/// <summary>
		/// Effective Date
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		public LocalizedSingleDate Effd( Rules.Effd_methods method)
		{// se e' festa devo valutare come ricavare il giorno feriale utile
			LocalizedSingleDate result = new LocalizedSingleDate( this);
			if( result.IsHoliday() )
			{// la scelta dipende da quanto stabilito dal contratto del titolo. La info e' nel db.
				if( method == Rules.Effd_methods.NextBusiness )
				{
					result = result.nextBusDay();
				}
				else if( method == Rules.Effd_methods.PrevBusiness)
				{
					result = result.prevBusDay();
				}
				else if( method == Rules.Effd_methods.ModNextBusiness)
				{
					result = result.modNextBusDay();
				}
				else if( method == Rules.Effd_methods.ModPrevBusiness)
				{
					result = result.modPrevBusDay();
				}
			}// else it's already working day -> effective==nominal.
			return result;
		}// end effd


		/// <summary>
		/// do reset
		/// </summary>
		/// <param name="lag">shift</param>
		/// <param name="is_bus">is business</param>
		/// <param name="method">reset method</param>
		/// <returns></returns>
		public LocalizedSingleDate Reset(
			int lag,
			bool is_bus,
			Rules.Reset_methods method )
		{
			LocalizedSingleDate result = new LocalizedSingleDate( this);
			//
			if( method == Rules.Reset_methods.StdReset )
			{
				if( is_bus) // business days
				{
					int tmp = System.Math.Abs( lag);
					while( tmp > 0)
					{
						result = result.prevBusDay();
						tmp--;
					}
				}// end if(is_bus) // business days
				else // calendar days
				{
					result.jd -= System.Math.Abs( lag);
				}
			}// end if( method==eStdReset): no other methods supported by now
			result.setCoordinates( result.jd );
			//
			return result;
		}// end reset




		public LocalizedSingleDate Exdiv(
			int lag,
			bool is_bus,
			Rules.Exdiv_methods method
		)
		{
			LocalizedSingleDate result = new LocalizedSingleDate( this);
			if ( method == Rules.Exdiv_methods.StdExdiv )
			{
				if( is_bus) // business days
				{
					int tmp = System.Math.Abs( lag);
					while( tmp > 0 )
					{
						result = result.prevBusDay();
						tmp--;
					}// end while
				}// end if( is_bus)
				else // calendar days
				{
					result.jd -= System.Math.Abs( lag);
				}
			}// end if (method==eStdExdiv)
			else if( method == Rules.Exdiv_methods.Eire )
			{
				int tmp, xdd, m_saturday;
				m_saturday = 15025; // reference Saturday jday.
				tmp = jd-21;
				xdd = ( tmp - m_saturday) % 7;
				if( xdd != 0 )
				{
					tmp += 4 - xdd;
				}
				else
				{
					tmp -= 3;
				}
				result.jd -= tmp;
			}// end else if( method==eEire)
			else if ( method == Rules.Exdiv_methods.Denmark )
			{ // to implement
			}
			else if ( method == Rules.Exdiv_methods.Hungary )
			{ //  to implement
			}
			//
			result.setCoordinates( result.jd );
			// ready
			return result;
		}// end LocalizedSingleDate::exdiv

		#endregion time_shift

	}// end class
}// end nmsp
