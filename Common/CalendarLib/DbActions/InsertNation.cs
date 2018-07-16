using System;

namespace Common.CalendarLib.DbActions
{

	[Serializable]
	public class InsertNation
	{
		private InsertNation()
		{
		}

		public static bool InsertionService(	string NationName,
			string Configuration )
		{
			System.Data.SqlClient.SqlCommand cmd = null;
			try
			{
				cmd = new System.Data.SqlClient.SqlCommand();
				cmd.Connection =
                    DbLayer.ConnectionManager.connectWithCustomSingleXpath(
                        "ProxyGeneratorConnections/strings",// compulsory xpath
                        "CalendarDB"
                    );
                if (null == cmd.Connection)
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
				System.Data.SqlClient.SqlParameter theConfig = new System.Data.SqlClient.SqlParameter();
				theConfig.Direction = System.Data.ParameterDirection.Input;
				theConfig.DbType = System.Data.DbType.Binary;
				theConfig.ParameterName = "@NationalConfiguration";
				byte[] binaryConfig = new byte[Configuration.Length];
				for( int c=0; c<Configuration.Length; c++)
				{
					binaryConfig[c] = (byte)(Configuration[c]);
				}
				theConfig.Value = binaryConfig;// after cast to binary format
				//
				cmd.Parameters.Add( theName );
				cmd.Parameters.Add( theConfig );
				//
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				cmd.CommandText = "usp_insertCalendarConfiguration";
				int rowsAffected = cmd.ExecuteNonQuery();
			}
			catch( System.Exception ex)
			{
				string msg = ex.Message;// Debug
				return false;
			}
			finally
            {
                if (null != cmd.Connection)
                {
                    if (System.Data.ConnectionState.Open == cmd.Connection.State)
                    {
                        cmd.Connection.Close();
                    }// else nothing.
                }// else nothing.
            }
			return true;
		}// end InsertionService


	}// end class
}
