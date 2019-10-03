using System;
using System.Threading.Tasks;
using AbeServices.Common.Models.Mock;
using AbeServices.Common.Protocols;
using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;

namespace AbeServices.DeviceEmulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //await TestMockCpabe("Test Mock CP-ABE");
            await TestKeyDistributionBuilder();
        }

        static async Task TestKeyDistributionBuilder()
        {
            string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
            string serviceKey = "b14ca5898a4e4132bbce2ea2315a1915";
            string abonent = "device_emulator";
            string keyService = "test";
            string authority = "test";
            string[] abonentAttributes = new string[] { "test" };

            //var cpabe = new MockCPAbe();
            //var keys = await cpabe.Setup();

            var encryptor = new DataSymmetricEncryption();
            var serializer = new ProtobufDataSerializer();
            var builder = new KeyDistributionBuilder(serializer, encryptor);

            var firstStepData = builder.BuildStepOne(abonentKey, abonent, keyService, authority, abonentAttributes);

            var deserializedFirstStep = builder.GetStepOne(firstStepData);
            var secondStepData = builder.BuildStepTwo(serviceKey, abonent, keyService, authority, abonentAttributes, deserializedFirstStep.Payload);

            var deserializedSecondStep = builder.GetStepTwo(secondStepData);
            var deserializedAbonentPayload = builder.GetRequestPayload(deserializedSecondStep.AbonentPayload, abonentKey);
            var deserializedServicePayload = builder.GetRequestPayload(deserializedSecondStep.KeyServicePayload, serviceKey);
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
