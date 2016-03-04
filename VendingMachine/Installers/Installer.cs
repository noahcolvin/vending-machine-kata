using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using VendingMachine.Managers;

namespace VendingMachine.Installers
{
    public class Installer : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Kernel.Register(Component.For<IMachine>().ImplementedBy<Machine>(),
                Component.For<ICoinManager>().ImplementedBy<CoinManager>());
        }
    }
}