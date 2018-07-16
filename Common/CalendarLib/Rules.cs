using System;

namespace Common.CalendarLib
{

	/// <summary>
	/// Rules stores the financial agreements.
	/// </summary>
	[Serializable]
	public class Rules
	{
		private Rules()
		{
		}//

		#region enumerations


		public enum DayCount_methods
		{
			// invalid
			Invalid = 0,
			///	day count conventions
			// actual / 360
			Act_360 = 1,
			// actual / 360
			Act_365 = 2,
			// actual / actual
			Act_Act = 3
		}// end DayCount_methods


		public enum Effd_methods
		{
			// invalid
			Invalid = 0,
			///	effective date conventions
			// nominal
			Nominal = 1,
			// go to the next busineess day
			NextBusiness = 2,
			// go to the previous busineess day
			PrevBusiness = 3,
			// go to the next busineess day without going to the next month
			ModNextBusiness = 4,
			// go to the previous busineess day without going to the previous month
			ModPrevBusiness = 5
		}// end  enum Effd_methods


		public enum EndOfMonth_methods
		{
			// invalid
			Invalid = 0,
			///	end of month conventions
			// force the end of the month
			ForceEnd = 1,
			// if on an invalid day go the the previous valid day
			PrevGood = 2,
			// if on an invalid day go the the next valid day
			NextGood = 3,
			// if n days past the first valid day go the the next n-th valid day
			NextInc  = 4
		}// end EndOfMonth_methods



		public enum Exdiv_methods
		{
			// invalid
			Invalid = 0,
			/// ex-dividend conventions
			// standard
			StdExdiv = 1,
			// irish: the wednesday (or first working day after) three weeks before the coupon payment date.
			Eire     = 2,
			// denmark
			Denmark  = 3,
			// denmark: ex-dividend without accrued interests
			Hungary  = 4
		}// end enum Exdiv_methods



		public enum Rate_methods
		{
			// invalid
			Invalid = 0,
			// simple
			Simple = 1,
			// commercial
			Commercial = 2,
			// compound
			Compound = 3
		}// end  enum Rate_methods


		public enum Reset_methods
		{
			// invalid
			Invalid = 0,
			///	reset conventions
			// standard
			StdReset = 1
		}// end  enum Reset_methods



		/// <summary>
		/// This flag specifies the operation that will be performed, in the considered LegEntry.
		/// The [Flags] attribute indicates that tihs is a bit field or flag enumeration:
		/// bitwise operators will be used.
		/// Each flag ( except Invalid) is an integer power of the two basis.
		/// </summary>
		[Flags]
		public enum TemporalTermNature
		{
			// invalid
			Invalid = 1,
			/// specifica della natura della scadenza in questione : bonus, coupon,...
			// emissione
			Issue = 2,
			// cedola
			Coupon = 4,
			// regalino
			Bonus = 8,
			// rimborso
			Redemption = 16
		}// end enum TemporalTermNature

		#endregion enumerations


	}// end class Rules
}// end nmsp
