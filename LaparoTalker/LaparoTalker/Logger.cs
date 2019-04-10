//#define EXTENDED_LOG

using System;
using System.IO;
using System.Reflection;

namespace LaparoTalker
{

    public class Logger
    {
        private string m_exePath = string.Empty;
        public Logger() {}
        public void LogWrite(string logMessage)
        {
            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + "log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
#if EXTENDED_LOG
                txtWriter.WriteLine("{0} {1}: {2}", DateTime.Now.ToLongTimeString(),DateTime.Now.ToShortDateString(), logMessage);
#else
                txtWriter.WriteLine("{0}", logMessage);
#endif
            }
            catch (Exception ex)
            {
            }
        }
    }

}
