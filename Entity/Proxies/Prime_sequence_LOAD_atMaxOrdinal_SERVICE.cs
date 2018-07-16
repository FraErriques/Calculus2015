using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace Entity.Proxies
{


    public abstract class Prime_sequence_LOAD_atMaxOrdinal_SERVICE
    {


        public static System.Data.DataTable Prime_sequence_LOAD_atMaxOrdinal(
            System.Data.SqlClient.SqlConnection conn
		)
		{
            //
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            if( null==cmd.Connection)
                return null;// no conn
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Prime_sequence_LOAD_atMaxOrdinal";
            //
            System.Data.DataTable resultset = new System.Data.DataTable("ResultSet");
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;// bind adapter to the command.
			//

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
					"eccezione in DataAccess::Prime_sequence_LOAD_atMaxOrdinal_SERVICE : " + ex.Message,
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
