using System.Collections.Generic;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core;
using SimChartMedicalOffice.Core.DropBox;
using System;

namespace SimChartMedicalOffice.Data
{
    public static class Respository
    {
        #region Repository Node Names
        private const string RootNode = "SimApp";
        private const string CourseNode = "Courses";
        private const string AssignmentNode = "Assignments";
        private const string AppointmentNode = "Appointments";
        private const string PatientNode = "Patient";
        public const string PatientsNode = "Patients";
        private const string AdminAssignmentRepository = "AssignmentRepository";
        private const string AdminPatientRepository = "PatientRepository";
        private const string AdminQuestionBank = "QuestionBank";
        private const string AdminSkillsetRepository = "SkillSetRepository";
        public const string Skillsets = "SkillSets";
        public const string QuestionItems = "QuestionItems";
        private const string MasterNode = "Master";
        private const string AttachmentNode = "Attachments";
        #endregion

        public static string GetDocumentPath(DocumentPath.Module documentPath, string courseId, string scenarioId, AppEnum.ApplicationRole role,
                                           string userId, string customValue)
        {
            List<string> nodeList = new List<string>();
            string currentUserRole = AppCommon.GetRoleDescription(role);
            if (documentPath == DocumentPath.Module.Masters)
            {
                nodeList.Add(GetMasterDocumentPath());
            }
            else if (documentPath == DocumentPath.Module.Attachments)
            {
                nodeList.Add(GetAttachmentDocumentPath());
            }
            else
            {
                nodeList.Add(GetBaseDocumentPath(courseId, currentUserRole));
            }
            bool isAdminUser = IsAdmin(currentUserRole);
            if (isAdminUser)
            {
            }
            else
            {
                nodeList.Add(userId);
                nodeList.Add(AssignmentNode);
                nodeList.Add(scenarioId);
            }
            switch (documentPath)
            {
                case DocumentPath.Module.PatientVisitAppointment:
                case DocumentPath.Module.BlockAppointment:
                case DocumentPath.Module.OtherAppointment:
                case DocumentPath.Module.RecurrenceGroup:
                    {
                        if (isAdminUser)
                        {

                        }
                        else
                        {

                            nodeList.Add(AppointmentNode);
                            if (documentPath == DocumentPath.Module.RecurrenceGroup)
                            {
                                nodeList.Add("Recurrence");
                                nodeList.Add("{0}");
                            }
                            else
                            {
                                nodeList.Add("Type");
                                switch (documentPath)
                                {
                                    case DocumentPath.Module.PatientVisitAppointment:
                                        {
                                            nodeList.Add("PatientVisit");

                                            break;
                                        }
                                    case DocumentPath.Module.OtherAppointment:
                                        {
                                            nodeList.Add("Other");
                                            break;
                                        }
                                    case DocumentPath.Module.BlockAppointment:
                                        {
                                            nodeList.Add("Block");
                                            break;
                                        }
                                }
                                nodeList.Add(customValue);
                                nodeList.Add("{0}");
                                switch (customValue)
                                {
                                    case AppConstants.CalendarMonthFilters:
                                        break;
                                    default:
                                        nodeList.Add("{1}");
                                        nodeList.Add("{2}");
                                        nodeList.Add("{3}");
                                        AppEnum.ProviderType providerType = (!AppCommon.CheckIfStringIsEmptyOrNull(customValue) ? (AppEnum.ProviderType)Enum.Parse(typeof(AppEnum.ProviderType), customValue) : AppEnum.ProviderType.SingleProvider);
                                        if (providerType == AppEnum.ProviderType.SingleProvider)
                                        {
                                            nodeList.Add("{4}");
                                        }                                        
                                        break;
                                }
                            }
                        }
                        break;
                    }
                case DocumentPath.Module.LCMFolders:
                    {
                        nodeList.Add("{0}");//Based on foldertype substitute with either QuestionBank,AssignmentRepository etc.,
                        nodeList.Add("SubFolders{1}");
                        break;
                    }
                case DocumentPath.Module.Assignments:
                    {
                        if (isAdminUser)
                        {
                            switch (customValue)
                            {
                                case AppConstants.Create:
                                    {
                                        nodeList.Add(AdminAssignmentRepository + "{0}");
                                        nodeList.Add(AssignmentNode);
                                        nodeList.Add("{1}");
                                        break;
                                    }
                                default:
                                    {
                                        nodeList.Add(AdminAssignmentRepository);
                                        break;
                                    }
                            }
                        }
                        break;
                    }
                case DocumentPath.Module.Patients:
                    {

                        switch (customValue)
                        {
                            case AppConstants.Read:
                                {
                                    nodeList.Add(isAdminUser ? AdminPatientRepository : PatientsNode);
                                    break;
                                }
                            case AppConstants.Create:
                                {
                                    if (isAdminUser)
                                    {
                                        nodeList.Add(AdminPatientRepository);
                                    }
                                    nodeList.Add(PatientsNode);
                                    nodeList.Add("{0}");
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }



                        break;
                    }
                case DocumentPath.Module.QuestionBank:
                    {
                        if (isAdminUser)
                        {
                            switch (customValue)
                            {
                                case AppConstants.Create:
                                    {
                                        nodeList.Add(AdminQuestionBank + "{0}");
                                        nodeList.Add(QuestionItems);
                                        nodeList.Add("{1}");
                                        break;
                                    }
                                default:
                                    {
                                        nodeList.Add(AdminQuestionBank);
                                        break;
                                    }
                            }

                        }
                        break;
                    }

                case DocumentPath.Module.Attachments:
                    {
                        switch (customValue)
                        {
                            case AppConstants.TransientAttachment:
                                {
                                    nodeList.Add(AppConstants.TransientAttachment);
                                    break;
                                }
                            default:
                                {
                                    nodeList.Add(AppConstants.PersistentAttachment);
                                    break;
                                }
                        }
                        nodeList.Add("{0}");
                        break;
                    }
                case DocumentPath.Module.SkillSets:
                    {
                        if (isAdminUser)
                        {

                            switch (customValue)
                            {
                                case AppConstants.Create:
                                    nodeList.Add(AdminSkillsetRepository + "{0}");// {0} will be replaced with FOLDER text
                                    nodeList.Add(Skillsets);
                                    nodeList.Add("{1}"); // {1} will be replaces with NEW GUID value for each skillset
                                    break;
                                default:
                                    nodeList.Add(AdminSkillsetRepository);
                                    break;
                            }
                        }
                        break;
                    }
                case DocumentPath.Module.ReferralForm:
                case DocumentPath.Module.PriorAuthorizationForm:
                case DocumentPath.Module.PatientRecordsAccessForms:
                case DocumentPath.Module.MedicalRecordsReleaseDocument:
                case DocumentPath.Module.BillOfRights:
                case DocumentPath.Module.NoticeOfPrivacyPractice:
                    {
                        if (isAdminUser)
                        {
                        }
                        else
                        {

                            nodeList.Add(PatientNode);
                            nodeList.Add("{0}");//Patient Guid Value
                            switch (documentPath)
                            {
                                case DocumentPath.Module.ReferralForm:
                                    {
                                        nodeList.Add("ReferralForms");
                                        break;
                                    }
                                case DocumentPath.Module.PriorAuthorizationForm:
                                    {
                                        nodeList.Add("PriorAuthorizationRequestForms");
                                        break;
                                    }
                                case DocumentPath.Module.PatientRecordsAccessForms:
                                    {
                                        nodeList.Add("PatientRecordsAccessForms");
                                        break;
                                    }
                                case DocumentPath.Module.MedicalRecordsReleaseDocument:
                                    {
                                        nodeList.Add("MedicalRecordsReleaseDocument");
                                        break;
                                    }
                                case DocumentPath.Module.BillOfRights:
                                    {
                                        nodeList.Add("BillOfRights");
                                        break;
                                    }
                                case DocumentPath.Module.NoticeOfPrivacyPractice:
                                    {
                                        nodeList.Add("NoticeOfPrivacyPractice");
                                        break;
                                    }
                                default: { break; }
                            }
                            if (documentPath != DocumentPath.Module.BillOfRights)
                            {
                                nodeList.Add("{1}");//Form Guid
                            }
                        }
                        break;
                    }
                case DocumentPath.Module.Masters:
                    {
                        switch (customValue)
                        {
                            case AppConstants.BillofRights:
                                {
                                    nodeList.Add("Forms");
                                    nodeList.Add(AppConstants.BillofRights);
                                    break;
                                }
                            case AppConstants.NoticePrivacyPractice:
                                {
                                    nodeList.Add("Forms");
                                    nodeList.Add(AppConstants.NoticePrivacyPractice);
                                    break;
                                }
                            case AppConstants.ApplicationModule:
                                {
                                    nodeList.Add(AppConstants.ApplicationModule);
                                    break;
                                }
                            case AppConstants.Competencies:
                                {
                                    nodeList.Add(AppConstants.Competencies + "{0}{1}");
                                    break;
                                }
                            case AppConstants.CompetencySource:
                                {
                                    nodeList.Add(AppConstants.CompetencySource);
                                    nodeList.Add("{0}");
                                    break;
                                }
                            case AppConstants.QuestionType:
                                {
                                    nodeList.Add(AppConstants.QuestionType);
                                    break;
                                }
                            default: { break; }
                        }
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
            return string.Join("/", nodeList);
        }
        public static string GetDocumentPath(DocumentPath.Module documentPath, DropBoxLink dropBox, string customValue)
        {
            return GetDocumentPath(documentPath, dropBox.Cid, dropBox.Sid, AppCommon.GetCurrentUserRole(dropBox.UserRole), dropBox.Uid, customValue);
        }
/*
        private static string GetBaseDocumentPath(DropBoxLink dropBox)
        {

            return GetBaseDocumentPath(dropBox.Cid, dropBox.UserRole);
        }
*/
        private static string GetBaseDocumentPath(string courseId, string role)
        {
            List<string> nodeList = new List<string> {RootNode, CourseNode, courseId, role};
            string baseDocumentPath = string.Join("/", nodeList);
            return baseDocumentPath;
        }
        private static string GetMasterDocumentPath()
        {
            List<string> nodeList = new List<string> {RootNode, MasterNode};
            string baseDocumentPath = string.Join("/", nodeList);
            return baseDocumentPath;
        }
        private static string GetAttachmentDocumentPath()
        {
            List<string> nodeList = new List<string> {RootNode, AttachmentNode};
            string baseDocumentPath = string.Join("/", nodeList);
            return baseDocumentPath;
        }
        private static bool IsAdmin(string userRole)
        {
            if (AppCommon.GetCurrentUserRole(userRole) == AppEnum.ApplicationRole.Admin)
            {
                return true;
            }
            return false;
        }
/*
        private static bool IsRead(HttpClient.RestfulMethods action)
        {
            if (action == HttpClient.RestfulMethods.GET)
            { return true; }
            return false;
        }
*/

    }
}
