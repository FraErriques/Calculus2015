using System;

namespace Common.CalendarLib.DbActions
{

	
	[Serializable]	
	public class CountNations
	{
		private CountNations()
		{
		}

		public static bool CountService( out int hmConfiguredNations )
		{
			System.Data.SqlClient.SqlDataAdapter da = null;
			hmConfiguredNations = -1;// NB.  out.  init -> valorizzazione specifica
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
				da.SelectCommand.CommandType = System.Data.CommandType.Text;
				da.SelectCommand.CommandText = "select count(*) from  [CalendarDB].[dbo].[CalendarConfiguration]";
				da.Fill( ds);
				object result = ds.Tables[0].Rows[0].ItemArray[0];// get scalar resultset
				hmConfiguredNations = (int)result;// NB.  out
			}
			catch( System.Exception ex)
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
		}// end CountService


	}
}
