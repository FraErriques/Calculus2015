using System;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace Entity.Proxies
{


    public abstract class Prime_sequence_INSERT_SERVICE
    {


        public static int Prime_sequence_INSERT(
			Int64 prime,
			System.Data.SqlClient.SqlConnection conn
		)
		{
            //
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            if( null==cmd.Connection)
                return -1;// no conn
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "Prime_sequence_INSERT";
            //
			int writingSucceeded = -1;// init to error:no_connection.
			//
			//
            System.Data.SqlClient.SqlParameter parprime = new SqlParameter();
            parprime.Direction = ParameterDirection.Input;
            parprime.DbType = DbType.Int64;
            parprime.ParameterName = "@prime";
			cmd.Parameters.Add( parprime);// add to command
			if( 0L<prime )
			{
				parprime.Value = prime;// checks ok -> ProxyParemeter value assigned to the SqlParameter.
			}
			else
			{
				parprime.Value = System.DBNull.Value;
			}

            //
            try
            {
				//
                int rowsWritten =
                    cmd.ExecuteNonQuery();
                //
                if (1 <= rowsWritten )
                    writingSucceeded = 0;// rows written ok
                else
                    writingSucceeded = 4;// errore logico senza exception
				//
				//
            }
            catch (Exception ex)
            {
				//
				//
				/// <returns>
				/// -1  no connection
				/// 0   ok
				/// 1   sqlException chiave duplicata
				/// 2   sqlException diversa da chiave duplicata
				/// 3   eccezione NON sql
				/// 4   errore logico senza Exception
				/// ...
				/// >4  altre eccezioni TODO:dettagliare in fututo
				/// 
				/// </returns>
                //
                //---------------------exception nature discrimination----------------------
                writingSucceeded =
                    LoggingToolsContainerNamespace.LoggingToolsContainer.DecideAndLog(
                        ex,
                        "eccezione in DataAccess::Prime_sequence_INSERT_SERVICE : " + ex.Message,
						0 // verbosity
                );
                //
            }// end catch
            finally
            {
                // NB. no closure. Same for all, i.e. sticky.
            }
            // ready
            return writingSucceeded;// writing result is an integer.
        }// end service


    }// end class
}// end namespace
