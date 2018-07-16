using System;

namespace Common.CryptoStore.Macro
{
	/// <summary>
	/// Switches between Criptazione and Decriptazione.
	/// </summary>
	public class CryptoEngine
	{
		public CryptoEngine()
		{
		}


		// dichiarazione del puntatore a questo tipo di funzioni
		// function pointer
		public delegate theReturnType GenericAlgoPointer( theReturnType input);

		// a structure to be returned as "object" type
		public struct theReturnType
		{
			public string cryptedSequence;
			public object kkey;
		}




		public theReturnType Criptazione(
			string input,
			GenericAlgoPointer ilServizio  )// qui deve puntare a "andata"
		{
			theReturnType result = new theReturnType();
			result.cryptedSequence = input;
			result.kkey = "0";// in chiaro di default. Se il delegato e' nonnullo sovrascivera' con la chiave utilizzata.
			// nel ritorno la chiave sara' nel db
			// qui nell'andata viene randomizzata da "ilServizio"
			if( null != ilServizio)
			{
				result = ilServizio( result );// qui deve puntare a "andata"
			}// else l'utente ha deciso di non alterare la stringa
			return result;
		}// end gestionePwd



		public theReturnType Decriptazione(
			string input,
			object kkey,
			GenericAlgoPointer ilServizio )// qui deve puntare a "ritorno"
		{
			theReturnType result = new theReturnType();
			result.cryptedSequence = input;
			// solo ramo ritorno : valorizzo la chiave da db
			result.kkey = kkey;
			// end solo ramo ritorno
			if( null != ilServizio)
			{
				result = ilServizio( result );// qui deve puntare a "ritorno"
			}// else l'utente ha deciso di non alterare la stringa
			return result;
		}// end gestionePwd


	}// end class

}// end nmsp
