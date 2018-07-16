using System;

namespace LogSinkDb.Library
{
	/// <summary>
	/// SinkDb is a Logging component, that uses a database as tracing deposit( sink).
	/// The database credentials are in the configuration file. The databese name is 
	/// compulsory and is "Logging". The table( unique) is custom and is application wide.
	/// Each application chooses its log name. Once the application is run with logging enabled,
	/// its table is created. From that time on the tracings will be appended there. It's good
	/// to provide a batch job to shrink the db and/or delete something, periodically.
	/// </summary>
	public class SinkDb : IDisposable
	{
		private System.Data.SqlClient.SqlConnection logDbConn = null;
		private string								TableName 	= "not yet initialized";
		private System.Collections.Stack			tagStack				= null;
		private bool            					hasPermissionsToWrite = false;// default
		private int									semaphore;
		private int									verbosity;
		private string								constructorException 	= "not yet initialized";
		private string								operationException   	= "not yet initialized";
		private int									stackDepth = 0;
		// data type for the generic log-entry
		private class CurrentTag
		{
			public string tagName;
			public int sectionVerbosity;
			// Ctor
			public CurrentTag( string tagName, int sectionVerbosity)
			{
				this.tagName = tagName;
				this.sectionVerbosity = sectionVerbosity;
			}// end Ctor
		}// end struct



		// NB. dato privato e di istanza( ma filtrato dal pattern_Singleton) e' la db-connection.
		// acquisirla in apertura dell'applicazione e rilasciarla alla dispose.
		public SinkDb( )
		{
			lock( typeof( LogSinkDb.Library.SinkDb))
			{
				// config-parsing
                try// silent log. All must be catched.
                {
                    // read the connection-string from configuration and try acquire such connection.
                    this.logDbConn = DbLayer.ConnectionManager.getCryptedConnection(
                        "LogSinkDb/connection");
                    if (null == this.logDbConn ||
                        System.Data.ConnectionState.Open != this.logDbConn.State)
                    {
                        throw new System.Exception("Impossibile acquisire la connessione al db. Verificare il file di configurazione.");
                    }// else ok -> go on
                    // read the rest of configuration.
                    ConfigurationLayer.ConfigurationService cs = new
                        ConfigurationLayer.ConfigurationService(
                            "LogSinkDb/logger_application");
                    this.TableName = cs.GetStringValue("table_name");
                    string semaphore = cs.GetStringValue("semaphore");
                    switch (semaphore)
                    {
                        case "on":
                            {
                                this.semaphore = 1;// green semaphore
                                break;
                            }
                        case "off":
                        default:
                            {
                                this.semaphore = 0;// red semaphore
                                break;
                            }
                    }// end switch on semaphore
                    string verbosity = cs.GetStringValue("verbosity");
                    this.verbosity = int.Parse(verbosity);// if this throws -> hasPermissionsToWrite=false
                    // end configuration acquisition. Implementation start
                    if (1 != this.semaphore)
                    {
                        hasPermissionsToWrite = false;
                    }
                    else // green semaphore
                    {
                        // prepare tag-stack
                        this.tagStack = new System.Collections.Stack();
                        // once the tableName is read from configuration -> try-create such table
                        if (!this.createTable(this.TableName))
                        {
                            throw new System.Exception("L'accesso alla tabella di log ha sollevato un'eccezione. Verificare il file di configurazione.");
                        }// else ok -> go on
                        hasPermissionsToWrite = true;
                    }// end else // green semaphore
                }// end try
                catch (Exception ex)
                {// if this throws -> hasPermissionsToWrite=false
                    this.constructorException = ex.Message;
                    hasPermissionsToWrite = false;
                }
                finally
                {
                    if (null != this.logDbConn)
                    {
                        if (System.Data.ConnectionState.Open == this.logDbConn.State)
                        {
                            this.logDbConn.Close();// no-persistency. Volatile connections.
                        }// else already closed
                    }// else no connection
                }
			}// end critical section
		}// end Ctor



		#region IDisposable Members

		public void Dispose()
		{
			lock( typeof( LogSinkDb.Library.SinkDb) )
			{
				if( null!= this.logDbConn)
				{
					if(System.Data.ConnectionState.Open==this.logDbConn.State)
					{
						this.logDbConn.Close();
					}// else already closed
				}// end if( null!= this.logDbConn). else conn==null
			}// end critical section
		}// end Dispose


		#endregion


		/// <summary>
		/// open the tag
		/// </summary>
		/// <param name="curTag"></param>
		/// <param name="sectionVerbosity"></param>
		public void SectionOpen( string curTag, int sectionVerbosity)
		{
			if( sectionVerbosity >= this.verbosity)
			{// maximum verbosity leves is zero. Higher verbosity-levels prune the lower-level messages.
				lock( typeof( LogSinkDb.Library.SinkDb ))
				{// the lock avoids multiple file-creations
					try// this should happen due to interrupts during the execution of the current lock-block content
					{
                        if (null != this.tagStack //NB. it's null on failed db-connection, and throws.
                             )
                        {
                            // push the Tag anyway on the stack. It is necessary even if it's below this.verbosity.
                            // when closing the section, the verbosity will be checked to decide wether to write.
                            CurrentTag currentTag = new CurrentTag(curTag, sectionVerbosity);
                            this.tagStack.Push(currentTag);
                            this.stackDepth++;// another method enters the stack.
                        }
                        else
                            return;// stack empty
                        //
						if( hasPermissionsToWrite )
						{
							bool result = // TODO result==false??
								this.trace( this.TableName,// current TableName
									'o',// 'o'==open
									stackDepth,
									curTag,
									"___opening___"   );// no content in a opening
						}// otherwise, without write-permission, silently skip
					}
					catch( Exception ex)
					{
						this.operationException = ex.Message;
					}
				}// end lock
			}// otherwise just skip, non-required tracing
		}// end Open





		/// <summary>
		/// Trace content, from inside a tag. No stackDepth increase; we're inside an
		/// existing method.
		/// </summary>
		/// <param name="what">the tracing content</param>
		/// <param name="sectionVerbosity"></param>
		public void SectionTrace(string what, int sectionVerbosity)
		{
			if( sectionVerbosity >= this.verbosity)
			{// maximum verbosity leves is zero. Higher verbosity-levels prune the lower-level messages.
				lock( typeof( LogSinkDb.Library.SinkDb ))
				{
					try// silent log
					{
						if( hasPermissionsToWrite )
						{
                            string currentTagName = "unspecified";
                            if (0 < this.tagStack.Count)
                            {
                                // peek current section, to give a signature to the tracing.
                                CurrentTag currentTag = ((CurrentTag)this.tagStack.Peek());
                                currentTagName = currentTag.tagName;
                            }// else currentTagName defaults on "unspecified".
							bool result = // result==false on db-write failure.
								this.trace( this.TableName,// current TableName
									't',// 't'==trace
									this.stackDepth,// current stack depth.
                                    currentTagName,//functionName: currentTagName defaults on "unspecified".
									what   );// tracing content.
						}// endif. otherwise, without write-permission, silently skip
					}// end try
					catch( Exception ex)
					{
						this.operationException = ex.Message;
					}
				}// end lock
			}// otherwise just skip, non-required tracing
		}// end trace method




		/// <summary>
		/// close the tag that is on the stack top.
		/// </summary>
		public void SectionClose( )
		{
			CurrentTag currentTag = null;
			if( null !=  this.tagStack //NB. it's null on failed db-connection, and throws.
				&& 0 < this.tagStack.Count   )
			{
				//  pop the section. It will be written down only if the section verbosity is above this.verbosity.
				currentTag = ((CurrentTag)this.tagStack.Pop() );
			}
			else
				return;// stack empty
			//
			if( currentTag.sectionVerbosity >= this.verbosity)
			{// maximum verbosity leves is zero. Higher verbosity-levels prune the lower-level messages.
				lock( typeof( LogSinkDb.Library.SinkDb ))
				{
					try
					{
						if( hasPermissionsToWrite )
						{
							bool result = // TODO result==false??
								this.trace( this.TableName,// current TableName
									'c',// 'c'==close
									this.stackDepth--,// delete from the stack, after exiting.
									currentTag.tagName,//functionName,
									"___closing___"   );// no other content in a closure.
						}// otherwise, without write-permission, silently skip
					}// end try
					catch( Exception ex)
					{
						this.operationException = ex.Message;
					}
				}// end lock
			}// otherwise just skip, non-required tracing
		}// end trace method




		/// <summary>
		/// segue T-sql della called procedure:
		/// 
		/// SET QUOTED_IDENTIFIER ON 
		/// GO
		///	SET ANSI_NULLS ON 
		///	GO
        ///
        ///CREATE         procedure createLogTable
        ///    @logname char(50)
        ///as
        ///
        ///IF OBJECT_ID(@logname) IS NULL
        ///BEGIN
        ///
        ///declare @cmd varchar( 8000)
        ///
        ///    SET @cmd = '
        ///            CREATE TABLE Logging..'+@logname+' (
        ///
        ///    [id] [int] IDENTITY (1, 1) NOT NULL ,
        ///    [when] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        ///    [row_nature] char(3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        ///    [stack_depth] varchar(5) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        ///    [function_name] varchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        ///    [content] varchar(7919) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        ///    PRIMARY KEY  CLUSTERED
        ///    (
        ///        [id]
        ///    )  ON [PRIMARY] 
        ///    ) ON [PRIMARY]'
        ///-- cmd text ready
        ///    exec (@cmd)
        ///END
        ///-- else table already exists
		///
		/// GO
		/// SET QUOTED_IDENTIFIER OFF 
		/// GO
		/// SET ANSI_NULLS ON 
		/// GO
		///
		/// </summary>
		/// <param name="tableName">the dynamic table name; each application writes on its own table.</param>
		private bool createTable( string tableName)
		{
			bool result = false;// init to invalid
			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
			cmd.Connection = DbLayer.ConnectionManager.getCryptedConnection( "LogSinkDb/connection");
            if (null == cmd.Connection || System.Data.ConnectionState.Open != cmd.Connection.State)
            {
                return false;// no connection->no db-writing. 
            }// else connection is OK->go on.
            // connection policy is "function-wide;no persistency."
			System.Data.SqlClient.SqlParameter tableNamePar = new System.Data.SqlClient.SqlParameter();
			tableNamePar.Direction = System.Data.ParameterDirection.Input;
			tableNamePar.DbType = System.Data.DbType.String;
			tableNamePar.ParameterName = "@logname";
			tableNamePar.Value = tableName;

			cmd.Parameters.Add( tableNamePar);

			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			cmd.CommandText = "createLogTable";
            try
            {
				int rowsAffected = cmd.ExecuteNonQuery();
				result = true;// ok
			}
			catch( System.Exception ex)
			{
				string exc = ex.Message;// debug
				result = false;
			}
			finally
            {// connection policy is "function-wide;no persistency."
                if (System.Data.ConnectionState.Open == cmd.Connection.State )
                {
                    cmd.Connection.Close();
                }// else no need to close
			}
			// ready
			return result;
		}// end CreateTable




		/// <summary>
		/// segue T-sql della called procedure:
		/// 
		///		SET QUOTED_IDENTIFIER ON
		/// 	GO
		/// 	SET ANSI_NULLS ON 
		/// 	GO
		/// 	CREATE         procedure trace
        ///            @logname char(50),
        ///            @when varchar(50),
        ///            @row_nature char(3),
        ///            @stack_depth varchar(5),
        ///            @function_name varchar(50),
        ///            @content varchar(7971)
        ///      as
        ///        declare @cmd char(8000)
        ///        IF OBJECT_ID('Logging..'+@logname) IS NOT NULL
        ///        BEGIN
        ///    SET @cmd = 'insert into Logging..'+
        ///    @logname+'(
        ///    [when],
        ///    [row_nature],
        ///    [stack_depth],
        ///    [function_name],
        ///    [content] 	  ) values('+
        ///        @when+', '+
        ///        @row_nature+', '+
        ///        @stack_depth+', '+
        ///        @function_name+', '+
        ///        @content+  ' )'
        /// -- print @cmd  debug only
        /// exec (@cmd)
        /// end
        ///     /*
        ///         else
        ///         begin
        ///         -- required table not found -> do nothing
        ///         END
        ///     */
		/// 		
		/// GO
		/// SET QUOTED_IDENTIFIER OFF 
		/// GO
		/// SET ANSI_NULLS ON 
		/// GO
		/// </suGOmmary>
		/// <param name="tableName">the dynamic table name; each application writes on its own table.</param>
		/// <param name="tracingContent">the dynamic content.</param>
		private bool trace(	string	tableName,
							char	row_nature,
							int		stack_depth,
							string	function_name,
							string	tracingContent	)
		{
			bool result = false;// init to invalid
			System.Data.SqlClient.SqlCommand cmd = new System.Data.SqlClient.SqlCommand();
            cmd.Connection = DbLayer.ConnectionManager.getCryptedConnection("LogSinkDb/connection");
            if (null == cmd.Connection || System.Data.ConnectionState.Open != cmd.Connection.State)
            {
                return false;// no connection->no db-writing. 
            }// else connection is OK->go on.
            // connection policy is "function-wide;no persistency."
			/*
				@logname char(50),
				@row_nature char(3),
				@stack_depth varchar(5),
				@function_name varchar(50),
				@content varchar(7971)
			 * */

			System.Data.SqlClient.SqlParameter tableNamePar = new System.Data.SqlClient.SqlParameter();
			tableNamePar.Direction = System.Data.ParameterDirection.Input;
			tableNamePar.DbType = System.Data.DbType.String;
			tableNamePar.ParameterName = "@logname";
			tableNamePar.Value = tableName;

            System.Data.SqlClient.SqlParameter whenPar = new System.Data.SqlClient.SqlParameter();
            whenPar.Direction = System.Data.ParameterDirection.Input;
            whenPar.DbType = System.Data.DbType.String;
            whenPar.ParameterName = "@when";
            string month = (DateTime.Now.Month  < 10) ? ("0" + DateTime.Now.Month.ToString()  ) : (DateTime.Now.Month.ToString()  );
            string   day = (DateTime.Now.Day    < 10) ? ("0" + DateTime.Now.Day.ToString()    ) : (DateTime.Now.Day.ToString()    );
            string hour  = (DateTime.Now.Hour   < 10) ? ("0" + DateTime.Now.Hour.ToString()   ) : (DateTime.Now.Hour.ToString()   );
            string min   = (DateTime.Now.Minute < 10) ? ("0" + DateTime.Now.Minute.ToString() ) : (DateTime.Now.Minute.ToString() );
            string sec   = (DateTime.Now.Second < 10) ? ("0" + DateTime.Now.Second.ToString() ) : (DateTime.Now.Second.ToString() );
			whenPar.Value =
				DateTime.Now.Year.ToString() +
				month +
				day +
				hour +
				min +
				sec;

			System.Data.SqlClient.SqlParameter rowNaturePar = new System.Data.SqlClient.SqlParameter();
			rowNaturePar.Direction = System.Data.ParameterDirection.Input;
			rowNaturePar.DbType = System.Data.DbType.String;
			rowNaturePar.ParameterName = "@row_nature";
			rowNaturePar.Value = "'"+row_nature+"'";

			System.Data.SqlClient.SqlParameter stackDepthPar = new System.Data.SqlClient.SqlParameter();
			stackDepthPar.Direction = System.Data.ParameterDirection.Input;
			stackDepthPar.DbType = System.Data.DbType.String;//NB. dynamic sql e' solo stringa. I tipi numerici vanno resi literals(i.e. costanti di testo).
			stackDepthPar.ParameterName = "@stack_depth";
			stackDepthPar.Value = "'"+stack_depth.ToString()+"'";

			System.Data.SqlClient.SqlParameter functionNamePar = new System.Data.SqlClient.SqlParameter();
			functionNamePar.Direction = System.Data.ParameterDirection.Input;
			functionNamePar.DbType = System.Data.DbType.String;
			functionNamePar.ParameterName = "@function_name";
			functionNamePar.Value = "'"+function_name+"'";

			System.Data.SqlClient.SqlParameter tracingContentPar = new System.Data.SqlClient.SqlParameter();
			tracingContentPar.Direction = System.Data.ParameterDirection.Input;
			tracingContentPar.DbType = System.Data.DbType.String;
			tracingContentPar.ParameterName = "@content";
			tracingContentPar.Value = "'"+tracingContent+"'";

			cmd.Parameters.Add( tableNamePar);
            cmd.Parameters.Add( whenPar);
			cmd.Parameters.Add( rowNaturePar);
			cmd.Parameters.Add( stackDepthPar);
			cmd.Parameters.Add( functionNamePar);
			cmd.Parameters.Add( tracingContentPar);

			cmd.CommandType = System.Data.CommandType.StoredProcedure;
			cmd.CommandText = "trace";
            try
            {
                int rowsAffected = cmd.ExecuteNonQuery();
                result = true;// ok
            }
            catch (System.Exception ex)
            {
                string exc = ex.Message;// debug
                result = false;
            }
            finally
            {// connection policy is "function-wide;no persistency."
                if (System.Data.ConnectionState.Open == cmd.Connection.State)
                {
                    cmd.Connection.Close();
                }// else no need to close
            }
			// ready
			return result;
		}// end Trace



		public string apiceFilter( string par)
		{
			System.Text.StringBuilder result = new System.Text.StringBuilder( par.Length);
			for( int c=0; c<par.Length; c++)
			{
				if( 39 != par[c] )// 39==''' i.e. the char '
				{
					result.Append( par[c]);
				}//	ex else skip the '
                else
                {
                    result.Append('\'');// double it, as SQL92 requires.
                    result.Append('\'');
                    //// NB. old & discarded.
                    //result.Append('_');
                }
			}
			return result.ToString();
		}



	}// end class SinkDb

}// end nmsp
