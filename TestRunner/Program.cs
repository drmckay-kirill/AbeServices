using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text.Json;
using AbeServices.Common.Models.Mock;
using AbeServices.Common.Protocols;
using AbeServices.Common.Helpers;
using AbeServices.Common.Models.Protocols;
using AbeServices.Common.Exceptions;
using NBomber.Contracts;
using NBomber.CSharp;

namespace TestRunner
{
    class Program
    {
        const int AttributesCount = 50; // 50

        static void Main(string[] args)
        {
            Console.WriteLine("Starting load testing of ABE services...");
            
            //KeyServiceScenario("sc1", 1, 5);
            //KeyServiceScenario("sc2", 10, 5);

            ProtocolScenario("protocol1", 1, 5);
        }

        static void ProtocolScenario(string prefix, int copies, int duration)
        {
            var testRunnerData = GetTestRunnerAttrbutePackages();
            foreach(var attrsData in testRunnerData)
            {
                var scenarionName = $"{prefix}_scenario_{attrsData.Length}";
                var stepName = $"{prefix}_step_{attrsData.Length}";

                var step = Step.Create(stepName, async context =>
                {
                    // Initialization
                    HttpClient client = new HttpClient();
                    const string SessionHeader = "X-Session";
                    const string HmacHeader = "X-HMAC";

                    string[] tgsAttr = new string[] { "iot", "sgt" };
                    
                    string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
                    string abonent = $"testEntity_{attrsData.Length}";
                    var abonentAttributes = attrsData;
                    string entityName = $"testEntity_{attrsData.Length}";
                
                    string keyService = "MachineService";
                    string authority = "AttributeAuthority";

                    string tgsUrl = "http://localhost:5011/api/tokens";
                    string iotaUrl = $"http://localhost:5010/api/fiware/{entityName}";
                    string keyServiceUrl = "http://localhost:5000/api/keys";

                    var decorator = AbeDecorator.Factory.Create(abonentKey, abonent, keyService, authority, abonentAttributes, keyServiceUrl);
                    await decorator.Setup();
                    var encryptor = new DataSymmetricEncryption();
                    var builder = new AbeAuthBuilder(new ProtobufDataSerializer(), decorator, encryptor);

                    // Init request to start AbeAuth protocol -> any data without session
                    var initRequest = new byte[] { 1 };

                    // Request to IoTA to get access policy
                    var stepOneResponse = await client.PostAsync(iotaUrl, new ByteArrayContent(initRequest));
                    
                    // Reading response from IoTA
                    var stepOneData = await stepOneResponse.Content.ReadAsByteArrayAsync();
                    var stepOne = builder.GetStepData<AbeAuthStepOne>(stepOneData);
                    var iotaSessionId = stepOneResponse.Headers.GetValues(SessionHeader).First();

                    // First request to TGS to start Token Generation Procedure
                    var (stepTwoData, nonceR1) = await builder.BuildStepTwo(stepOne.AccessPolicy, abonentAttributes, tgsAttr, stepOne.Z);
                    var stepTwoRequest = new ByteArrayContent(stepTwoData);
                    var stepTwoResponse = await client.PostAsync(tgsUrl, stepTwoRequest);

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

                    // Reading second response from TGS
                    var stepFiveData = await stepFourResponse.Content.ReadAsByteArrayAsync();
                    var stepFive = builder.GetStepData<AbeAuthStepFive>(stepFiveData);

                    // Send final request to IoTA
                    var sharedKey = await decorator.Decrypt(stepFive.CtAbonent);
                    var (stepSixData, hmac) = builder.BuildStepSix(stepFive.CtPep, stepFive.Z, sharedKey);           
                    
                    var stepSixRequest = new ByteArrayContent(stepSixData);
                    stepSixRequest.Headers.Add(SessionHeader, iotaSessionId);
                    var stepSixResponse = await client.PostAsync(iotaUrl, stepSixRequest);
                    
                    // Read final response from IoTA
                    var stepSevenData = await stepSixResponse.Content.ReadAsByteArrayAsync();
                    var stepSeven = builder.GetStepData<AbeAuthStepSeven>(stepSevenData);
                    var iotaHMAC = CryptoHelper.ComputeHash(hmac, sharedKey);
                    if (!iotaHMAC.SequenceEqual(stepSeven.HMAC))
                        throw new ProtocolArgumentException("HMAC is incorrect!");

                    return Response.Ok();
                });

                var scenario = ScenarioBuilder
                    .CreateScenario(scenarionName, new[] { step })
                    .WithConcurrentCopies(copies)
                    .WithDuration(TimeSpan.FromSeconds(duration));
                NBomberRunner
                    .RegisterScenarios(scenario)
                    .RunTest();
            }
        }

        static void KeyServiceScenario(string prefix, int copies, int duration)
        {
            string abonent = "test_runner";
            string keyServiceUrl = "http://localhost:5000/api/keys";
            string abonentKey = "b14ca5898a4e4133bbce2ea2315a1916";
            string keyService = "MachineService";
            string authority = "AttributeAuthority";

            var testRunnerData = GetTestRunnerAttrbutePackages();

            foreach(var attrsData in testRunnerData)
            {
                var scenarionName = $"{prefix}_scenario_{attrsData.Length}";
                var stepName = $"{prefix}_step_{attrsData.Length}";

                var step = Step.Create(stepName, async context =>
                {
                    var client = new HttpClient();
                    var encryptor = new DataSymmetricEncryption();
                    var serializer = new ProtobufDataSerializer();
                    var builder = new KeyDistributionBuilder(serializer, encryptor);
            
                    var (firstRequestData, abonentNonce) = builder.BuildStepOne(abonentKey, abonent, keyService, authority, attrsData);
                    //Console.WriteLine($"Generated nonce = {abonentNonce}");

                    var requestContent = new ByteArrayContent(firstRequestData);
                    var response = await client.PostAsync(keyServiceUrl, requestContent);
                    //Console.WriteLine($"Http response status code = {response.StatusCode}");
                    
                    //var responseData = await response.Content.ReadAsByteArrayAsync(); 
                    //var payload = builder.GetPayload<KeyDistributionAuthToAbonent>(responseData, abonentKey);
                    //Console.WriteLine($"Received nonce = {payload.Nonce}");
                    
                    return Response.Ok();
                });

                var scenario = ScenarioBuilder
                    .CreateScenario(scenarionName, new[] { step })
                    .WithConcurrentCopies(copies)
                    .WithDuration(TimeSpan.FromSeconds(duration));
                NBomberRunner
                    .RegisterScenarios(scenario)
                    .RunTest();
            }
        }

        static List<string[]> GetTestRunnerAttrbutePackages()
        {
            string[] attrs = new string[AttributesCount];
            for(int i = 0; i < AttributesCount; i++)
            {
                string attrName = $"iot_{i+1}";
                attrs[i] = attrName;
            }

            var res = new List<string[]>();
            for(int i = 0; i < AttributesCount; i++)
            {
                var attrsPackage = new string[i+1];
                Array.Copy(attrs, 0, attrsPackage, 0, i + 1);
                res.Add(attrsPackage);
            }

            return res;
        }
    }
}
