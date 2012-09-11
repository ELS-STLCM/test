using System.Collections.Generic;
using SimChartMedicalOffice.Core.ProxyObjects;
using SimChartMedicalOffice.Core.QuestionBanks;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder
{
    public interface IAssignmentRepositoryDocument : IKeyValueRepository<Core.AssignmentBuilder.AssignmentRepository>
    {
        Folder GetAssignmentRepository();
    }
}
