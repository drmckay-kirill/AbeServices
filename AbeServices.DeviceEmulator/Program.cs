using System;
using System.Threading.Tasks;
using AbeServices.Common.Models.Mock;

namespace AbeServices.DeviceEmulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Starting");

            var cpabe = new MockCPAbe();
            var keys = await cpabe.Setup();

            Console.WriteLine("Finishing");
        }
    }
}
