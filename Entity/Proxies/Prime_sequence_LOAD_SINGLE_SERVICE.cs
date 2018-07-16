using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace Entity.Proxies
{


    public abstract class Prime_sequence_LOAD_SINGLE_SERVICE
    {


        public static System.Data.DataTable Prime_sequence_LOAD_SINGLE(
			Int64 ordinal,
            System.Data.SqlClient.SqlConnection conn
		)
		{
            //
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            if( null==cmd.Connection)
                return null;// no conn
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Prime_sequence_LOAD_SINGLE";
            //
            System.Data.DataTable resultset = new System.Data.DataTable("ResultSet");
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;// bind adapter to the command.
			//
			//
            System.Data.SqlClient.SqlParameter parordinal = new SqlParameter();
            parordinal.Direction = ParameterDirection.Input;
            parordinal.DbType = DbType.Int64;
            parordinal.ParameterName = "@ordinal";
			cmd.Parameters.Add( parordinal);// add to command
			if( 0L<ordinal )
			{
				parordinal.Value = ordinal;// checks ok -> ProxyParemeter value assigned to the SqlParameter.
			}
			else
			{
				parordinal.Value = System.DBNull.Value;
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
					"eccezione in DataAccess::Prime_sequence_LOAD_SINGLE_SERVICE : " + ex.Message,
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
