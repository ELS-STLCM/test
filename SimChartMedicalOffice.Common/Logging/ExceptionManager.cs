using System;

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
        static readonly Log4NetLogger LoggingHandler = new Log4NetLogger();
        public static void Error(Exception applicationException)
        {
            LoggingHandler.Error(applicationException);
        }
        public static void Error(string information,Exception applicationException)
        {
            LoggingHandler.Error(information,applicationException);
        }
        public static void Fatal(Exception applicationException)
        {
            LoggingHandler.Fatal(applicationException);
        }
        public static void Info(string message)
        {
            LoggingHandler.Error(message);
        }
    }
}
