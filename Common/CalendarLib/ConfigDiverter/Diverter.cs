using System;


namespace Common.CalendarLib.ConfigDiverter
{


    public static class Diverter
    {


        public static bool DiverterMethod(
            string NationName,
            out string Configuration  )// NB.  out
        {
            bool res = false;
            Configuration = null;// NB.  out.  init -> valorizzazione specifica
            try
            {
                // try in config
                System.Collections.Specialized.NameValueCollection calendarModels =
                    ConfigurationLayer2008.CustomSectionInOneShot.GetCustomSectionInOneShot(
                        "CalendarVariety/holidayList");
                if (null == calendarModels)
                {
                    res = false;
                    throw new System.Exception("ERROR in retrieving \"CalendarVariety/holidayList\" config section.");
                }// else ok
                else
                {
                    Configuration = calendarModels[ NationName];// TODO test onWrongName
                }
                res = true;
            }
            catch( System.Exception ex)
            {
                string dbg = ex.Message +" __ "+ ex.StackTrace;
                //
                // if ok res should be true by now, else
                if (!res)
                {
                    res = DbActions.ExtractNation.ExtractionService(
                        NationName
                        , out Configuration);// NB. out
                }// else already ok.
            }
            finally
            {
            }
            // ready
            return res;
        }// end DiverterMethod


    }// end class


}// end nmsp
