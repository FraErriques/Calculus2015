using System;

namespace Common.CalendarLib
{

	/// <summary>
	/// GenericSingleDate represents a single day, with capabilities of moving
	/// along the time axis. The time shifts are intended independent from the
	/// space. Only day, month, year, JulianDay, weekDay and yearProgressive are
	/// supported.
	/// To know about a specific nation holidays, a son class is provided, which
	/// is based on the required national calendar.
	/// </summary>
	[Serializable]
	public class GenericSingleDate
	{

		# region data


		public enum WeekDays
		{
			Invalid		= 0,
			Monday		= 1,
			Tuesday		= 2,
			Wednesday	= 3,
			Thursday	= 4,
			Friday		= 5,
			Saturday	= 6,
			Sunday		= 7
		}

		private struct Coupling
		{// Il giorno di AGGANCIO e' 01/01/1950	33282 domenica.
			//
			public const int Year = 1950;
			public const int Month = 01;
			public const int Day = 01;
			//
			public const int JulianDay = 33282;
			//
			public const WeekDays WeekDay = WeekDays.Sunday;
			//
			// ? moon ?
		}// end Coupling


		/// day
		protected int d;
		/// month
		protected int m;
		/// year
		protected int y;
		/// julian day
		protected int  jd;
		/// day progressive within current year
		protected int yearPosition;
		/// name of the day, within the week
		protected GenericSingleDate.WeekDays weekDay;

		# endregion data

		# region Ctors

		// default-constructor hidden

		/// <summary>
		/// build from a .Net DateTime
		/// </summary>
		/// <param name="dt"></param>
		public GenericSingleDate( System.DateTime dotNet_DateTime )
		{
			int day = dotNet_DateTime.Day;
			int month = dotNet_DateTime.Month;
			int year = dotNet_DateTime.Year;
			// build on suggested date
			this.setCoordinates( day, month, year );
		}

		// copy constructor
		public GenericSingleDate( GenericSingleDate dt)
		{
			setCoordinates( dt.jd );
		}// end Ctor

		// construct from a given julian day
		public GenericSingleDate( int my_jday )
		{
			setCoordinates( my_jday );
		}

		// construct from a given day/month/year specification
		public GenericSingleDate( int my_day, int my_month, int my_year)
		{
			setCoordinates( my_day, my_month, my_year);
		}

		# endregion Ctors

		#region public helpers

		/// <summary>
		/// get the day as an integer
		/// </summary>
		/// <returns></returns>
		public int Day( )
		{
			return this.d;
		}//

		/// <summary>
		/// get the month as an integer
		/// </summary>
		/// <returns></returns>
		public int Month( )
		{
			return this.m;
		}//

		/// <summary>
		/// get the year as an integer
		/// </summary>
		/// <returns></returns>
		public int Year( )
		{
			return this.y;
		}//

		/// <summary>
		/// Gets the date as a julian day, i.e. an integer accumulated from a basis day.
		/// </summary>
		/// <returns>Gets the date as a julian day.</returns>
		public int Jday( )
		{
			return jd;
		}//

		/// <summary>
		/// returns the name of this day within the week, i.e. Monday, etc..
		/// </summary>
		/// <returns></returns>
		public GenericSingleDate.WeekDays WeekDayName( )
		{
			return this.weekDay;
		}

		/// <summary>
		/// query if the year of "this" is leap
		/// </summary>
		/// <returns>a bool : true==leap</returns>
		public bool IsLeap( )
		{
			return GenericSingleDate.IsLeap( this.y);
		}// is_Leap

		#region static

		/// <summary>
		/// query if the specified year is leap
		/// </summary>
		/// <param name="year">the specified year</param>
		/// <returns>a bool : true==leap</returns>
		public static bool IsLeap( int year)
		{// query whether a year is leap
			return( (year%4==0 && year%100!=0) || year%400==0 );
		}// end IsLeap


		/// <summary>
		/// Dato (mese, anno, bisestilita') torna il numero dell'ultimo
		/// giorno di tale mese. Tiene conto dell'anno e della bisestilita'.
		/// </summary>
		/// <param name="month">numero mese</param>
		/// <param name="year">numero anno</param>
		/// <param name="use_leap">bool: considerare la bisestilita' ?</param>
		/// <returns>torna il numero dell'ultimo giorno del mese specificato</returns>
		public static int LastMonthDay(
			int month,		// mese
			int year,			// anno
			bool use_leap	)	// is bisestile
		{// dato (mese, anno, bisestilita') torna il numero dell'ultimo giorno di tale mese
			if (month<1||month>12)
			{
				throw new System.Exception( "Month out of range");
			}
			// array dei giorni finali dei dodici mesi.
			int[] days = new int[12]
				{
					31,		// gennaio
					28,		// febbraio
					31,		// marzo
					30,		// aprile
					31,		// maggio
					30,		// giugno
					31,		// luglio
					31,		// agosto
					30,		// settembre
					31,		// ottobre
					30,		// novembre
					31		// dicembre
				};// end array
			//
			int max_day = days[month-1];// scelgo il mese che mi e' stato richiesto
			// verifiche per la modifica del febbraio
			if( use_leap && // se il contratto prevede di tenere conto della bisestilita'
				IsLeap( year) && // e se l'anno in questione e bisestile
				max_day==28     )// e se il mese in questione e' febbraio
			{
				max_day = 29;// allora passo al "febbraio-allungato"
			}// end adeguamento del febbraio
			// ready
			return max_day;
		}// end LastMonthDay



		public static int LastYearJDay( int nYear)
		{// torna il JulianDay del 31 dicembre dell'anno specificato
			int year_idx = (int)(nYear-1900);
			//
			int n_leap = (int) System.Math.Floor( (double)(year_idx)/4.0);
			n_leap -= (int) System.Math.Floor( (double)(year_idx)/100.0);
			n_leap += (int) System.Math.Floor( (double)(year_idx+300L)/400.0);
			//
			int l_jd = year_idx * 365 + n_leap + 366;
			l_jd += 15018;// NB. shift_Excel := arretramento di 15018
			// ready
			return l_jd;
		}// end LastYearJDay



        /// <summary>
        /// domenica di Pasqua Cattolica. E' la prima domenica non precedente il quarto plenilunio,
        /// dall'inizio dell'anno solare. Oscilla e va ricalcolata per inserirla a mano nella stringa di
        /// festivita' da mettere nel db.
        /// </summary>
        /// <param name="nYear"></param>
        /// <returns></returns>
		public static int RomanEasterSunday( int nYear )
		{
			int nCent = (int)(nYear/100);
			int nRemain19 = (int)(nYear%19);
			// --- n1 is the number of days since 21-Mar of the PFM
			int n1 = (int)( (nCent-15)/2 + 202 - 11*nRemain19 );// ?
			//
			if( nCent>20)
			{
				if( nCent>26)
				{
					--n1;// non mettere a fattor comune:
				}// ciascuna di queste condizioni provoca un ulteriore shift
				if(nCent>38)
				{// e' necessario valutarle separatamente per poter accumulare gli shift
					--n1;
				}
				if( nCent==21 ||
					nCent==24 ||
					nCent==25 ||
					nCent==33 ||
					nCent==36 ||
					nCent==37		)
				{
					--n1;
				}
			}// end if( nCent>20)
			//
			n1 %= 30;
			if( n1==29 || (n1==28 && nRemain19>10) )
			{
				--n1;
			}
			// --- This can only be in March or April
			int e_day, e_month;
			if( n1>10)
			{
				e_month = 4;
				e_day = (int)(n1-10);
			}
			else
			{
				e_month = 3;
				e_day = (int)(n1+21);
			}
			GenericSingleDate dt = new GenericSingleDate( e_day, e_month, nYear);
			int e_jd = dt.Jday( ) + (7-(dt.Jday( )-4)%7);
			// ready
			return e_jd;
		}// end RomanEasterSunday







        /// <summary>
        /// 
        ///   domenica di Pasqua Ortodossa.
        ///   returns a Julian Day.
        /// 
        ///     This function is based off a non program based algoritm
        ///     described in cssa.stanford.edu/~marcos/ortheast.html.
        ///     The code to find the Paschal Full Moon is adapted from a
        ///     Visual Basic program found on www.auslink.net/~gmarts.
        ///     
        /// 
        /// </summary>
        /// <param name="nYear"></param>
        /// <returns></returns>
        public static int OrthodoxEasterSunday(int nYear)
        {
            int nRemain19 = nYear % 19;
            int nRemain7 = nYear % 7;
            int nRemain4 = nYear % 4;
            // - Things are a bit simpler in the Julian Calendar, this is
            // a formula by Gauss for the number of days after 21-Mar. 
            int n1 = (19 * nRemain19 + 16) % 30;
            int n2 = (2 * nRemain4 + 4 * nRemain7 + 6 * n1) % 7;
            int n3 = n1 + n2;
            // - Then convert to the Gregorian Calendar (1583 onwards)
            int nCent = nYear / 100;
            n3 += nCent - nCent / 4 - 2;
            // - The Orthodox Easter in the Gregorian calendar can fall in May
            // (and can not possibly fall in March anytime after year 1582).
            int dt_m_year, dt_m_month, dt_m_date;
            dt_m_year = nYear;
            if (n3 > 40)
            {
                dt_m_month = 4;
                dt_m_date = n3 - 40;
            }
            else if (n3 > 10)
            {
                dt_m_month = 3;
                dt_m_date = n3 - 10;
            }
            else
            {
                dt_m_month = 2;
                dt_m_date = n3 + 21;
            }
            Common.CalendarLib.GenericSingleDate dt = new GenericSingleDate(
                dt_m_date,
                dt_m_month + 1,// this algorithm counts months from zero, i.e. January==0.
                dt_m_year
            );
            // ready
            return dt.jd;
        }// end Ortodox Easter Sunday

		#endregion static



		
		#region time_shift
		
		/// <summary>
		/// opera uno slittamento in avanti di "lag" giorni, rispetto alla
		/// data "this".
		/// </summary>
		/// <param name="lag">ritardo in avanti, espresso in giorni</param>
		/// <returns>GenericSingleDate</returns>
		private GenericSingleDate add_day( int lag)
		{
			GenericSingleDate result = clone();
			result.setCoordinatesFrom_jd( result.jd + lag);
			return result;
		}// end add_day


		/// <summary>
		/// opera uno slittamento in avanti di 12/freq mesi, rispetto alla
		/// data "this".
		/// </summary>
		/// <param name="freq">determina l'entita' dello shift. E' una grandezza espressa in "numero di volte all'anno". Quindi lo shift e' il suo  reciproco ed e' espresso in mesi. es. freq==3 -> lag=(12/3)==4</param>
		/// <param name="is_fwd">direzione dello shift</param>
		/// <param name="method">convenzione di calcolo adottata</param>
		/// <param name="use_leap">uso della bisestilita'</param>
		/// <returns>GenericSingleDate</returns>
		private GenericSingleDate  add_month(
			int							freq,
			bool						is_fwd,
			Rules.EndOfMonth_methods	method,
			bool						use_leap )
		{
			int lag = (freq==0) ? 0 : 12/ System.Math.Abs( freq);
			if( ! is_fwd)// se non e' all'avanti
			{
				lag = -lag;// allora e' all'indietro
			}
			return add_month( (int)lag, method, use_leap);
		}// end add_month



		private GenericSingleDate add_month( 
			int							lag,
			Rules.EndOfMonth_methods	method,
			bool						use_leap )
		{
			int last = LastMonthDay( m, y, use_leap);
			GenericSingleDate result = this.clone();
			result.m += lag;
			if( lag<0)
			{
				while( result.m <= 0 )
				{
					result.y--;
					result.m += 12;
				}
			}// endif
			else// lag>=0
			{
				while( result.m > 12 )
				{
					result.y++;
					result.m -= 12;
				}
			}// endif
			int max_day = LastMonthDay( result.m, result.y, use_leap );
			if( result.d > max_day)
			{
				switch( method)
				{
					case Rules.EndOfMonth_methods.ForceEnd:
					case Rules.EndOfMonth_methods.PrevGood:
					{
						result.d = max_day;
						break;
					}// end case ePrevGood, eForceEnd
					case Rules.EndOfMonth_methods.NextGood:
					{
						result.d = 1;
						result.m++;
						if( result.m==13)
						{
							result.m = 1;
							result.y++;
						}
						break;
					}// end case eNextGood
					case Rules.EndOfMonth_methods.NextInc:
					{
						result.d -= max_day;
						result.m++;
						if( result.m == 13)
						{
							result.m = 1;
							result.y++;
						}
						break;
					}// end case eNextInc
				}// end switch
			}// end if( d > max_day)
			if( method==Rules.EndOfMonth_methods.ForceEnd && result.d==last)
			{
				result.d = max_day;
			}// endif
			// after modifying the Coordinates, register them
			result.setCoordinates( result.d, result.m, result.y );
			// ready
			return result;
		}// end GenericSingleDate add_month( int lag, int method, bool use_leap)



		/// <summary>
		/// opera uno shift in avanti, espresso in anni.
		/// </summary>
		/// <param name="lag">numero di anni</param>
		/// <returns>GenericSingleDate</returns>
		public GenericSingleDate AddYear( int lag)
		{
			GenericSingleDate result = this.clone();
			result.y += lag;
			result.setCoordinatesFrom_dmy( result.d, result.m, result.y );
			return result;
		}// end add_year



		/// <summary>
		/// opera uno shift, permettendo di scegliere l'unita' di misura in cui
		/// esprimerlo. Si puo' scegliere anche la convenzione di calcolo
		/// eom=="End of Month" e l'uso della bisestilita'.
		/// NON modifica i data members coordinata-tempo, costruisce
		/// una nuova classe con le coordinate richieste.
		/// </summary>
		/// <param name="lag">ritardo</param>
		/// <param name="unit">unita' di misura</param>
		/// <param name="eom">convenzione contrattuale per la fine-mese( i.e. EOM)</param>
		/// <param name="use_leap"></param>
		/// <returns>trattamento bisestilita'</returns>
		public GenericSingleDate AddTime(
			int							lag,
			char						unit,
			Rules.EndOfMonth_methods	eom,
			bool						use_leap	)
		{
			GenericSingleDate result = null;
			switch( unit)// a seconda della unita' di misura
			{
				case 'd':// giorni
				case 'D':
				{
					result = add_day( lag);
					break;
				}
				case 'w':// settimane
				case 'W':
				{
					result = add_day( (int)(7*(int)lag));
					break;
				}
				case 'm':// mesi. Caso di default
				case 'M':
				default:
				{
					result = add_month( lag, eom, use_leap);
					break;
				}
				case 'y':// anni
				case 'Y':
				{
					result = AddYear( lag);
					break;
				}
			}// end switch
			// ready
			return result;
		}// end add_time

		#endregion time_shift


		#endregion public helpers

		# region NOTpublic


		private string str( )
		{
			string theStamp = "";
			if(10> this.d) theStamp += "0";// else do nothing
			theStamp += this.d.ToString() + "/";
			if(10>this.m) theStamp += "0";// else do nothing
			theStamp += this.m.ToString() + "/";
			theStamp += this.y.ToString();
			//
			theStamp += " " + this.jd.ToString();
			theStamp += " " + this.weekDay.ToString();
			// set on-demand parameter. It cannot be set at build-time, since it calls a
			// constructor for a temporary, and would cause a stack overflow.
			this.setYearPosition();
			theStamp += " " + this.yearPosition.ToString();
			// ready
			return theStamp;
		}// end coreStampBuilder


		/// <summary>
		/// set all the coordinates, from day_month_year
		/// </summary>
		/// <param name="my_day"></param>
		/// <param name="my_month"></param>
		/// <param name="my_year"></param>
		protected void setCoordinates(
			int my_day,
			int my_month,
			int my_year		)
		{
			// set  day_month_year and JulianDay
			this.setCoordinatesFrom_dmy(// dmy means day_month_year
				my_day,
				my_month,
				my_year		);
			// set week-day-name
			this.setWeekDay();
			// CANNOT call this.setYearPosition() because it calls a constructor and
			// would cause a stack overflow. It's an "on-demand" parameter.
		}// end setCoordinates

		/// <summary>
		/// set all the coordinates, from JulianDay
		/// </summary>
		/// <param name="my_jday"></param>
		protected void setCoordinates( int my_jday )
		{
			// set  day_month_year and JulianDay
			this.setCoordinatesFrom_jd(// jd means JulianDay
				my_jday );
			// set week-day-name
			this.setWeekDay();
			// CANNOT call this.setYearPosition() because it calls a constructor and
			// would cause a stack overflow. It's an "on-demand" parameter.
		}// end setCoordinates




		/// <summary>
		/// serve a ininzializzare la classe "Date" ai valori
		/// dd-mm-yyyy che arrivano a parametro
		/// </summary>
		/// <param name="my_day">giorno</param>
		/// <param name="my_month">mese</param>
		/// <param name="my_year">anno</param>
		private void setCoordinatesFrom_dmy(// dmy means day_month_year
			int my_day,
			int my_month,
			int my_year		)
		{
			if(	my_year<1600 || // minimum supported year
				// month of year validity check
				(my_month<1 || my_month>12) ||
				// NB day of month validity check
				(my_day<1 || my_day>LastMonthDay( my_month, my_year, true) )
				)
			{
				throw new System.Exception( "Date out of range");
			}// end checks
			//
			// set aliases to work on
			y = my_year;
			m = my_month;
			d = my_day;
			//
			jd = -679004;
			if(m==1||m==2)// se e' gennaio o febbraio
			{
				--y;
				jd += (int)((m+13)*30.6001);
				jd += (int)(y/400);
				jd -= (int)(y/100);
				jd += (int)(y*365.25);
				++y;
			}
			else
			{
				jd += (int)((m+1)*30.6001);
				jd += (int)(y/400);
				jd -= (int)(y/100);
				jd += (int)(y*365.25);
			}
			// ready
			jd += (int)(d);
		}// end setCoordinates dd-mm-yyyy



		private void setCoordinatesFrom_jd( int my_jday )// jd means JulianDay
		{
			if( my_jday < -94553 )// minimum supported JulianDay. It's 01/01/1600
			{
				throw new System.Exception( "Date out of range");
			}// end checks
			// set some temporaries, to work on.
			jd = my_jday;
			int ia, ib, ic, id, ie, if_;
			ia = jd + 2400001;
			if(ia<2299161)
			{
				ic = ia + 1524;
			}
			else
			{
				ib = (int)((ia-1867216.25)/36524.25);
				ic = ia + ib - (int)(ib/4.0) + 1525;
			}// endif
			//
			id  = (int)((ic-122.1)/365.25);
			ie  = (int)(id*365.25);
			ia  = ic-ie;
			if_ = (int)(ia/30.6001);
			//
			d = (int)(ia-(int)(if_*30.6001));
			m = (int)(if_-1-(int)(if_/14.0)*12);
			y = (int)(id-4715-(int)((m+7)/10.0));
			// ready
		}// end "setCoordinates" method


		/// <summary>
		/// set the week-day (i.e. Monday, Tuesday,..)
		/// NB. this method is suitable to be called ONLY AFTER the initialization
		/// of this.jd (the JulianDay).
		/// </summary>
		private void setWeekDay( )
		{// coupling=aggancio =:   01/01/1950	33282	domenica
			int weekDayIndex = (int)GenericSingleDate.Coupling.WeekDay;
			int distanzaJd	= this.jd - GenericSingleDate.Coupling.JulianDay;
			int theShift = 0;// init
			if( 0 != distanzaJd)
			{
				theShift = distanzaJd / System.Math.Abs( distanzaJd);
			}// else -> theShift = 0;
			if( theShift == 0)// ready
			{
				this.jd = GenericSingleDate.Coupling.JulianDay;
				this.weekDay = (GenericSingleDate.WeekDays) weekDayIndex;
			}
			else// shift !=0
			{// complete weeks bring to the same week-day
				double remainderHmWeeks =// the remainder instead, shifts.
					System.Math.Abs( (double)distanzaJd) % 7.0;
				// so loop on the remainder: the steps are less then seven.
				for( int c=0; c<remainderHmWeeks; c++ )
				{
					if( theShift > 0)
					{
						if( 7==weekDayIndex)// update weekDayIndex
						{
							weekDayIndex = 1;//  goto monday. NB. 0==Invalid
						}
						else
						{
							++weekDayIndex;// go ahead
						}
					}
					else if( theShift < 0)// shift==0 already considered
					{
						if( 1==weekDayIndex)// update weekDayIndex
						{
							weekDayIndex = 7;//  goto sunday. NB. 0==Invalid
						}
						else
						{
							--weekDayIndex;// go backwards
						}
					}
				}// when exiting the weekDayIndex should be the right one.
				// set day-name
				this.weekDay = ( GenericSingleDate.WeekDays )weekDayIndex;
				// ready
			}// end loop on the remainder
		}// end setWeekDay method


		private void setYearPosition()
		{// plus one to count January first as one and not as zero.
			this.yearPosition = this.jd - new GenericSingleDate(01,01,this.y).jd + 1;
		}

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
		protected virtual GenericSingleDate clone()
		{
			return new GenericSingleDate( this );
		}

		# endregion NOTpublic

		#region operators


		public enum OrderRelation
		{
			smaller,
			greater,
			equal
		}// end enum OrderRelation


		/// <summary>
		/// metodi di supporto agli operatori
		/// </summary>
		private static OrderRelation evaluateOrderRelation( GenericSingleDate left, GenericSingleDate right )
		{
			if( left.y < right.y)
			{
				return GenericSingleDate.OrderRelation.smaller;
			}
			else if( left.y > right.y)
			{
				return GenericSingleDate.OrderRelation.greater;
			}
			else// if( left.y == right.y)
			{
				if( left.m < right.m)
				{
					return GenericSingleDate.OrderRelation.smaller;
				}
				else if( left.m > right.m)
				{
					return GenericSingleDate.OrderRelation.greater;
				}
				else// if( left.m == right.m)
				{
					if( left.d < right.d)
					{
						return GenericSingleDate.OrderRelation.smaller;
					}
					else if( left.d > right.d)
					{
						return GenericSingleDate.OrderRelation.greater;
					}
					else// if( left.d == right.d)
					{// date identiche
						return GenericSingleDate.OrderRelation.equal;
					}// end confronto-giorni
				}// end confronto-mesi
			}// end confronto-anni
		}// end evaluateOrderRelation method



        public static int operator -(GenericSingleDate left, GenericSingleDate right)
        {
            int minuendo = left.Jday();
            int sottraendo = right.Jday();
            int delta = minuendo - sottraendo;
            return delta;
        }// end -


		public static bool operator< ( GenericSingleDate left, GenericSingleDate right )
		{
			GenericSingleDate.OrderRelation order = GenericSingleDate.evaluateOrderRelation( left, right);
			if( order == GenericSingleDate.OrderRelation.smaller)
			{
				return true;
			}
			return false;
		}// end <


		public static bool operator> ( GenericSingleDate left, GenericSingleDate right )
		{
			GenericSingleDate.OrderRelation order = GenericSingleDate.evaluateOrderRelation( left, right);
			if( order == GenericSingleDate.OrderRelation.greater)
			{
				return true;
			}
			return false;
		}// end >

		public static bool operator<= ( GenericSingleDate left, GenericSingleDate right )
		{
			GenericSingleDate.OrderRelation order = GenericSingleDate.evaluateOrderRelation( left, right);
			if( order != GenericSingleDate.OrderRelation.greater) // !>
			{
				return true;
			}
			return false;
		}// end <=

		public static bool operator>= ( GenericSingleDate left, GenericSingleDate right )
		{
			GenericSingleDate.OrderRelation order = GenericSingleDate.evaluateOrderRelation( left, right);
			if( order != GenericSingleDate.OrderRelation.smaller) // !<
			{
				return true;
			}
			return false;
		}//



		public static bool operator==  ( GenericSingleDate left, GenericSingleDate right )
		{
			GenericSingleDate.OrderRelation order = GenericSingleDate.evaluateOrderRelation( left, right);
			if( order == GenericSingleDate.OrderRelation.equal ) // ==
			{
				return true;
			}
			return false;
		}// end ==

		public static bool operator!=  ( GenericSingleDate left, GenericSingleDate right )
		{
			GenericSingleDate.OrderRelation order = GenericSingleDate.evaluateOrderRelation( left, right);
			if( order != GenericSingleDate.OrderRelation.equal ) // !=
			{
				return true;
			}
			return false;
		}// end !=



		//		/// assigment operator non e' overloadable
		//		basta usare quello predefinito e le reference puntano
		//		alla stessa memoria. I member poi sono value-type. Quindi
		//		l'assegnamento e' sufficiente.
		//		public static Date operator=( Date dt)
		//		{
		//			if( dt!=this)
		//			{
		//				setCoordinates( dt.jday());
		//			}
		//			return this;
		//		}

		// NB. non e' consentito l'overload di operator==
		public override bool Equals(object obj)// e' sostituito da questo
		{// sono reference types; confronto i pointers
			return System.Object.ReferenceEquals( this, obj);
			// queste invece confrontano i pointee: NON fare
			//  NON fare return object.Equals( this, obj);
			//  NON fare return base.Equals (obj);
		}// end override Equals


		public override int GetHashCode()// override sollecitato dal compilatore
		{// a causa degli override di "==", "!=", Object.Equals
			return base.GetHashCode ();// tuttavia non ho da fare personalizzazioni
		}

		public override string ToString()// questo e' importante
		{// torno "dd/mm/yyyy JulianDay weekDay yearPosition"
			return this.str();
		}// NB. return base.ToString(); tornerebbe "GenericSingleDate"

		public static explicit operator int( GenericSingleDate currentDate)
		{
			GenericSingleDate d = new GenericSingleDate( currentDate);
			return d.jd;// returns the JulianDay
		}// end operatore cast esplicito

		public static explicit operator string( GenericSingleDate currentDate)
		{// genero un tmp "d" perche' l'operator e' statico e non puo' usare this.str
			// this.str non puo' essere statico perche' usa this.y....etc
			GenericSingleDate d = new GenericSingleDate( currentDate);
			return d.str();// returns the dd/mm/yyyy string
		}// end operatore cast esplicito


		#endregion  operatori

	}// end class


}// end nmsp
