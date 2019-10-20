using System;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
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
            //await TestKeyDistributionBuilder();
            //await TestKeyDistributionService();
            await TestAbeAuth();
        }

        static async Task TestAbeAuth()
        {
            // Initialization
            HttpClient client = new HttpClient();
            const string SessionHeader = "X-Session";

            string[] tgsAttr = new string[] { "iot", "sgt" };
            
            string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
            string abonent = "device_emulator";
            string[] abonentAttributes = new string[] { "teapot", "iot", "science" };
            string entityName = "teapot2";
        
            string keyService = "MachineService";
            string authority = "AttributeAuthority";

            string tgsUrl = "http://localhost:5011/api/tokens";
            string iotaUrl = $"http://localhost:5010/api/fiware/{entityName}";
            string keyServiceUrl = "http://localhost:5000/api/keys";
            
            var decorator = AbeDecorator.Factory.Create(abonentKey, abonent, keyService, authority, abonentAttributes, keyServiceUrl);
            await decorator.Setup();
            var builder = new AbeAuthBuilder(new ProtobufDataSerializer(), decorator);
            
            // FIWARE Context Broker request
            var initRequest = new byte[] { 0 }; // TODO replace to FIWARE NGSI

            // Request to IoTA to get access policy
            var stepOneResponse = await client.PostAsync(iotaUrl, new ByteArrayContent(initRequest));
            Console.WriteLine($"First step Http response status code = {stepOneResponse.StatusCode}");
            
            // Reading response from IoTA
            var stepOneData = await stepOneResponse.Content.ReadAsByteArrayAsync();
            var stepOne = builder.GetStepData<AbeAuthStepOne>(stepOneData);
            Console.WriteLine($"Access policy: {String.Join(" ", stepOne.AccessPolicy)}");

            // First request to TGS to start Token Generation Procedure
            var (stepTwoData, nonceR1) = await builder.BuildStepTwo(stepOne.AccessPolicy, abonentAttributes, tgsAttr, stepOne.Z);
            var stepTwoRequest = new ByteArrayContent(stepTwoData);
            var stepTwoResponse = await client.PostAsync(tgsUrl, stepTwoRequest);
            Console.WriteLine($"Second step, http response from TGS status code = {stepTwoResponse.StatusCode}");

            // Reading first response from TGS
            var stepThreeData = await stepTwoResponse.Content.ReadAsByteArrayAsync();
            var stepThree = builder.GetStepData<AbeAuthStepThree>(stepThreeData);
            var tgsSessionId = stepTwoResponse.Headers.GetValues(SessionHeader).First();

            // Second request to TGS to confirm nonce values
            var abonentNonceBytes = await decorator.Decrypt(stepThree.CtAbonent);
            var abonentNonce = BitConverter.ToInt32(abonentNonceBytes);
            var accesNonceBytes = await decorator.Decrypt(stepThree.CtAccess);
            var accessNonce = BitConverter.ToInt32(accesNonceBytes);
            var stepFourData = await builder.BuildStepFour(abonentNonce, accessNonce);
            var stepFourRequest = new ByteArrayContent(stepFourData);
            stepFourRequest.Headers.Add(SessionHeader, tgsSessionId);
            var stepFourResponse = await client.PostAsync(tgsUrl, stepFourRequest);
            Console.WriteLine($"Fourth step, http response from TGS status code = {stepFourResponse.StatusCode}");

            // Reading second response from TGS
            
        }

        static async Task TestKeyDistributionService()
        {
            string keyServiceUrl = "http://localhost:5000/api/keys";
            HttpClient client = new HttpClient();

            string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
            string abonent = "device_emulator";
            string keyService = "MachineService";
            string authority = "AttributeAuthority";
            string[] abonentAttributes = new string[] { "teapot", "iot", "science" };

            var encryptor = new DataSymmetricEncryption();
            var serializer = new ProtobufDataSerializer();
            var builder = new KeyDistributionBuilder(serializer, encryptor);
            
            var (firstRequestData, abonentNonce) = builder.BuildStepOne(abonentKey, abonent, keyService, authority, abonentAttributes);
            Console.WriteLine($"Generated nonce = {abonentNonce}");

            var requestContent = new ByteArrayContent(firstRequestData);
            var response = await client.PostAsync(keyServiceUrl, requestContent);
            Console.WriteLine($"Http response status code = {response.StatusCode}");
            
            var responseData = await response.Content.ReadAsByteArrayAsync(); 
            var payload = builder.GetPayload<KeyDistributionAuthToAbonent>(responseData, abonentKey);
            Console.WriteLine($"Received nonce = {payload.Nonce}");

            var cpabe = new MockCPAbe();

            MockSecretKey abonentPrivateKey = new MockSecretKey();
            abonentPrivateKey.Value = new byte[payload.SecretKey.Length];
            payload.SecretKey.CopyTo(abonentPrivateKey.Value , 0);

            MockPublicKey authorityPublicKey = new MockPublicKey();
            authorityPublicKey.Value = new byte[payload.PublicKey.Length];
            payload.PublicKey.CopyTo(authorityPublicKey.Value, 0);

            var ct = await cpabe.Encrypt("TestKeyDistributionService", authorityPublicKey, 
                                new MockAttributes(abonentAttributes));
            string message = await cpabe.Decrypt(ct, authorityPublicKey, abonentPrivateKey);
            Console.WriteLine(message);  

        }

        static async Task TestKeyDistributionBuilder()
        {
            string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
            string serviceKey = "b14ca5898a4e4132bbce2ea2315a1915";
            string abonent = "device_emulator";
            string keyService = "test";
            string authority = "test";
            string[] abonentAttributes = new string[] { "test" };

            var cpabe = new MockCPAbe();
            var keys = await cpabe.Setup();

            var encryptor = new DataSymmetricEncryption();
            var serializer = new ProtobufDataSerializer();
            var builder = new KeyDistributionBuilder(serializer, encryptor);

            var (firstStepData, abonenNonce) = builder.BuildStepOne(abonentKey, abonent, keyService, authority, abonentAttributes);

            var deserializedFirstStep = builder.GetStepData<KeyDistrubutionStepOne>(firstStepData);
            var (secondStepData, serviceNonce) = builder.BuildStepTwo(serviceKey, abonent, keyService, authority, abonentAttributes, deserializedFirstStep.Payload);

            var deserializedSecondStep = builder.GetStepData<KeyDistributionStepTwo>(secondStepData);
            var deserializedAbonentPayload = builder.GetPayload<KeyDistributionRequestPayload>(deserializedSecondStep.AbonentPayload, abonentKey);
            var deserializedServicePayload = builder.GetPayload<KeyDistributionRequestPayload>(deserializedSecondStep.KeyServicePayload, serviceKey);
            var secretKey = await cpabe.Generate(keys.MasterKey, keys.PublicKey, new MockAttributes(deserializedAbonentPayload.Attributes));
            var thirdStepData = builder.BuildStepThree(abonentKey, serviceKey, 
                    deserializedAbonentPayload.Nonce, deserializedServicePayload.Nonce,
                    keys.PublicKey.Value, secretKey.Value);

            var deserializedThirdStep = builder.GetStepData<KeyDistributionStepThree>(thirdStepData);
            var abonentResult = builder.GetPayload<KeyDistributionAuthToAbonent>(deserializedThirdStep.AbonentPayload, abonentKey);
            var serviceResult = builder.GetPayload<KeyDistributionAuthToService>(deserializedThirdStep.ServicePayload, serviceKey);

            MockSecretKey abonentPrivateKey = new MockSecretKey();
            abonentPrivateKey.Value = new byte[abonentResult.SecretKey.Length];
            abonentResult.SecretKey.CopyTo(abonentPrivateKey.Value , 0);
            
            MockPublicKey authorityPublicKey = new MockPublicKey();
            authorityPublicKey.Value = new byte[abonentResult.PublicKey.Length];
            abonentResult.PublicKey.CopyTo(authorityPublicKey.Value, 0);

            var ct = await cpabe.Encrypt("TestKeyDistributionBuilder", authorityPublicKey, 
                                new MockAttributes(abonentAttributes));
            string message = await cpabe.Decrypt(ct, authorityPublicKey, abonentPrivateKey);
            Console.WriteLine(message);           
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
