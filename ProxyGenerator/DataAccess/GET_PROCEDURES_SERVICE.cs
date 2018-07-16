using System;
using System.Text;
using System.Data;

namespace ProxyGenerator.DataAccess
{

    public class GET_PROCEDURES_SERVICE
    {

        public static System.Data.DataTable GET_PROCEDURES(
            System.Data.SqlClient.SqlConnection conn
            )
        {// NB. on requested db, not on model or master
            //
            System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = conn;
            if (null == cmd.Connection)
                return null;// exit on no-conn.
            cmd.CommandType = CommandType.Text;// NB. on requested db, not on model or master
            cmd.CommandText = 
                " select " +
                " [name] Name, " +
                " [id] [Id] " +
                " from sysobjects " +
                " where xtype='P' " +
                " order by name asc ";
            //
            System.Data.SqlClient.SqlDataAdapter da = new System.Data.SqlClient.SqlDataAdapter();
            da.SelectCommand = cmd;
            System.Data.DataTable dt = new DataTable("allProcedures");
            try
            {
                da.Fill(dt);
            }
            catch (System.Exception ex)
            {
                string s = ex.Message;
                dt = null;
            }
            finally
            {
                if (null != cmd.Connection)
                    if (System.Data.ConnectionState.Open == cmd.Connection.State)
                        cmd.Connection.Close();
            }
            // ready
            return dt;
        }// end service


    }// end class
}
