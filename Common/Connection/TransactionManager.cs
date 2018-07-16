using System;


namespace Common.Connection
{


    public static class TransactionManager
    {


        public static System.Data.SqlClient.SqlTransaction trxOpener(
            string sectionGroup_section,// compulsory xpath for ProxyGenerator: "ProxyGeneratorConnections/strings"
            string keyName // eg. "ProcBBT_app"
          )
        {
            //-------START---------------- blocchi open-close transazione -------------
            //---transazione-----
            System.Data.SqlClient.SqlTransaction trx = null;
            System.Data.SqlClient.SqlConnection conn =
                DbLayer.ConnectionManager.connectWithCustomSingleXpath(
                    sectionGroup_section,
                    keyName
                );
            if (null != conn)
                trx = conn.BeginTransaction();
            else
                trx = null;// no db connection.
            //--END--open transazione-----
            // ready
            return trx;
        }//




        public static void trxCloser(
            System.Data.SqlClient.SqlTransaction trx,
            bool mustCommit // the ultimate decision.
          )
        {
            if( mustCommit)// success
            {
                trx.Commit();
            }
            else// failure
            {
                if (null != trx)
                {
                    trx.Rollback();
                }// else means the the trxOpener was unable to connect.
            }
            if (null != trx)
            {
                if (null != trx.Connection)
                {
                    if (System.Data.ConnectionState.Open == trx.Connection.State)
                    {
                        trx.Connection.Close();
                    }// else aready closed.
                }//else Commit or Rollback succeeded and let trx.Connection==null.
            }// else means the the trxOpener was unable to connect.
            //--END--close transazione------
            trx = null;// anyway garbage collect.
            // ready
        }// end trxCloser


    }//
}//
