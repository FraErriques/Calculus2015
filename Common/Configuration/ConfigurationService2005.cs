

namespace ConfigurationLayer
{


    /// <summary>
    /// Questa classe è un helper per accedere alla configurazione
    /// </summary>
    public class ConfigurationService
    {
        private System.Collections.Specialized.NameValueCollection theRetrievedSection;
        public System.Collections.Specialized.NameValueCollection Get_theRetrievedSection
        {
            get 
            {
                return this.theRetrievedSection;
            }
            //
            // NB. no set.
        }// end property.


        /// <summary>
        /// no section specified. Use this constructor to get multiple sections subsequently,
        /// using the method GetSection()
        /// </summary>
        public ConfigurationService()
        {
        }// end ctor


        /// <summary>
        /// specialized Constructor, to get a single section and immediately die.
        /// </summary>
        /// <param name="configurationSectionXpath"></param>
        public ConfigurationService(string configurationSectionXpath)
        {
            object o =
                System.Configuration.ConfigurationManager.GetSection(
                    configurationSectionXpath);
            if (null == o)
            {
                throw new System.Exception("la sezione richiesta non e' presente in configurazione.");
            }// else continue
            this.theRetrievedSection =
                (System.Collections.Specialized.NameValueCollection)o;
        }// end ctor



        /// <summary>
        /// public helper, to set the active section. Call it and successively call GetStringValue,
        /// to acquire each voice content, within the current section.
        /// </summary>
        /// <param name="configurationSectionXpath"></param>
        public bool GetSection(string configurationSectionXpath)
        {
            bool result = false;
            //
            object o =
                System.Configuration.ConfigurationManager.GetSection(
                    configurationSectionXpath);
            if (null == o)
            {
                // la sezione richiesta non e' presente in configurazione.
                result = false;
            }// else continue
            else
            {
                this.theRetrievedSection =
                    (System.Collections.Specialized.NameValueCollection)o;
                result = true;
            }
            // ready
            return result;
        }// end GetSection() helper.



        public static string GetSingleVoice(
            string sectionName,
            string keyName
                )
        {
            System.Collections.Specialized.NameValueCollection requiredSection =
                (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationManager.GetSection( sectionName);
            if( null == requiredSection)
            {
                throw (new System.Exception("la sezione richiesta non e' presente in configurazione"));
            }// else continue
            string requiredValue = requiredSection.Get( keyName);
            if( null==requiredValue)
            {
                throw (new System.Exception("la chiave richiesta non è presente nella sezione"));
            }// else continue
            return requiredValue;
        }//



        // Fabrizio 260207
        // Metodo statico che fornisce l'accesso al file di configurazione dei messaggi di help dell'applicazione
        public static string GetHelpMessageString(string KeyName)
        {
            string sectionName = "HelpMessages";
            System.Collections.Specialized.NameValueCollection errorList =
                (System.Collections.Specialized.NameValueCollection)System.Configuration.ConfigurationManager.GetSection(sectionName);
            if (errorList == null)
            {
                throw (new System.Exception("la sezione richiesta non e' presente in configurazione"));
            }
            string errorMessage = errorList.Get(KeyName);

            if (errorMessage == null)
            {
                throw (new System.Exception("la chiave richiesta non è presente nella sezione"));
            }
            return errorMessage;
        }


        #region ConnectionStringsSection_management
        /// <summary>
        /// retrieve the entire ConnectionStringsSection
        /// </summary>
        /// <param name="configurationSectionXpath"></param>
        /// <returns></returns>
        public static System.Configuration.ConnectionStringsSection getConnectionStringsSection(string configurationSectionXpath)
        {
            System.Configuration.ConnectionStringsSection result =
                (System.Configuration.ConnectionStringsSection)
                    System.Configuration.ConfigurationManager.GetSection(
                        configurationSectionXpath);
            if (null == result)
            {
                throw new System.Exception("la sezione richiesta non e' presente in configurazione.");
            }// else continue
            //
            return result;
        }//end static getConnectionStringsSection

        /// <summary>
        /// retrieve the specified connectionStringName in the ConnectionStringsSection
        /// </summary>
        /// <param name="connectionStringsSection"></param>
        /// <param name="connectionStringName"></param>
        /// <returns></returns>
        public static string getSingleConnectionStringInSection(
            System.Configuration.ConnectionStringsSection connectionStringsSection,
            string connectionStringName)
        {// returns null on wrong connectionStringName
            return connectionStringsSection.ConnectionStrings[connectionStringName].ConnectionString;
        }// end getSingleConnectionStringInSection
        #endregion ConnectionStringsSection_management


		


        /// <summary>
        /// Questo metodo riorna il valore della proprietà stringa della configurazione richiesto
        /// </summary>
        /// <param name="key">Nome della proprietà</param>
        /// <returns>Valore della proprietà</returns>
        public string GetStringValue(string key)
        {
            string res = null;
            try
            {
                res = this.theRetrievedSection[key];
            }
            catch (System.Exception)
            {
                throw new System.Exception("la chiave richiesta non e' presente nella sezione corrente.");
            }
            return res;
        }



        /// <summary>
        /// Questo metodo verifica se una proprietà esiste o meno in configurazione.
        /// </summary>
        /// <param name="key">Nome della proprietà</param>
        /// <returns>Ritorna true se la proprietà esiste altrimenti false.</returns>
        public bool ExistsProperty(string key)
        {
            return theRetrievedSection.Get(key) != null ? true : false;
        }

        /// <summary>
        /// Questo metodo verifica se una proprietà è impostata oppore è vuota
        /// </summary>
        /// <param name="key">Nome della proprietà</param>
        /// <returns>Ritorna true se la proprietà non esiste o è vuota, altrimenti false.</returns>
        public bool IsEmptyProperty(string key)
        {
            if (!ExistsProperty(key))
                return true;
            else if (theRetrievedSection[key] == "")
                return true;
            else
                return false;
        }//


		
        /// <summary>
        /// 
        /// </summary>
        /// <param name="VoiceKey"></param>
        /// <returns></returns>
        public static string getAppSettingsVoice(string VoiceKey)
        {
            return System.Configuration.ConfigurationManager.AppSettings[VoiceKey];
        }//
		


    }// end class
}// end nmsp
