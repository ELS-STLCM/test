using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class QuestionBankFolder : DocumentEntity
    {
        public QuestionBankFolder() { }
        /// <summary>
        /// This proprty holds the value of Question Bank Tab Folder
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// This property holds all Questions either Custom or Masters 
        /// </summary>
        public Dictionary<string, Question> QuestionItems { get; set; }

        /// <summary>
        /// This property holds the user created Folders and their children and grand children.
        /// </summary>
        public Dictionary<string, Folder> SubFolders { get; set; }
    }
}