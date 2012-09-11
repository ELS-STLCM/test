using System.Collections.Generic;
using System;
using SimChartMedicalOffice.Common;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class Question : DocumentEntity
    {
        public Question() { }
        /// <summary>
        /// This property is a reference to Object Competency
        /// Relationship is one to one.
        /// </summary>
        public string CompetencyReferenceGUID { get; set; }

        /// <summary>
        /// This property is a reference to Object QuestionBank with a particular Folder property
        /// Relationship is one to one.
        /// </summary>
        //public string FolderReference { get; set; }

        /// <summary>
        /// This property is a reference to Object Attachments
        /// Relationship is one to one.
        /// </summary>
        public string QuestionImageReference { get; set; }

        /// <summary>
        /// This property holds the Text for a particular Question.
        /// </summary>
        public string QuestionText { get; set; }

        /// <summary>
        /// This property holds the Type of a particular Question.
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// This property holds the Rationale for a question
        /// </summary>
        public string Rationale { get; set; }

        /// <summary>
        /// This property is a for correct order.
        /// </summary>
        public List<string> CorrectOrder { get; set; }

        /// <summary>
        /// This property holds all the possible answers for a particular Question.
        /// </summary>
        public List<AnswerOption> AnswerOptions { get; set; }

        /// <summary>
        /// This property holds the value of the Blank Orientation field in Fill In the Blank qustion type
        /// </summary>
        public string BlankOrientation { get; set; }

        /// <summary>
        /// This property holds the value of the No of labels field in Labeling qustion type
        /// </summary>
        public string NoOfLabels { get; set; }

        /// <summary>
        /// This property holds the int value of SequenceNumber of skill set 
        /// </summary>
        public int SequenceNumber { get; set; }

        /// <summary>
        /// This property holds the bool value of Template status of skill set 
        /// </summary>
        public bool IsQuestionFromTemplate { get; set; }

        public string TemplateSequenceNumber { get; set; }

        public string SkillSetReferenceUrlOfQuestion { get; set; }

        public Question Clone()
        {
            Type type = this.GetType();
            Question questionData = new Question();
            foreach (System.Reflection.PropertyInfo objProp in type.GetProperties())
            {
                if (objProp.PropertyType.IsGenericType == true)
                {
                    if (objProp.Name.ToUpper() == "ANSWEROPTIONS")
                    {
                        List<AnswerOption> listAnswerOptionsData = new List<AnswerOption>();

                        if (this.AnswerOptions != null && this.AnswerOptions.Count > 0)
                        {
                            foreach (AnswerOption answerOptionData in this.AnswerOptions)
                            {
                                listAnswerOptionsData.Add(answerOptionData.Clone());
                            }
                            questionData.AnswerOptions = listAnswerOptionsData;
                        }
                    }
                    else if (objProp.Name.ToUpper() == "CORRECTORDER")
                    {
                        questionData.CorrectOrder = this.CorrectOrder;
                    }
                    else
                    {
                        objProp.SetValue(questionData, type.GetProperty(objProp.Name).GetValue(this, null), null);
                    }
                }
                else if (objProp.CanWrite && objProp.Name.ToUpper() != "UNIQUEIDENTIFIER" ||
                    objProp.CanWrite && objProp.Name.ToUpper() != "URL")
                {
                    objProp.SetValue(questionData, type.GetProperty(objProp.Name).GetValue(this, null), null);
                }

            }
            return questionData;
        }
    }
}