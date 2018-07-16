using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace Entity.Proxies
{


    public abstract class Prime_sequence_LOAD_MULTI_SERVICE
    {


        public static System.Data.DataTable Prime_sequence_LOAD_MULTI(
			Int64 start_ordinal,
			Int64 end_ordinal,
            System.Data.SqlClient.SqlConnection conn
		)
		{
            //
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            if( null==cmd.Connection)
                return null;// no conn
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Prime_sequence_LOAD_MULTI";
            //
            System.Data.DataTable resultset = new System.Data.DataTable("ResultSet");
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;// bind adapter to the command.
			//
			//
            System.Data.SqlClient.SqlParameter parstart_ordinal = new SqlParameter();
            parstart_ordinal.Direction = ParameterDirection.Input;
            parstart_ordinal.DbType = DbType.Int64;
            parstart_ordinal.ParameterName = "@start_ordinal";
			cmd.Parameters.Add( parstart_ordinal);// add to command
			if( 0L<start_ordinal )
			{
				parstart_ordinal.Value = start_ordinal;// checks ok -> ProxyParemeter value assigned to the SqlParameter.
			}
			else
			{
				parstart_ordinal.Value = System.DBNull.Value;
			}
			//
            System.Data.SqlClient.SqlParameter parend_ordinal = new SqlParameter();
            parend_ordinal.Direction = ParameterDirection.Input;
            parend_ordinal.DbType = DbType.Int64;
            parend_ordinal.ParameterName = "@end_ordinal";
			cmd.Parameters.Add( parend_ordinal);// add to command
			if( 0L<end_ordinal )
			{
				parend_ordinal.Value = end_ordinal;// checks ok -> ProxyParemeter value assigned to the SqlParameter.
			}
			else
			{
				parend_ordinal.Value = System.DBNull.Value;
			}

            //
            try
            {
				//
                da.Fill(resultset);
				//
				//
            }
            catch (Exception ex)
            {
				//
				//
                resultset = null;
                //
                //---------------------exception nature discrimination----------------------
                // no integer map in the return value.
                LoggingToolsContainerNamespace.LoggingToolsContainer.DecideAndLog(
					ex,
					"eccezione in DataAccess::Prime_sequence_LOAD_MULTI_SERVICE : " + ex.Message,
                    0 // verbosity
                );
                //
            }// end catch
            finally
            {
                // NB. no closure. Same for all, i.e. sticky.
            }
            // ready
            return resultset;// one or more datatables.
        }// end service


    }// end class
}// end namespace
