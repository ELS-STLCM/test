using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SimChartMedicalOffice.Core.DataInterfaces;
using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.Authoring;
using SimChartMedicalOffice.Core.DataInterfaces.Competency;
using SimChartMedicalOffice.Core.DataInterfaces.Forms;
using SimChartMedicalOffice.Core.DataInterfaces.FrontOffice;
using SimChartMedicalOffice.Core.DataInterfaces.Patient;
using SimChartMedicalOffice.Core.DataInterfaces.QuestionBanks;
using SimChartMedicalOffice.Core.DataInterfaces.SkillSetBuilder;
using SimChartMedicalOffice.Core.DataInterfaces.TempObject;
using SimChartMedicalOffice.Data;
using SimChartMedicalOffice.Data.QuestionBanks;
using SimChartMedicalOffice.Data.SkillSet;
using SimChartMedicalOffice.Data.TempObject;
namespace SimChartMedicalOffice.Infrastructure.Installer
{
    public class DataRepositoriesInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<ISimAppDocument>().ImplementedBy<SimAppDocument>(),
                Component.For<IPriorAuthorizationRequestFormDocument>().ImplementedBy
                    <Data.Forms.PriorAuthorizationRequestFormDocument>(),
                Component.For<IPatientDocument>().ImplementedBy<Data.Forms.PatientDocument>(),
                 Component.For<IPatientRecordsAccessFormDocument>().ImplementedBy<Data.Forms.PatientRecordsAccessFormDocument>(),
                 Component.For<IMedicalRecordsRelease>().ImplementedBy<Data.Forms.MedicalRecordsReleaseDocument>(),
                 Component.For<IReferralFormDocument>().ImplementedBy<Data.Forms.ReferralFormDocument>(),
                Component.For<IRegistrationDocument>().ImplementedBy<RegistrationDocument>(),
                Component.For<Core.DataInterfaces.FrontOffice.IAppointmentDocument>().ImplementedBy<Data.FrontOffice.AppointmentDocument>(),
                Component.For<IAttachmentDocument>().ImplementedBy<AttachmentDocument>(),
                Component.For<IAuthoringDocument>().ImplementedBy<AuthoringDocument>(),                
                Component.For<ICompetencyDocument>().ImplementedBy<CompetencyDocument>(),
                Component.For<IAnswerOptionDocument>().ImplementedBy<AnswerOptionDocument>(),
                Component.For<IQuestionDocument>().ImplementedBy<QuestionDocument>(),
                Component.For<ISkillSetDocument>().ImplementedBy<SkillSetDocument>(),
                Component.For<IFolderDocument>().ImplementedBy<FolderDocument>(),
                Component.For<IQuestionBankDocument>().ImplementedBy<QuestionBankDocument>(),
                Component.For<IAssignmentDocument>().ImplementedBy<Data.AssignmentBuilder.AssignmentDocument>(),
                Component.For<IAssignmentRepositoryDocument>().ImplementedBy<Data.AssignmentBuilder.AssignmentReposistoryDocument>(),
                Component.For<ISkillSetRepositoryDocument>().ImplementedBy<SkillSetReposistoryDocument>(),
                Component.For<ITestDocument>().ImplementedBy<TestDocument>(),
                Component.For<IBillOfRightsDocument>().ImplementedBy
                <Data.Forms.BillOfRightsDocument>(),
                Component.For<IApplicationModuleDocument>().ImplementedBy
                <Data.Competency.ApplicationModuleDocument>(),
                Component.For<ICompetencySourceDocument>().ImplementedBy
                <Data.Competency.CompetencySourceDocument>(),
                Component.For<INoticeOfPrivacyPracticeDocument>().ImplementedBy
                <Data.Forms.NoticeOfPrivacyPracticeDocument>(),
                Component.For<IMasterDocument>().ImplementedBy<MasterDocument>(),
                Component.For<IPatientVisitAppointmentDocument>().ImplementedBy<Data.FrontOffice.PatientVisitAppointmentDocument>(),
                Component.For<IOtherAppointmentDocument>().ImplementedBy<Data.FrontOffice.OtherAppointmentDocument>(),
                Component.For<IBlockAppointmentDocument>().ImplementedBy<Data.FrontOffice.BlockAppointmentDocument>(),
                Component.For<IRecurrenceGroupDocument>().ImplementedBy<Data.FrontOffice.RecurrenceGroupDocument>());

            //container.Register(AllTypes.FromAssemblyNamed("SimChartMedicalOffice.Data.TempObject")
            //   .Where(type => type.Name.EndsWith("Document"))
            //   .LifestyleSingleton());
        }
    }
}