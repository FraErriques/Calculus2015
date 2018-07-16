

namespace DbLayer
{
    /// <summary>
    /// Summary description for dbCredentials.
    /// </summary>
    public class DbCredentials
    {
        private DbCredentials()
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
            ConfigurationLayer.ConfigurationService theConfigService =
                new ConfigurationLayer.ConfigurationService(
                    xpath);
            res.hostname_sql_instance = theConfigService.GetStringValue("hostname_sql_instance");
            res.db_name = theConfigService.GetStringValue("db_name");
            res.sql_instance = theConfigService.GetStringValue("sql_instance");
            res.usr = theConfigService.GetStringValue("usr");
            res.pwd = theConfigService.GetStringValue("pwd");
            return res;
        }// end get_DatabaseCredentials


    }// end class
}// end nmsp
