using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Script.Serialization;
using System.Linq.Expressions;
using System.Reflection;
using System.Collections;


namespace SimChartMedicalOffice.Common
{
    public static class AppCommon
    {
        public enum FolderType
        {
            QuestionBank = 1,
            PatientRepository = 2,
            AssignmentRepository = 3,
            SkillSetRepository = 4,
            None = 0
        }
        public static string GetFolderType(int folderType) {
            switch ((FolderType)folderType) { 
                case FolderType.QuestionBank:
                    return "QuestionBank";
                case FolderType.PatientRepository:
                    return "PatientRepository";
                case FolderType.AssignmentRepository:
                    return "AssignmentRepository";
                case FolderType.SkillSetRepository:
                    return "SkillSetRepository";
                default: return "";
            }
        }
        public static string GetFolderTypeName(int folderType)
        {
            switch ((FolderType)folderType)
            {
                case FolderType.QuestionBank:
                    return "Question Bank";
                case FolderType.PatientRepository:
                    return "Practice";
                case FolderType.AssignmentRepository:
                    return "Assignment";
                case FolderType.SkillSetRepository:
                    return "Skill Set";
                default: return "";
            }
        }
        public static string GetRoleDescription(AppEnum.ApplicationRole roleName)
        {
            if (roleName == AppEnum.ApplicationRole.Admin)
            {
                return AppConstants.ADMIN_ROLE;
            }
            else if (roleName == AppEnum.ApplicationRole.Instructor)
            {
                return AppConstants.INSTRUCTOR_ROLE;
            }
            else
            {
                return AppConstants.STUDENT_ROLE;
            }
        }
       
        public static string GetFormName(int formNumber)
        {
            switch ((AppEnum.FormsRepository)formNumber)
            {
                case AppEnum.FormsRepository.HipaaForm:
                    return "Notice of Privacy Practice";
                case AppEnum.FormsRepository.ReferrelForm:
                    return "Referral";
                case AppEnum.FormsRepository.BillOfRights:
                    return "Patient Bill of Rights";
                case AppEnum.FormsRepository.PatientInformation:
                    return "Patient Information";
                case AppEnum.FormsRepository.PatientRecordAccess:
                    return "Patient Records Access Request";
                case AppEnum.FormsRepository.PriorAuthorizationRequestForm:
                    return "Prior Authorization Request";
                default: return "";
            }
        }
        public static void RegisterLog4Net()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
        public static string GetAppSettingValue(string key,string defaultValue)
        {
            string appSettingValue;
            appSettingValue = System.Configuration.ConfigurationManager.AppSettings[key];
            if (appSettingValue != null)
            {
                return appSettingValue;
            }
            return defaultValue;
        }
        public static string GetAppSettingValue(string key)
        {
            return GetAppSettingValue(key, "");
        }
        private static string GetRepositoryUrl()
        {
            string connectionString = "";
            string connection = "RepositoryUrl";//System.Configuration.ConfigurationManager.AppSettings["RepositoryUrl"].ToString();
            if (System.Configuration.ConfigurationManager.AppSettings[connection] != null)
            {
                connectionString = System.Configuration.ConfigurationManager.AppSettings[connection].ToString();
            }
            else
            {
                connectionString = "http://simoffice-sandbox.apto.elsevier.com/Test-Environment"; 
            }
            return connectionString;
        }
        #region Finding executing Environment
        public static AppEnum.ApplicationEnvironment GetEnvironment()
        {
            if (IsDevEnvironment())
            {
                return AppEnum.ApplicationEnvironment.Dev;
            }
            if (IsCertEnvironment())
            {
                return AppEnum.ApplicationEnvironment.Cert;
            }            
            if (IsIntEnvironment())
            {
                return AppEnum.ApplicationEnvironment.Int;
            }
            return AppEnum.ApplicationEnvironment.Prod;
        }
        public static bool IsDevEnvironment()
        {
            if (GetAppSettingValue("Environment").ToUpper().Equals("DEV"))
            {
                return true;
            }
            return false;
        }
        public static bool IsCertEnvironment()
        {
            if (GetAppSettingValue("Environment").ToUpper().Equals("CERT"))
            {
                return true;
            }
            return false;
        }
        public static bool IsProdEnvironment()
        {
            if (GetAppSettingValue("Environment").ToUpper().Equals("PROD"))
            {
                return true;
            }
            return false;
        }
        public static bool IsIntEnvironment()
        {
            if (GetAppSettingValue("Environment").ToUpper().Equals("INT"))
            {
                return true;
            }
            return false;
        }
        #endregion

        public static string GetDocumentUrl(string nodes)
        {
            string connection = GetRepositoryUrl();
            nodes = nodes.TrimEnd('/');
            if (nodes == "")
            {
                return connection +".json";
            }
            else
            {
                return connection + "/" + nodes +".json";
            }
        }
        public static string Help()
        {
            return "";
        }

        public static bool CheckIfStringIsEmptyOrNull(string checkString)
        {
            return (string.IsNullOrEmpty(checkString) || checkString == "null");
        }

        

        public static void AddCookie(string cookieName, object cookieValue)
        {
            HttpCookie hc = new HttpCookie("test", "123");
            HttpContext.Current.Request.Cookies.Add(hc);
        }
        public static string GetCookieValue(string cookieName)
        {
            if (HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                return HttpContext.Current.Request.Cookies[cookieName].Value;
            }
            return "";
        }
        public static string ReplaceEscapeCharacterinJson(string jsonString)
        {
            return jsonString.Replace("\n", "\\n").Replace("'", "\'").Replace("u0027", "\\'");
        }
        public static string ReplacehtmlcharInString(string jsonString)
        {
            return jsonString.Replace(">", "&#62;").Replace("<", "&#60;").Replace("  ", "&nbsp;&nbsp;");
        }
        public static string ReplaceEscapeCharacterinJsonString(string jsonString)
        {
            return jsonString.Replace("\n", "\\n");
        }
        public static string ReplaceEscapeCharaterInReport(string stringValue)
        {
            return stringValue.Replace("\\\"", "\"");
        }
        public static string ReplaceEscapeCharaterInString(string stringValue)
        {
            return stringValue.Replace("\n", "\\n").Replace("\"", "\\\\\"").Replace("'", "\\'").Replace("%", "%25").Replace("+", "%2B");
        }
        public static string ReplaceEscapeCharacterWithHtml(string jsonString)
        {
            return jsonString.Replace("\\n", "<br/>").Replace("'", "\\'");
        }
        //This method is used over in reports to display a line break for new line characters.
        public static string ReplaceEscapeCharacterWithHtmlForReports(string inputString)
        {
            return inputString.Replace("\n", "<br/>");
        }

        public static string celciusToFarenheit(string celcius)
        {
            if (!String.IsNullOrEmpty(celcius))
                return ((Convert.ToDouble(celcius) * 9 / 5) + 32).ToString();
            else
                return AppConstants.EmptyDataForReports;
        }

        /// <summary>
        /// Method to calculate the BMI for the given value
        /// </summary>
        /// <param name="weightInPounds"></param>
        /// <param name="weightInOunces"></param>
        /// <param name="heightInFeet"></param>
        /// <param name="heightInInches"></param>
        /// <returns></returns>
        public static float CalculateBMI(float weightInPounds, float weightInOunces,
                            float heightInFeet, float heightInInches)
        {
            float feetToInches;
            float totalHeightInInches;
            double heightInMeter;
            float poundsToOunces;
            float weightInKilograms;
            float bmiValue = 0;
            float roundBMIValue;
            feetToInches = heightInFeet * 12;
            totalHeightInInches = feetToInches + heightInInches;
            heightInMeter = (float)(totalHeightInInches * 0.0254);
            poundsToOunces = (weightInPounds * 16) + weightInOunces;
            weightInKilograms = (float)(poundsToOunces * 0.02835);
            if (heightInMeter != 0)
            {
                bmiValue = (float)(weightInKilograms / (heightInMeter * heightInMeter));
            }
            roundBMIValue = (float)(System.Math.Round(bmiValue, 1));
            return (float)roundBMIValue;
        }
        public static double CalculateHeight(double heightInFeet, double heightInInches)
        {
            double feetToInches;
            double totalHeightInInches;
            double heightInCm;
            double roundheight;
            feetToInches = heightInFeet * 12;
            totalHeightInInches = feetToInches + heightInInches;
            heightInCm = (double)(totalHeightInInches * 2.54);
            roundheight = (double)(System.Math.Round(heightInCm, 1));
            return roundheight;
        }
        public static double CalculateWeight(double weightInPounds, double weightInOunces)
        {
            double poundsToOunces;
            double weightInKilograms;
            double roundweight;
            poundsToOunces = (weightInPounds * 16) + weightInOunces;
            weightInKilograms = (double)(poundsToOunces * 0.02835);
            roundweight = (double)(System.Math.Round(weightInKilograms, 1));
            return roundweight;

        }

        public static int GenerateRandomNumber()
        {
            int iRandomNumber;
            iRandomNumber = GenerateRandomNumberWithinRange(1000000, 9999999);
            return iRandomNumber;
        }

        public static int GenerateRandomNumberWithinRange(int iMinNumber, int iMaxNumber)
        {
            Random iRandomNumber = new Random();
            return iRandomNumber.Next(iMinNumber, iMaxNumber);
        }

        public static int ConvertHoursToMinuets(decimal hours)
        {
            return Convert.ToInt32(hours * 60);
        }
        public static DateTime GetDateTime(string datePart, string timePart)
        {
            DateTime datePartValue = Convert.ToDateTime(datePart);
            return new DateTime(datePartValue.Year, datePartValue.Month, datePartValue.Day, GetHour(timePart), GetMinute(timePart), 0);
        }
        public static int GetHour(string time)
        {
            string[] hour = time.Split(':');
            if (hour.Length == 2)
            {
                return Convert.ToInt32(hour[0]);
            }
            else
            {
                return 0;
            }
        }

        public static int GetMinute(string time)
        {
            string[] minute = time.Split(':');
            if (minute.Length == 2)
            {
                return Convert.ToInt32(minute[1]);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Get the miliatary time format for the datetime
        /// </summary>
        /// <param name="dtDateTime"></param>
        /// <returns></returns>
        public static string GetMilitaryTime(DateTime dtDateTime)
        {
            string militaryTime = "";
            if (dtDateTime != null)
            {
                militaryTime = string.Format("{0:HH:mm}", dtDateTime);
                return militaryTime;
            }
            return militaryTime;
        }
        
        /// <summary>
        ///  //To validate for standard file formats  .pdf .jpeg .png .gif .bmp
        /// </summary>
        /// <param name="strTypeOfContent"></param>
        /// <returns>true if valid formats</returns>
        public static bool IsValidImageFormatForSimOffice(string strTypeOfContent)
        {
            return ((strTypeOfContent == "image/x-png" || strTypeOfContent == "image/pjpeg" || strTypeOfContent == "image/bmp" || strTypeOfContent == "image/gif" || strTypeOfContent == "image/jpeg" || strTypeOfContent == "image/png") ? true : false);
        }
        /// <summary>
        /// Method to set value in a multiple values cookie
        /// </summary>
        /// <typeparam name="T">Object type of the cookie value</typeparam>
        /// <param name="cookieName">Name of the cookie</param>
        /// <param name="jsonCookieString">Json string of the value to be added to the cookie</param>
        public static void SetMultipleValueCookie<T>(string cookieName, string jsonCookieString)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<T> cookieValueList = GetMultipleValueCookie<T>(cookieName);
            T cookieValue = js.Deserialize<T>(jsonCookieString);
            if (cookieValueList.Count > 0)
            {
                System.Web.HttpContext.Current.Response.Cookies.Remove(cookieName);
            }
            cookieValueList.Add(cookieValue);
            string serializedCookieValue = js.Serialize(cookieValueList);
            System.Web.HttpContext.Current.Response.Cookies[cookieName].Value = serializedCookieValue;
        }

        /// <summary>
        /// Method to get the values from a multiple valued cookie
        /// </summary>
        /// <typeparam name="T">Object type of the cookie value</typeparam>
        /// <param name="cookieName">Name of the cookie</param>
        /// <returns>List of object type cookie values</returns>
        public static List<T> GetMultipleValueCookie<T>(string cookieName)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            List<T> cookieValueList = new List<T>();
            if (System.Web.HttpContext.Current.Request.Cookies[cookieName] != null)
            {
                cookieValueList = js.Deserialize<List<T>>(System.Web.HttpContext.Current.Request.Cookies[cookieName].Value.ToString());
            }
            return cookieValueList;
        }

        /// <summary>
        ///  //To validate excel upload file
        /// </summary>
        /// <param name="strTypeOfContent"></param>
        /// <returns>true if valid formats</returns>
        public static bool IsValidFileForExcelUpload(string strTypeOfContent)
        {
            return ((strTypeOfContent == "xls" || strTypeOfContent == "xlsx" || strTypeOfContent == "application/vnd.ms-excel" || strTypeOfContent == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") ? true : false);
        }
        /// <summary>
        /// Return Key of Question based on value
        /// </summary>
        /// <param name="iQuestionType"></param>
        /// <returns></returns>
        public static string GetKeyBasedOnQuestionType(int iQuestionType)
        {
            var keyValuePairOfQuestion = AppCommon.QuestionOptionsPage.Single(x => x.Value == iQuestionType);
            return keyValuePairOfQuestion.Key;
        }

        public static Dictionary<int, string> QuestionTypeOptions = new Dictionary<int, string>()
        {
            {1, "-SELECT-"},
            {2, "Charting Exercise"},
            {3, "Correct Order"},
            {4, "Fill-in-the-Blank"},
            {9, "Labeling"},
            {5, "Matching"},
            {6, "Multiple Choice"},
            {7, "Short Answer"},
            {8, "True/False"}
            
        };

        public static Dictionary<string, int> QuestionOptionsPage = new Dictionary<string, int>()
        {
            {"_ChartingExercise",2},
            {"_CorrectOrder",3},
            {"_FillInTheBlank",4},
            {"_Labeling", 9},
            {"_Matching",5},
            {"_MultipleChoice", 6},
            {"_ShortAnswer", 7},
            {"_TrueFalseQuestionSetUp", 8}
            
        };

        public static List<string> BlankOrientation = new List<string>()
        {
            "-Select-",
            "(Blank) (Text)",
            "(Text) (Blank) (Text)",
            "(Text) (Blank)",
            "(Text) (Blank) (Text) (Blank)",
            "(Blank) (Text) (Blank) (Text)",
            "(Blank) (Blank) (Text)",
            "(Text) (Blank) (Blank)"                                                     
        };
        public static Dictionary<int, string> QuestionTypeOptionsForLanding = new Dictionary<int, string>()
        {
            //{1, "Filter by question type"},
            {2, "Charting Exercise"},
            {3, "Correct Order"},
            {4, "Fill-in-the-Blank"},
            {9, "Labeling"},
            {5, "Matching"},
            {6, "Multiple Choice"},
            {7, "Short Answer"},
            {8, "True/False"}
        };
        public static Dictionary<int, string> SourceOptions = new Dictionary<int, string>()
        {
            {1, "-SELECT-"},
            {2, "ABHES"},
            {3, "CAAHEP"},
            {4, "MAERB"},
        };
        public static Dictionary<int, string> SourceOptionsForLanding = new Dictionary<int, string>()
        {
            //{1, "Filter by Source"},
            {2, "ABHES"},
            {3, "CAAHEP"},
            {4, "MAERB"},
        };
        public static List<string> NoOfLabels = new List<string>()
        {
            "-Select-",
            "1",
            "2",
            "3",
            "4",
            "5",
            "6",
            "7",
            "8"                                       
        };

        public static List<string> NoOfAttempts = new List<string>()
        {
            "1",
            "2",
            "3"
        };

        public static  string GetObjectPropertyVal(Object inputVal, bool isBlank)
        {
            if (isBlank==true)
            {
                return "";
            }
            else if (inputVal!=null && inputVal.ToString()!="")
            {
                return inputVal.ToString();
            }
            else 
            {
                return "--";
            }

        }
        /// <summary>
        /// Method used to serialize a Json object to string 
        /// </summary>
        /// <param name="obj"> Json object value</param>
        /// <returns></returns>
        public static string serializeToJson(Object obj)
        {
            string Json;
            JavaScriptSerializer js = new JavaScriptSerializer();
            Json = js.Serialize(obj).Replace("\"\\/Date(", "new Date(parseInt(").Replace(")\\/\"", "))");
            return Json;
        }

        public static string CheckIfArrayHasValueAndReturn(string[] arrStringToCheck, string strToCheck)
        {
            string strToRet = ((Array.IndexOf(arrStringToCheck, strToCheck) != -1) && strToCheck != null) ? "checked" : "";
            return strToRet;
        }

        public static string CheckForFlagAndReturnValue(string[] arrStringToCheck, string strToCheck)
        {
            string strToRet = CheckIfArrayHasValueAndReturn(arrStringToCheck, strToCheck);
            return strToRet;
        }
        public static string[] GetStringArrayAfterSplitting(string strStringWithDelimiter)
        {
            string[] strArray = null;
            if (strStringWithDelimiter != string.Empty)
            {
                strArray = strStringWithDelimiter.Split(AppConstants.demiliter);
            }
            strArray = (strArray == null) ? new string[1] : strArray;
            return strArray;
        }

        public static string[] gridColumnForQuestionSearchList = { "Text", "LinkedItemReference", "FolderName", "TypeOfQuestion", "CreatedTimeStamp" };
        public static string QuestionBankLandingPageFolderName = "Question Bank";
        public static string AssignmentLandingPageFolderName = "Assignment Builder";
        public static string[] gridColumnForPatientSearchList = { "FirstName", "LastName", "Sex", "DateOfBirth", "AgeInYears", "CreatedTimeStamp", "Status" };
        public static string[] gridColumnForPatientList = { "FirstName", "LastName", "Sex", "DateOfBirth", "AgeInYears", "CreatedTimeStamp", "Status" };
        public static string[] gridColumnForSkillSetSearchList = { "SkillSetTitle", "LinkedCompetenciesText", "SourceNameText", "CreatedOn", "Status" };
        public static string[] gridColumnForAppointmentPatientList = { "StartDateTime", "Type", "Status", "ProviderId" };
        public static string SkillSetLandingPageFolderName = "SkillSet";
        public static string Status_Published = "Published";
        public static string Status_InProgress = "Unpublished";
        public static string[] gridColumnForAssignmentSearchList = { "Patient", "AssignmentTitle", "Module", "LinkedCompetencies", "Duration", "FolderName", "CreatedTimeStamp", "status" };

        public static List<string> officeTypeOptions = new List<string>()
        {
            "-Select-",
            "Family Practice",
            "Geriatric",
            "OBGYN",
            "Pediatric"                                                              
        };

        public static List<string> providerOptions = new List<string>()
        {
            "-SELECT-",
            "Dr. James A. Martin, MD",
            "Dr. Julie Walden, MD",            
            "Jean Burke, NP",                                                             
        };

        public static string breakWord(string word, int breakLength)
        {
            string outputStr = Regex.Replace(word, @"([\w-*]{" + breakLength + "}(?=\\w+))", "$1\n");
            return outputStr;
        }

        public static List<string> CalculatePassRate(double noOfQuestions)
        {
            List<string> passRateValuesToReturn = new List<string>();
            passRateValuesToReturn.Add(AppConstants.Select_DropDown);
            double breakValue = 100/noOfQuestions;
            double percentageValue = 0;
            for (int questionCount = 0; questionCount < noOfQuestions; questionCount++ )
            {
                percentageValue = percentageValue + breakValue;
                passRateValuesToReturn.Add(Convert.ToInt32(Math.Round(percentageValue)).ToString() + "%");
            }
            return passRateValuesToReturn;
        }

        public static bool CheckIfPublished(string strStatus)
        {
            if (!String.IsNullOrEmpty(strStatus) && strStatus.ToLower().Equals(Status_Published.ToLower()))
            {
                return true;
            }
            return false;
        }

        public enum SkillSetPage
        {
            Step1Metadata = 1,
            Step2Structure = 2,
            Step3QnAns = 3,
            Step4PreviewPublish = 4
        }

        public enum SkillSetButton
        {
            Back = 1,
            Save = 2,
            SaveProceed = 3,
            Cancel = 4,
            Proceed = 5,
            Preview = 6,
            Publish = 7 
        }

        public enum AuthoringType
        {
            SkillSet = 1,
            AssignmentBuilder = 2,
        }

        public static List<string>AssignmentDurationHrs = new List<string>()
        {
            "-Select-",
            "1",
            "2",
            "3",
            "4",
            "5"
        };

        public static List<string> AssignmentDurationMns = new List<string>()
        {
            "-Select-",
            "5",
            "10",
            "15",
            "20",
            "25",
            "30",
            "35",
            "40",
            "45",
            "50",
            "55"
        };

        public static List<string> AppointmentVisitType = new List<string>()
        {
            "-Select-",
            "New Patient Visit",
            "Wellness Exam",
            "Urgent Visit",
            "Follow-Up/Established Visit",
            "Annual Exam"
        };

        public static List<string> ExamRoom = new List<string>()
        {
            "-SELECT-",
            "Exam Room 1", 
            "Exam Room 2",
            "Exam Room 3", 
            "Exam Room 4", 
            "Exam Room 5", 
            "Exam Room 6",
            "Exam Room 7", 
            "Exam Room 8", 
            "Exam Room 9", 
            "Exam Room 10"
        };

        public static List<string> TimeList = new List<string>()
        {
            "-Select-",
            "12:00 AM",
            "12:15 AM",
            "12:30 AM",
            "12:45 AM"
        };

        public static string QuestionsForCOmpetencies_Confirmation = "Removing this competency will automatically remove the following questions from this skill set";

        /// <summary>
        /// Get formatted Identifier for a month
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="courseId"></param>
        /// <param name="role"></param>
        /// <param name="assignmentScenarioId"></param>
        /// <param name="appointmentDate"></param>
        /// <returns></returns>
        public  static string GetUrlForMonthFilter(string Url, string courseId, AppEnum.ApplicationRole role, string assignmentScenarioId, DateTime appointmentDate,string uid)
        {
            return string.Format(Url, courseId, AppCommon.GetRoleDescription(role),uid, assignmentScenarioId, string.Format("{0:yyyyMM}", appointmentDate));
        }

        /// <summary>
        /// Get identifier for A Date
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="appointmentDate"></param>
        /// <returns></returns>
        public static string GetUrlForDateFilter(string Url, DateTime appointmentDate)
        {
            return Url +"/" + string.Format("Day" + string.Format("{0:dd}", appointmentDate));
        }

        /// <summary>
        /// To check whether Json is valid
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static bool isValidJsonString(string jsonString)
        {
            if (!String.IsNullOrEmpty(jsonString) && jsonString != "null")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// GetUserRole Based on Dropbox user role string
        /// </summary>
        /// <param name="roleString"></param>
        /// <returns></returns>
        public static AppEnum.ApplicationRole GetCurrentUserRole(string roleString)
        {
            AppEnum.ApplicationRole roleType = (AppEnum.ApplicationRole)Enum.Parse(typeof(AppEnum.ApplicationRole), roleString, true);
            return roleType;
        }

        public static string DROPDOWN_SELECT = "-Select-";
        public static string VIEW_MORE = "View All";
        /// <summary>
        /// to get the value of provider with the providerId
        /// </summary>
        public static Dictionary<int, string> providerList = new Dictionary<int, string>()
        {
            {1004, "-Select-"},
            {1003, "Dr. James A. Martin, MD"},
            {1002, "Dr. Julie Walden, MD"},
            {1001, "Jean Burke, NP"},        
        };


        public static Dictionary<int, string> AppointmentStatus = new Dictionary<int, string>()
        {
            {0,"-Select-"},
            {1,"Scheduled"},
            {2,"Arrived on-time"},
            {3,"Arrived Late"},
            {4,"Canceled"},
            {5,"Left without being seen"},
            {6,"No show"},
            {7,"Checked Out"}
            
        };

        public static Dictionary<int, string> StatusLocation = new Dictionary<int, string>()
        {
            {0,"-Select-"},
            {1,"Waiting room"},
            {2,"Exam room"},
            {3,"With MA"},
            {4,"With Provider"}
        };

        public static string GetStatusLocationString(int statusLocationKey)
        {
            return statusLocationKey!=0?StatusLocation.Where(s => s.Key.Equals(statusLocationKey)).Select(s => s.Value.ToString()).SingleOrDefault():String.Empty;
        }
    }
}
