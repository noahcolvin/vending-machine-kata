using Castle.Windsor;
using Castle.Windsor.Installer;

namespace VendingMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var container = new WindsorContainer().Install(FromAssembly.This()))
            {
                //Resolve machine and implement human interface
                var machine = container.Resolve<IMachine>();
                while (true)
                {
                    //Console.WriteLine(machine.Display);
                }
            }
        }
    }
}
