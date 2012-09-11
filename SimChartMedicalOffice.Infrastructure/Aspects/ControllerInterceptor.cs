using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace SimChartMedicalOffice.Infrastructure.Aspects
{
    public class ControllerInterceptor:IInterceptor
    {
        //public bool EatAll { get; set; }
        public void Intercept(IInvocation invocation)
        {
            switch (invocation.Method.Name)
            {
                case "OnActionExecuting":
                    OnActionExecuting(invocation);
                    break;
                case "OnActionExecuted":
                    OnActionExecuted(invocation);
                    break;
            }
            invocation.Proceed();
        }
        private void OnActionExecuted(IInvocation invocation)
        {

        }

        private void OnActionExecuting(IInvocation invocation)
        {

        }
    }
}
