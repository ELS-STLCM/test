using System;

namespace SimChartMedicalOffice.Core.QuestionBanks
{
    public class AnswerOption : DocumentEntity
    {
        /// <summary>
        /// This property is a reference to Object Attachments
        /// Relationship is one to one.
        /// </summary>
        public string AnswerImageReference { get; set; }

        /// <summary>
        /// This property holds the Rationale for a correct answer
        /// </summary>
        public string Rationale { get; set; }

        /// <summary>
        /// This property holds the Text for a particular Answer
        /// </summary>
        public string AnswerText { get; set; }

        /// <summary>
        /// This property holds the Matching Answer Text for a Matching Type
        /// </summary>
        public string MachingText { get; set; }

        /// <summary>
        /// This property holds the sequence number for each answer option
        /// </summary>
        public string AnswerSequence { get; set; }


        /// <summary>
        /// This property holds whether the answer is correct answer - Just to set the AnswerKey correctly from UI
        /// Has no data in real
        /// </summary>
        public bool IsCorrectAnswer { get; set; }


        public AnswerOption Clone()
        {
            Type type = GetType();
            AnswerOption answerOptionData = new AnswerOption();
            foreach (System.Reflection.PropertyInfo objProp in type.GetProperties())
            {
                if (objProp.CanWrite && objProp.Name.ToUpper() != "UNIQUEIDENTIFIER" ||
                    objProp.CanWrite && objProp.Name.ToUpper() != "URL")
                {
                    objProp.SetValue(answerOptionData, type.GetProperty(objProp.Name).GetValue(this, null), null);
                }

            }
            return answerOptionData;
        }
    }
}