using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using ElleFramework.Utils.Utility;

namespace ElleFramework.Utils
{
    public class Tracer
    {
        private static readonly string sourceName;
        private static readonly bool useTrace;

        static Tracer()
        {
            Config conf = ApplicationUtils.Configuration; 
            sourceName = conf.Logging.TraceSourceName;
            useTrace = conf.Logging.UseTrace;

            if (string.IsNullOrWhiteSpace(sourceName))
            {
                Assembly asm = Assembly.GetEntryAssembly();

                if (asm != null)
                    sourceName = asm.GetName(false).Name;
                else
                    sourceName = "SourceNotFound";
            }
        }

        private Tracer()
        {
        }

        public static bool TraceAppLog(string msg, Exception ex)
        {
            StringBuilder str = new StringBuilder(msg);

            str.Append("\n\n");

            while (ex != null)
            {

                str.Append(ex.Message).Append("\n").Append(ex.StackTrace).Append("\n\n");

                ex = ex.InnerException;
            }

            return TraceAppLog(str.ToString(), EventLogEntryType.Error);
        }

        public static bool TraceAppLog(string msgToTrace, EventLogEntryType logType)
        {
            bool ret = true;

            try
            {
                if (useTrace)
                {
                    Trace.WriteLine("");
                    Trace.WriteLine(logType + ":");
                    Trace.WriteLine(msgToTrace);
                    Trace.WriteLine("");
                }

                using (EventLog log = new EventLog { Source = sourceName })
                    log.WriteEntry(msgToTrace, logType);
            }
            catch
            {
                ret = false;
            }

            return ret;
        }


        public static void TraceFS(string msgToTrace)
        {
            TraceFS(msgToTrace, "", 0);
        }

        public static void TraceFS(string msgToTrace, int lvl)
        {
            TraceFS(msgToTrace, "", lvl);
        }

        public static void TraceFS(string msgToTrace, string categoryLog)
        {
            TraceFS(msgToTrace, categoryLog, 0);
        }

        public static void TraceFS(string msgToTrace, string categoryLog, int lvl)
        {
            using (Process process = Process.GetCurrentProcess())
            {
                string name = process.ProcessName.Trim();

                using (TextWriterTraceListener listener = new TextWriterTraceListener(name + ".log", name))
                {
                    string dataString = DateTime.Now.ToString("u");

                    Trace.Listeners.Add(listener);

                    if (lvl > 0)
                        Trace.IndentLevel += lvl;

                    if (string.IsNullOrWhiteSpace(categoryLog))
                        Trace.WriteLine(dataString + " - " + msgToTrace);
                    else
                        Trace.WriteLine(dataString + " - " + msgToTrace, categoryLog);

                    Trace.Flush();

                    if (lvl > 0)
                        Trace.IndentLevel -= lvl;

                    Trace.Listeners.Remove(listener);
                    listener.Close();
                }
            }
        }
    }
}
