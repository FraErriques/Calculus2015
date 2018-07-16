using System;
using Common.CryptoStore.Macro;

namespace Common.CryptoStore.Micro
{

	/// <summary>
	/// XorMultiCryptoAlgo implements a xor, with a different key for each letter.
	/// </summary>
	internal class MultiXor
	{
		private MultiXor()
		{}// functor class


		// La segnatura dell'attuale delegate va bene anche per questo algo:
		// struct della coppia( pwd, kkey) sia in ingresso che in uscita.
		// Ciascun passo del loop avra' la chiave dinamica aggiornata.
		// Nel giro di andata verra' randomizzata dal MonteCarlo ciascuna figura
		// della stringa-chiave. Nel giro di ritorno la stringa-chiave verra' letta
		// da db e ad ogni passo verra' scandita la figura successiva e, conseguentemente
		// invocato il core.
		// La stringa-chiave va in un campo object della struct-coppia( pwd, kkey).
		// Su db va messo in un varchar o text, che deve essere NOT NULL e DEFAULT '0'.



		/// <summary>
		/// operazione bitwise:
		/// ----------------------------------------------------------
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
		/// 			1110001   AND
		///				0010100
		///		   ---------------------
		///			bin	0010000
		///			dec		 16
		///			
		///			0010100   AND
		///			0010000
		///			----------------
		///			0010000 == 16 dec quindi il risultato e' saturo in AND
		///				
		///				
		///				1110001    OR
		///			    0010100
		///		   ---------------------
		///			bin	1110101
		///			dec     117
		///			
		///			    1110101   OR
		///			    0010100
		///		   ---------------------
		///			    1110101 == 117 dec quindi il risultato e' saturo in OR
		///		
		///				
		///			113	XOR		
		///			020		
		///		  ------- 
		///			101		
		///				
		///				1110001    XOR        uguali a 0, diversi a 1
		///			    0010100
		///		   ---------------------
		///			bin	1100101
		///			dec     101    	
		///			
		///				
		///						
		///								
		///								
		///								
		/// </summary>


		// una specifica implementazione di una funzione di questa categoria
		internal static CryptoEngine.theReturnType xorMultiCrypto_andata(
			CryptoEngine.theReturnType input )
		{
			int sequenceLength = input.cryptedSequence.Length;
            Common.CryptoStore.Macro.MonteCarlo mc = new Common.CryptoStore.Macro.MonteCarlo();// seme dipendente dall'orario
			System.Text.StringBuilder kkey_accumulator = new System.Text.StringBuilder( sequenceLength);
			System.Text.StringBuilder sequence_temp = new System.Text.StringBuilder( sequenceLength );
			for( int c=0; c<sequenceLength; c++)
			{
				// BEGIN solo andata
				int scalar_key = mc.next_integer( 32, 125);
				string scalar_key_str = "";
				if( 100 > scalar_key)
				{
					scalar_key_str += "0";
				}// else e' gia' di tre cifre
				scalar_key_str += scalar_key.ToString();
				kkey_accumulator.Append( scalar_key_str );
				// END solo andata
				sequence_temp.Append( (char)( input.cryptedSequence[c]^scalar_key ) );
			}
			input.cryptedSequence = sequence_temp.ToString();// pwd
			input.kkey = kkey_accumulator.ToString();// kkey multi
			// ready
			return input;
		}// end xorMultiCrypto_andata


		// una specifica implementazione di una funzione di questa categoria
		internal static CryptoEngine.theReturnType xorMultiCrypto_ritorno(
			CryptoEngine.theReturnType input )
		{
			try
			{
				// solo ritorno: viene richiesta una decription, quindi uso il valore indicato
				int sequenceLength = input.cryptedSequence.Length;
				System.Text.StringBuilder sequence_temp = new System.Text.StringBuilder( sequenceLength );
				for( int c=0, acc=0; c<sequenceLength; c++, acc+=3)
				{
					// BEGIN solo ritorno
					string scalar_key_str = "";
					scalar_key_str += ((string)input.kkey)[acc];
					scalar_key_str += ((string)input.kkey)[acc+1];
					scalar_key_str += ((string)input.kkey)[acc+2];
					int scalar_key = int.Parse( scalar_key_str);
					// END solo ritorno
					sequence_temp.Append( (char)( input.cryptedSequence[c]^scalar_key ) );
				}
				input.cryptedSequence = sequence_temp.ToString();// pwd
				// NB la kkey e' di sola lettura nel ritorno: non va sovrascritta
			}
			catch( System.Exception ex)
			{
				string s = ex.Message;// dbg
				input.cryptedSequence = "The sequence is not crypted with this algorithm. Unable to decrypt.";
				// NB la kkey e' di sola lettura nel ritorno: non va sovrascritta
			}// no finally
			// ready
			return input;
		}// end xorMultiCrypto_ritorno


	}// end class

}
