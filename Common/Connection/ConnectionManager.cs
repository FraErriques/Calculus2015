

namespace DbLayer
{
    /// <summary>
    /// ConnectionManager e' la classe responsabile delle connessioni a DB
    /// </summary>
    public class ConnectionManager
    {
        private ConnectionManager()
        {// no constructor
        }


		
        public static System.Data.SqlClient.SqlConnection connectWithPreparedString(
            string connectionStringName)
        {
            System.Data.SqlClient.SqlConnection result = new System.Data.SqlClient.SqlConnection();
            try
            {
                // the following lines throw if required connection string is absent in "connectionStrings"
                System.Configuration.ConnectionStringsSection connectionStringsSection =
                    ConfigurationLayer.ConfigurationService.getConnectionStringsSection(
                        "connectionStrings");// this section name is compulsory
                result.ConnectionString = ConfigurationLayer.ConfigurationService.getSingleConnectionStringInSection(
                    connectionStringsSection,
                    connectionStringName);
                // try open
                result.Open();// returns null on non-opened connection
            }
            catch (System.Exception ex)
            {
                string error = ex.Message;
                return null;
            }
            return result;
        }// end connectWithPreparedString


        public static System.Data.SqlClient.SqlConnection connectWithCustomSingleToken(
            string wholeConnectionStringContent
           )
        {
            System.Data.SqlClient.SqlConnection result = new System.Data.SqlClient.SqlConnection();
            try
            {
                result.ConnectionString = wholeConnectionStringContent;
                // try open
                result.Open();// returns null on non-opened connection
            }
            catch (System.Exception ex)
            {
                string error = ex.Message;
                return null;
            }
            return result;
        }// end connectWithCustomSingleToken



        public static System.Data.SqlClient.SqlConnection connectWithCustomSingleXpath(
            string xpath,
            string connectionStringName
          )
        {
            string desiredConnectionString =
                DbLayer.DbCredentials2008.get_DatabaseSingleCredential(
                    xpath,
                    connectionStringName
                );
            return
                connectWithCustomSingleToken(
                    desiredConnectionString
                );
        }// end connectWithCustomSingleXpath



        public static System.Data.SqlClient.SqlConnection getConnection(
           string db_name,
           string hostname_sql_instance,
           string usr,
           string pwd,
           string sql_instance
           )
        {
            System.Data.SqlClient.SqlConnection result = null;
            try
            {
                string connectionString =
                   "Initial Catalog=" + db_name + ";" +	   // db name
                   "Data Source=" + hostname_sql_instance + ";" +       // hostname\sql-instance
                   "User ID=" + usr + ";" +                    // usr
                   "Pwd=" + pwd + ";" +                        // pwd
                   "Application Name = some_application;" +
                   "Workstation ID=" + sql_instance + ";" +        // sql-instance
                   "Persist Security Info=False;" +
                   "Packet Size=4096";

                result = new System.Data.SqlClient.SqlConnection( connectionString);// sql throws here on wrong connection-string syntax.
                result.Open();// sql throws here on wrong connection parameters.
            }
            catch (System.Exception ex)
            {
                string error = ex.Message;
                return null;
            }
            return result;
        }//

        public static System.Data.SqlClient.SqlConnection getConnection( string xpath)
        {
            DbLayer.DbCredentials.DatabaseCredentials dbConfig =
                DbLayer.DbCredentials.get_DatabaseCredentials( xpath);
            return getConnection(
                dbConfig.db_name,
                dbConfig.hostname_sql_instance,
                dbConfig.usr,
                dbConfig.pwd,
                dbConfig.sql_instance);
        }// end getConnection()


        public static System.Data.SqlClient.SqlConnection getCryptedConnection(
            string xpath )
        {
            DbLayer.DbCredentials2008.DatabaseCredentials dbConfig =
                DbLayer.DbCredentials2008.get_DatabaseCredentials( xpath);
            return getConnection(
                dbConfig.db_name,
                dbConfig.hostname_sql_instance,
                dbConfig.usr,
                dbConfig.pwd,
                dbConfig.sql_instance);
        }// end getConnection()


    }
}
