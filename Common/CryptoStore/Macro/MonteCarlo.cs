using System;

namespace Common.CryptoStore.Macro
{

	/// <summary>
	/// questa classe e' un generatore di numeri casuali.
	/// </summary>
	public class MonteCarlo
	{
		private System.Random theGenerator;

		public MonteCarlo()
		{
			theGenerator = new System.Random();
		}

		public int next_integer( int min, int sup)
		{// returns from [min, sup)
			return this.theGenerator.Next( min, sup );
		}

		public void next_byteArray( byte[] theBuffer )
		{// produce un vettore il cui scalare e' un intero nonnegativo
			theGenerator.NextBytes( theBuffer );
		}

		public double next_probabilityMeasure( )
		{// produce un razionale su [0, 1]
			return theGenerator.NextDouble();
		}

	}// end class

}
