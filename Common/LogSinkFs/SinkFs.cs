using System;
using System.IO;


namespace LogSinkFs.Library
{

    /// <summary>
    /// Sink File System. Stream Volatile. Open-Write-Close. No handle hung.
    /// Log rotation: one file per day.
    /// </summary>
    public class SinkFs : System.IDisposable
    {
        # region data

        private StreamWriter where = null;// the stream to log on
        private string fName = "";
        private System.Collections.Stack tagStack = null;
        private int indentmentLevel = 0;
        private bool hasPermissionsToWrite;
        private int semaphore;
        private int verbosity;
        private string constructorException = "not yet initialized";
        private string operationException = "not yet initialized";
        private struct CurrentTag
        {
            public string tagName;
            public int sectionVerbosity;
            // Ctor
            public CurrentTag(string tagName, int sectionVerbosity)
            {
                this.tagName = tagName;
                this.sectionVerbosity = sectionVerbosity;
            }// end Ctor
        }// end struct


        /// <summary>
        /// read only property.
        /// strings are arrays, but are returned by copy.
        /// </summary>
        /// <returns></returns>
        public string GetCurrentLogName()
        {
            return this.fName;
        }//

        # endregion data





        /// <summary>
        /// Constructor
        /// </summary>
        public SinkFs()
        {
            lock (typeof(SinkFs))
            {// the lock avoids multiple file-creations
                try// this should happen due to interrupts during the execution of the current lock-block content
                {
                    // read configuration

                    ConfigurationLayer.ConfigurationService cs = new
                        ConfigurationLayer.ConfigurationService("LogSinkFs/LogSinkFsConfig");
                    string logFullPathPrefix = cs.GetStringValue("path");
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
                    string verbosity = cs.GetStringValue("verbosity");// <!-- threshold above which tracings are considered -->
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
                        // prepare log-stream
                        // desinenza dinamica per log rotation.
                        string dynamicEnding = DateTime.Now.ToShortDateString();
                        dynamicEnding = dynamicEnding.Replace('\\', '_');
                        dynamicEnding = dynamicEnding.Replace('/', '_');
                        fName = logFullPathPrefix + "LogStream_" + dynamicEnding + "_.log";
                        // tryOpen here: on fail throws
                        tryOpen();
                        where.WriteLine("\r\n £=start, $=content, ^=close -=separator\r\n");// legend
                        where.Close();// volatile connection -> close at once and reopen when required.
                    }// end else // green semaphore
                }// end try
                catch (Exception ex)
                {// if this throws -> hasPermissionsToWrite=false
                    this.constructorException = ex.Message;
                    hasPermissionsToWrite = false;
                }
            }// end lock
        }//end constructor




        /// <summary>
        /// on fail throws
        /// </summary>
        private void tryOpen()
        {
            lock (typeof(SinkFs))
            {// the lock avoids multiple file-creations
                // tryOpen
                if (System.IO.File.Exists(fName)) // file exists -> Append
                {
                    where = new StreamWriter(
                        fName,
                        true, // Append
                        System.Text.Encoding.Default,
                        20 * 1024  // 20k supposed average buffer size. Feel free of tuning it.
                        );
                    hasPermissionsToWrite = true;// on throw caller sets it to false
                }// endif file exists -> Append
                else // file does NOT exist -> Create
                {
                    where = new StreamWriter(
                        fName,
                        false, // Append
                        System.Text.Encoding.Default,
                        20 * 1024  // 20k supposed average buffer size. Feel free of tuning it.
                        );
                    hasPermissionsToWrite = true;// on throw caller sets it to false
                } // endif file does NOT exist -> Create
            }// end lock
        }// end tryOpen






        /// <summary>
        /// open the tag
        /// </summary>
        /// <param name="curTag"></param>
        /// <param name="sectionVerbosity"></param>
        public void SectionOpen(string curTag, int sectionVerbosity)
        {
            if (sectionVerbosity >= this.verbosity)
            {// maximum verbosity leves is zero. Higher verbosity-levels prune the lower-level messages.
                lock (typeof(SinkFs))
                {// the lock avoids multiple file-creations
                    try// this should happen due to interrupts during the execution of the current lock-block content
                    {
                        CurrentTag currentTag;
                        if (null != this.tagStack)
                        {
                            // push the Tag anyway on the stack. It is necessary even if it's below this.verbosity.
                            // when closing the section, the verbosity will be checked to decide wether to write.
                            currentTag = new CurrentTag(curTag, sectionVerbosity);
                            this.tagStack.Push(currentTag);
                        }
                        else
                            return;// skip writing when no stack has been prepared
                        //
                        if (hasPermissionsToWrite)
                        {
                            tryOpen();// volatile connection -> close at once and reopen when required.
                            ++indentmentLevel;
                            where.Write("-\r\n£");
                            indent();
                            where.WriteLine(((CurrentTag)this.tagStack.Peek()).tagName + "_______start");
                            where.Flush();
                            ++indentmentLevel;
                            where.Close();// volatile connection -> close at once and reopen when required.
                        }// otherwise, without write-permission, silently skip
                    }
                    catch (Exception ex)
                    {
                        this.operationException = ex.Message;
                    }
                }// end lock
            }// otherwise just skip, non-required tracing
        }//end constructor


        /// <summary>
        /// trace content, from inside a tag
        /// </summary>
        /// <param name="what"></param>
        /// <param name="sectionVerbosity"></param>
        public void SectionTrace(string what, int sectionVerbosity)
        {
            if (sectionVerbosity >= this.verbosity)
            {// maximum verbosity leves is zero. Higher verbosity-levels prune the lower-level messages.
                lock (typeof(SinkFs))
                {// the lock avoids multiple file-creations
                    try// this should happen due to interrupts during the execution of the current lock-block content
                    {
                        CurrentTag currentTag;
                        if (null != this.tagStack
                            && 0 < this.tagStack.Count)
                        {
                            // peek current section, to give a signature to the tracing.
                            currentTag = ((CurrentTag)this.tagStack.Peek());
                        }
                        else// chiamata alla sola content, senza open e/o close.
                        {
                            currentTag = new CurrentTag("unspecified", 0);// persa obbligatorieta'.
                        }
                        //
                        if (hasPermissionsToWrite)
                        {
                            tryOpen();// volatile connection -> close at once and reopen when required.
                            where.Write("$");
                            indent();
							string month = (DateTime.Now.Month  < 10) ? ("0" + DateTime.Now.Month.ToString()  ) : (DateTime.Now.Month.ToString()  );
							string   day = (DateTime.Now.Day    < 10) ? ("0" + DateTime.Now.Day.ToString()    ) : (DateTime.Now.Day.ToString()    );
							string hour =  (DateTime.Now.Hour   < 10) ? ("0" + DateTime.Now.Hour.ToString()   ) : (DateTime.Now.Hour.ToString()   );
							string min  =  (DateTime.Now.Minute < 10) ? ("0" + DateTime.Now.Minute.ToString() ) : (DateTime.Now.Minute.ToString() );
							string sec  =  (DateTime.Now.Second < 10) ? ("0" + DateTime.Now.Second.ToString() ) : (DateTime.Now.Second.ToString() );
							string timestamp =
								DateTime.Now.Year.ToString() +"_"+
								month +"_"+
								day +"_"+
								hour +"_"+
								min +"_"+
								sec;
                            where.WriteLine("inside " + currentTag.tagName + ": "+ timestamp+" "+ what);
                            where.Flush();
                            where.Close();// volatile connection -> close at once and reopen when required.
                        }// endif. otherwise, without write-permission, silently skip
                    }// end try
                    catch (Exception ex)
                    {
                        this.operationException = ex.Message;
                    }
                }// end lock
            }// otherwise just skip, non-required tracing
        }// end trace method


        /// <summary>
        /// close the tag that is on the stack top.
        /// </summary>
        public void SectionClose()
        {
            CurrentTag currentTag;
            if (null != this.tagStack
                && 0 < this.tagStack.Count)
            {
                //  pop the section. It will be written down only if the section verbosity is above this.verbosity.
                currentTag = ((CurrentTag)this.tagStack.Pop());
            }
            else
                return;// skip writing when no stack has been prepared
            //
            if (currentTag.sectionVerbosity >= this.verbosity)
            {// maximum verbosity leves is zero. Higher verbosity-levels prune the lower-level messages.
                lock (typeof(SinkFs))
                {// the lock avoids multiple file-creations
                    try// this should happen due to interrupts during the execution of the current lock-block content
                    {
                        if (hasPermissionsToWrite)
                        {
                            --indentmentLevel;
                            tryOpen();// volatile connection -> close at once and reopen when required.
                            where.Write("^");
                            indent();
                            where.WriteLine("closing____" + currentTag.tagName + "_______end");
                            where.Flush();
                            where.Close();// volatile connection -> close at once and reopen when required.
                            --indentmentLevel;
                        }// otherwise, without write-permission, silently skip
                    }// end try
                    catch (Exception ex)
                    {
                        this.operationException = ex.Message;
                    }
                }// end lock
            }// otherwise just skip, non-required tracing
        }// end trace method




        /// <summary>
        /// technical private method.
        /// </summary>
        private void indent()
        {
            lock (typeof(SinkFs))
            {// the lock avoids multiple file-creations
                try// this should happen due to interrupts during the execution of the current lock-block content
                {
                    if (hasPermissionsToWrite)
                    {
                        for (int c = 0; c < indentmentLevel; c++)
                        {
                            where.Write("   ");
                        }
                    }// otherwise, without write-permission, silently skip
                }
                catch (Exception ex)
                {
                    this.operationException = ex.Message;
                }
            }// end lock
        }// end indent



        #region IDisposable Members

        public void Dispose()
        {
            lock (typeof(LogSinkFs.Library.SinkFs))
            {
                if (hasPermissionsToWrite)
                {
                    try
                    {
                        if (null != where)
                        {
                            where.Close();
                            where = null;
                        }// endif
                    }// end try block
                    catch (System.Exception ex)
                    {
                        this.operationException = ex.Message;
                    }
                }
            }// end critical section
        }// end Dispose

        #endregion
    }// end class SinkFs


}// end namespace LogLibrary
