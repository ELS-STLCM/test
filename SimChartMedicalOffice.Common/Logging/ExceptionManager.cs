using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Common.Logging
{
    public static class ExceptionManager
    {
        public enum ExceptionType
        {
            Info=1,
            Warn=2,
            Error=3
        }
        static Log4NetLogger loggingHandler = new Log4NetLogger();
        public static void Error(Exception applicationException)
        {
            loggingHandler.Error(applicationException);
        }
        public static void Error(string information,Exception applicationException)
        {
            loggingHandler.Error(information,applicationException);
        }
        public static void Fatal(Exception applicationException)
        {
            loggingHandler.Fatal(applicationException);
        }
        public static void Info(string message)
        {
            loggingHandler.Error(message);
        }
    }
}
