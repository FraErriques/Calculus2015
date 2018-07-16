using System;
using Common.CryptoStore.Macro;

namespace Common.CryptoStore.Micro
{


	/// <summary>
	/// SimpleXor implements the simple bitwise-xor, with the same key for
	/// each letter of the input string.
	/// </summary>
	internal class SimpleXor
	{
		private SimpleXor()
		{}

		/// <summary>
		/// operazione bitwise:
		///		dove sono UGUALI i bit, il risultato ha il bit ZERO
		///		dove sono DIVERSI i bit, il risultato ha il bit UNO
		///			schema:
		///			 UGUALI  -> ZERO
		///			 DIVERSI -> UNO
		///			 
		/// La sequenza di bit zero e' l'elemento neutro dell'operatore xor:
		///		i bit zero dell'altro operando vanno in zero perche' uguali
		///		i bit uno dell'altro operando vanno in uno perche' diversi
		///		
		/// La sequenza di bit uno e' l'operando che scambia i valori di ciascun bit:
		///		i bit zero dell'altro operando vanno in uno perche' diversi
		///		i bit uno dell'altro operando vanno in zero perche' uguali
		///
		///	Il core deve essere chiamato da un ciclo, che sara' posto sia nel
		/// giro di andata, che in quello di ritorno.
		///
		///
		/// xor
		/// -----
		/// L'uso di un operando "alpha" su un operando "x" produce un risultato
		/// "y". L'applicazione dell'operatore xor agli operandi "alpha" e "y"
		/// porta in "x". Questa ciclicita' dell'operatore e' utilizzata nel
		/// presente algoritmo.
		/// 
		/// 
		/// and
		/// -----
		/// Considero ora l'operatore "and". L'uso di un operando "alpha"
		/// su un operando "x" produce un risultato "y". Tale risultato e'
		/// la "saturazione" verso il basso di "x" mediante "alpha". Quindi
		/// "y" minore o uguale a "x" e le successive applicazioni di
		/// "alpha" ad "y" mediante "and" portano ancora a "y".
		/// 
		/// or
		/// ------
		/// Considero ora l'operatore "or". L'uso di un operando "alpha"
		/// su un operando "x" produce un risultato "y". Tale risultato e'
		/// la "saturazione" verso l'alto di "x" mediante "alpha". Quindi
		/// "y" maggiore o uguale a "x" e le successive applicazioni di
		/// "alpha" ad "y" mediante "or" portano ancora a "y".
		/// 
		/// 
		/// </summary>
		/// <param name="input">la coppia "pwd" e "kkey" </param>
		/// <returns>la coppia di ingresso, con la "pwd" rivisitata</returns>
		internal static CryptoEngine.theReturnType commonCore(
			CryptoEngine.theReturnType input )
		{
			CryptoEngine.theReturnType result = input;
			int sequenceLength = input.cryptedSequence.Length;
			System.Text.StringBuilder temp = new System.Text.StringBuilder( sequenceLength );
			string input_kkey_str = (string)input.kkey;
			int input_kkey_int = int.Parse( input_kkey_str);
			for( int c=0; c<sequenceLength; c++)
			{
				temp.Append( (char)( input.cryptedSequence[c]^input_kkey_int ) );
			}
			result.cryptedSequence = temp.ToString();// scopo del commonCore
			// ready
			return result;
		}// end commonCore



		// una specifica implementazione di una funzione di questa categoria
		internal static CryptoEngine.theReturnType xorCrypto_andata(
			CryptoEngine.theReturnType input )
		{
			// solo in andata: preparo la chiave di criptazione
            Common.CryptoStore.Macro.MonteCarlo mc = new Common.CryptoStore.Macro.MonteCarlo();// seme dipendente dall'orario
			input.kkey = mc.next_integer( 32, 125).ToString();// il commonCore lo vuole stringa per compatibilita' col db (quindi col ritorno)
			// end solo in andata
			return commonCore( input );
		}// end xorCryptoService


		// una specifica implementazione di una funzione di questa categoria
		internal static CryptoEngine.theReturnType xorCrypto_ritorno(
			CryptoEngine.theReturnType input )
		{
			// solo ritorno: viene richiesta una decription, quindi uso il valore indicato
			return commonCore( input );
		}// end xorCryptoService



	}// end class

}
