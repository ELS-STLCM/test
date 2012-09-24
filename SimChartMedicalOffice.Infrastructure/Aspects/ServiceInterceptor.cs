using System;
using System.Diagnostics;
using Castle.DynamicProxy;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Logging;

namespace SimChartMedicalOffice.Infrastructure.Aspects
{
    public class ServiceInterceptor : IInterceptor
    {
        //public bool EatAll { get; set; }
        public void Intercept(IInvocation invocation)
        {           
            //HttpContext.Current.Request.Cookies
            string environment = AppCommon.GetAppSettingValue("Environment");
            Stopwatch sw = Stopwatch.StartNew();
            if (environment == "DEV")
            {
                sw = Stopwatch.StartNew();
                ExceptionManager.Info(invocation.TargetType.FullName + "." + invocation.Method.Name +
                                      " - Start, StartTime: " + DateTime.Now.ToString());
            }
            OnActionExecuting(invocation);
            invocation.Proceed();
            OnActionExecuted(invocation);
            if (environment == "DEV")
            {
                sw.Stop();
                ExceptionManager.Info(invocation.TargetType.FullName + "." + invocation.Method.Name +
                                      " - End, EndTime: " + DateTime.Now.ToString() + ", Elapsed Time: " +
                                      sw.Elapsed.ToString());
            }
        }
        private void OnActionExecuted(IInvocation invocation)
        {

        }

        private void OnActionExecuting(IInvocation invocation)
        {

        }
    }
}
