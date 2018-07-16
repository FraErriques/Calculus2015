using System;

namespace Common.CalendarLib.DbActions
{

	[Serializable]
	public static class GetNationNames
	{

		public static bool GetNationNamesService( out string[] theConfiguredNames )
		{
			System.Data.SqlClient.SqlDataAdapter da = null;
			theConfiguredNames = null;
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
				da.SelectCommand.CommandText = "select NationName from  [CalendarDB].[dbo].[CalendarConfiguration]";
				da.Fill( ds);
				int result_cardinality = ds.Tables[0].Rows.Count;
				theConfiguredNames = new string[result_cardinality];
				for(int c=0; c<result_cardinality; c++)
				{
					theConfiguredNames[c] = (string)(ds.Tables[0].Rows[c].ItemArray[0]);// get a NationName
				}
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
		}// end GetNationNamesService


	}
}
