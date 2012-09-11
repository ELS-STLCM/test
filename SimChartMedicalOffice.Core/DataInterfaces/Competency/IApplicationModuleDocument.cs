using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimChartMedicalOffice.Data.Repository;

namespace SimChartMedicalOffice.Core.DataInterfaces.Competency
{
    public interface IApplicationModuleDocument : IKeyValueRepository<Core.Competency.ApplicationModules>
    {
        IList<Core.Competency.ApplicationModules> GetAllApplicationModules();
        string FormApplicationModulesUrl(string applicationModulesUrl, bool isEditMode, Core.Competency.ApplicationModules applicationModulesUIObject);
    }

}
