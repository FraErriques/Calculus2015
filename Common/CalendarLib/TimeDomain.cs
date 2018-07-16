using System;

namespace Common.CalendarLib
{
	/// <summary>
	/// TimeDomain represents the chosen time-interval.
	/// TimeDomain is composed of two GenericSingleDate instances, which
	/// represent the lowerBound and the upperBound of the interval.
	/// A Coupling-date structure maps the relation between dates, Julian-days
	/// and week-days, in the GenericSingleDate class.
	/// TimeDomain is a necessary instrument to bootstrap
	/// the SingleNationMap objects. The SingleNation map objects are
	/// necessary to bootstrap the LocalizedSingleDate.
	/// The SingleNation map objects are filtered by the CalendarStore.
	/// </summary>
	public class TimeDomain
	{
		#region Data&Ctors

		public readonly GenericSingleDate lowerBound;
		public readonly GenericSingleDate upperBound;
		public readonly int map_size = -1;// init to invalid
		public readonly int first_saturday = -1;// init to invalid


		/// <summary>
		/// Valorizza gli estremi dell'intervallo, la cardinalita' del
		/// vettore completo di risposta ed il primo sabato.
		/// </summary>
		/// <param name="lowerDay"></param>
		/// <param name="lowerMonth"></param>
		/// <param name="lowerYear"></param>
		/// <param name="upperDay"></param>
		/// <param name="upperMonth"></param>
		/// <param name="upperYear"></param>
		public TimeDomain(	int lowerDay,int lowerMonth,int lowerYear,
							int upperDay,int upperMonth,int upperYear  )
		{
			this.lowerBound = new GenericSingleDate( lowerDay, lowerMonth, lowerYear );
			this.upperBound = new GenericSingleDate( upperDay, upperMonth, upperYear );
			if( this.upperBound <= this.lowerBound)
			{
				throw new System.Exception("The start year must precede the end year.");
			}
			// set the map-size
			this.map_size = System.Math.Abs( // NB +1 perche' e' compatto
				this.upperBound.Jday() - this.lowerBound.Jday() ) + 1;
			// now set the first saturday Jday in the selected domain
			this.first_saturday = this.lowerBound.Jday();
			int currentLowerBoundDayName = (int)this.lowerBound.WeekDayName();
			for( ; ; )
			{
				if( currentLowerBoundDayName==(int)GenericSingleDate.WeekDays.Saturday )
				{
					break;
				}
				else
				{
					++this.first_saturday;
					if( 7==currentLowerBoundDayName)
					{
						currentLowerBoundDayName = 1;// goto monday, skipping the 0
					}
					else
					{
						currentLowerBoundDayName++;// go ahead
					}
				}// end  go ahead searching the first saturday in the domain
			}
		}// end Ctor


		#endregion Data&Ctors


		#region public

		public override string ToString()
		{
			return( this.lowerBound.ToString() + " -> " +
					this.upperBound.ToString()				);
		}

		#endregion

	}// end class

}// end nmsp
