using Castle.Windsor;
using FluentAssertions;
using NUnit.Framework;
using VendingMachine.Installers;
using VendingMachine.Managers;

namespace VendingMachine.Test.Integration.Installers
{
    [TestFixture]
    public class InstallerShould
    {
        [Test]
        public void RegisterMachine()
        {
            IWindsorContainer container = new WindsorContainer().Install(new Installer());

            container.Kernel.HasComponent(typeof(IMachine)).Should().BeTrue();
        }

        [Test]
        public void BankManager()
        {
            IWindsorContainer container = new WindsorContainer().Install(new Installer());

            container.Kernel.HasComponent(typeof(IBankManager)).Should().BeTrue();
        }

        [Test]
        public void CoinManager()
        {
            IWindsorContainer container = new WindsorContainer().Install(new Installer());

            container.Kernel.HasComponent(typeof(ICoinManager)).Should().BeTrue();
        }

        [Test]
        public void ProductManager()
        {
            IWindsorContainer container = new WindsorContainer().Install(new Installer());

            container.Kernel.HasComponent(typeof(IProductManager)).Should().BeTrue();
        }
    }
}