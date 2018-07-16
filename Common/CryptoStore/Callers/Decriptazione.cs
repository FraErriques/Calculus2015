using System;
using Common.CryptoStore.Macro;

namespace Common.CryptoStore.Callers
{
	/// <summary>
	/// Summary description for Decriptazione:
	///		scans the crypto-key and retieves which algorithm has been used.
	///		decrypts the sequence.
	///		returns the clear sequence.
	/// </summary>
	public abstract class Decriptazione
	{
		private Decriptazione()
		{}

		public static string DecriptazioneSequenza(
			string sequenza, // crypted
			string kkey_str,
			char mode
			)
		{
			//  ramoRitorno della criptazione
			CryptoEngine cs = new CryptoEngine();
			CryptoEngine.GenericAlgoPointer specificaFunzione = null;// puntatore al servizio di criptazione
			/*
				La BPL chiamante fornisce {sequenza, kkey, mode}.
				Il par 'mode' prende valori in {'s', 'm', 'o'}, che significano rispettivamente {simple, multi, off}.
				Il default e' 'multi'.
				Qualora fosse mal configurato il campo 'mode', l'adozione del default 'multi' provochera' un'eccezione
				in Common.CryptoStore.Micro.Multi.
			*/
			switch( mode)
			{
				case 's':
				{
					specificaFunzione =
						new CryptoEngine.GenericAlgoPointer(
						CryptoStore.Micro.SimpleXor.xorCrypto_ritorno
						);
					break;
				}
				default:
				case 'm':
				{
					specificaFunzione =
						new CryptoEngine.GenericAlgoPointer(
						CryptoStore.Micro.MultiXor.xorMultiCrypto_ritorno
						);
					break;
				}
				case 'o':
				{
					specificaFunzione = null;
					break;
				}
			}
			// la chiave l'ho letta sul db (kkey_str)
			CryptoEngine.theReturnType loginResult =
				cs.Decriptazione(
					sequenza,
					kkey_str,
					specificaFunzione
				);
			// ready
			return loginResult.cryptedSequence;
		}// end


	}// end class
}
