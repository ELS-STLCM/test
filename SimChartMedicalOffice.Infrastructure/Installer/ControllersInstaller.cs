using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using SimChartMedicalOffice.Web.Controllers;

namespace SimChartMedicalOffice.Infrastructure.Installer
{
    public class ControllersInstaller : IWindsorInstaller
    {

        /// <summary>
        /// Installs the controllers
        /// </summary>
        /// <param name="container"></param>
        /// <param name="store"></param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(AllTypes.FromAssemblyNamed("SimChartMedicalOffice.Web.Controllers")
                .BasedOn<IController>()
                .If(t => t.Name.EndsWith("Controller"))
                .If(Component.IsInSameNamespaceAs<HomeController>())
               .LifestylePerWebRequest());
            
        }
    }
}
