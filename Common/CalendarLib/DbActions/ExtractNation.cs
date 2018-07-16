using System;

namespace Common.CalendarLib.DbActions
{

    [Serializable]
    public class ExtractNation
    {

        private ExtractNation()
        {
        }

        public static bool ExtractionService(string NationName,
            out string Configuration)// NB.  out
        {
            System.Data.SqlClient.SqlDataAdapter da = null;
            Configuration = null;// NB.  out.  init -> valorizzazione specifica
            try
            {
                da = new System.Data.SqlClient.SqlDataAdapter();
                System.Data.DataSet ds = new System.Data.DataSet();
                da.SelectCommand = new System.Data.SqlClient.SqlCommand();
                da.SelectCommand.Connection =
                    DbLayer.ConnectionManager.connectWithCustomSingleXpath(
                        "ProxyGeneratorConnections/strings",// compulsory xpath
                        "CalendarDB"
                    );
                if (null == da.SelectCommand.Connection)
                {
                    return false;// no conn; else go on : connection is open.
                }
                //
                System.Data.SqlClient.SqlParameter theName = new System.Data.SqlClient.SqlParameter();
                theName.Direction = System.Data.ParameterDirection.Input;
                theName.DbType = System.Data.DbType.String;
                theName.ParameterName = "@NationName";
                theName.Value = NationName;
                //
                da.SelectCommand.Parameters.Add(theName);
                //
                da.SelectCommand.CommandType = System.Data.CommandType.StoredProcedure;
                da.SelectCommand.CommandText = "usp_extractCalendarConfiguration";
                da.Fill(ds);
                object result = ds.Tables[0].Rows[0].ItemArray[0];// get scalar resultset
                byte[] result_bytes = (byte[])result;
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int c = 0; c < result_bytes.Length; c++)
                {
                    sb.Append((char)(result_bytes[c]));
                }
                Configuration = sb.ToString();// NB.  out
            }
            catch (System.Exception ex)
            {
                string msg = ex.Message;// Debug
                return false;
            }
            finally
            {
                if (null != da.SelectCommand.Connection)
                {
                    if (System.Data.ConnectionState.Open == da.SelectCommand.Connection.State)
                    {
                        da.SelectCommand.Connection.Close();
                    }// else nothing.
                }// else nothing.
            }
            return true;
        }// end ExtractionService


    }// end class
}
