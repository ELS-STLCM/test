using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Core.Patient;

namespace SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder
{
    public interface IAssignmentDocument : IKeyValueRepository<Core.AssignmentBuilder.Assignment>
    {
       string FormAssignmentUrl(string courseId,string guid);
       IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, int folderType, string courseId);
       IList<Core.SkillSetBuilder.SkillSet> GetSkillSetsForAnAssignment(string assignmentUrl);
        Core.Patient.Patient GetPatientFromPatientRepository(string patientUrl);
    }
}
