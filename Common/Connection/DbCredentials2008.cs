
namespace DbLayer
{


    public class DbCredentials2008
    {
        private DbCredentials2008()
        {// no inst
        }


        public struct DatabaseCredentials
        {
            public string hostname_sql_instance;
            public string db_name;
            public string sql_instance;
            public string usr;
            public string pwd;
        }// end struct


        public static DatabaseCredentials get_DatabaseCredentials(string xpath)
        {
            DatabaseCredentials res = new DatabaseCredentials();
            System.Collections.Specialized.NameValueCollection databaseCredentialsCouples =
                ConfigurationLayer2008.CustomSectionInOneShot.GetCustomSectionInOneShot(
                        xpath);
            if (null == databaseCredentialsCouples) return res;// the struct will contain empty fields.
            res.hostname_sql_instance = (string)(databaseCredentialsCouples["hostname_sql_instance"]);
            res.db_name =               (string)(databaseCredentialsCouples["db_name"]);
            res.sql_instance =          (string)(databaseCredentialsCouples["sql_instance"]);
            res.usr =                   (string)(databaseCredentialsCouples["usr"]);
            res.pwd =                   (string)(databaseCredentialsCouples["pwd"]);
            return res;
        }// end get_DatabaseCredentials


        public static string get_DatabaseSingleCredential( 
            string xpath,
            string connectionStringName
          )
        {
            System.Collections.Specialized.NameValueCollection databaseCredentialsCouples =
                ConfigurationLayer2008.CustomSectionInOneShot.GetCustomSectionInOneShot(
                        xpath);
            if (null == databaseCredentialsCouples)
            {
                throw new System.Exception("la sezione richiesta non e' presente in configurazione.");
            }// else can continue.
            string wholeConnectionString = (string)(databaseCredentialsCouples[connectionStringName]);
            return wholeConnectionString;
        }// end get_DatabaseCredentials


    }// end class
}// end nmsp
