using System.Collections.Generic;
using SimChartMedicalOffice.Core.AssignmentBuilder;
using SimChartMedicalOffice.Data.Repository;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Core.DropBox;

namespace SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder
{
    public interface IAssignmentDocument : IKeyValueRepository<Assignment>
    {
       string FormAssignmentUrl(DropBoxLink dropBox,string guid);
       IList<Assignment> GetAssignmentItems(string parentFolderIdentifier, int folderType, DropBoxLink dropBox);
       IList<Core.SkillSetBuilder.SkillSet> GetSkillSetsForAnAssignment(string assignmentUrl);
        Core.Patient.Patient GetPatientFromPatientRepository(string patientUrl);
        Folder GetAssignmentRepository();
    }
}
