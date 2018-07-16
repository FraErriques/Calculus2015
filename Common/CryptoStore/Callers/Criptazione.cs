using System;
using Common.CryptoStore.Macro;

namespace Common.CryptoStore.Callers
{
	/// <summary>
	/// Summary description for Criptazione:
	///		scans the configuration file of the AppDomain-starter and retrieves the active crypto-configuration.
	///		crypts the sequence.
	///		returns the encrypted sequence.
	/// </summary>
	public abstract class Criptazione
	{
		private Criptazione()
		{}


		/// <summary>
		/// La BPL chiamante fornisce {sequenza, out mode}.
		/// Il par 'mode' prende valori in {'s', 'm', 'o'}, che significano rispettivamente {simple, multi, off}.
		/// Il default e' 'multi'.
		/// </summary>
		/// <param name="sequenza"></param>
		/// <param name="mode"></param>
		/// <returns></returns>
		public static CryptoEngine.theReturnType CriptazioneSequenza(
			string sequenza, // in chiaro
			out char mode
			)
		{
			// Criptazione
			//  ramoAndata della criptazione
			
			CryptoStore.Macro.CryptoEngine cs = new CryptoStore.Macro.CryptoEngine();
			CryptoStore.Macro.CryptoEngine.GenericAlgoPointer specificaFunzione = null;// puntatore al servizio di criptazione
			/* per decidere come valorizzare il puntatore a funzione,
				 * vado in web.config a leggere allo xpath "Criptazione/CryptoService"
				 * per vedere se la chiave "interruttore" e' su "acceso" o "spento"
				 * e se la chiave "specificaFunzione" punta a un algoritmo esistente o
				 * a un nome invalido.
				 */
			ConfigurationLayer.ConfigurationService myConfig =
				new ConfigurationLayer.ConfigurationService(
				"Criptazione/CryptoService");
			string AlgorithmSwitch = myConfig.GetStringValue( "switch");
			// NB. <!-- switch: {"simple", "multi", "off"} -->
			switch( AlgorithmSwitch)
			{
				case "simple":
				{
					specificaFunzione =
						new CryptoEngine.GenericAlgoPointer(
						CryptoStore.Micro.SimpleXor.xorCrypto_andata
						);
					mode = 's';// simple
					break;
				}
				default://NB. Error in web.config: key "switch" in {"simple", "multi", "off"}.
				case "multi":
				{
					specificaFunzione =
						new CryptoEngine.GenericAlgoPointer(
						CryptoStore.Micro.MultiXor.xorMultiCrypto_andata
						);
					mode = 'm';//multi
					break;
				}
				case "off":
				{
					specificaFunzione = null;// no crypto.
					mode = 'o';// off
					break;
				}
			}// end switch( AlgorithmSwitch)
			//
			CryptoStore.Macro.CryptoEngine.theReturnType cryptedValues =
				cs.Criptazione(
					sequenza,
					specificaFunzione
				);
			// ready
			return cryptedValues;
		}//


	}// end class
}
