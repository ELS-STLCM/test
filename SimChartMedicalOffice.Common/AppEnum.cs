using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimChartMedicalOffice.Common
{
    public class AppEnum
    {
        public enum AttachmentFlagsForStatus
        {
             ExistsInDbNotInUI=1,
             ExistsInUiNotInDb=2,
             NotExistsInUiAndDb=3,
             ExistsInUiAndDb=4
        }
        public enum RecurrencePattern
        {
            Daily = 1,
            Weekly = 2,
            Monthly = 3
        }
        public enum AttachmentActions
        {
            RemoveTransient=1,
            RemovePersistent=2,
            RemoveTransientAndCreatePersistent = 3,
            CreateTransient=4,
            CreatePersistent=5,
            RemoveTransientPersistentAndCreatePersistent=6,
            None=7
        }
        public enum WhereOperation
        {
            Equal = 0,
            NotEqual = 1,
            Contains = 2
        }
        public enum FormsRepository
        {
            HipaaForm=1,
            ReferrelForm=2,
            BillOfRights=3,
            PatientInformation=4,
            PatientRecordAccess=5,
            PriorAuthorizationRequestForm=6,
            Confirmation=7
        }
        /// <summary>
        /// To load the master page in Admin Login
        /// </summary>
        public enum Master
        {
            Competency = 1
        }

        /// <summary>
        /// To Set Ajax call result type 
        /// </summary>
        public enum ResultType
        {
            Error = 1,
            Success = 2,
            None = 3
        }
        /// <summary>
        /// Question Type Enum - Should match with Dictionary in AppCommon
        /// </summary>
        public enum QuestionTypes
        {

            CorrectOrder = 3,
            FillInTheBlank = 4,
            MultipleChoice = 6,
            ShortAnswer = 7,
            TrueFalseQuestionSetUp = 8,
            None =99999
        }
        public enum ApplicationEnvironment
        {
            Dev=1,
            Int=2,
            Cert=3,
            Prod=4
        }
        public enum ApplicationRole
        {
            Student=1,
            Admin=2,
            Instructor=3
        }
        public enum AppointmentTypes
        {
            PatientVisit=1,
            Block=2,
            Other=3,
            All=4
        }
        public enum CalendarFilterTypes
        {
            ExamRoom = 1,
            Provider = 2,
            AppointmentType = 3,
            None = 4
        }
        public enum CalendarViewTypes
        {
            month=1,
            agendaDay=2,
            agendaWeek=3,
            resourceDay=4
        }

        public enum EditStatus
        { 
            All = 1,
            Current = 2,
            CurrentFucture = 3
        }

        public enum AppointmentStatus
        { 
            Scheduled = 1,
            ArrivedOnTime =2,
            ArrivedLate =3,
            Canceled=4,
            LeftWithoutBeingSeen=4,
            NoShow=5,
            CheckedOut=6
        }

        //public enum AppointmentRecurrenceType
        //{
        //    RecurrenceToRecurrence = 1, 
        //    RecurrenceToNonRecurrence = 2,
        //    NonRecurrenceToRecurrence = 3,
        //    NonRecurrenceToNonRecurrence = 4
        //}
    }
}
