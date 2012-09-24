using SimChartMedicalOffice.Core.DataInterfaces.AssignmentBuilder;
using SimChartMedicalOffice.Data.Repository;
namespace SimChartMedicalOffice.Data.AssignmentBuilder
{
    public class AssignmentReposistoryDocument : KeyValueRepository<Core.AssignmentBuilder.AssignmentRepository>, IAssignmentRepositoryDocument
    {
        //protected override string NewUrl
        //{
        //    get
        //    {
        //        return "SimApp/Courses/ELSEVIER_CID/Admin/AssignmentRepository";
        //    }
        //}

       
    }
}
