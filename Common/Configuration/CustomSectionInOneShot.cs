using System;
using System.Collections.Generic;


namespace ConfigurationLayer2008
{

    /// <summary>
    /// gets an hashtable containing an entire config section, in clear mode, which may be crypted in the config file.
    /// sono richieste le chiavi kkkey_key e vkkey_key, oltre a quelle applicative.
    /// </summary>
    public static class CustomSectionInOneShot
    {


        /// <summary>
        /// gets a NameValueCollection containing an entire config section, in clear mode, which may be crypted in the config file.
        /// sono richieste le chiavi kkkey_key e vkkey_key, oltre a quelle applicative.
        /// </summary>
        /// <param name="configurationSectionXpath"></param>
        /// <returns></returns>
        public static System.Collections.Specialized.NameValueCollection GetCustomSectionInOneShot(
            string configurationSectionXpath // xml path of the section
        )
        {
            System.Collections.Specialized.NameValueCollection result = null;
            System.Collections.Specialized.NameValueCollection theRetrievedSection = null;
            //
            //
            object o =
                System.Configuration.ConfigurationManager.GetSection(
                    configurationSectionXpath);
            if (null == o)
            {
                // la sezione richiesta non e' presente in configurazione.
                return result;// which, by now, is null.
            }// else continue
            //
            theRetrievedSection =
                (System.Collections.Specialized.NameValueCollection)o;
            //
            //---for the keys---
            string kkkey_value = null;
            kkkey_value = theRetrievedSection.Get( "kkkey_key");
            if (null == kkkey_value)
            {
                throw (new System.Exception("la chiave richiesta non è presente nella sezione"));
            }// else continue
            //
            //---for the values---
            string vkkey_value = null;
            vkkey_value = theRetrievedSection.Get("vkkey_key");
            if (null == vkkey_value)
            {
                throw (new System.Exception("la chiave richiesta non è presente nella sezione"));
            }// else continue
            //
            //--start retrieving keys---
            int cardKeys = theRetrievedSection.Count;
            const int cardPreDefinedKeyValCouples = 2;// sono richieste le due chiavi kkkey_key e vkkey_key, oltre a quelle applicative.
            if (cardKeys <= cardPreDefinedKeyValCouples)
            {
                throw (new System.Exception("la sezione non e' stata configurata nel modo appropriato: sono richieste le chiavi kkkey_key e vkkey_key, oltre a quelle applicative."));
            }// else continue
            result = new System.Collections.Specialized.NameValueCollection( cardKeys - cardPreDefinedKeyValCouples);// the remaining entries of the section, excluded the kkkkkkeys.
            for (int c = cardPreDefinedKeyValCouples; c < cardKeys; c++)//[0..2] busy for kkkkeysss etc..  :-)
            {
                //  get binary string from base64 for key:
                string inBinary = null;
                if ("0" != kkkey_value.Trim())
                {
                    inBinary = ConfigurationLayer2008.CryptoWithinText.fromBase64ToBinary(theRetrievedSection.Keys[c]);
                }
                else
                {
                    inBinary = theRetrievedSection.Keys[c];// key is in clear
                }
                Common.CryptoStore.Macro.CryptoEngine.theReturnType configurationElements;
                configurationElements.cryptedSequence = inBinary;
                configurationElements.kkey = kkkey_value;// NB. in simpleMode the mode depends on the kkey.
                Common.CryptoStore.Macro.CryptoEngine.theReturnType clearElements =
                    Common.CryptoStore.Micro.SimpleXor.commonCore(
                        configurationElements);
                string tmpKey = clearElements.cryptedSequence;
                result[tmpKey] = null;// it's null until its value gets retrieved.
                //
                //  get binary string from base64 for val:
                if ("0" != vkkey_value.Trim())
                {
                    inBinary = ConfigurationLayer2008.CryptoWithinText.fromBase64ToBinary( theRetrievedSection.Get(theRetrievedSection.Keys[c]));
                }
                else
                {
                    inBinary = theRetrievedSection.Get(theRetrievedSection.Keys[c]);// val is in clear
                }
                configurationElements.cryptedSequence = inBinary;
                configurationElements.kkey = vkkey_value;// NB. in simpleMode the mode depends on the kkey.
                clearElements =
                    Common.CryptoStore.Micro.SimpleXor.commonCore(
                        configurationElements);
                string tmpVal = clearElements.cryptedSequence;
                result[tmpKey] = tmpVal;// value retrieved and assigned to its key.
            }
            // ready
            return result;
        }// end GetCustomSectionInOneShot


    }// end class


}
