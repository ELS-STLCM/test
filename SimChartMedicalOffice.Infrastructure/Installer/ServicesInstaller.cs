﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.MicroKernel.SubSystems.Configuration;
using SimChartMedicalOffice.ApplicationServices;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface;
using Castle.Core;
using SimChartMedicalOffice.Infrastructure.Aspects;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.TempObject;
using SimChartMedicalOffice.ApplicationServices.TempObject;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Forms;
using SimChartMedicalOffice.ApplicationServices.Forms;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Builder;
using SimChartMedicalOffice.ApplicationServices.Builder;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.Competency;
using SimChartMedicalOffice.ApplicationServices.Competency;
using SimChartMedicalOffice.ApplicationServices.ApplicationServiceInterface.FrontOffice;
using SimChartMedicalOffice.ApplicationServices.FrontOffice;
//using SimChartMedicalOffice.ApplicationServices.Aspects;

namespace SimChartMedicalOffice.Infrastructure.Installer
{
    public class ServicesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {            
            container.Register(
                Component.For<ServiceInterceptor>().LifeStyle.Singleton,
                Component.For<TestService>()
                .ImplementedBy<TestService>()
                .LifeStyle.Singleton
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<ITestService>()
                .ImplementedBy<Test2Service>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<IRegistrationService>()
                .ImplementedBy<RegistrationService>(),
                Component.For<IFormsService>()
                .ImplementedBy<FormsService>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<IQuestionBankService>()
                .ImplementedBy<QuestionBankService>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<ICompetencyService>()
                    .ImplementedBy<CompetencyService>()
                    .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                    Component.For<IPatientService>()
                .ImplementedBy<PatientService>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<ISkillSetService>()
                .ImplementedBy<SkillSetService>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<IAssignmentService>()
                .ImplementedBy<AssignmentService>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,
                Component.For<IAppointmentService>().ImplementedBy<AppointmentService>().Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First,

                Component.For<IMasterService>()
                .ImplementedBy<MasterService>()
                .Interceptors(new InterceptorReference(typeof(ServiceInterceptor))).First
                    
               );
        }
    }
}
