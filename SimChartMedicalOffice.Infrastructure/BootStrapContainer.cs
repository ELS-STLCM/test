using System.Web.Mvc;
using Castle.Windsor;
using SimChartMedicalOffice.Infrastructure.Installer;
using SimChartMedicalOffice.Web.Controllers;
namespace SimChartMedicalOffice.Infrastructure
{
    public static class BootStrapContainer
    {
        public static void Install()
        {
            var container = new WindsorContainer();     // Create a container to hold the dependencies
            var controllerFactory = new WindsorControllerFactory(container);     // Create a new instance
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);          // Use my factory instead of default

            container.Install(
                new ControllersInstaller(),
                new ServicesInstaller(),
                new DataRepositoriesInstaller()
            );      // Scan this assembly for all IWindsorInstaller
        }
    }
}
