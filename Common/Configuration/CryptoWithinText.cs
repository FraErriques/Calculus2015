using System;
using System.Text;


namespace ConfigurationLayer2008
{


    /// <summary>
    /// Make use of CryptoStore, but let the results available for xml and html, i.e. exclude symbols
    /// which ASCII code is smaller than 32. Do it by means of Base64.
    /// </summary>
    public static class CryptoWithinText
    {


        /// <summary>
        /// Encrypt a Config entry with simple xor and then let it available in text format, i.e. in base64
        /// suitable for xml and html.
        /// --- in: string clearSequence, singleKey
        /// --- out: string suitableForXml
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="kkey"></param>
        /// <returns></returns>
        public static string EncryptForConfiguration(
            string sequence,
            int kkey // only simple xor, i.e. one integer kkey.
            )
        {//-----------------start ramo andata-----------------------------------
            if (
                0 > kkey
                || 255 < kkey
                )
            {
                throw new System.Exception("the key must be an integer in [0..255].");
            }// else can continue.
            Common.CryptoStore.Macro.CryptoEngine.theReturnType input;// it's a value type. It's enough to declare it to have i t on the stack.
            input.cryptedSequence = sequence;
            input.kkey = kkey.ToString();// it's an object and the library casts it to string.
            Common.CryptoStore.Macro.CryptoEngine.theReturnType cryptoResult =
                Common.CryptoStore.Micro.SimpleXor.commonCore(
                    input
                );
            //
            string suitableForXml = null;
            if (0 != kkey)
            {
                suitableForXml = ConfigurationLayer2008.CryptoWithinText.fromBinaryToBase64(
                    cryptoResult.cryptedSequence
                );
            }
            else// clear
            {
                suitableForXml = cryptoResult.cryptedSequence;
            }
            // ready
            return suitableForXml;
        }//-----------------fine ramo andata-----------------------------------




        /// <summary>
        /// Decrypt a Config entry, i.e. from Base64 to binary string and then to clear string by
        /// means of simple xor.
        /// --- in: string suitableForXml, singleKey
        /// --- out: string clearSequence
        /// </summary>
        /// <param name="sequence"></param>
        /// <param name="kkey"></param>
        /// <returns></returns>
        public static string DecryptForConfiguration(
            string suitableForXml,// a string in Base64, coming directly from a crypted configuration.
            int kkey // only simple xor, i.e. one integer kkey.
            )
        {//---start ritorno-------------------------------
            if (
                0 > kkey
                || 255 < kkey
                )
            {
                throw new System.Exception("the key must be an integer in [0..255].");
            }// else can continue.
            string backInBinary = null;
            if (0 != kkey)
            {
                backInBinary = ConfigurationLayer2008.CryptoWithinText.fromBase64ToBinary(
                    suitableForXml
                );
            }
            else// clear
            {
                backInBinary = suitableForXml;
            }
            //
            Common.CryptoStore.Macro.CryptoEngine.theReturnType toBeDecrypted;
            toBeDecrypted.kkey = kkey.ToString();// it's an object and the library casts it to string.
            toBeDecrypted.cryptedSequence = backInBinary;
            Common.CryptoStore.Macro.CryptoEngine.theReturnType decryptoResult =
                Common.CryptoStore.Micro.SimpleXor.commonCore(
                    toBeDecrypted
                );
            // ready
            return decryptoResult.cryptedSequence;
        }//-------------end ritorno------





        /// <summary>
        /// the binarySequence comes as string but, being binary cannot fit into an xml or html.
        /// </summary>
        /// <param name="binarySequence"></param>
        /// <returns></returns>
        public static string fromBinaryToBase64(
            string binarySequence// comes as string
           )
        {
            byte[] bytearray = new byte[binarySequence.Length];
            for (int c = 0; c < binarySequence.Length; c++)
            {
                bytearray[c] = (byte)(binarySequence[c]);
            }
            string encrInB64 =
                System.Convert.ToBase64String(
                    bytearray,
                    Base64FormattingOptions.None
                );
            return encrInB64;
        }// end fromBinaryToBase64


        /// <summary>
        /// the resulting sequence is contained in a string, but comes with symbols, which ASCII code can be<32.
        /// </summary>
        /// <param name="cryptedSequence"></param>
        /// <returns></returns>
        public static string fromBase64ToBinary(
            string base64Sequence
           )
        {
            byte[] bytearray =// binary sequence.
                System.Convert.FromBase64String(
                    base64Sequence
                );
            System.Text.StringBuilder sb = new StringBuilder(bytearray.Length);
            for (int c = 0; c < bytearray.Length; c++)
            {
                sb.Append((char)(bytearray[c]));// convert to text.
            }
            string binaryString = sb.ToString();
            return binaryString;
        }// end fromBase64ToBinary



        /// <summary>
        /// utility to dump on a temp file, the crypted section to be included into a config.
        /// </summary>
        /// <param name="kkey_k"></param>
        /// <param name="kkey_v"></param>
        /// <param name="couples"></param>
        /// <param name="fpath"></param>
        /// <returns></returns>
        public static bool PrepareCryptedSectionOnStream(
            int kkey_k,
            int kkey_v,
            System.Collections.Specialized.NameValueCollection couples,
            string fpath
          )
        {
            bool result = false;
            int nCouples = couples.Count;// NB. two entries more will be used, to configure the crypted section.
            System.Collections.Specialized.NameValueCollection cryptedCouples = new System.Collections.Specialized.NameValueCollection( nCouples+2);
            cryptedCouples.Add( "kkkey_key", kkey_k.ToString() );//--add fixed entries, to configure the crypted section.
            cryptedCouples.Add( "vkkey_key", kkey_v.ToString() );
            for (int c = 0; c < nCouples; c++)
            {
                cryptedCouples.Add(
                    ConfigurationLayer2008.CryptoWithinText.EncryptForConfiguration(couples.AllKeys[c], kkey_k),
                    ConfigurationLayer2008.CryptoWithinText.EncryptForConfiguration(couples[c], kkey_v)
                );
            }
            //---dump-----
            try
            {
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fpath);
                sw.WriteLine("---------------start keys------------");
                for (int c = 0; c < nCouples + 2; c++)// NB. two entries more will be used, to configure the crypted section.
                {
                    if (1 < c)
                    {
                        sw.Write( couples.AllKeys[c-2] + "  ");
                    }// else we are still in kkkeys section: they belong only to the crypted section.
                    sw.WriteLine( cryptedCouples.AllKeys[c]);
                }
                sw.WriteLine("---------------end keys------------");
                //
                sw.WriteLine("---------------start vals------------");
                for (int c = 0; c < nCouples + 2; c++)// NB. two entries more will be used, to configure the crypted section.
                {
                    if (1 < c)
                    {
                        sw.Write( couples[c-2] + "  ");
                    }// else we are still in kkkeys section: they belong only to the crypted section.
                    sw.WriteLine( cryptedCouples[c]);
                }
                sw.WriteLine("---------------end vals------------");
                sw.WriteLine("\r\n\r\n---------------start realistic section------------");
                for (int c = 0; c < nCouples + 2; c++)// NB. two entries more will be used, to configure the crypted section.
                {
                    sw.WriteLine( "<add key=\"" + cryptedCouples.AllKeys[c] + "\"   value=\"" + cryptedCouples[c] + "\"/>");
                }
                sw.WriteLine("---------------end realistic section------------");
                sw.Flush();
                sw.Close();
                result = true;
            }
            catch( System.Exception ex)
            {
                string s = ex.Message;// dbg
                result = false;
            }
            // ready
            return result;
        }//


    }// end class


}
