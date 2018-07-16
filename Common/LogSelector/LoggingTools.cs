using System;
using System.Text;

namespace LoggingToolsContainerNamespace
{

    public class LoggingToolsContainer
    {


        /// <summary>
        /// If the user wants to call such an utility from inside the catch block of the proxies,
        /// it must be implemented in a project referenced by the proxies project. The actual calls
        /// to the logging engines( both DbSink and FileSystemSink) are examples only.
        /// The user has to provide an actual logging engine, such as ACA or EntLib.
        /// Static utility that decides where to log( sinkFs or Db) based on what kind of exception
        /// occurred.
        /// If the exception is a sql-exception, due to TCP transport fault, FileSystem is choosen,
        /// since DbSink is unavailable.
        /// When the exception nature indicates no risk of db unavaliability, SinkDb is the 
        /// preferred channel.
        /// </summary>
        /// <param name="originalExceptionType">used to discriminate between exception categories</param>
        /// <param name="wholeMessage">Commonly the original exception Message field, plus a custom part.</param>
        ///<returns>
        /// an integer, used by the Entity.DataAccess.Proxies as the operation-result-code.
        /// 
        ///    -1  no connection
        ///    0   ok
        ///    1   sqlException : duplicated key
        ///    2   sqlException : other than duplicated key
        ///    3   NON sql exception
        ///    4   logical fault without exception
        ///    ...
        ///    >4  other exceptions TODO:  customize adding specific events You want to consider.
        ///</returns>
        public static int DecideAndLog(
            System.Exception originalExceptionType,
            string wholeMessage,
            int verbosity
            )
        {
            int result = -1;// default
            //---------------------exception nature discrimination----------------------
            if (null != originalExceptionType)
            {
                if (originalExceptionType is System.Data.SqlClient.SqlException)
                {/* no more databese usage on sqlException, if and only iff sqlException.subtype
                  * is TCP-transport-type. */
                    if (2627 == ((System.Data.SqlClient.SqlException)originalExceptionType).Number)// chiave duplicata e simili
                    {
                        result = 1;// duplicated key-> db still reachable
                        LogSinkDataBase( wholeMessage, verbosity);// can log on db.
                    }
                    else if(547 == ((System.Data.SqlClient.SqlException)originalExceptionType).Number)// chiave duplicata e simili
                    {
                        result = 1;// duplicated key-> db still reachable
                        LogSinkDataBase( wholeMessage, verbosity);// can log on db.
                    }
                    /* TODO   customize adding specific SqlException.Number You want to consider.
                     *      add here:
                     *   else if( 
                     *      xxxx==((System.Data.SqlClient.SqlException)originalExceptionType).Number)
                     *      // reason for sql-exception non-transport type ->  db still reachable
                     *   then...LogSinkDataBase(wholeMessage);// // can log on db.
                     */
                    else// sql exception kind TRANSPORT-FAULT -> connection unreachable
                    {
                        result = 2;// sql exception, other than duplicated key.
                        // connection unreachable -> cannot log on sink-db -> log on sink fileSystem.
                        LogSinkFileSystem( wholeMessage, verbosity);// FileSystem log, eg. on ftp virtual-dir.
                    }// will be easy to read production-server-logs, via ftp.
                }
                else// non-sql exception -> no reason to think db is unavailable.
                {
                    result = 3;// NON sql exception
                    LogSinkDataBase( wholeMessage, verbosity);
                }
            }
            else// no exception -> no reason to think db is unavailable.
            {
                LogSinkDataBase(wholeMessage, verbosity);
            }
            // ready
            return result;
        }// end DecideAndLog



        public static void LogAllSinks(
            string wholeMessage,
            int verbosity
            )
        {
            LogSinkDataBase(wholeMessage, verbosity);
            LogSinkFileSystem(wholeMessage, verbosity);
            System.Console.WriteLine(wholeMessage);
        }// end LogAllSinks


        public static void LogBothSinks_DbFs(
            string wholeMessage,
            int verbosity
            )
        {
            LogSinkDataBase(wholeMessage, verbosity);
            LogSinkFileSystem(wholeMessage, verbosity);
            // no console.
        }// end LogAllSinks



        public static void LogSinkDataBase(string wholeMessage, int verbosity)
        {
            LogSinkDb.Wrappers.LogWrappers.SectionContent(
                wholeMessage,
                verbosity
            );
        }

        public static void LogSinkFileSystem( string wholeMessage, int verbosity)
        {
            LogSinkFs.Wrappers.LogWrappers.SectionContent(
                wholeMessage,
                verbosity
            );
        }


    }// end class


}// end nmsp
