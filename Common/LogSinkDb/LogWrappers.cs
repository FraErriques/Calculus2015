using System;

namespace LogSinkDb.Wrappers
{
	/// <summary>
	/// Summary description for Wrappers.
	/// </summary>
	public class LogWrappers
	{
		private LogWrappers()
		{// functor for wrappers. No instance allowed.
		}

		//
		public static void SectionOpen( string name, int sectionVerbosity)
		{
            Common.Template_Singleton.TSingleton<LogSinkDb.Library.SinkDb>.instance().SectionOpen(
                Common.Template_Singleton.TSingleton<LogSinkDb.Library.SinkDb>.instance().apiceFilter(name),
                sectionVerbosity);

            //LogSinkDb.Library.Singleton.instance().SectionOpen(
            //    LogSinkDb.Library.Singleton.instance().apiceFilter( name),
            //    sectionVerbosity);
		}
		//
		public static void SectionContent( string content, int sectionVerbosity)
		{
            Common.Template_Singleton.TSingleton<LogSinkDb.Library.SinkDb>.instance().SectionTrace(
                Common.Template_Singleton.TSingleton<LogSinkDb.Library.SinkDb>.instance().apiceFilter(content),
                sectionVerbosity);

            //LogSinkDb.Library.Singleton.instance().SectionTrace(
            //    LogSinkDb.Library.Singleton.instance().apiceFilter( content),
            //    sectionVerbosity);
		}
		//
		public static void SectionClose( )
		{
            Common.Template_Singleton.TSingleton<LogSinkDb.Library.SinkDb>.instance().SectionClose();

			//LogSinkDb.Library.Singleton.instance().SectionClose( );
		}
		//
	}// end class Wrappers


}// end namespace LogSinkDb.Library
