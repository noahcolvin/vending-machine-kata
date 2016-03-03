using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                var machine = container.Resolve<IMachine>();
                while (true)
                {
                    //machine.Vend();
                }
            }
        }
    }
}
