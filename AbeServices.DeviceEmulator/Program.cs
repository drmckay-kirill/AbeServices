using System;
using System.Threading.Tasks;
using AbeServices.Common.Models.Mock;
using AbeServices.Common.Helpers;

namespace AbeServices.DeviceEmulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await TestMockCpabe("Test Mock CP-ABE");
            string key = "b14ca5898a4e4133bbce2ea2315a1916";
            string message = "322_LOL_OMG";
            var ct = SymmetricEncryption.EncryptString(key, message);
            var res = SymmetricEncryption.DecryptToString(key, ct);
            Console.WriteLine(res);
        }

        static async Task TestMockCpabe(string plainText)
        {
            Console.WriteLine("Starting");

            var cpabe = new MockCPAbe();
            var keys = await cpabe.Setup();

            var secret = await cpabe.Generate(keys.MasterKey, keys.PublicKey,
                                new MockAttributes("test1 test2"));

            Console.WriteLine(plainText);
            var ct = await cpabe.Encrypt(plainText, keys.PublicKey, 
                                new MockAttributes("test1 test2"));

            string message = await cpabe.Decrypt(ct, keys.PublicKey, secret);
            Console.WriteLine(message);

            Console.WriteLine("Finishing");
        }
    }
}
