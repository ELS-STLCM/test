using SimChartMedicalOffice.Common.Utility;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;
using SimChartMedicalOffice.Core.QuestionBanks;
using System.Collections.Generic;
using System.Linq;
using System;
using SimChartMedicalOffice.Common;
using SimChartMedicalOffice.Core.ProxyObjects;
namespace SimChartMedicalOffice.Data.AssignmentBuilder
{
    public class AssignmentReposistoryDocument : KeyValueRepository<Core.AssignmentBuilder.AssignmentRepository>, IAssignmentRepositoryDocument
    {
        public override string Url
        {
            get
            {
                return "SimApp/Courses/ELSEVIER_CID/Admin/AssignmentRepository";
            }
        }

        /// <summary>
        /// To get Assignment object
        /// </summary>
        /// <returns></returns>
        public Folder GetAssignmentRepository()
        {
            string jsonString = GetJsonDocument(this.Url);
            Folder assignment = JsonSerializer.DeserializeObject<Folder>(jsonString);
            return assignment;
        }
    }
}
