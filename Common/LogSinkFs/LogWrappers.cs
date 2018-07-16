

namespace LogSinkFs.Wrappers
{

	public class LogWrappers
	{
		//
		public static void SectionOpen( string name, int sectionVerbosity)
		{
            Common.Template_Singleton.TSingleton<LogSinkFs.Library.SinkFs>.instance().SectionOpen(name, sectionVerbosity);
			//LogSinkFs.Library.Singleton.instance().SectionOpen( name, sectionVerbosity);
		}
		//
		public static void SectionContent( string content, int sectionVerbosity)
		{
            Common.Template_Singleton.TSingleton<LogSinkFs.Library.SinkFs>.instance().SectionTrace(content, sectionVerbosity);
            //LogSinkFs.Library.Singleton.instance().SectionTrace(content, sectionVerbosity);
		}
		//
		public static void SectionClose( )
		{
            Common.Template_Singleton.TSingleton<LogSinkFs.Library.SinkFs>.instance().SectionClose();
            //LogSinkFs.Library.Singleton.instance().SectionClose();
		}
		//
	}// end class LogWrappers


}// end namespace LogService
