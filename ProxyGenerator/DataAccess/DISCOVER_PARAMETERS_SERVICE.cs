using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;


namespace ProxyGenerator.DataAccess
{


    class DISCOVER_PARAMETERS_SERVICE
    {


        public static SqlParameter[] DiscoverSpParameterSet(
            System.Data.SqlClient.SqlConnection conn,
            string spName,
            bool includeReturnValueParameter )
        {
            if (conn == null)
                return null;
            if (spName == null || spName.Length == 0)
                return null;

            // Original cmd object
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = spName;

            // Hack to check for schema name in the spName
            string schemaName = "dbo";
            int firstDot = spName.IndexOf('.');
            if (firstDot > 0)
            {
                schemaName = spName.Substring(0, firstDot);
                spName = spName.Substring(firstDot + 1);
            }

            // Now this is not neat - I am trying the SQL2000 version and if it fails
            // I go to the manual SQL2005 version
            try
            {
                // First, attempt the SQL2000 version (no schema)
                SqlCommandBuilder.DeriveParameters(cmd);
            }
            catch( System.Exception ex)
            {
                string s = ex.Message;
                // If we are here, SQL2000 call failed
                // Manually run the 'derive params' SP
                // this time with the schema name parameter
                SqlCommand getParams = new SqlCommand("sp_procedure_params_rowset", conn);
                getParams.CommandType = CommandType.StoredProcedure;
                getParams.Parameters.AddWithValue( "@procedure_name", spName);
                getParams.Parameters.AddWithValue( "@procedure_schema", schemaName);
                SqlDataReader sdr = getParams.ExecuteReader();

                // Do we have any rows?
                if (sdr.HasRows)
                {
                    using (sdr)
                    {
                        // Read the parameter information
                        int ParamNameCol = sdr.GetOrdinal("PARAMETER_NAME");
                        int ParamSizeCol = sdr.GetOrdinal("CHARACTER_MAXIMUM_LENGTH");
                        int ParamTypeCol = sdr.GetOrdinal("TYPE_NAME");
                        int ParamNullCol = sdr.GetOrdinal("IS_NULLABLE");
                        int ParamPrecCol = sdr.GetOrdinal("NUMERIC_PRECISION");
                        int ParamDirCol = sdr.GetOrdinal("PARAMETER_TYPE");
                        int ParamScaleCol = sdr.GetOrdinal("NUMERIC_SCALE");

                        // Loop through and read the rows
                        while (sdr.Read())
                        {
                            string name = sdr.GetString(ParamNameCol);
                            string datatype = sdr.GetString(ParamTypeCol);
                            // Is this xml?
                            // ADO.NET 1.1 does not support XML, replace with text
                            if (0 == String.Compare("xml", datatype, true))
                            {
                                datatype = "Text";
                            }
                            object parsedType = Enum.Parse(typeof(SqlDbType), datatype, true);
                            SqlDbType type = (SqlDbType)parsedType;
                            bool Nullable = sdr.GetBoolean(ParamNullCol);
                            SqlParameter param = new SqlParameter(name, type);
                            // Determine parameter direction
                            int dir = sdr.GetInt16(ParamDirCol);
                            switch (dir)
                            {
                                case 1:
                                    param.Direction = ParameterDirection.Input;
                                    break;
                                case 2:
                                    param.Direction = ParameterDirection.Output;
                                    break;
                                case 3:
                                    param.Direction = ParameterDirection.InputOutput;
                                    break;
                                case 4:
                                    param.Direction = ParameterDirection.ReturnValue;
                                    break;
                            }
                            param.IsNullable = Nullable;
                            if (!sdr.IsDBNull(ParamPrecCol))
                            {
                                param.Precision = (Byte)sdr.GetInt16(ParamPrecCol);
                            }
                            if (!sdr.IsDBNull(ParamSizeCol))
                            {
                                param.Size = sdr.GetInt32(ParamSizeCol);
                            }
                            if (!sdr.IsDBNull(ParamScaleCol))
                            {
                                param.Scale = (Byte)sdr.GetInt16(ParamScaleCol);
                            }
                            cmd.Parameters.Add(param);
                        }
                    }
                }
            }
            finally
            {
                conn.Close();
            }

            if (!includeReturnValueParameter)
            {
                if( cmd.Parameters.Count > 0 )
                    cmd.Parameters.RemoveAt(0);
            }// else keep it. it's already there at index 0.
            // prepare the return value
            SqlParameter[] discoveredParameters = new SqlParameter[cmd.Parameters.Count];
            // flush the sqlCommandParameters into the return value.
            cmd.Parameters.CopyTo(discoveredParameters, 0);
            //
            // WORKAROUND begin
            foreach (SqlParameter sqlParam in discoveredParameters)
            {
                if ((sqlParam.SqlDbType == SqlDbType.VarChar) &&
                    (sqlParam.Size == Int32.MaxValue))
                {
                    sqlParam.SqlDbType = SqlDbType.Text;
                }
            }
            // WORKAROUND end
            //
            // Init the parameters with a DBNull value
            foreach (SqlParameter discoveredParameter in discoveredParameters)
            {
                discoveredParameter.Value = DBNull.Value;
            }
            return discoveredParameters;
        }// end service


    }// end class
}
