using System;

namespace Common.CalendarLib
{

	/// <summary>
	/// SingleNationMap is the day-map of a single nation.
	/// It spans on the domain, defined by its father class.
	/// </summary>
	[Serializable]
	public class SingleNationMap : TimeDomain
	{

		#region Data & Ctors

		private string nation_name = null;
		private byte[]  all_days = null;
		private System.Collections.ArrayList subscribers = null;// references to the objects that subscribe this map
		#if slow_DEBUG
			private int[]  synthesisVector	= null;
		#endif

		public string CalendarFullName()
		{
			return this.nation_name;
		}


		/// <summary>
		/// Costruttore del vettore di date, relativo ad una singola nazione e dominio.
		/// NB. non puo' essere privato perche' CalendarStore ha bisogno di
		/// fare new delle singole nazioni, per poterle aggregare. Ma e' bene
		/// che sia internal, perche' dalle altre dll dell'applicazione nessuno
		/// deve istanziare la SingleNationMap deve essere incapsulata nel
		/// contenitore multithread-safe CalendarStore.
		/// </summary>
		/// <param name="nation">the name of the desired nation.</param>
		internal SingleNationMap(
			string nation,
			int lowerDay,int lowerMonth,int lowerYear,
			int upperDay,int upperMonth,int upperYear  ) :
				// build the father, to prepare the domain features.
				base(	lowerDay, lowerMonth, lowerYear,
						upperDay, upperMonth, upperYear  )
		{
			this.nation_name = nation + " " + base.ToString();
			this.subscribers = new System.Collections.ArrayList();
		}// end Ctor

		/// <summary>
		/// The real initializer. All of the algorithms are called only from Init. The constructor
		/// builds the father class, to prepare the time-domain and let the calendar-signature
		/// available for the CalendarStore. With such signature the CalendarStore checks the
		/// availability of the required calendar. Only if such calendar is not yet available
		/// in the Store, the Init will be called to build the new calendar map.
		/// </summary>
		/// <param name="nation"></param>
		internal void Init( string nation, object subscriber)
		{
			// call the year prototype builder. It will then call the "spanner".
			bool nation_localization_result =
				this.buildYearPrototype( nation );
			#if slow_DEBUG
			// NB systhesis vectors resulted useful for debug purposes only.
			// they cast quickly to printable JulianDates, but require more flops than all_days maps.
			this.z__dbg_buildSynthesisVector();
			#endif
			// error management
			if( ! nation_localization_result)
			{
				// gestione fallimento costruzione per nome sbagliato o config-key assente
				throw new System.Exception("sezione o nazione assenti o errate in configurazione.");
			}// else can continue. Nation has been built.
			// if no exception was thrown, let the caller be a subscriber of the newborn map.
			this.subscribe( subscriber);// subscribe current client.
		}// end Init method


		#endregion Data & Ctors

		#region public

		public int howManySubscribers()
		{
			return this.subscribers.Count;
		}

		public void subscribe( object subscriber)
		{
			// NB. verified that is better to do not perform a presence-check of the subscriber.
			this.subscribers.Add( subscriber );
		}// end subscribe


		public void unsubscribe( object subscriber)
		{
			for( int c=0; c<this.subscribers.Count; c++)
			{
				if( object.ReferenceEquals( this.subscribers[c], subscriber) )
				{
					this.subscribers.RemoveAt( c);
				}
			}// if not found all loop through -> client was not member
		}

		#endregion public

		# region NOTpublic


		/// <summary>
		/// prevederne l'uso con cautela. Ricostruire tutti il calendario  costa.
		/// </summary>
		private void drop( )
		{
			this.all_days = null;
			this.nation_name = null;
			#if slow_DEBUG
				this.synthesisVector = null;
			#endif
		}// end drop

		#if slow_DEBUG
		private void z__dbg_buildSynthesisVector()
		{
			System.Collections.ArrayList tmp = new System.Collections.ArrayList();
			for( int c=0; c<this.all_days.Length; c++)
			{
				if( 0 != this.all_days[ c] )
				{
					tmp.Add( c + base.lowerBound.Jday() );
				}// else skip
			}// all_days completely scanned
			this.synthesisVector = new int[ tmp.Count];
			for( int c=0; c< tmp.Count; c++)
			{
				this.synthesisVector[c] = (int)tmp[c];
			}// copy performed. Now I can throw away the temporary.
			tmp = null;
//			this.__debug_sythesisV_checher();
//			this._statistics();
		}// end buildSynthesisVector

		private void z__dbg_sythesisV_checher()
		{
			string[] stamps = new string[ this.synthesisVector.Length];
			for( int c=0; c<this.synthesisVector.Length ; c++)
			{
				stamps[c] = new GenericSingleDate( this.synthesisVector[c]).ToString();
			}// copy performed. Now I can throw away the temporary.
			int d = 0;// breakpoint here, to read the stamps.
			d++;
		}//

		private void z__dbg_statistics()
		{
			double synthesys_data_weight =  (double)this.synthesisVector.Length /
											(double)this.all_days.Length;
			double m = (double)(	this.synthesisVector[ this.synthesisVector.Length -1] -
									this.synthesisVector[ 0]	) /
						(double)this.synthesisVector.Length;
			const int searchedValue = 50005;
			int Dl = searchedValue - this.synthesisVector[0];
			int Dr = searchedValue - this.synthesisVector[ this.synthesisVector.Length-1];
			double candidateIndex = (double)(searchedValue-this.synthesisVector[0]) / m;
			bool found = false;
			const int epsilon = 6;// span a round of the candidateIndex, since holidays distribution is not uniform.
			int inf = (int)candidateIndex - epsilon;
			int sup = (int)candidateIndex + epsilon;
			for( int c=inf; c<sup; c++)
			{
				if( 0==(searchedValue-this.synthesisVector[c]) )
				{
					found = true;
					break;
				}
			}
			int d = 0;// breakpoint here, to read the stamps.
			if(found) d++;// eliminate a warn
		}
		#endif


		/// <summary>
		/// an indexer to overload the operator[] for the "all_days" member
		/// of this class.
		/// </summary>
		internal byte this [int index]
		{
			get
			{
				// Check the index limits.
				if( index < 0 || index >= this.map_size )
				{
					return 2;
				}
				else
				{
					return this.all_days [ index];
				}
			}// end get

		}// end property. It's for "internal" access only.


		# endregion NOTpublic


		# region config_reading


		/// <summary>
		/// Torna la configurazione di una singola nazione, da Config-file.
		/// </summary>
		/// <param name="calendarName">nome della nazione scelta</param>
		/// <returns>configurazione della nazione scelta</returns>
		private string calendarSpecificsFile( string calendarName)
		{
			ConfigurationLayer.ConfigurationService cs = null;
			try
			{
				cs =
					new ConfigurationLayer.ConfigurationService(
						"Finance/Calendars" );
			}
			catch( System.Exception ex)
			{
				string msg = ex.Message;// Debug
				cs = null;// puo' essere null anche senza throw
			}
			if( null == cs)// in ogni caso, se e' null
			{
				return null;// torno calendario nullo
			}// else can continue
			// se la sezione-calendari viene trovata, ne prendo il contenuto.
			string theRequiredCal = cs.GetStringValue( calendarName);
			if( null == theRequiredCal)
			{// se non esiste la nazione richiesta;
				return null;
			}// else can continue
			return theRequiredCal;
		}// end calendarSpecifics


        ///// <summary>
        ///// Torna la configurazione di una singola nazione, da db.
        ///// </summary>
        ///// <param name="calendarName">nome della nazione scelta</param>
        ///// <returns>configurazione della nazione scelta</returns>
        //private string calendarSpecificsDb( string calendarName)
        //{
        //    string theRequiredCal = null;
        //    bool isConfigRead =
        //        DbActions.ExtractNation.ExtractionService(
        //            calendarName,
        //            out theRequiredCal );
        //    if( ! isConfigRead )
        //    {
        //        theRequiredCal = null;
        //    }// else Calendar has been retrieved
        //    return theRequiredCal;
        //}// end calendarSpecifics



        /// <summary>
        /// Torna la configurazione di una singola nazione, da db.
        /// </summary>
        /// <param name="calendarName">nome della nazione scelta</param>
        /// <returns>configurazione della nazione scelta</returns>
        private string calendarSpecificsDb(string calendarName)
        {
            string theRequiredCal = null;
            // ------- call to diverter_method----------
            bool isConfigRead =
                ConfigDiverter.Diverter.DiverterMethod(
                // DbActions.ExtractNation.ExtractionService(  NB. diverted----------!!!!!!!----for working with a .config file, instead of a db-------------
                    calendarName,
                    out theRequiredCal);
            if (!isConfigRead)
            {
                theRequiredCal = null;
            }// else Calendar has been retrieved
            return theRequiredCal;
        }// end calendarSpecifics


		# endregion config_reading

		#region Algoritmi


		/// <summary>
		/// Costituisce il primo passo della costruzione del calendario di una singola
		/// nazione.
		/// Il secondo passo e' costituito dalla build_step_two(), che viene chiamata
		/// solo da questo metodo. Entrambi sono invocati solo
		/// dal costruttore di SingleNationMap. La build_step_one torna un bool; se e'
		/// false il costruttore lancia un'eccezione, cosicche' il chiamante(CalendarStore)
		/// non aggiunge la nazione fra quelle disponibili.
		/// Il costruttore di SingleNationMap e', a sua volta, chiamabile solo da
		/// CalendarStore::init_single.
		/// La build_step_one riempie un vettore di double; e' una versione preliminare e
		/// sintetica della mappa-nazione. La versione definitiva viene generata da
		/// build_step_two.
		/// //
		/// This method fills the YearPrototype, which is a map of the national holidays,
		/// in the generic year, assumed not to be leap.
		/// A second method will span through all of the time domain, performing the
		/// appropriate correction for leap years.
		/// </summary>
		/// <param name="whichCalendar">the xpath of the desired calendar, in the config</param>
		/// <returns>false on error, true on ok</returns>
		private bool buildYearPrototype( string whichCalendar)
		{
			// acquisizione da Config
			string theCalendar =
				this.calendarSpecificsDb( whichCalendar);
			if( null == theCalendar)
			{
				this.nation_name = null;
				this.all_days = null;
				return false;// calendar not available in configuration
			}
			else// else can continue
			{
				// calendar name has already been set by the constructor
				// map-size has been set by the father
				this.all_days = new byte[ base.map_size ];
			}
			//
			try
			{// parse the national calendar specifics.
				string[] calendarTokens = theCalendar.Split(',');
				const int MAX_N_HOLIDAYS = 30;// NB. by law holidays must never be more than these.
				double[] hcode = new double[MAX_N_HOLIDAYS+1]; // codes identifying holidays
				string tmp = "";
				//
				int j=0;// mi serve anche dopo il ciclo. Devo risalire a quante festivita'
				for( ; j<calendarTokens.Length; j++)// ciclo sugli attributi del singolo calendario
				{
					int strl = calendarTokens[j].Length;
					if( calendarTokens[j].ToLower() == "sat" )// Saturdays
					{
						hcode[j] = 2000.6;
					}
					else if( calendarTokens[j].ToLower() == "sun" )// Sundays
					{
						hcode[j]=2000.7;
					}
					else if(	strl>2 &&
						( calendarTokens[j][0]=='e' ||
						calendarTokens[j][0]=='E'	 )
						)// Easter related holidays
					{
						if( calendarTokens[j][1] == '+')
						{
							hcode[j] = 1000.1;
						}
						else if( calendarTokens[j][1] == '-')
						{
							hcode[j] = 1000.0;
						}
						hcode[j] += double.Parse( calendarTokens[j].Substring( 2)) / 1000.0;
					}// end // Easter related holidays
					else if(	strl>3 &&
						( calendarTokens[j][0]=='m' ||
						calendarTokens[j][0]=='M'		)
						)// Month-Week-Day holidays
					{
						hcode[j] = 500.0;
						hcode[j] += double.Parse( calendarTokens[j].Substring( 3)) / 10000.0;
					}// end Month-Week-Day holidays
					else if(	strl>2 &&
						( calendarTokens[j][strl-2]=='b' ||
						calendarTokens[j][strl-2]=='B'    )
						)// Fixed year holidays (working days)
					{
						if( calendarTokens[j][strl-1]=='+')
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j] += 0.1; // first working day after
						}
						else if( calendarTokens[j][strl-1] == '-')
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j] += 0.2; // first working day before
						}
						else if( calendarTokens[j][strl-1]=='>')
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j]+=0.3; // first working day after (saturday is wd)
						}
						else if( calendarTokens[j][strl-1]=='<')
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j]+=0.4; // first working day before (saturday is wd)
						} 
						else if(	calendarTokens[j][strl-1]=='n' ||
							calendarTokens[j][strl-1]=='N'	    )
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j]+=0.5; // nearest working day
						}
						else if(	calendarTokens[j][strl-1]=='l' ||
							calendarTokens[j][strl-1]=='L'     )
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j]+=0.6; // nearest working day (saturday is wd)
						}
					}// end  Fixed year holidays (working days)
					else if(  strl>2 &&
						( calendarTokens[j][strl-2]=='m' ||
						calendarTokens[j][strl-2]=='f'    )
						) // Fixed year holidays (first friday after, first monday before)
					{
						if(  calendarTokens[j][strl-1] == '+')
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j] += 0.7; // first friday after
						}
						else if(  calendarTokens[j][strl-1] == '-')
						{
							tmp = calendarTokens[j].Substring( strl-2);
							hcode[j] = double.Parse( tmp);
							hcode[j] += 0.8; // first monday before
						}
					}// end Fixed year holidays (first friday after, first monday before)
					else // Fixed year holidays (calendar days)
					{
						hcode[j] = double.Parse( calendarTokens[j]);
					}
					//
					if( j > MAX_N_HOLIDAYS)
					{
						throw new System.Exception( "Maximum of holidays exceeded. Check calendar ini file.");
					}
				}// end for
				//
				// build the current nation, identified by "whichCalendar"
				this.spanTimeDomain( hcode, (int)j );// NB. j==n_local_holidays in the current nation, for each year. Local means not counting saturdays and sundays.
				// il chiamante( i.e. CalendarStore::init_single provvede ad inserire
				// il calendario della nazione corrente nella collezione di coppie
				// ( nation_name, SingleNationMap).
			}// end try
			catch( System.Exception ex)
			{
				string exm = ex.Message;// Debug purpose
				return false;
			}// end catch
			finally
			{
			}
			// ready
			return true;
		}// end SingleNationMap::build_step_one (i.e. primo passo della costruzione del calendario di una singola nazione )



		
		/// <summary>
		/// The modf function breaks down the floating-point value
		/// x into fractional and integer parts, each of which
		/// has the same sign as x.
		/// The signed fractional portion of x is returned.
		/// The integer portion is stored as a floating-point
		/// value at intptr.
		/// </summary>
		/// <param name="x">the original double value</param>
		/// <param name="intptr">a pointer to the memory where the integer part will be written, with the same sign as x</param>
		/// <returns>the fractional portion, with the same sign as x</returns>
		private double modf( double x, out double intptr)
		{
			// NB. NON puoi usare System.Math.Floor( x)
			//	perche' Floor(-14.8)=-15
			// ma qui bisogna rispondere -14. Quindi fare
			// il Trunc, non il Floor o Ceil.
			// es. x = -14.8 -> z=modf(x) ->
			// ->returns -0.8, intptr==-14
			intptr = (int)x;
			double fractional = x - intptr;
			return fractional;
		}// end modf


		/// <summary>
		/// Second step in the calendar building process. The first step is buildYearPrototype.
		/// It produces a template of the generic year holidays, in the considered nation.
		/// Such template consists of a double array.
		/// This second phase goes all over the selected time domain, and applies the year-template
		/// produced in the preceding phase, performing the correction for leap-years.
		/// The output is an int32 array.
		/// </summary>
		/// <param name="holidays">il vettore di double predisposto da SingleNationMap::build_step_one</param>
		/// <param name="n_holidays">numero vacanze all'anno, nella nazione considerata</param>
		private void spanTimeDomain(
			double[] hcode, // the year-template
			int n_holidays  // number of annual holidays in the considered nation.
							// Saturdays and Sundays count two entries in the template.
							// Each local holiday counts one entry.
		)
		{
			double		tmp, tmp2;
			double[]	ho_jday = new double[200];// local buffer necessary to implement the algo. Fix size!
			int			mjd, e_jd;
			int			d_shift, ban_flag, month, week, day;
			int			n_diff2, m, n;
			bool		is_curyear_leap;
			bool		is_sat = false, is_sun = false;
			// dates to loop on
			GenericSingleDate d_end = new GenericSingleDate( base.lowerBound.Jday() );
			d_end = d_end.AddYear( base.upperBound.Year() - base.lowerBound.Year() );
			GenericSingleDate current = new GenericSingleDate( base.lowerBound.Jday() );
			// the main loop : on each mapped year
			for( ;
				 current < d_end;
				 current = current.AddYear( 1)
			)
			{
				int curr_year = current.Year();
				int year_idx = (int)(curr_year - base.lowerBound.Year() );
				mjd = current.Jday( );
				// --- Check if current year is leap
				is_curyear_leap = current.IsLeap( );
				//        Documentazione
				// --- Create my own fixed year holidays array for the given calendar.
				// --- If past 28th Feb and on leap year add one day.
				// --- If sunday or saturday go to the first working day after,
				// before or nearest when required.
				// --- This routine does not use passed fixed week days holidays but
				// saturday and sunday only.
				// --- Adopting separate algorithm if saturday is considered working day.
				// --- If target calendar is optimized, fix holidays to first
				// day of the year and Christmas only, and perform a separate
				// holidays counting.
				//
				const double SMALL = +1.0e-10;// prepare an "epsilon"
				// inner loop : on each holiday of the generic year
				for( m=0; m<n_holidays; m++)// this loop spans an year, as minimal chunk
				{// valuto la parte intera di holidays[m], che e' double
					if(// saturday and sunday check
						System.Math.Abs(
						(int)( hcode[m] )// la parte intera di...
						-2000.0 // meno questo
						) < SMALL // e' im modulo minore di epsilon( SMALL)
						)
					{// saturday and sunday check
						tmp = 10.0 * modf( hcode[m], out tmp2);
						if( System.Math.Abs( tmp-6.0)<SMALL) // saturday
						{
							is_sat = true;
						}
						else if( System.Math.Abs( tmp-7.0) < SMALL) // sunday
						{
							is_sun = true;
						}
						ho_jday[m] = 0;
						continue;
					}// end //  saturday and sunday check
					else if(// Easter related holidays
						System.Math.Abs(
						(int)( hcode[m] )// la parte intera di...
						-1000.0 // meno questo
						) < SMALL // e' im modulo minore di epsilon( SMALL)
						)
					{// Easter related holidays
						tmp = 10.0 * modf( hcode[m], out tmp2);
						e_jd = GenericSingleDate.RomanEasterSunday(
									year_idx + base.lowerBound.Year() );
						if( tmp>1.0) // after easter
						{
							ho_jday[m] = (int)(
								e_jd +
								(int)(100.0 * modf( tmp, out tmp2) + SMALL) +
								SMALL
								);
						}// end if( tmp>1.0)
						else // before easter
						{
							ho_jday[m] = (int)(
								e_jd -
								(int)(100.0 * modf( tmp, out tmp2) + SMALL) +
								SMALL
								);
						}// end else before easter
					}// end else // Easter related holidays
					else if( // Set month-week-day holidays
						System.Math.Abs(
						(int)( hcode[m] )// la parte intera di...
						-500.0 // meno questo
						) < SMALL // e' im modulo minore di epsilon( SMALL)
						)// Set month-week-day holidays
					{// vacanze varie. Tipo "Befana", etc...Set month-week-day holidays
						tmp = modf( hcode[m], out tmp2) * 100.0;
						month = (int)( tmp+SMALL);
						tmp = 10.0 * modf( tmp+SMALL, out tmp2);
						week = (int)( tmp+SMALL);
						tmp = 10.0 * modf( tmp+SMALL, out tmp2);
						day = (int)( tmp+SMALL);
						ho_jday[m] = find_day( curr_year, month, day, week);// WARN chiamata alla FAMOSA find_day
					}
					else
					{// Set fixed days, working days after or before,
						// nearest working days or monday-before, friday-after holidays
						/*       Tipologie
						 *       ----------
						 * fixed days
						 * working days after
						 * working days before
						 * nearest working days
						 * monday-before holidays
						 * friday-after holidays
						 * 
						 * */
						d_shift = 0;
						ho_jday[m] = (int)( hcode[m] +
							GenericSingleDate.LastYearJDay( year_idx + base.lowerBound.Year()-1 )
						);
						if( is_curyear_leap &&
							System.Math.Abs(
							hcode[m]
							) > 59.2
							)// nel caso l'anno sia bisestile e la festivita' sia dopo Febbraio
						{// adeguamento della coordinata giorno-festivo della configuraz.
							ho_jday[m] += 1;// mediante shift di un giorno
						}
						ban_flag = (int)
							System.Math.Floor(
							10.0 *
							( hcode[m] - (int)(hcode[m]) ) + SMALL
							);
						ho_jday[m] = (int)( ho_jday[m] + SMALL);
						//
						switch( ban_flag)
						{
							case 1:// First working day after when required
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2 > 1)
								{
									n_diff2 = 2;
								}
								ho_jday[m] += 2 - n_diff2;
								d_shift = 1;
								break;
							}
							case 2: // First working day before when required
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2 > 1)
								{
									n_diff2 = 1;
								}
								ho_jday[m] -= 1 + n_diff2;
								d_shift = -1;
								break;
							}
							case 3: // First working day after when required (saturday is considered working day)
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2>1 || n_diff2==0)
								{
									n_diff2 = 2;
								}
								ho_jday[m] += 2 - n_diff2;
								d_shift = 1;
								break;
							}
							case 4: // First working day before when required (saturday is considered working day)
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2 > 1)
								{
									n_diff2 = 0;
								}
								ho_jday[m] -= n_diff2;
								d_shift = -1;
								break;
							}
							case 5: // Nearest working day when required
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2 == 0)
								{
									ho_jday[m] -= 1;
									d_shift = -1;
								}
								else if( n_diff2 == 1)
								{
									ho_jday[m] += 1;
									d_shift = 1;
								}
								break;
							}
							case 6: // Nearest working day when required (saturday is considered working day)
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2 == 1)
								{
									ho_jday[m] += 1;
									d_shift = 1;
								}
								break;
							}
							case 7: // First friday after when required
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								ho_jday[m] += 6 - n_diff2;
								d_shift = 1;
								break;
							}
							case 8: // First monday before when required
							{
								n_diff2 = (int)(
									( (int)ho_jday[m] - first_saturday + 7) % 7);
								if( n_diff2 < 2)
								{
									ho_jday[m] -= 5 + n_diff2;
								}
								else
								{
									ho_jday[m] -= n_diff2 - 2;
								}
								d_shift = -1;
								break;
							}
						}// end switch
						//
						if( ban_flag!=0 && m>0) // if it's already holiday, move to the next working day
						{ // if it's already holiday, move to the next working day
							for( n=0; n<m; n++)
							{
								if( ho_jday[m] == ho_jday[n] )
								{
									ho_jday[m] += d_shift;
								}// end if
							}// end for
						}// end if
					}
					this.all_days[ (int)(ho_jday[m] + SMALL) - base.lowerBound.Jday() ] = 1;
					//
				}// end interno for( m=0; m<n_holidays; m++)
			}// end for
			//
			if( is_sat || is_sun)
			{
				for( int l=first_saturday; l<d_end.Jday( ); l+=7)// ahead a week each step
				{
					if( is_sat)
					{
						this.all_days[l - base.lowerBound.Jday() ] = 1;
					}
					if( is_sun)
					{
						if( ( l - base.lowerBound.Jday() - 6) > 0)
						{// if this sunday is contained in the domain
							this.all_days[ l - base.lowerBound.Jday() - 6] = 1;
						}
					}
				}// end for // ahead a week each step
			}// end if( is_sat || is_sun)
			// ready
		}// end build_step_two( ex fill_holiday) method




		/// <summary>
		/// metodo ad uso interno di fill_holiday. Crea istanze temporanee di
		/// GenericSingleDate.
		/// </summary>
		/// <param name="my_year">anno</param>
		/// <param name="my_month">mese</param>
		/// <param name="week_day">giorno della settimana</param>
		/// <param name="week_num">progressivo della settimana</param>
		/// <returns></returns>
		private int find_day(	int my_year,
								int my_month,
								int week_day,
								int week_num		)
		{// chiamato da build_step_two : istanzia GenericSingleDate
			int	last_d,
				month = my_month,
				year = my_year,
				week_d = week_day;
			int rval, ref_jday;
			//
			if( week_d < 1)
			{
				week_d = 1;
			}
			else if( week_d>7)
			{
				week_d = 7;
			}
			//
			if( month < 1)
			{
				month = 1;
			}
			else if( month > 12)
			{
				month = 12;
			}
			//
			if( week_num < 1)
			{
				week_num = 1;
			}
			//
			ref_jday = 15026 + week_d;
			//
			if( week_num < 5)
			{
				GenericSingleDate first_day = new GenericSingleDate( (int)1, month, year);
				rval = (int)(first_day.Jday( ))+(week_num-1) * 7;
				//
				if( rval < ref_jday)
				{
					return ref_jday;
				}
				if( ((rval-ref_jday)%7) != 0)
				{
					rval += 7-((rval-ref_jday) % 7);
				}
			}// end if( week_num < 5)
			else
			{
				last_d = GenericSingleDate.LastMonthDay( month, year, true);
				GenericSingleDate first_day = new GenericSingleDate( last_d, month, year);
				rval = first_day.Jday( );
				//
				if( rval < ref_jday)
				{
					return ref_jday;
				}
				if( week_num == 5)
				{
					rval -= ( (rval-ref_jday) % 7);
				}
			}// end else
			//ready
			return rval;
		}// end find_day method

		#endregion Algoritmi

	}// end class SingleNationMap
}// end nmsp
