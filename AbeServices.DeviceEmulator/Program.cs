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

            var secret = await cpabe.Generate(keys.MasterKey, keys.PublicKey,
                                new MockAttributes("test1 test2"));

            var ct = await cpabe.Encrypt("some text 322", keys.PublicKey, 
                                new MockAttributes("test1 test2"));

            string message = await cpabe.Decrypt(ct, keys.PublicKey, secret);
            Console.WriteLine(message);

            Console.WriteLine("Finishing");
        }
    }
}
